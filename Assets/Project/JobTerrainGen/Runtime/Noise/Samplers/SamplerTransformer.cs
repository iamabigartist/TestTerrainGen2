using Unity.Mathematics;
namespace Project.JobTerrainGen.Noise.Samplers
{
	public struct SamplerTransformer
	{
		float2 offset;
		float2 scale;
		public float2 Transform(float2 pos) => pos * scale + offset;

		public SamplerTransformer(float2 Offset, float2 Scale)
		{
			offset = Offset;
			scale = Scale;
		}
	}
}