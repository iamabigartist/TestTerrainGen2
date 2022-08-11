using Unity.Collections;
using Utils;
using Utils.JobUtil.Template;
namespace JobTerrainGen.EnlargeFractal.Seed
{
	public struct GenSeedRand : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		IndexRandGenerator rand_gen;
		NativeArray<int> data;
		public void Execute(int i_seed)
		{
			rand_gen.Gen(i_seed, out var rand);
			data[i_seed] = rand.NextInt();
		}
		public GenSeedRand(out NativeArray<int> data, int length, uint rand_seed)
		{
			this.data = new(length, Allocator.TempJob);
			rand_gen = new(rand_seed);
			data = this.data;
		}
	}
}