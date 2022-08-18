using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Biome
{
	public struct GenSeedTemperatureDataByDimension : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_temperature_data.Length, 1024);

		Index2D i;
		NativeArray<float> seed_temperature_data;
		public void Execute(int i_seed)
		{
			var cur_dimension = 1 - i[i_seed].y / (float)i.Size.y;
			seed_temperature_data[i_seed] = cur_dimension;
		}

		public GenSeedTemperatureDataByDimension(out NativeArray<float> SeedTemperatureData, int2 Size)
		{
			i = new(Size);
			seed_temperature_data = new(Size.area(), Allocator.TempJob);

			SeedTemperatureData = seed_temperature_data;
		}
	}
}