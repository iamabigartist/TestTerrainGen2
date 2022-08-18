using System;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Utils
{
	public struct MapData<TData> : IDisposable
		where TData : struct
	{
		public int2 size;
		public NativeArray<TData> data;

	#region Indexer

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int IndexByPosition(int x, int y)
		{
			return x + y * size.x;
		}

		public TData this[int x, int y]
		{
			get => data[IndexByPosition(x, y)];
			set => data[IndexByPosition(x, y)] = value;
		}

		public TData this[int i]
		{
			get => data[i];
			set => data[i] = value;
		}

	#endregion

	#region Entry

		public MapData(int2 size, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			data = new(size.x * size.y, allocator, options);
			this.size = size;
		}

		public void Dispose()
		{
			data.Dispose();
		}

	#endregion

	#region Util

		public int2 PositionByIndex(int i)
		{
			int y = i / size.x;
			i -= y * size.x;
			int x = i;

			return new(x, y);
		}

	#endregion

	}
}