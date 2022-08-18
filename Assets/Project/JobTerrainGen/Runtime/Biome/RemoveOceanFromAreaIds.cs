using Unity.Collections;
namespace Project.JobTerrainGen.Biome
{
	public static class RemoveOceanFromAreaIds
	{
		public static void Run(NativeHashSet<int> id_set) { id_set.Remove(0); }
	}
}