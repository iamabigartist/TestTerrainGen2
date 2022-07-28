using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utils;
namespace JobTerrainGen.EnlargeFractal.View
{
	[RequireComponent(typeof(TextureTester))]
	public class TestSeed2DataColor : MonoBehaviour
	{
		
		public uint rand_seed;
		public int2 seed_size;
		public EnlargeUtil.Stage[] stage_list;
		void Start()
		{
			var gen_seed_jh = GenSeedData.Plan(out var data, seed_size.area());
			var enlarge_jh = EnlargeUtil.EnlargePlan(data, seed_size, out var results, stage_list, rand_seed, gen_seed_jh);
			var gen_gray_jh = SeedDataToColor.Plan( /*data_2x2*/ results.Last(), out var color, /*enlarge1_jh*/enlarge_jh);
			gen_gray_jh.Complete();

			var result_size = seed_size * (int)math.pow(2, stage_list.Length);
			var i = new Index2D(result_size);
			color[i[1, 1]] = new(0, 0, 0);
			color[i[2, 4]] = new(0, 0, 0);
			color[i[4, 8]] = new(0, 0, 0);


			var tester = GetComponent<TextureTester>();
			tester.InitTexture(result_size);
			tester.GetTextureSlice<float3>(out var rgb, 0);

			rgb.CopyFrom(color.Slice());

			tester.ApplyTexture();

			data.Dispose();
			foreach (var native_array in results)
			{
				native_array.Dispose();
			}
			color.Dispose();

		}
	}
}