using System.Linq;
using JobTerrainGen.EnlargeFractal;
using JobTerrainGen.Pipeline;
using JobTerrainGen.Util;
using JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utils;
namespace Labs.TestTerrain
{
	public class TestSeed2DataColor : TerrainDataTester
	{
		protected override int enlarge_count => stage_list.Length;

		public TerrainGenStage[] stage_list;
		void MarkCoordinate(NativeArray<float3> result_color)
		{
			var i = new Index2D(ResultSize);
			result_color[i[1, 1]] = new(0, 0, 0);
			result_color[i[2, 4]] = new(0, 0, 0);
			result_color[i[4, 8]] = new(0, 0, 0);
		}

		[ContextMenu("Run")]
		protected override void Run()
		{
			var jh = new JobHandle();
			GenSeedData.Plan(out var data, seed_size.area(), ref jh);
			PlaneUtil.EnlargePlan(data, seed_size, out var results, stage_list, rand_seed, ref jh);
			SeedDataToColorRand.Plan(results.Last(), out var result_color, ref jh);
			jh.Complete();
			MarkCoordinate(result_color);
			ApplyResultToTexture(result_color.Slice(), 0);
			PlanDispose(data, result_color, results);
		}
	}
}