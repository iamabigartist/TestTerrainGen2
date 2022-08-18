using Unity.Jobs;
namespace Project.JobTerrainGen.Utils.JobUtil
{
	public static class ScheduleUtil
	{
		public interface IForSchedule
		{
			(int ExecuteLen, int InnerLoopBatchCount) ScheduleParam { get; }
		}
		public static void PlanFor<TJob>(TJob job, ref JobHandle deps)
			where TJob : struct, IJobFor, IForSchedule
		{
			var (execute_len, inner_loop_batch_count) = job.ScheduleParam;
			deps = job.ScheduleParallel(execute_len, inner_loop_batch_count, deps);
		}

		public static void PlanSingle<TJob>(TJob job, ref JobHandle deps)
			where TJob : struct, IJob
		{
			deps = job.Schedule(deps);
		}
	}
}