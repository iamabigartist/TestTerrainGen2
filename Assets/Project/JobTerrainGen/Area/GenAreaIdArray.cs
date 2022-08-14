using Unity.Collections;
using Utils.JobUtil.Template;
namespace JobTerrainGen.Area
{
	public struct GenAreaIdArray : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (area_terrain.Length, 1024);

		NativeArray<int> area_terrain;
		NativeHashSet<int>.ParallelWriter id_set;
		public void Execute(int i_pixel)
		{
			id_set.Add(area_terrain[i_pixel]);
		}
		public GenAreaIdArray(NativeArray<int> Source, out NativeHashSet<int> IdSet)
		{
			area_terrain = Source;

			IdSet = new(Source.Length, Allocator.Persistent);
			id_set = IdSet.AsParallelWriter();
		}
	}
}