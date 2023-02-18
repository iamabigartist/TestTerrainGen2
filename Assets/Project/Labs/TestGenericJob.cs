using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Jobs;
using UnityEngine;
namespace Labs
{
	public interface IInner
	{
		void Ainner();
	}
	public struct TestInner : IInner
	{
		public void Ainner() {}
	}
	public struct GenericRunner<TInner> : IJobForRunner
		where TInner : struct, IInner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (10, 1);
		TInner inner;
		public void Execute(int i_seed) {}
		public GenericRunner(TInner inner, int len)
		{
			this.inner = inner;
		}
	}
	public class TestGenericJob : MonoBehaviour
	{
		void Start()
		{
			var jh = new JobHandle();
			JobFor<GenericRunner<TestInner>>.Plan(new(new(), 100), ref jh);
			jh.Complete();
		}
	}
}