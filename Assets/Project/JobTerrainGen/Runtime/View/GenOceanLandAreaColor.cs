using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.View
{
	public struct LandAreaToColor : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (area_ids.Length, 1);

		float3 ocean_color;
		NativeArray<int> area_ids;
		[ReadOnly] NativeHashMap<int, bool> land_by_area_id;
		NativeHashMap<int, float3>.ParallelWriter color_by_area_id;
		public void Execute(int i_area)
		{
			var cur_area_id = area_ids[i_area];
			if (!land_by_area_id[cur_area_id])
			{
				color_by_area_id.TryAdd(cur_area_id, ocean_color);
			}
			else
			{
				var rand = Random.CreateFromIndex((uint)cur_area_id);
				color_by_area_id.TryAdd(cur_area_id, rand.NextFloat3(new(1, 1, 1)));
			}
		}

		public LandAreaToColor(NativeArray<int> AreaIds, NativeHashMap<int, bool> LandByAreaId, float3 OceanColor, out NativeHashMap<int, float3> ColorByAreaId)
		{
			var area_count = AreaIds.Length;
			ocean_color = OceanColor;
			area_ids = AreaIds;
			land_by_area_id = LandByAreaId;

			ColorByAreaId = new(area_count + 1, Allocator.Persistent);
			ColorByAreaId[0] = OceanColor;
			color_by_area_id = ColorByAreaId.AsParallelWriter();
		}
	}
}