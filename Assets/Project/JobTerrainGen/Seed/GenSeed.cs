using Unity.Collections;
using Utils.JobUtil.Template;
namespace JobTerrainGen.EnlargeFractal.Seed
{
	public struct GenSeed : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		NativeArray<int> data;
		public void Execute(int i_seed)
		{
			data[i_seed] = i_seed + 1;
		}
		public GenSeed(out NativeArray<int> Result, int Length)
		{
			data = new(Length, Allocator.TempJob);
			Result = data;
		}
	}
}