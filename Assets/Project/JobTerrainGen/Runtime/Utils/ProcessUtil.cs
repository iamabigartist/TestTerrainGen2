using System.Linq;
using Project.JobTerrainGen.Area;
using Project.JobTerrainGen.Biome;
using Project.JobTerrainGen.Biome.BiomeSelector;
using Project.JobTerrainGen.EnlargeFractal.Enlarge;
using Project.JobTerrainGen.EnlargeFractal.Samplers;
using Project.JobTerrainGen.Land;
using Project.JobTerrainGen.Noise.Samplers;
using Project.JobTerrainGen.Pipeline;
using Project.JobTerrainGen.Seed;
using Project.JobTerrainGen.Transform;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Project.JobTerrainGen.Biome.BiomeSelector.LandBiomeTableBiomeSelector;
using static Unity.Jobs.JobHandle;
using static Project.JobTerrainGen.Utils.JobUtil.NativeContainerUtils;
namespace Project.JobTerrainGen.Utils
{
	public static class ProcessUtil
	{
		public static void PlanEnlarge(NativeArray<int> data, int2 data_size, out NativeArray<int>[] Results, TerrainGenStage[] EnlargeStages, uint plan_rand_seed, ref JobHandle deps)
		{
			var stage_count = EnlargeStages.Length;
			Results = new NativeArray<int>[stage_count];
			var rand = Random.CreateFromIndex(plan_rand_seed);

			for (int i = 0; i < stage_count; i++)
			{
				var stage_rand_seed = rand.NextUInt();
				switch (EnlargeStages[i])
				{
					case NormalEnlargeStage:
						{
							JobFor<Enlarge2X2<Compare11Sampler>>.Plan(new(data, data_size, out data, new(), stage_rand_seed), ref deps);
						}
						break;
					case SawtoothEnlarge:
						{
							JobFor<Enlarge2X2<Rand11Sampler>>.Plan(new(data, data_size, out data, new(), stage_rand_seed), ref deps);
						}
						break;
					default: throw new();
				}
				data_size *= 2;
				Results[i] = data;
			}
		}
		
		public static void RunGenerateLandTerrain(
			int2 SeedSize, TerrainGenStage[] GenStages, float LandRatio, uint RandSeed,
			out NativeArray<int> LandAreaTerrain, out NativeArray<int> AreaIdArray,
			out NativeHashMap<int, bool> LandByAreaID, out NativeArray<bool> LandTerrain)
		{
			var result_size = TerrainGenStage.GetResultSize(SeedSize, GenStages);
			var jh = new JobHandle();
			JobFor<GenSeedWithAroundOcean>.Plan(new(out var seed_data, SeedSize), ref jh);
			PlanEnlarge(seed_data, SeedSize, out var area_results, GenStages, RandSeed, ref jh);
			var enlarge_result = area_results.Last();
			var shift = new int2(1, 1) * ((result_size / SeedSize).x / 2);
			JobFor<RotateShift>.Plan(new(enlarge_result, result_size, shift, out LandAreaTerrain), ref jh);
			JobFor<GenAreaIdArray>.Plan(new(LandAreaTerrain, out var area_ids_set), ref jh);
			jh.Complete();
			AreaIdSetToLandIdArray.Run(area_ids_set, out AreaIdArray);
			AreaToOceanLandRandomSelect.Run(AreaIdArray, LandRatio, RandSeed, out LandByAreaID);
			JobFor<AreaTerrainToLandTerrain>.Plan(new(LandAreaTerrain, LandByAreaID, out LandTerrain), ref jh);
			jh.Complete();
			Dispose(seed_data, area_results, area_ids_set);
		}

		public static void RunGenerateBiomeTerrain(
			int2 SeedSize, TerrainGenStage[] GenStages, NativeArray<bool> LandTerrain, uint RandSeed, float HumidityNoiseScale, float BiomeWeightPow,
			NativeArray<LandBiomeInfo> BiomeTable, NativeHashSet<int> NotLandBiomeIds,
			out NativeHashMap<int, int> AreaSeedIndexByAreaID, out NativeArray<int> BiomeAreaTerrain,
			out NativeArray<int> SeedOceanData, out NativeArray<float> SeedHumidityData, out NativeArray<float> SeedTemperatureData, out NativeArray<int> SeedBiomeData, out NativeArray<int> BiomeTerrain)
		{
			var result_size = TerrainGenStage.GetResultSize(SeedSize, GenStages);
			var main_deps = new JobHandle();
			JobFor<GenSeed>.Plan(new(out var seed_data, SeedSize.area()), ref main_deps);
			JobHandle gen_seed_dict_deps, gen_land_set_deps;
			gen_seed_dict_deps = gen_land_set_deps = main_deps;
			JobFor<GenSeedIndexById>.Plan(new(seed_data, out AreaSeedIndexByAreaID), ref gen_seed_dict_deps);
			PlanEnlarge(seed_data, SeedSize, out var area_results, GenStages, RandSeed, ref gen_land_set_deps);
			var biome_area_terrain = area_results.Last();
			JobFor<MaskAreaWithOcean>.Plan(new(biome_area_terrain, LandTerrain, out var area_data_with_ocean), ref gen_land_set_deps);
			JobFor<GenAreaIdArray>.Plan(new(area_data_with_ocean, out var land_set), ref gen_land_set_deps);
			main_deps = CombineDependencies(gen_seed_dict_deps, gen_land_set_deps);
			main_deps.Complete();
			RemoveOceanFromAreaIds.Run(land_set);
			JobHandle ocean_deps, humidity_deps, temperature_deps;
			ocean_deps = humidity_deps = temperature_deps = main_deps;
			JobFor<GenSeedOceanData>.Plan(new(seed_data, land_set, out var seed_ocean_data_0), ref ocean_deps);
			JobFor<SpreadShallowOceanFromDeepOcean>.Plan(new(seed_ocean_data_0, SeedSize, RandSeed, out var seed_ocean_data_1), ref ocean_deps);
			JobFor<SpreadShallowOceanFromDeepOcean>.Plan(new(seed_ocean_data_1, SeedSize, RandSeed, out var seed_ocean_data_2), ref ocean_deps);
			var seed_ocean_data = seed_ocean_data_2;
			JobFor<GenSeedHumidityDataByOcean<PNoiseSampler>>.Plan(new(out SeedHumidityData, SeedSize, new(SeedSize, HumidityNoiseScale, RandSeed)), ref humidity_deps);
			JobFor<GenSeedTemperatureDataByDimension>.Plan(new(out SeedTemperatureData, SeedSize), ref temperature_deps);
			main_deps = CombineDependencies(ocean_deps, humidity_deps, temperature_deps);
			JobFor<GenSeedBiomeData<LandBiomeTableBiomeSelector>>.Plan(new(new(BiomeTable, BiomeWeightPow), seed_ocean_data, SeedHumidityData, SeedTemperatureData, out SeedBiomeData, RandSeed), ref main_deps);
			JobFor<GenBiomeTerrain>.Plan(new(biome_area_terrain, AreaSeedIndexByAreaID, NotLandBiomeIds, SeedBiomeData, LandTerrain, out BiomeTerrain), ref main_deps);
			main_deps.Complete();
			BiomeAreaTerrain = new(biome_area_terrain, Allocator.TempJob);
			SeedOceanData = new(seed_ocean_data_2, Allocator.TempJob);
			Dispose(seed_data, area_results, area_data_with_ocean, land_set, seed_ocean_data_0, seed_ocean_data_1, seed_ocean_data_2);
		}
	}
}