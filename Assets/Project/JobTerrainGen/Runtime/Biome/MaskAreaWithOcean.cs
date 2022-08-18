using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Biome
{
	public struct MaskAreaWithOcean : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (area_terrain.Length, 1024);

		public NativeArray<int> area_terrain;
		public NativeArray<bool> land_terrain;
		public NativeArray<int> area_terrain_with_ocean;
		public void Execute(int i_pixel)
		{
			area_terrain_with_ocean[i_pixel] = land_terrain[i_pixel] ? area_terrain[i_pixel] : 0;
		}

		public MaskAreaWithOcean(NativeArray<int> AreaTerrain, NativeArray<bool> LandTerrain, out NativeArray<int> AreaDataWithOcean)
		{
			area_terrain = AreaTerrain;
			land_terrain = LandTerrain;
			area_terrain_with_ocean = new(area_terrain.Length, Allocator.TempJob);

			AreaDataWithOcean = area_terrain_with_ocean;
		}

	}
}