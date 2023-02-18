using System;
using Project.JobTerrainGen.Utils;
using Unity.Collections;
using Random = Unity.Mathematics.Random;
namespace Project.JobTerrainGen.EnlargeFractal.Samplers
{
	[Serializable]
	public class NormalEnlargeStage : Enlarge2X2Stage {}

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
						int a = seed_array[0];
						int b = seed_array[1];
						int n_a = count_array[0];
						int n_b = count_array[1];
						if (n_a > n_b) { result11 = a; }
						else if (n_a < n_b) { result11 = b; }
						else { result11 = rand.Select2(a, b); }
						break;
					}
				case 3:
					{
						if (count_array[0] == 2) { result11 = seed_array[0]; }
						else if (count_array[1] == 2) { result11 = seed_array[1]; }
						else { result11 = seed_array[2]; }
						break;
					}
				case 4:
					{
						result11 = rand.SelectArray(seed_array.AsArray());
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