using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.View
{
	public struct DataToColorByTable : IJobForRunner
	{
		
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (data.Length, 1024);

		[ReadOnly] NativeHashMap<int, float3> color_table;
		NativeArray<int> data;
		NativeArray<float3> color;
		public void Execute(int i)
		{
			color[i] = color_table[data[i]];
		}

		public DataToColorByTable(NativeArray<int> Data, NativeHashMap<int, float3> ColorTable, out NativeArray<float3> ResultColor)
		{
			color_table = ColorTable;
			data = Data;
			color = new(data.Length, Allocator.TempJob);

			ResultColor = color;
		}
	}
}