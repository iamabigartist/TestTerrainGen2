using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Biome
{
	/// <summary>
	///     fixed: 1 for deep ocean, 2 for shallow ocean, 3 for river
	/// </summary>
	public struct GenBiomeTerrain : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (area_terrain.Length, 1024);

		NativeArray<int> area_terrain;
		[ReadOnly] NativeHashMap<int, int> area_seed_index_by_area_id;
		[ReadOnly] NativeHashSet<int> not_land_biome_ids;
		NativeArray<int> seed_biome_data;
		NativeArray<bool> land_terrain;
		NativeArray<int> biome_terrain;
		public void Execute(int i_pixel)
		{
			int result_biome_terrain;
			var cur_area_id = area_terrain[i_pixel];
			var cur_land_terrain = land_terrain[i_pixel];
			var cur_area_seed_index = area_seed_index_by_area_id[cur_area_id];
			var cur_area_biome_id = seed_biome_data[cur_area_seed_index];
			if (!not_land_biome_ids.Contains(cur_area_biome_id) && !cur_land_terrain)
				//not on land terrain but is land biome
			{
				result_biome_terrain = 2;
			}
			else { result_biome_terrain = cur_area_biome_id; }
			biome_terrain[i_pixel] = result_biome_terrain;
		}

		public GenBiomeTerrain(NativeArray<int> AreaTerrain, NativeHashMap<int, int> AreaSeedIndexByAreaID, NativeHashSet<int> NotLandBiomeIds, NativeArray<int> SeedBiomeData, NativeArray<bool> LandTerrain, out NativeArray<int> BiomeTerrain)
		{
			area_terrain = AreaTerrain;
			area_seed_index_by_area_id = AreaSeedIndexByAreaID;
			not_land_biome_ids = NotLandBiomeIds;
			seed_biome_data = SeedBiomeData;
			land_terrain = LandTerrain;
			biome_terrain = new(area_terrain.Length, Allocator.TempJob);

			BiomeTerrain = biome_terrain;
		}
	}
}