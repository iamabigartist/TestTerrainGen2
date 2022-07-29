using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace JobTerrainGen.EnlargeFractal.Area
{
	public static class GenAreaIdList
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

		[BurstCompile(
			DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
			CompileSynchronously = true)]
		public struct IdSetToList : IJob
		{
			NativeHashSet<int> set;
			NativeArray<int> list;
			public void Execute()
			{
				list.CopyFrom(set.ToNativeArray(Allocator.Temp));
			}

			public static void Plan(NativeHashSet<int> set, out NativeArray<int> list, ref JobHandle deps)
			{
				list = new(set.Count(), Allocator.TempJob);
				var job = new IdSetToList() { set = set, list = list };
				deps = job.Schedule(deps);
			}
		}

		public static void Plan(NativeArray<int> data, out NativeArray<int> list, ref JobHandle deps)
		{
			GenAreaIdSet.Plan(data, out var set, ref deps);
			IdSetToList.Plan(set, out list, ref deps);
			deps = set.Dispose(deps);
		}
	}


}