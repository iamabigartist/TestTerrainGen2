using Unity.Burst;
using Unity.Jobs;
namespace Utils.JobUtil.Template
{
	public interface IJobForRunner
	{
		int ExecuteLen { get; }
		int InnerLoopBatchCount { get; }
		void Execute(int i);
	}

	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
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
			deps = job.ScheduleParallel(Runner.ExecuteLen, Runner.InnerLoopBatchCount, deps);
		}
	}
}