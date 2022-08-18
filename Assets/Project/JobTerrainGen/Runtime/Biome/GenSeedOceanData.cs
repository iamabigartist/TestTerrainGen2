using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Biome
{
	public struct GenSeedOceanData : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_data.Length, 1024);

		NativeArray<int> seed_data;
		[ReadOnly] NativeHashSet<int> land_set;
		NativeArray<int> seed_ocean_data;
		public void Execute(int i_seed)
		{
			var cur_area_id = seed_data[i_seed];
			seed_ocean_data[i_seed] = land_set.Contains(cur_area_id) ? 0 : 1;
		}

		public GenSeedOceanData(NativeArray<int> SeedData, NativeHashSet<int> LandSet, out NativeArray<int> SeedOceanData)
		{
			seed_data = SeedData;
			land_set = LandSet;
			seed_ocean_data = new(seed_data.Length, Allocator.TempJob);

			SeedOceanData = seed_ocean_data;
		}
	}
}