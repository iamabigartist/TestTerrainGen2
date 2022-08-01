using System.Linq;
using JobTerrainGen.EnlargeFractal;
using JobTerrainGen.EnlargeFractal.Area;
using JobTerrainGen.Landform;
using JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Utils;
using static JobTerrainGen.EnlargeFractal.EnlargeUtil;
using static JobTerrainGen.EnlargeFractal.EnlargeUtil.Stage;
namespace Labs.TestTerrain
{
	public class TestLandformAlgo : TerrainDataTester
	{
		public float land_ratio;

		protected override int enlarge_count => enlarge_stages.Length;

		public Stage[] enlarge_stages =
		{
			Normal,
			Sawtooth,
			Normal,
			Normal,
			Normal
		};

		[ContextMenu("Run")]
		protected override void Run()
		{
			var jh = new JobHandle();
			GenSeedData.Plan(out var seed_data, seed_size.area(), ref jh);
			EnlargePlan(seed_data, seed_size, out var area_results, enlarge_stages, rand_seed, ref jh);
			var area_data = area_results.Last();
			GenAreaIdArray.Plan(area_data, out var gen_area_ids, ref jh);
			jh.Complete();
			gen_area_ids(out var area_ids);
			AreaToOceanLandRandom.Plan(area_ids, land_ratio, rand_seed, out var area_landforms, ref jh);
			GenOceanLandAreaColor.Plan(area_ids, area_landforms, Color.blue.f3(), out var area_colors, ref jh);
			DataToColorByTable.Plan(area_data, area_colors, out var rgb, ref jh);
			jh.Complete();
			ApplyResultToTexture(rgb.Slice(), 0);
			PlanDispose(seed_data, area_results, area_ids, area_landforms, area_colors, rgb);
		}
	}
}