using Unity.Collections;
using Unity.Mathematics;
using Utils;
using Utils.JobUtil.Template;
namespace JobTerrainGen.Seed
{
	public struct GenSeedWithAroundOcean : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_data.Length, 1024);

		Index2D i;
		NativeArray<int> seed_data;
		public void Execute(int i_seed)
		{
			if (i.IsEdge(i[i_seed])) { seed_data[i_seed] = 0; }
			else { seed_data[i_seed] = i_seed + 1; }
		}

		public GenSeedWithAroundOcean(out NativeArray<int> Result, int2 Size)
		{
			i = new(Size);
			seed_data = new(Size.area(), Allocator.TempJob);
			Result = seed_data;
		}
	}
}