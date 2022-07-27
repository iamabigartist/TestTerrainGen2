using MyTerrainGen.Noise.EnlargeFractal.Samplers;
using Unity.Mathematics;
using UnityEngine;
using Utils;
namespace MyTerrainGen.Noise.EnlargeFractal.View
{
	[RequireComponent(typeof(TextureTester))]
	public class TestSeed2DataColor : MonoBehaviour
	{
		public uint rand_seed;
		int2 size;
		void Start()
		{
			var tester = GetComponent<TextureTester>();
			size = tester.TextureSize / 32;
			tester.GetTextureSlice<float>(out var red, 0);
			tester.GetTextureSlice<float>(out var green, 1);
			tester.GetTextureSlice<float>(out var blue, 2);
			var gen_seed_jh = GenSeedData.Plan(out var data, size.area(), rand_seed);
			var enlarge2_jh = Enlarge2X2<Compare11Sampler>.Plan(data, size, out var data_2, new(), rand_seed, gen_seed_jh);
			var enlarge4_jh = Enlarge2X2<Compare11Sampler>.Plan(data_2, size * 2, out var data_4, new(), rand_seed, enlarge2_jh);
			var enlarge8_jh = Enlarge2X2<Compare11Sampler>.Plan(data_4, size * 4, out var data_8, new(), rand_seed, enlarge4_jh);
			var enlarge16_jh = Enlarge2X2<Compare11Sampler>.Plan(data_8, size * 8, out var data_16, new(), rand_seed, enlarge8_jh);
			var enlarge32_jh = Enlarge2X2<Compare11Sampler>.Plan(data_16, size * 16, out var data_32, new(), rand_seed, enlarge16_jh);
			var gen_gray_jh = SeedDataToGray.Plan( /*data_2x2*/ data_32, out var gray, /*enlarge1_jh*/enlarge32_jh);
			gen_gray_jh.Complete();

			red.CopyFrom(gray);
			green.CopyFrom(red);
			blue.CopyFrom(red);

			tester.ApplyTexture();

			data.Dispose();
			data_2.Dispose();
			data_4.Dispose();
			data_8.Dispose();
			data_16.Dispose();
			data_32.Dispose();
			gray.Dispose();

		}
	}
}