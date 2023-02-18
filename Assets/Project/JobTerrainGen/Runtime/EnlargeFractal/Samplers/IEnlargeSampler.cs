using System;
using Project.JobTerrainGen.Pipeline;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;
namespace Project.JobTerrainGen.EnlargeFractal.Samplers
{
	[Serializable]
	public abstract class Enlarge2X2Stage : TerrainGenStage
	{
		protected override void ModifySize(ref int2 size) { size *= 2; }
	}
	public interface IEnlargeSampler
	{
		void Sample(
			Random rand,
			int seed00, int seed10, int seed01, int seed11,
			out int result00, out int result10, out int result01, out int result11);
	}
}