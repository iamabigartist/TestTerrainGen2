using Unity.Collections;
using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.Land
{
	public static class AreaToOceanLandRandomSelect
	{
		public static void Run(NativeArray<int> area_ids, float land_ratio, uint rand_seed, out NativeHashMap<int, int> area_landforms)
		{
			var area_count = area_ids.Length;
			area_landforms = new(area_count + 1, Allocator.TempJob);
			var land_count = (int)(area_count * land_ratio);
			var rand = Random.CreateFromIndex(rand_seed);
			rand.SelectMultiple(land_count, area_count, out var land_list);
			foreach (int i in land_list)
			{
				area_landforms[i] = 1;
			}
		}
	}
}