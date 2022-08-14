using Unity.Mathematics;
namespace JobTerrainGen.Noise.Samplers
{
	public interface INoiseSampler
	{
		float Sample(float2 pos);
	}
}