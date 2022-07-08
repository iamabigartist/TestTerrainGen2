using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Utils
{
	public static class TileMapUtil
	{
		// public static void MapDataToTileArray(MapData<float> terrain_tile_map,out Tile )
		// {
		//     
		// }
		[BurstCompile(DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance)]
		public struct MapDataToHeightTexture : IJobParallelFor
		{
			[ReadOnly] public MapData<float> TerrainTileMap;
			[WriteOnly] public NativeArray<Color> HeightTexture;
			public void Execute(int i)
			{
				float height = TerrainTileMap[i];
				HeightTexture[i] = new(height, height, height);
			}
		}
		public static void ColorArrayToTexture(this Color[] colors, int2 size, out Texture2D texture)
		{
			texture = new(size.x, size.y, TextureFormat.RGB24, 0, true);
			texture.SetPixels(colors);
		}
	}
}