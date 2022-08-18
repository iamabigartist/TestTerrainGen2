using Project.JobTerrainGen.Biome.BiomeSelector;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
namespace Project.JobTerrainGen.Biome
{
	public struct GenSeedBiomeData<TBiomeSelector> : IJobForRunner
		where TBiomeSelector : struct, IBiomeSelector
	{

		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (seed_biome_data.Length, 1);

		IndexRandGenerator rand_gen;
		TBiomeSelector selector;
		NativeArray<int> seed_ocean_data;
		NativeArray<float> seed_humidity_data;
		NativeArray<float> seed_temperature_data;
		NativeArray<int> seed_biome_data;

		public void Execute(int i_seed)
		{
			var cur_ocean = seed_ocean_data[i_seed];
			var cur_humidity = seed_humidity_data[i_seed];
			var cur_temperature = seed_temperature_data[i_seed];
			rand_gen.Gen(i_seed, out var rand);
			selector.GetBiome(rand, cur_temperature, cur_humidity, cur_ocean, out var result_biome_id);
			seed_biome_data[i_seed] = result_biome_id;
		}

		public GenSeedBiomeData(TBiomeSelector Selector, NativeArray<int> SeedOceanData, NativeArray<float> SeedHumidityData, NativeArray<float> SeedTemperatureData, out NativeArray<int> SeedBiomeData, uint RandSeed)
		{
			rand_gen = new(RandSeed);
			selector = Selector;
			seed_ocean_data = SeedOceanData;
			seed_humidity_data = SeedHumidityData;
			seed_temperature_data = SeedTemperatureData;
			seed_biome_data = new(SeedOceanData.Length, Allocator.TempJob);

			SeedBiomeData = seed_biome_data;
		}
	}
}