using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Utils.JobUtil;
namespace JobTerrainGen.EnlargeFractal.Area
{
	public static class GenAreaIdArray
	{
		[BurstCompile(
			DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
			CompileSynchronously = true)]
		public struct GenAreaIdSet : IJobFor
		{
			[ReadOnly] NativeArray<int> data;
			[WriteOnly] NativeHashSet<int>.ParallelWriter id_set;
			public void Execute(int i_pixel)
			{
				id_set.Add(data[i_pixel]);
			}

			public static void Plan(NativeArray<int> data, out NativeHashSet<int> id_set, ref JobHandle deps)
			{
				var len = data.Length;
				id_set = new(len, Allocator.TempJob);
				var job = new GenAreaIdSet() { data = data, id_set = id_set.AsParallelWriter() };
				deps = job.ScheduleParallel(len, 1024, deps);
			}
		}

		public static void Plan(NativeArray<int> data, out ResultGen<NativeArray<int>> array_gen, ref JobHandle deps)
		{
			GenAreaIdSet.Plan(data, out var set, ref deps);
			array_gen = (out NativeArray<int> array) =>
			{
				array = set.ToNativeArray(Allocator.TempJob);
				set.Dispose();
			};
		}
	}


}