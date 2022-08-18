using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Land
{
	public struct AreaTerrainToLandTerrain : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (area_terrain.Length, 1024);

		NativeArray<int> area_terrain;
		[ReadOnly] NativeHashMap<int, bool> land_by_area_id;
		NativeArray<bool> land_terrain;
		public void Execute(int i_pixel)
		{
			land_terrain[i_pixel] = land_by_area_id[area_terrain[i_pixel]];
		}

		public AreaTerrainToLandTerrain(NativeArray<int> AreaTerrain, NativeHashMap<int, bool> LandByAreaID, out NativeArray<bool> Mask)
		{
			area_terrain = AreaTerrain;
			land_by_area_id = LandByAreaID;
			land_terrain = new(area_terrain.Length, Allocator.TempJob);

			Mask = land_terrain;
		}

	}
}