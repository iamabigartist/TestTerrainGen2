using System;
using System.Linq;
using Project.JobTerrainGen.Pipeline;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Project.JobTerrainGen.View;
using PrototypeUtils;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
using static Project.JobTerrainGen.Biome.BiomeSelector.LandBiomeTableBiomeSelector;
using static Project.JobTerrainGen.Utils.ProcessUtil;
using static Unity.Collections.Allocator;
using static Project.JobTerrainGen.Utils.JobUtil.NativeContainerUtils;
using static Project.JobTerrainGen.Utils.TileMapUtil;
using Random = Unity.Mathematics.Random;
namespace Labs.TestTerrain
{
	public class TestBiomeTerrain : MonoBehaviour
	{
		[Serializable]
		public struct IdColorUnit
		{
			public int id;
			public Color color;
			public IdColorUnit(int ID, Color Color)
			{
				id = ID;
				color = Color;
			}
		}

		[Serializable]
		public struct IdTileUnit
		{
			public int id;
			public Tile tile;
			public IdTileUnit(int ID, Tile Tile)
			{
				id = ID;
				tile = Tile;
			}
		}


	#region Reference

		public Tilemap MyTileMap;
		public TextureTester Tester;

	#endregion

	#region Config

		public uint RandSeed;
		public int2 LandSeedSize;
		public float LandRatio;
		public float HumidityNoiseScale;
		public float BiomeWeightPow;

		[SerializeReference] [PolymorphicSelect]
		public TerrainGenStage[] LandTerrainGenStage;
		public LandBiomeInfo[] BiomeTable;

		public IdColorUnit[] BiomeColors;
		public IdTileUnit[] BiomeTiles;

	#endregion

	#region TempData
		
		NativeArray<LandBiomeInfo> biome_table;
		NativeHashMap<int, float3> biome_colors;
		NativeHashSet<int> NotLandBiomeId;

	#endregion

	#region Property

		int2 BiomeSeedSize => LandSeedSize * 16;
		int2 ResultSize => TerrainGenStage.GetResultSize(LandSeedSize, LandTerrainGenStage);
		TerrainGenStage[] BiomeTerrainGenStage => LandTerrainGenStage[..^4];

	#endregion

	#region Process

		void SetTileMap(NativeArray<int> BiomeTerrain)
		{
			DataToTileMap(
				BiomeTerrain.ToArray(), ResultSize,
				BiomeTiles.ToDictionary(u => u.id, u => u.tile),
				out var positions, out var tiles);
			MyTileMap.SetTiles(positions, tiles);
			MyTileMap.ResizeBounds();
		}

	#region Run

		void RunInit()
		{
			biome_table = BiomeTable.ToNativeArray(Persistent);
			biome_colors = BiomeColors.ToDictionary(u => u.id, u => u.color.f3()).ToNativeHashMap(Persistent);
		}

		void RunTerminate()
		{
			biome_table.Dispose();
			biome_colors.Dispose();
		}

		void RunProcess()
		{
			RunGenerateLandTerrain(LandSeedSize, LandTerrainGenStage, LandRatio, RandSeed,
				out var land_area_terrain, out var area_id_array, out var land_by_area_id, out var land_terrain);
			RunGenerateBiomeTerrain(BiomeSeedSize, BiomeTerrainGenStage, land_terrain, RandSeed, HumidityNoiseScale, BiomeWeightPow, biome_table, NotLandBiomeId,
				out var area_seed_index_by_area_id, out var biome_area_terrain, out var seed_ocean_data, out var seed_humidity_data, out var seed_temperature_data, out var seed_biome_data, out var biome_terrain);
			var deps = new JobHandle();


			JobFor<DataToColorByTable>.Plan(new(biome_terrain, biome_colors, out var result_color), ref deps);
			deps.Complete();
			Tester.InitTexture(ResultSize);
			Tester.SetTextureSlice(result_color, 0);

			// Tester.InitTexture(BiomeSeedSize);
			// Tester.SetTextureSlice(seed_humidity_data, 0);
			// Tester.SetTextureSlice(seed_humidity_data, 1);
			// Tester.SetTextureSlice(seed_humidity_data, 2);
			// Tester.SetTextureSlice(seed_temperature_data, 0);
			// Tester.SetTextureSlice(seed_temperature_data, 1);
			// Tester.SetTextureSlice(seed_temperature_data, 2);
			Tester.ApplyTexture();

			SetTileMap(biome_terrain);

			Dispose(land_area_terrain, area_id_array, land_by_area_id, land_terrain, area_seed_index_by_area_id, biome_area_terrain, seed_ocean_data, seed_humidity_data, seed_temperature_data, seed_biome_data, biome_terrain, result_color);
		}

	#endregion

	#endregion

	#region UnityEntry

		void Start()
		{
			NotLandBiomeId = new(3, Persistent) { 1, 2, 3 };

			Run();
		}

		void OnDestroy()
		{
			NotLandBiomeId.Dispose();
		}
		
		[ContextMenu("RandRun")]
		void RandRun()
		{
			RandSeed = Random.CreateFromIndex(RandSeed).NextUInt();
			Run();
		}

		[ContextMenu("Run")]
		void Run()
		{
			RunInit();
			RunProcess();
			RunTerminate();
		}

	#endregion
		

		


		
	}
}