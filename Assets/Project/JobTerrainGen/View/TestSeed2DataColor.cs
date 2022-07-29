using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using Utils;
namespace JobTerrainGen.EnlargeFractal.View
{
	public class TestSeed2DataColor : MonoBehaviour
	{
		public int2 ResultSize;
		public TextureTester Tester;
		public uint rand_seed;
		public int2 seed_size;
		public EnlargeUtil.Stage[] stage_list;


		void OnValidate()
		{
			ResultSize = seed_size * (int)math.pow(2, stage_list.Length);
		}

		void Start()
		{
			var jh = new JobHandle();
			GenSeedData.Plan(out var data, seed_size.area(), ref jh);
			EnlargeUtil.EnlargePlan(data, seed_size, out var results, stage_list, rand_seed, ref jh);
			SeedDataToColorRand.Plan( /*data_2x2*/ results.Last(), out var color, ref /*enlarge1_jh*/ jh);
			jh.Complete();

			var result_size = seed_size * (int)math.pow(2, stage_list.Length);
			var i = new Index2D(result_size);
			color[i[1, 1]] = new(0, 0, 0);
			color[i[2, 4]] = new(0, 0, 0);
			color[i[4, 8]] = new(0, 0, 0);


			Tester.InitTexture(result_size);
			Tester.GetTextureSlice<float3>(out var rgb, 0);

			rgb.CopyFrom(color.Slice());

			Tester.ApplyTexture();

			data.Dispose();
			foreach (var native_array in results)
			{
				native_array.Dispose();
			}
			color.Dispose();

		}
	}
}