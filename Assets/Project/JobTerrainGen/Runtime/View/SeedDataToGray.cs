using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace Project.JobTerrainGen.View
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct SeedDataToGray : IJobFor
	{
		[ReadOnly] NativeArray<int> data;
		[WriteOnly] NativeArray<float> color;
		public void Execute(int i_pixel)
		{
			var id = data[i_pixel];
			var rand = Random.CreateFromIndex((uint)id);
			color[i_pixel] = rand.NextFloat(1f);
		}

		public static void Plan(NativeArray<int> data, out NativeArray<float> color, ref JobHandle deps)
		{
			color = new(data.Length, Allocator.TempJob);
			var job = new SeedDataToGray()
			{
				data = data,
				color = color
			};
			deps = job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}