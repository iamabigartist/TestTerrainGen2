using Project.JobTerrainGen.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Labs
{

	[BurstCompile(
		FloatPrecision.High, FloatMode.Fast,
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct VoronoiJob : IJobFor
	{
		[ReadOnly] Index2D index;
		[WriteOnly] NativeSlice<float> dst;
		[ReadOnly] FastNoiseLiteBurst noise_generator;

		public void Execute(int y)
		{
			for (int x = 0; x < index.Size.y; x++)
			{
				// dst[x, y] = noise.snoise(new float2(x, y) * 0.001f);
				dst[index[x, y]] = noise_generator.GetNoise(x, y);
			}
		}

		public static JobHandle ScheduleParallel(
			NativeSlice<float> dst,
			int2 resolution,
			FastNoiseLiteBurst noise_generator,
			JobHandle deps = default
		)
		{
			var job = new VoronoiJob()
			{
				index = new(resolution),
				dst = dst,
				noise_generator = noise_generator
			};
			return job.ScheduleParallel(resolution.y, 1, deps);
		}
	}

	[BurstCompile(
		FloatPrecision.High, FloatMode.Fast,
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct VoronoiPointJob : IJobFor
	{
		[ReadOnly] Index2D index;
		[WriteOnly] NativeSlice<float2> dst;
		[ReadOnly] FastNoiseLiteBurst noise_generator;

		public void Execute(int y)
		{
			for (int x = 0; x < index.Size.y; x++)
			{
				// dst[x, y] = noise.snoise(new float2(x, y) * 0.001f);
				dst[index[x, y]] = noise_generator.GetCellularF1Point(x, y);
			}
		}

		public static JobHandle ScheduleParallel(
			NativeSlice<float2> dst,
			int2 resolution,
			FastNoiseLiteBurst noise_generator,
			JobHandle deps = default
		)
		{
			var job = new VoronoiPointJob()
			{
				index = new(resolution),
				dst = dst,
				noise_generator = noise_generator
			};
			return job.ScheduleParallel(resolution.y, 1, deps);
		}
	}

	public class TestFastNoiseLite : MonoBehaviour
	{
	#region Reference

		public Renderer mRenderer;

	#endregion

	#region Config

		public int resolution = 1024;
		public float voronoi_jitter = 0.2f;
		public float voronoi_interval = 200;

	#endregion

	#region Tool

		FastNoiseLiteBurst noise_generator;

	#endregion

	#region Data

		int2 texture_size;
		Texture2D mTexture;
		NativeSlice<float> red;
		NativeSlice<float> green;
		NativeSlice<float> blue;
		NativeSlice<float2> red_green;
		NativeSlice<float3> red_green_blue;

	#endregion
		void OnEnable()
		{
			texture_size = new(resolution, resolution);

			noise_generator = new();
			noise_generator.SetNoiseType(FastNoiseLiteBurst.NoiseType.Cellular);
			noise_generator.SetCellularReturnType(FastNoiseLiteBurst.CellularReturnType.CellValue);
			noise_generator.SetCellularDistanceFunction(FastNoiseLiteBurst.CellularDistanceFunction.Hybrid);
			noise_generator.SetFrequency(0.005f);
			noise_generator.SetCellularJitter(1.0f);

			mTexture = new(resolution, resolution, TextureFormat.RGBAFloat, false) { filterMode = FilterMode.Point };
			mRenderer.material.mainTexture = mTexture;
			red = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 0);
			green = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 1);
			blue = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 2);
			red_green = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float2>(sizeof(float) * 0);
			red_green_blue = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float3>(sizeof(float) * 0);

			//Normal voronoi texture
			// var voronoi_job_handle = VoronoiJob.ScheduleParallel(red, new(resolution, resolution), noise_generator);
			// voronoi_job_handle.Complete();
			// green.CopyFrom(red);
			// blue.CopyFrom(red);

			//Try show the voronoi points with 2 channel
			// var voronoi_point_job_handle = VoronoiPointJob.ScheduleParallel(red_green, new(resolution, resolution), noise_generator);
			// voronoi_point_job_handle.Complete();

			//Voronoi seed points
			// var seeds_size = (int2)floor((float2)texture_size / voronoi_interval) - new int2(1, 1);
			// var seed_matrix = new NativeArray<float2>(seeds_size.area(), Allocator.TempJob);
			// var jh = new JobHandle();
			// VoronoiSeedsJob.Plan(seed_matrix, seeds_size, voronoi_interval, voronoi_jitter, 100, ref jh);
			// VoronoiSeedsTextureJob.Plan(seed_matrix, texture_size, red_green_blue, ref jh);
			// jh.Complete();
			// seed_matrix.Dispose();

			mTexture.Apply();
		}
	}

}