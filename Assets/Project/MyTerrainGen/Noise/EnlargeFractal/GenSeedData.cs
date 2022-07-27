using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Utils;
namespace MyTerrainGen.Noise.EnlargeFractal
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenSeedData : IJobFor
	{
		[ReadOnly] IndexRandGenerator rand_gen;
		[WriteOnly] NativeArray<int> data;
		public void Execute(int i_seed)
		{
			rand_gen.GenRand(i_seed, out var rand);
			data[i_seed] = rand.NextInt();
		}

		public static JobHandle Plan(out NativeArray<int> data, int length, uint rand_seed, JobHandle deps = default)
		{
			data = new(length, Allocator.TempJob);
			var job = new GenSeedData()
			{
				rand_gen = new(rand_seed),
				data = data
			};
			return job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}