using Unity.Collections;
using Utils;
using Utils.JobUtil.Template;
namespace JobTerrainGen.EnlargeFractal.Seed
{
	public struct GenSeedRand : IJobForRunner
	{
		public int ExecuteLen => data.Length;
		public int InnerLoopBatchCount => 1024;

		IndexRandGenerator rand_gen;
		NativeArray<int> data;
		public void Execute(int i_seed)
		{
			rand_gen.Gen(i_seed, out var rand);
			data[i_seed] = rand.NextInt();
		}
		public GenSeedRand(out NativeArray<int> Data, int Length, uint rand_seed)
		{
			data = new(Length, Allocator.TempJob);
			rand_gen = new(rand_seed);
			Data = data;
		}
	}
}