using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Tilemaps;
namespace Project.JobTerrainGen.Utils
{
	public static class TileMapUtil
	{
		public static void DataToTileMap<TTile>(int[] Data, int2 Size, Dictionary<int, TTile> TileTable, out Vector3Int[] Positions, out TTile[] Tiles)
			where TTile : TileBase
		{
			int data_len = Data.Length;
			var i_data = new Index2D(Size);
			var positions = new Vector3Int[data_len];
			var tiles = new TTile[data_len];
			Parallel.For(0, data_len, i =>
			{
				tiles[i] = TileTable.TryGetValue(Data[i], out var tile) ? tile : null;
				var data_pos = i_data[i];
				positions[i] = new(data_pos.x, data_pos.y);
			});

			Positions = positions;
			Tiles = tiles;
		}
	}
}