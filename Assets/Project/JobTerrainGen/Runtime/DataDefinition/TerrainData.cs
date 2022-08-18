using Project.JobTerrainGen.Runtime.Pipeline;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Runtime.DataDefinition
{
	public abstract class TerrainData
	{
		public NativeArray<int> data;
		public int2 size;

		public abstract void Generate(int2 InitSize, TerrainGenStage[] GenStages, uint RandSeed);
	}
}