using Unity.Jobs;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Noise.Samplers
{
	public interface INoiseSampler
	{
		float Sample(float2 pos);
	}
}