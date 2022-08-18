using Unity.Mathematics;
namespace Project.JobTerrainGen.Runtime.Noise.Samplers
{
	public interface INoiseSampler
	{
		float Sample(float2 pos);
	}
}