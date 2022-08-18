using Unity.Mathematics;
namespace Project.JobTerrainGen.Runtime.Biome.BiomeSelector
{
	public interface IBiomeSelector
	{
		void GetBiome(Random rand,
			float Temperature, float Humidity, int Ocean, out int ResultBiomeID);
	}
}