using System.Linq;
using JobTerrainGen.EnlargeFractal;
using JobTerrainGen.EnlargeFractal.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.View
{
	public class TestSeed2DataColor : TerrainDataTester
	{
		public EnlargeUtil.Stage[] stage_list;
		protected override int enlarge_count => stage_list.Length;

		void MarkCoordinate(NativeArray<float3> result_color)
		{
			var i = new Index2D(ResultSize);
			result_color[i[1, 1]] = new(0, 0, 0);
			result_color[i[2, 4]] = new(0, 0, 0);
			result_color[i[4, 8]] = new(0, 0, 0);
		}
		
		void Start()
		{
			var jh = new JobHandle();
			GenSeedData.Plan(out var data, seed_size.area(), ref jh);
			EnlargeUtil.EnlargePlan(data, seed_size, out var results, stage_list, rand_seed, ref jh);
			SeedDataToColorRand.Plan(results.Last(), out var result_color, ref jh);
			jh.Complete();

			MarkCoordinate(result_color);

			ApplyResultToTexture(result_color.Slice(), 0);

			PlanDispose(data, result_color, results);
		}
	}
}