using Unity.Collections;
using Unity.Mathematics;
using Utils;
namespace MyTerrainGen.Noise.EnlargeFractal.Samplers
{
	public struct Compare11Sampler : IEnlargeSampler
	{
		static void AddOptionSeed(ref NativeList<int> seed_array, ref NativeList<int> count_array, int seed)
		{
			var seed_i = seed_array.IndexOf(seed);
			if (seed_i == -1)
			{
				seed_array.Add(seed);
				count_array.Add(1);
			}
			else
			{
				count_array[seed_i]++;
			}
		}
		public void Sample(Random rand, int seed00, int seed10, int seed01, int seed11, out int result00, out int result10, out int result01, out int result11)
		{
			result00 = seed00;
			result10 = rand.Select2(seed00, seed10);
			result01 = rand.Select2(seed00, seed01);

		#region Compare to select sample11

			var seed_array = new NativeList<int>(4, Allocator.Temp);
			var count_array = new NativeList<int>(4, Allocator.Temp);
			AddOptionSeed(ref seed_array, ref count_array, seed00);
			AddOptionSeed(ref seed_array, ref count_array, seed10);
			AddOptionSeed(ref seed_array, ref count_array, seed01);
			AddOptionSeed(ref seed_array, ref count_array, seed11);
			switch (seed_array.Length)
			{
				case 1:
					{
						result11 = seed00;
						break;
					}
				case 2:
					{
						int a = count_array[0];
						int b = count_array[1];
						if (a > b) { result11 = a; }
						else if (a < b) { result11 = b; }
						else { result11 = rand.Select2(a, b); }
						break;
					}
				case 3:
					{
						if (count_array[0] == 2) { result11 = count_array[0]; }
						else if (count_array[1] == 2) { result11 = count_array[1]; }
						else { result11 = count_array[2]; }
						break;
					}
				case 4:
					{
						result11 = rand.Select4(seed00, seed10, seed01, seed11);
						break;
					}
				default:
					{
						result11 = default;
						break;
					}
			}

		#endregion

		}
	}
}