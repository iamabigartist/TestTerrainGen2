using Unity.Mathematics;
namespace JobTerrainGen.EnlargeFractal.Samplers
{
	public interface IEnlargeSampler
	{
		void Sample(
			Random rand,
			int seed00, int seed10, int seed01, int seed11,
			out int result00, out int result10, out int result01, out int result11);
	}
}