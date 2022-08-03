using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Utils;
using Random = Unity.Mathematics.Random;
namespace JobTerrainGen.View
{
	public abstract class TerrainDataTester : MonoBehaviour
	{

	#region Property

		[Tooltip("Current result texture size after apply all the terrain process")]
		public int2 CurrentResultSize;
		public int EnlargeScale => (int)math.pow(2, enlarge_count);
		public int2 ResultSize => seed_size * EnlargeScale;

	#endregion

	#region Reference

		public TextureTester Tester;

	#endregion

	#region Config

		public uint rand_seed;
		public int2 seed_size;

	#endregion
	#region Data

		List<IDisposable> dispose_list = new();

	#endregion

	#region Template

		protected abstract int enlarge_count { get; }
		protected abstract void Run();

	#endregion

	#region Process

		void Dispose()
		{
			foreach (var disposable in dispose_list)
			{
				disposable.Dispose();
			}
			dispose_list.Clear();
		}

		protected void PlanDispose(params object[] disposables)
		{
			foreach (var obj in disposables)
			{
				if (obj is Array array)
				{
					dispose_list.AddRange(array.Cast<IDisposable>().ToArray());
				}
				else
				{
					dispose_list.Add((IDisposable)obj);
				}
			}
		}

		protected void ApplyResultToTexture<TStride>(NativeSlice<TStride> result_color, int float_offset_count) where TStride : struct
		{
			Tester.InitTexture(ResultSize);
			Tester.GetTextureSlice<TStride>(out var texture_slice, float_offset_count);
			texture_slice.CopyFrom(result_color);
			Tester.ApplyTexture();
		}

	#endregion

	#region UnityEntry

		[ContextMenu("RandRun")]
		void RandRun()
		{
			rand_seed = Random.CreateFromIndex(rand_seed).NextUInt();
			Run();
		}

		protected void Start()
		{
			Run();
		}

		protected void OnValidate()
		{
			CurrentResultSize = ResultSize;
		}

		protected void OnDestroy()
		{
			Dispose();
		}

	#endregion
	}
}