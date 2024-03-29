﻿using Unity.Burst;
using Unity.Jobs;
namespace Project.JobTerrainGen.Utils.JobUtil.Template
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct JobFor<TJobForRunner> : IJobFor
		where TJobForRunner : struct, IJobForRunner
	{
		public TJobForRunner runner;
		public void Execute(int i)
		{
			runner.Execute(i);
		}
		public static void Plan(TJobForRunner Runner, ref JobHandle Deps)
		{
			var job = new JobFor<TJobForRunner>() { runner = Runner };
			var (execute_len, inner_loop_batch_count) = Runner.ScheduleParam;
			Deps = job.ScheduleParallel(execute_len, inner_loop_batch_count, Deps);
		}
	}
	public interface IJobForRunner
	{
		(int ExecuteLen, int InnerLoopBatchCount) ScheduleParam { get; }
		void Execute(int i);

	}
}