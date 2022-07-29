using Unity.Collections;
using Unity.Jobs;
using Utils;
namespace JobTerrainGen.Landform
{
	public struct AreaToOceanLandRandom : IJobFor
	{
		[ReadOnly] float land_ratio;
		[ReadOnly] IndexRandGenerator rand_gen;
		[ReadOnly] NativeArray<int> area_ids;
		[WriteOnly] NativeArray<int> area_landforms;
		public void Execute(int i_area)
		{
			var cur_area_id = area_ids[i_area];
			rand_gen.GenRand(cur_area_id, out var rand);
			area_landforms[i_area] = rand.NextFloat(1f) < land_ratio ? 1 : 0;
		}
	}
}