using Unity.Collections;
using static Utils.JobUtil.Utils;
namespace JobTerrainGen.Land
{
	public class LandData : TerrainData
	{
		public static DataModify<NativeHashSet<int>> LandAreaIdSetModify =
			(ref NativeHashSet<int> set) => set.Remove(0);
		public NativeArray<int> area_ids;
		// public NativeArray<int> 

		public LandData(out int a)
		{
			a = 10;
		}
	}
}