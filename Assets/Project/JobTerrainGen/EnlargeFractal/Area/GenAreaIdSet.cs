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

			public static JobHandle Plan(NativeArray<int> data, out NativeHashSet<int> id_set, JobHandle deps = default)
			{
				var len = data.Length;
				id_set = new(len, Allocator.TempJob);
				var job = new GenAreaIdSet()
				{
					data = data,
					id_set = id_set.AsParallelWriter()
				};
				return job.ScheduleParallel(len, 1024, deps);
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

			public static JobHandle Plan(NativeHashSet<int> set, out NativeArray<int> list, JobHandle deps = default)
			{
				list = new(set.Count(), Allocator.TempJob);
				var job = new IdSetToList()
				{
					set = set,
					list = list
				};
				return job.Schedule(deps);
			}
		}

		public static JobHandle Plan(NativeArray<int> data, out NativeArray<int> list, JobHandle deps = default)
		{
			var gen_jh = GenAreaIdSet.Plan(data, out var set, deps);
			var convert_jh = IdSetToList.Plan(set, out list, gen_jh);
			var dispose_jh = set.Dispose(convert_jh);
			return dispose_jh;
		}
	}


}