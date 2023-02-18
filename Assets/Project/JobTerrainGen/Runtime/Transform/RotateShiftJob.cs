using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Transform
{
	public struct RotateShift : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		int2 shift;
		Index2D i;
		NativeArray<int> data;
		NativeArray<int> shifted_data;
		public void Execute(int i_pixel)
		{
			var shifted_pos = i.RepeatWrap(i[i_pixel] + shift);
			shifted_data[i[shifted_pos]] = data[i_pixel];
		}
		public RotateShift(NativeArray<int> Source, int2 Size, int2 Shift, out NativeArray<int> Result)
		{
			shift = Shift;
			i = new(Size);
			data = Source;
			shifted_data = new(Source.Length, Allocator.TempJob);
			Result = shifted_data;
		}
	}
}