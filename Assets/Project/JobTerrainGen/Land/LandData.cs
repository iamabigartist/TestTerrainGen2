using Unity.Collections;
namespace JobTerrainGen.Land
{
	public class LandData : TerrainData
	{
		public NativeArray<int> area_ids;
		// public NativeArray<int> 

		public LandData(out int a)
		{
			a = 10;
		}
	}
}