using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace JobTerrainGen.EnlargeFractal
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenSeedData : IJobFor
	{
		[WriteOnly] NativeArray<int> data;
		public void Execute(int i_seed)
		{
			data[i_seed] = i_seed;
		}

		public static JobHandle Plan(out NativeArray<int> data, int length, JobHandle deps = default)
		{
			data = new(length, Allocator.TempJob);
			var job = new GenSeedData()
			{
				data = data
			};
			return job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}