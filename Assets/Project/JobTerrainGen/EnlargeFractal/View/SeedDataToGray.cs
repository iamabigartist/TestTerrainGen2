using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.EnlargeFractal.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct SeedDataToGray : IJobFor
	{
		[ReadOnly] NativeArray<int> data;
		[WriteOnly] NativeArray<float> color;
		public void Execute(int i)
		{
			var id = data[i];
			var rand = Random.CreateFromIndex((uint)id);
			color[i] = rand.NextFloat(1f);
		}

		public static JobHandle Plan(NativeArray<int> data, out NativeArray<float> color, JobHandle deps = default)
		{
			color = new(data.Length, Allocator.TempJob);
			var job = new SeedDataToGray()
			{
				data = data,
				color = color
			};
			return job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}