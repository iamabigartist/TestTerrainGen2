using Unity.Collections;
using Utils.JobUtil.Template;
using static Utils.JobUtil.Utils;
namespace JobTerrainGen.EnlargeFractal.Area
{
	public struct GenAreaIdArray : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		NativeArray<int> data;
		NativeHashSet<int>.ParallelWriter id_set_w;
		public void Execute(int i_pixel)
		{
			id_set_w.Add(data[i_pixel]);
		}
		public GenAreaIdArray(NativeArray<int> Source, out ResultGen<NativeArray<int>> GenResult, DataModify<NativeHashSet<int>> ModifySet = null)
		{
			var id_set = new NativeHashSet<int>(Source.Length, Allocator.Persistent);
			data = Source;
			id_set_w = id_set.AsParallelWriter();
			GenResult = (out NativeArray<int> array) =>
			{
				ModifySet?.Invoke(ref id_set);
				array = id_set.ToNativeArray(Allocator.TempJob);
				id_set.Dispose();
			};
		}
	}
}