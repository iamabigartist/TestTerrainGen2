using Project.JobTerrainGen.EnlargeFractal.Samplers;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Pipeline.Nodes
{
	public class Enlarge2X2Stage<TEnlargeSampler> : PipelineStage
		where TEnlargeSampler : IEnlargeSampler
	{
		public PipelineNodePort data_2x2;
		public Enlarge2X2Stage(PipelineNodePort data, int2 size, TEnlargeSampler sampler, uint rand_seed) {}
	}
}