using System.Linq;
using JobTerrainGen.EnlargeFractal;
using JobTerrainGen.EnlargeFractal.Area;
using JobTerrainGen.Land;
using JobTerrainGen.Pipeline;
using JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using static JobTerrainGen.Pipeline.TerrainGenStage;
using static JobTerrainGen.Util.PlaneUtil;
namespace Labs.TestTerrain
{
	public class TestLandformAlgo : TerrainDataTester
	{
		public float land_ratio;

		protected override int enlarge_count => enlarge_stages.Length;

		public TerrainGenStage[] enlarge_stages =
		{
			NormalEnlarge,
			SawtoothEnlarge,
			NormalEnlarge,
			NormalEnlarge,
			NormalEnlarge
		};

		[ContextMenu("Run")]
		protected override void Run()
		{
			var jh = new JobHandle();
			GenSeedDataWithAroundOcean.Plan(out var seed_data, seed_size, ref jh);
			// GenSeedData.Plan(out var seed_data, seed_size.area(), ref jh);
			EnlargePlan(seed_data, seed_size, out var area_results, enlarge_stages, rand_seed, ref jh);
			var enlarge_result = area_results.Last();
			var shift = new int2(1, 1) * (EnlargeScale / 2);
			RotateShift.Plan(enlarge_result, ResultSize, shift, out var area_data, ref jh);
			GenAreaIdArray.Plan(area_data, out var gen_area_ids, ref jh);
			jh.Complete();
			gen_area_ids(out var area_ids);
			AreaToOceanLandRandomSelect.Run(area_ids, land_ratio, rand_seed, out var area_landforms);
			GenOceanLandAreaColor.Plan(area_ids, area_landforms, Color.blue.f3(), out var area_colors, ref jh);
			DataToColorByTable.Plan(area_data, area_colors, out var rgb, ref jh);
			jh.Complete();
			ApplyResultToTexture(rgb.Slice(), 0);
			PlanDispose(seed_data, area_results, area_data, area_ids, area_landforms, area_colors, rgb);
		}
	}
}