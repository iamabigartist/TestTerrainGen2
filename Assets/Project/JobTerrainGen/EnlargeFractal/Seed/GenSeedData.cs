using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace JobTerrainGen.EnlargeFractal.Seed
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenSeedData : IJobFor
	{
		[WriteOnly] NativeArray<int> data;
		public void Execute(int i_seed)
		{
			data[i_seed] = i_seed + 1;
		}

		public static void Plan(out NativeArray<int> data, int length, ref JobHandle deps)
		{
			data = new(length, Allocator.TempJob);
			var job = new GenSeedData() { data = data };
			deps = job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}