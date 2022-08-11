using Unity.Collections;
using Unity.Mathematics;
using Utils;
using Utils.JobUtil.Template;
namespace JobTerrainGen.EnlargeFractal.Seed
{
	public struct GenSeedWithAroundOcean : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		Index2D i;
		NativeArray<int> data;
		public void Execute(int i_seed)
		{
			if (i.IsEdge(i[i_seed])) { data[i_seed] = 0; }
			else { data[i_seed] = i_seed + 1; }
		}

		public GenSeedWithAroundOcean(out NativeArray<int> Result, int2 Size)
		{
			i = new(Size);
			data = new(Size.area(), Allocator.TempJob);
			Result = data;
		}
	}
}