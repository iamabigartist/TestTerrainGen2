using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Utils
{
	public static class TileMapUtil
	{
		public static void DataToTileMap(int[] Data, int2 Size, Dictionary<int, Tile> TileTable, out Vector3Int[] Positions, out Tile[] Tiles)
		{
			int data_len = Data.Length;
			var i_data = new Index2D(Size);
			var positions = new Vector3Int[data_len];
			var tiles = new Tile[data_len];
			Parallel.For(0, data_len, i =>
			{
				tiles[i] = TileTable[Data[i]];
				var data_pos = i_data[i];
				positions[i] = new(data_pos.x, data_pos.y);
			});

			Positions = positions;
			Tiles = tiles;
		}
	}
}