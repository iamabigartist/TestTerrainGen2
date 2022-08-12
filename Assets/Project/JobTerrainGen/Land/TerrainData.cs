using JobTerrainGen.Pipeline;
using Unity.Collections;
using Unity.Mathematics;
namespace JobTerrainGen.Land
{
	public abstract class TerrainData
	{
		public int2 init_size;
		public TerrainGenStage[] stage_list;
		public NativeArray<int> data;
		public int2 size;
	}
}