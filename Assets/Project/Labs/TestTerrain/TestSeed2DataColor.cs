using System.Linq;
using JobTerrainGen.EnlargeFractal.Samplers;
using JobTerrainGen.EnlargeFractal.Seed;
using JobTerrainGen.Pipeline;
using JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Utils.JobUtil.Template;
using static JobTerrainGen.Util.PlaneUtil;
namespace Labs.TestTerrain
{
	public class TestSeed2DataColor : TerrainDataTester
	{
		public override int enlarge_count => stage_list.Length;

		[SerializeReference]
		[SerializeReferenceButton]
		public TerrainGenStage[] stage_list =
		{
			new NormalEnlarge(),
			new SawtoothEnlarge(),
			new NormalEnlarge(),
			new NormalEnlarge(),
			new NormalEnlarge()
		};

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
			JobFor<GenSeed>.Plan(new(out var data, seed_size.area()), ref jh);
			EnlargePlan(data, seed_size, out var results, stage_list.ToArray(), rand_seed, ref jh);
			JobFor<SeedDataToColorRand>.Plan(new(results.Last(), out var result_color), ref jh);
			jh.Complete();
			MarkCoordinate(result_color);
			ApplyResultToTexture(result_color.Slice(), 0);
			PlanDispose(data, result_color, results);
		}
	}
}