using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Labs
{
	public static class IndexerUtil
	{
		public static Indexer2D<NativeArrayIndexer<T>, T> GetIndexer2D<T>(this NativeArray<T> data, int2 size)
			where T : struct
		{
			return new(size, new(data));
		}
		public static Indexer2D<NativeArrayIndexer<T>, T> GetIndexer3D<T>(this NativeArray<T> data, int2 size)
			where T : struct
		{
			return new(size, new(data));
		}
		public static Indexer2D<NativeSliceIndexer<T>, T> GetIndexer2D<T>(this NativeSlice<T> data, int2 size)
			where T : struct
		{
			return new(size, new(data));
		}
		public static Indexer2D<NativeSliceIndexer<T>, T> GetIndexer3D<T>(this NativeSlice<T> data, int2 size)
			where T : struct
		{
			return new(size, new(data));
		}
	}

	public interface IIndexer<T>
		where T : struct
	{
		T this[int index] { get; set; }
	}

	public struct NativeArrayIndexer<T> : IIndexer<T> where T : struct
	{
		NativeArray<T> native_array;
		public T this[int index]
		{
			get => native_array[index];
			set => native_array[index] = value;
		}
		public NativeArrayIndexer(NativeArray<T> native_array)
		{
			this.native_array = native_array;
		}
	}

	public struct NativeSliceIndexer<T> : IIndexer<T> where T : struct
	{
		NativeSlice<T> native_array;
		public T this[int index]
		{
			get => native_array[index];
			set => native_array[index] = value;
		}
		public NativeSliceIndexer(NativeSlice<T> native_array)
		{
			this.native_array = native_array;
		}
	}

	public struct Indexer2D<TIndexer, TData>
		where TData : struct
		where TIndexer : struct, IIndexer<TData>
	{
		public readonly int2 Size;
		TIndexer indexer;

		public Indexer2D(int2 size, TIndexer indexer)
		{
			Size = size;
			this.indexer = indexer;
		}

	#region Property

		public int Count => Size.x * Size.y;
		public int2 CenterPoint => Size / 2;

	#endregion

	#region Indexer

		public TData this[int x, int y]
		{
			get => indexer[x + y * Size.x];
			set => indexer[x + y * Size.x] = value;
		}

	#endregion

	#region Util

		public int2 PositionByIndex(int i)
		{
			int y = i / Size.x;
			i -= y * Size.x;
			int x = i;

			return new(x, y);
		}

		public int IndexByPosition(int x, int y)
		{
			return x + y * Size.x;
		}

		public bool OutOfRange(int x, int y)
		{
			return
				x < 0 || x > Size.x - 1 ||
				y < 0 || y > Size.y - 1;
		}

	#endregion
	}

	public struct Indexer3D<TIndexer, TData>
		where TData : struct
		where TIndexer : struct, IIndexer<TData>
	{
		public readonly int3 Size;
		IIndexer<TData> indexer;

		public Indexer3D(int3 size, IIndexer<TData> indexer)
		{
			Size = size;
			this.indexer = indexer;
		}

	#region Property

		public int Count => Size.x * Size.y * Size.z;
		public int3 CenterPoint => Size / 2;

	#endregion

	#region Indexer

		public TData this[int x, int y, int z]
		{
			get => indexer[x + y * Size.x + z * Size.y * Size.x];
			set => indexer[x + y * Size.x + z * Size.y * Size.x] = value;
		}

	#endregion

	#region Util

		public int3 PositionByIndex(int i)
		{
			int z = i / (Size.x * Size.y);
			i -= z * Size.x * Size.y;
			int y = i / Size.x;
			i -= y * Size.x;
			int x = i;

			return new(x, y, z);
		}

		public int IndexByPosition(int x, int y, int z)
		{
			return x + y * Size.x + z * Size.y * Size.x;
		}

		public bool OutOfRange(int x, int y, int z)
		{
			return
				x < 0 || x > Size.x - 1 ||
				y < 0 || y > Size.y - 1 ||
				z < 0 || z > Size.z - 1;
		}

	#endregion
	}

	[BurstCompile(
		FloatPrecision.High, FloatMode.Fast,
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct VoronoiJob : IJobFor
	{
		[WriteOnly] Indexer2D<NativeSliceIndexer<float>, float> dst;
		[ReadOnly] FastNoiseLiteBurst noise_generator;

		public void Execute(int y)
		{
			for (int x = 0; x < dst.Size.y; x++)
			{
				// dst[x, y] = noise.snoise(new float2(x, y) * 0.001f);
				dst[x, y] = noise_generator.GetNoise(x, y);
			}
		}

		public static JobHandle ScheduleParallel(
			NativeSlice<float> dst,
			int2 resolution,
			JobHandle deps = default
		)
		{
			var noise_generator = new FastNoiseLiteBurst();
			noise_generator.SetNoiseType(FastNoiseLiteBurst.NoiseType.Cellular);
			noise_generator.SetCellularReturnType(FastNoiseLiteBurst.CellularReturnType.CellValue);
			noise_generator.SetFrequency(0.005f);
			noise_generator.SetCellularJitter(0.5f);
			var job = new VoronoiJob()
			{
				dst = dst.GetIndexer2D(resolution),
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

	#endregion

	#region Data

		Texture2D mTexture;
		NativeSlice<float> red;
		NativeSlice<float> green;
		NativeSlice<float> blue;

	#endregion
		void OnEnable()
		{
			mTexture = new(resolution, resolution, TextureFormat.RGBAFloat, false);
			mRenderer.material.mainTexture = mTexture;
			red = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 0);
			green = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 1);
			blue = mTexture.GetRawTextureData<float4>().Slice().SliceWithStride<float>(sizeof(float) * 2);
			var voronoi_job_handle = VoronoiJob.ScheduleParallel(red, new(resolution, resolution));
			voronoi_job_handle.Complete();
			green.CopyFrom(red);
			blue.CopyFrom(red);
			mTexture.Apply();
		}
	}

}