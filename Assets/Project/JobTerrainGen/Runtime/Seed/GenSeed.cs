using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Seed
{
	public struct GenSeed : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_data.Length, 1024);

		[WriteOnly] NativeArray<int> seed_data;
		public void Execute(int i_seed)
		{
			seed_data[i_seed] = i_seed + 1;
		}
		public GenSeed(out NativeArray<int> Result, int Length)
		{
			seed_data = new(Length, Allocator.TempJob);
			Result = seed_data;
		}
	}
}