using System.Linq;
using Project.JobTerrainGen.Utils;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Land
{
	public static class AreaToOceanLandRandomSelect
	{
		public static void Run(NativeArray<int> AreaIds, float LandRatio, uint RandSeed, out NativeHashMap<int, bool> LandByAreaId)
		{
			var area_count = AreaIds.Length;
			LandByAreaId = new(area_count + 1, Allocator.TempJob);
			var land_count = (int)(area_count * LandRatio);
			var rand = Random.CreateFromIndex(RandSeed);
			rand.SelectMultiple(land_count, area_count, out var land_list);
			var land_set = land_list.ToHashSet();
			LandByAreaId[0] = false;
			for (int i = 0; i < area_count; i++)
			{
				var cur_area_id = AreaIds[i];
				LandByAreaId[cur_area_id] = land_set.Contains(i);
			}
		}
	}
}