using UnityEngine;
namespace Utils
{
	public class MapData<TData>
	{
		public Vector2Int size;
		public TData[] data;
		public int IndexByPosition(int x, int y)
		{
			return x + y * size.x;
		}

		public Vector2Int PositionByIndex(int i)
		{
			int y = i / size.x;
			i -= y * size.x;
			int x = i;

			return new(x, y);
		}
	}
}