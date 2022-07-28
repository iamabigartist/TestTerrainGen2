using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.EnlargeFractal.Samplers
{
	public struct Rand11Sampler : IEnlargeSampler
	{
		public void Sample(Random rand, int seed00, int seed10, int seed01, int seed11, out int result00, out int result10, out int result01, out int result11)
		{
			result00 = seed00;
			result10 = rand.Select2(seed00, seed10);
			result01 = rand.Select2(seed00, seed01);
			result11 = rand.Select4(seed00, seed10, seed01, seed11);
		}
	}
}