using Project.JobTerrainGen.Utils;
using Unity.Collections;
using Unity.Jobs;
namespace Project.JobTerrainGen.Land
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
			rand_gen.Gen(cur_area_id, out var rand);
			area_landforms[i_area] = rand.NextFloat(1f) < land_ratio ? 1 : 0;
		}

		public static void Plan(NativeArray<int> area_ids, float land_ratio, uint rand_seed, out NativeArray<int> area_landforms, ref JobHandle jh)
		{
			var area_count = area_ids.Length;
			area_landforms = new(area_count, Allocator.TempJob);
			var land_count = (int)(area_count * land_ratio);
			jh = new AreaToOceanLandRandom()
			{
				land_ratio = land_ratio,
				rand_gen = new(rand_seed),
				area_ids = area_ids,
				area_landforms = area_landforms
			}.ScheduleParallel(area_count, 4, jh);
		}
	}
}