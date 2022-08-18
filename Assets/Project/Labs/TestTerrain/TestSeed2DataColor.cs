using System.Linq;
using Project.JobTerrainGen.Seed;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Project.JobTerrainGen.View;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace Labs.TestTerrain
{
	public class TestSeed2DataColor : TerrainDataTester
	{
		void MarkCoordinate(NativeArray<float3> result_color)
		{
			var i = new Index2D(TerrainResultSize);
			result_color[i[1, 1]] = new(0, 0, 0);
			result_color[i[2, 4]] = new(0, 0, 0);
			result_color[i[4, 8]] = new(0, 0, 0);
		}

		protected override void OnGenerate(out int2 TextureResultSize, out NativeArray<float3> ResultRGB, out float Alpha)
		{
			var jh = new JobHandle();
			JobFor<GenSeed>.Plan(new(out var data, seed_size.area()), ref jh);
			ProcessUtil.PlanEnlarge(data, seed_size, out var results, stage_list.ToArray(), rand_seed, ref jh);
			JobFor<SeedDataToColorRand>.Plan(new(results.Last(), out var result_color), ref jh);
			jh.Complete();
			MarkCoordinate(result_color);
			PlanDispose(data, result_color, results);

			TextureResultSize = TerrainResultSize;
			ResultRGB = result_color;
			Alpha = 1;
		}
	}
}