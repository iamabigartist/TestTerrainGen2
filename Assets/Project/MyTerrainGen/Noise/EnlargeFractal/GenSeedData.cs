using Unity.Collections;
using Unity.Jobs;
using Utils;
namespace MyTerrainGen.Noise.EnlargeFractal
{
	public struct GenSeedData : IJobFor
	{
		[ReadOnly] IndexRandGenerator rand_gen;
		[WriteOnly] NativeSlice<int> data;
		public void Execute(int i_seed)
		{
			rand_gen.GenRand(i_seed, out var rand);
			data[i_seed] = rand.NextInt();
		}

		public static JobHandle Plan(NativeSlice<int> data, uint rand_seed, JobHandle deps = default)
		{
			var job = new GenSeedData()
			{
				rand_gen = new(rand_seed),
				data = data
			};
			return job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}