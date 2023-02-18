using Project.JobTerrainGen.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;
namespace Project.JobTerrainGen.Noise.PositionalVoronoi
{
	[BurstCompile(
		FloatPrecision.High, FloatMode.Fast,
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct VoronoiSeedsJob : IJobFor
	{
		[ReadOnly] float seed_position_interval;
		[ReadOnly] float seed_position_jitter;
		[ReadOnly] Index2D i;
		[WriteOnly] NativeArray<float2> seeds_matrix;
		Random rand;
		public void Execute(int y)
		{
			for (int x = 0; x < i.Size.x; x++)
			{
				float2 uniform_position = new float2(x + 1, y + 1);
				float2 jitter_vector = rand.NextFloat2(new(-1, -1), new(1, 1)) * seed_position_jitter;
				seeds_matrix[i[x, y]] = (uniform_position + jitter_vector) * seed_position_interval;
			}
		}

		public static void Plan(NativeArray<float2> seeds, int2 seeds_size, float interval, float jitter, uint rand_seed, ref JobHandle deps)
		{
			var job = new VoronoiSeedsJob()
			{
				seed_position_interval = interval,
				seed_position_jitter = jitter,
				i = new(seeds_size),
				seeds_matrix = seeds,
				rand = new(rand_seed)
			};
			deps = job.ScheduleParallel(seeds_size.y, 16, deps);
		}
	}


	[BurstCompile(
		FloatPrecision.High, FloatMode.Fast,
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct VoronoiSeedsTextureJob : IJobFor
	{
		[ReadOnly] NativeArray<float2> seeds_matrix;
		[ReadOnly] Index2D i;
		[WriteOnly] NativeSlice<float3> texture;
		public void Execute(int i_seed)
		{
			int2 seed_position = (int2)round(seeds_matrix[i_seed]);
			texture[i[seed_position.x, seed_position.y]] = new(1, 1, 1);
			texture[i[seed_position.x - 1, seed_position.y - 1]] = new(1, 1, 1);
			texture[i[seed_position.x + 1, seed_position.y + 1]] = new(1, 1, 1);
			texture[i[seed_position.x - 1, seed_position.y + 1]] = new(1, 1, 1);
			texture[i[seed_position.x + 1, seed_position.y - 1]] = new(1, 1, 1);
		}

		public static void Plan(NativeArray<float2> seeds, int2 texture_size, NativeSlice<float3> texture, ref JobHandle deps)
		{
			var job = new VoronoiSeedsTextureJob()
			{
				seeds_matrix = seeds,
				i = new(texture_size),
				texture = texture
			};
			deps = job.ScheduleParallel(seeds.Length, 512, deps);
		}

	}
}