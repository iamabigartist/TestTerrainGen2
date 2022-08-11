using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Jobs;
namespace Utils.JobUtil.Template
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	[StructLayout(LayoutKind.Sequential)]
	public struct JobFor<TJobForRunner> : IJobFor
		where TJobForRunner : IJobForRunner
	{
		public TJobForRunner runner;
		public void Execute(int i)
		{
			runner.Execute(i);
		}
		public static void Plan(TJobForRunner Runner, ref JobHandle deps)
		{
			var job = new JobFor<TJobForRunner>() { runner = Runner };
			var (execute_len, inner_loop_batch_count) = Runner.ScheduleParam;
			deps = job.ScheduleParallel(execute_len, inner_loop_batch_count, deps);
		}
	}
	public interface IJobForRunner
	{
		(int ExecuteLen, int InnerLoopBatchCount) ScheduleParam { get; }
		void Execute(int i);

	}
}