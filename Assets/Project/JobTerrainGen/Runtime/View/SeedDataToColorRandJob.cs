using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.View
{
	public struct SeedDataToColorRand : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		IndexRandGenerator rand_gen;
		NativeArray<int> data;
		NativeArray<float3> color;
		public void Execute(int i_pixel)
		{
			var id = data[i_pixel];
			rand_gen.Gen(id, out var rand);
			color[i_pixel] = rand.NextFloat3(new(1, 1, 1));
		}
		public SeedDataToColorRand(NativeArray<int> Source, out NativeArray<float3> Result)
		{
			rand_gen = new();
			data = Source;
			color = new(Source.Length, Allocator.TempJob);
			Result = color;
		}
	}
}