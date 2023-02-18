using Unity.Mathematics;
namespace Project.JobTerrainGen.Noise.Samplers
{
	public struct PNoiseSampler : INoiseSampler
	{
		SamplerTransformer transformer;
		float2 rep;

		public float Sample(float2 pos) => noise.pnoise(transformer.Transform(pos), rep);

		public PNoiseSampler(SamplerTransformer Transformer, float2 Rep)
		{
			transformer = Transformer;
			rep = Rep;
		}

		public PNoiseSampler(int2 SampleSize, float NoiseScale, uint OffsetRandSeed)
		{
			var rand = Random.CreateFromIndex(OffsetRandSeed);
			transformer = new(rand.NextFloat2(), new float2(1, 1) * NoiseScale);
			rep = (float2)SampleSize * NoiseScale * 2;
		}
	}
}