using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenOceanLandAreaColor : IJobFor
	{
		[ReadOnly] float3 ocean_color;
		[ReadOnly] NativeArray<int> area_ids;
		[ReadOnly] NativeHashMap<int, int> area_landforms;
		[WriteOnly] NativeHashMap<int, float3>.ParallelWriter area_colors;
		public void Execute(int i_area)
		{
			var cur_area_landform = area_landforms[i_area];
			var cur_area_id = area_ids[i_area];
			if (cur_area_landform == 0)
			{
				area_colors.TryAdd(cur_area_id, ocean_color);
			}
			else
			{
				var rand = Random.CreateFromIndex((uint)cur_area_id);
				area_colors.TryAdd(cur_area_id, rand.NextFloat3(new(1, 1, 1)));
			}
		}

		public static void Plan(NativeArray<int> area_ids, NativeHashMap<int, int> area_landforms, float3 ocean_color, out NativeHashMap<int, float3> area_colors, ref JobHandle deps)
		{
			var area_count = area_ids.Length;
			area_colors = new(area_count + 1, Allocator.Persistent);
			area_colors[0] = ocean_color;
			var job = new GenOceanLandAreaColor()
			{
				ocean_color = ocean_color,
				area_ids = area_ids,
				area_landforms = area_landforms,
				area_colors = area_colors.AsParallelWriter()
			};
			deps = job.ScheduleParallel(area_count, 1024, deps);
		}
	}
}