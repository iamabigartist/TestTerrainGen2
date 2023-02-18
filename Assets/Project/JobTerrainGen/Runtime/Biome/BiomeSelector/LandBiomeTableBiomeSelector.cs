using System;
using Project.JobTerrainGen.Utils;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using Random = Unity.Mathematics.Random;
namespace Project.JobTerrainGen.Biome.BiomeSelector
{

	public struct LandBiomeTableBiomeSelector : IBiomeSelector
	{
		[Serializable]
		public struct LandBiomeInfo
		{
			public int id;
			public float humidity;
			public float temperature;
			public LandBiomeInfo(int ID, float Humidity, float Temperature)
			{
				id = ID;
				humidity = Humidity;
				temperature = Temperature;
			}
		}

		float max_index_distance;
		float weight_pow;
		NativeArray<LandBiomeInfo> biome_table;

		public void GetBiome(Random rand, float Temperature, float Humidity, int Ocean, out int ResultBiomeID)
		{
			if (Ocean != 0) { ResultBiomeID = Ocean; } //Only control land
			else
			{
				/// dry,hot 0,1		wet,hot 1,1
				/// dry,cold 0,0	wet,cold 1,0
				var cur_seed_position = new float2(Humidity, Temperature);
				var biome_weights = new NativeArray<float>(biome_table.Length, Allocator.Temp);
				for (int i = 0; i < biome_table.Length; i++)
				{
					var table_biome_info = biome_table[i];
					var table_biome_position = new float2(table_biome_info.humidity, table_biome_info.temperature);
					var position_weight = max_index_distance - distance(cur_seed_position, table_biome_position);
					var table_biome_weight = pow(position_weight, weight_pow);
					biome_weights[i] = table_biome_weight;
				}
				var result_index = rand.SelectWithProbability(biome_weights);
				ResultBiomeID = biome_table[result_index].id;
			}
		}

		public LandBiomeTableBiomeSelector(NativeArray<LandBiomeInfo> BiomeTable, float WeightPow)
		{
			max_index_distance = pow(2, 0.5f);
			weight_pow = WeightPow;
			biome_table = BiomeTable;
		}
	}
}