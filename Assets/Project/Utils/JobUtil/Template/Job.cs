using Unity.Burst;
using Unity.Jobs;
namespace Utils.JobUtil.Template
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct Job<TJobRunner> : IJob
		where TJobRunner : IJobRunner
	{
		public TJobRunner runner;
		public void Execute()
		{
			runner.Execute();
		}
	}
	public interface IJobRunner
	{
		void Execute();
		public static void Plan<TJobRunner>(TJobRunner Runner, ref JobHandle deps)
			where TJobRunner : IJobRunner
		{
			var job = new Job<TJobRunner>() { runner = Runner };
			deps = job.Schedule(deps);
		}
	}
}