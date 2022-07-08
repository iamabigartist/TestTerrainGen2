using UnityEngine;
namespace Labs
{
	public class TestGenTerrain : MonoBehaviour
	{
		Terrain mTerrainComponent;
		TerrainData mTerrainData;
		void OnEnable()
		{
			mTerrainData = new();
			mTerrainComponent = Terrain.CreateTerrainGameObject(mTerrainData).GetComponent<Terrain>();
		}
	}
}