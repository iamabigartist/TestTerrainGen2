using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.EnlargeFractal.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct SeedDataToColorRand : IJobFor
	{
		[ReadOnly] NativeArray<int> data;
		[WriteOnly] NativeArray<float3> color;
		public void Execute(int i)
		{
			var id = data[i];
			var rand = Random.CreateFromIndex((uint)id);
			color[i] = rand.NextFloat3(new(1, 1, 1));
		}

		public static void Plan(NativeArray<int> data, out NativeArray<float3> color, ref JobHandle deps)
		{
			color = new(data.Length, Allocator.TempJob);
			var job = new SeedDataToColorRand() { data = data, color = color };
			deps = job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}