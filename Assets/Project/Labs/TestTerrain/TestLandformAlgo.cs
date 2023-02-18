using System.Linq;
using Project.JobTerrainGen.Area;
using Project.JobTerrainGen.Land;
using Project.JobTerrainGen.Seed;
using Project.JobTerrainGen.Transform;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Project.JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Labs.TestTerrain
{
	public class TestLandformAlgo : TerrainDataTester
	{
		public float land_ratio;

		protected override void OnGenerate(out int2 TextureResultSize, out NativeArray<float3> ResultRGB, out float Alpha)
		{
			var jh = new JobHandle();
			JobFor<GenSeedWithAroundOcean>.Plan(new(out var seed_data, seed_size), ref jh);
			ProcessUtil.PlanEnlarge(seed_data, seed_size, out var area_results, stage_list, rand_seed, ref jh);
			var enlarge_result = area_results.Last();
			var shift = new int2(1, 1) * (EnlargeScale / 2);
			JobFor<RotateShift>.Plan(new(enlarge_result, TerrainResultSize, shift, out var area_data), ref jh);
			JobFor<GenAreaIdArray>.Plan(new(area_data, out var area_ids_set), ref jh);
			jh.Complete();
			AreaIdSetToLandIdArray.Run(area_ids_set, out var area_ids);
			AreaToOceanLandRandomSelect.Run(area_ids, land_ratio, rand_seed, out var land_by_area_id);
			JobFor<LandAreaToColor>.Plan(new(area_ids, land_by_area_id, Color.blue.f3(), out var area_colors), ref jh);
			JobFor<DataToColorByTable>.Plan(new(area_data, area_colors, out var rgb), ref jh);
			jh.Complete();
			PlanDispose(seed_data, area_results, area_data, area_ids_set, area_ids, land_by_area_id, area_colors, rgb);

			TextureResultSize = TerrainResultSize;
			ResultRGB = rgb;
			Alpha = 1f;
		}
	}
}