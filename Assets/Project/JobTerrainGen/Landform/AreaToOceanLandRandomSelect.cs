using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.Landform
{
	public static class AreaToOceanLandRandomSelect
	{
		public static void Run(NativeArray<int> area_ids, float land_ratio, uint rand_seed, out NativeArray<int> area_landforms)
		{
			var area_count = area_ids.Length;
			area_landforms = new(Enumerable.Repeat(0, area_count).ToArray(), Allocator.TempJob);
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