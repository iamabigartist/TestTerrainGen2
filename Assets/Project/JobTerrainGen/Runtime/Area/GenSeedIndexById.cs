using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Area
{
	public struct GenSeedIndexById : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_data.Length, 1024);

		[ReadOnly] NativeArray<int> seed_data;
		NativeHashMap<int, int>.ParallelWriter area_seed_index_by_area_id;
		public void Execute(int i_seed)
		{
			var cur_area_id = seed_data[i_seed];
			area_seed_index_by_area_id.TryAdd(cur_area_id, i_seed);
		}

		public GenSeedIndexById(NativeArray<int> SeedData, out NativeHashMap<int, int> AreaSeedIndexByAreaID)
		{
			seed_data = SeedData;

			AreaSeedIndexByAreaID = new(seed_data.Length, Allocator.TempJob);
			area_seed_index_by_area_id = AreaSeedIndexByAreaID.AsParallelWriter();
		}
	}
}