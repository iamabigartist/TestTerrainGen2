using Unity.Collections;
namespace Project.JobTerrainGen.Land
{
	public static class AreaIdSetToLandIdArray
	{
		public static void Run(NativeHashSet<int> id_set, out NativeArray<int> id_array)
		{
			id_set.Remove(0);
			id_array = id_set.ToNativeArray(Allocator.TempJob);
		}
	}
}