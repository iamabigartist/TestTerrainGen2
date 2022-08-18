using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
namespace Project.JobTerrainGen.Utils.JobUtil
{
	public static class NativeContainerUtils
	{
		public static void Dispose(params object[] disposables)
		{
			var dispose_list = new List<IDisposable>();
			foreach (var obj in disposables)
			{
				switch (obj)
				{
					case Array array:
						{
							foreach (var disposable in array.Cast<IDisposable>())
							{
								disposable.Dispose();
							}
							break;
						}
					case IDisposable disposable:
						disposable.Dispose();
						break;
				}
			}
		}

		public static NativeHashSet<T> ToNativeHashSet<T>(this T[] source, Allocator Allocator) where T : unmanaged, IEquatable<T>
		{
			var hash_set = new NativeHashSet<T>(source.Length, Allocator);
			for (int i = 0; i < source.Length; i++)
			{
				hash_set.Add(source[i]);
			}
			return hash_set;
		}

		public static NativeHashMap<TKey, TValue> ToNativeHashMap<TKey, TValue>(this Dictionary<TKey, TValue> source, Allocator Allocator)
			where TKey : struct, IEquatable<TKey>
			where TValue : struct
		{
			var hash_map = new NativeHashMap<TKey, TValue>(source.Count, Allocator);
			foreach ((TKey key, TValue value) in source)
			{
				hash_map.Add(key, value);
			}
			return hash_map;
		}

		public static NativeArray<T> ToNativeArray<T>(this T[] source, Allocator Allocator) where T : struct
		{
			var array = new NativeArray<T>(source.Length, Allocator);
			for (int i = 0; i < source.Length; i++)
			{
				array[i] = source[i];
			}
			return array;
		}
	}
}