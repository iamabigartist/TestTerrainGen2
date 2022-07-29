using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.EnlargeFractal.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenOceanLandAreaColor : IJobFor
	{
		[ReadOnly] float3 ocean_color;
		[ReadOnly] NativeArray<int> area_ids;
		[WriteOnly] NativeArray<float3> area_colors;
		public void Execute(int i_area)
		{
			var cur_area_id = area_ids[i_area];
			if (cur_area_id == 0)
			{
				area_colors[i_area] = ocean_color;
			}
			else
			{
				var rand = Random.CreateFromIndex((uint)cur_area_id);
				area_colors[i_area] = rand.NextFloat3(new(1, 1, 1));
			}
		}

		public static void Plan(NativeArray<int> area_ids, float3 ocean_color, out NativeArray<float3> area_colors, ref JobHandle deps)
		{
			var area_count = area_ids.Length;
			area_colors = new(area_count, Allocator.Persistent);
			var job = new GenOceanLandAreaColor()
			{
				ocean_color = ocean_color,
				area_ids = area_ids,
				area_colors = area_colors
			};
			deps = job.ScheduleParallel(area_count, 1024, deps);
		}
	}
}