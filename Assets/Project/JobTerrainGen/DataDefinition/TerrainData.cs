using JobTerrainGen.Pipeline;
using Unity.Collections;
using Unity.Mathematics;
namespace JobTerrainGen.DataDefinition
{
	public abstract class TerrainData
	{
		public NativeArray<int> data;
		public int2 size;

		public abstract void Generate(int2 InitSize, TerrainGenStage[] GenStages, uint RandSeed);
	}
}