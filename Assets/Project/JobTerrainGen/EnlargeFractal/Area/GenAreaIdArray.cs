using Unity.Collections;
using Utils.JobUtil.Template;
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
		public GenAreaIdArray(NativeArray<int> Source, out NativeHashSet<int> id_set)
		{
			id_set = new(Source.Length, Allocator.Persistent);
			data = Source;
			id_set_w = id_set.AsParallelWriter();
		}
	}
}