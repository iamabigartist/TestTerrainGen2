using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct DataToColorByTable : IJobFor
	{
		[ReadOnly] NativeHashMap<int, float3> color_table;
		[ReadOnly] NativeArray<int> data;
		[WriteOnly] NativeArray<float3> color;
		public void Execute(int i)
		{
			color[i] = color_table[data[i]];
		}

		public static void Plan(NativeArray<int> data, NativeHashMap<int, float3> color_table, out NativeArray<float3> color, ref JobHandle deps)
		{
			color = new(data.Length, Allocator.TempJob);
			var job = new DataToColorByTable()
				{ color_table = color_table, data = data, color = color };
			deps = job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}