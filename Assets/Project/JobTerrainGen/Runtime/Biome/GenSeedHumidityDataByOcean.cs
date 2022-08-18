using Project.JobTerrainGen.Noise.Samplers;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Biome
{
	public struct GenSeedHumidityDataByOcean<TNoiseSampler> : IJobForRunner
		where TNoiseSampler : struct, INoiseSampler
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_humidity_data.Length, 1024);

		TNoiseSampler sampler;
		Index2D i;
		NativeArray<float> seed_humidity_data;
		public void Execute(int i_seed)
		{
			var pos = i[i_seed];
			var result = sampler.Sample(pos);
			seed_humidity_data[i_seed] = result;
		}

		public GenSeedHumidityDataByOcean(out NativeArray<float> SeedHumidityData, int2 Size, TNoiseSampler Sampler)
		{
			sampler = Sampler;
			i = new(Size);
			seed_humidity_data = new(Size.area(), Allocator.TempJob);

			SeedHumidityData = seed_humidity_data;
		}
	}
}