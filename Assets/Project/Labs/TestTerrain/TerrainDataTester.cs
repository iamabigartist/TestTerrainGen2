using System.Collections.Generic;
using System.Linq;
using Project.JobTerrainGen.EnlargeFractal.Samplers;
using Project.JobTerrainGen.Pipeline;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil;
using PrototypeUtils;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
namespace Labs.TestTerrain
{
	public abstract class TerrainDataTester : MonoBehaviour
	{
	#region View

		[Tooltip("Current result texture size after apply all the terrain process")]
		public int2 CurrentTerrainResultSize;

	#endregion

	#region Reference

		public TextureTester Tester;

	#endregion

	#region Config

		public uint rand_seed;
		public int2 seed_size;

	#endregion

	#region Property

		public int2 TerrainResultSize => TerrainGenStage.GetResultSize(seed_size, stage_list);
		public int EnlargeScale => (TerrainResultSize / seed_size).x; //Assume the ratio doesn't change

	#endregion

	#region Data

		[SerializeReference] [PolymorphicSelect]
		public TerrainGenStage[] stage_list =
		{
			new NormalEnlargeStage(),
			new SawtoothEnlarge(),
			new NormalEnlargeStage(),
			new NormalEnlargeStage(),
			new NormalEnlargeStage()
		};

	#endregion

	#region Template

		protected abstract void OnGenerate(out int2 TextureResultSize, out NativeArray<float3> ResultRGB, out float Alpha);

	#endregion

	#region Process

		void CleanGenerate()
		{
			Dispose();
		}

	#region Dispose

		List<object> dispose_list = new();

		void Dispose()
		{
			NativeContainerUtils.Dispose(dispose_list);
			dispose_list.Clear();
		}

		protected void PlanDispose(params object[] disposables)
		{
			dispose_list.AddRange(disposables);
		}

	#endregion

		void ApplyResultToTexture(int2 TextureResultSize, NativeSlice<float3> ResultRGB, float Alpha)
		{
			Tester.InitTexture(TextureResultSize);

			Tester.GetTextureSlice<float3>(out var rgb_slice, 0);
			rgb_slice.CopyFrom(ResultRGB);

			Tester.GetTextureSlice<float>(out var alpha_slice, 3);
			alpha_slice.CopyFrom(Enumerable.Repeat(Alpha, alpha_slice.Length).ToArray());

			Tester.ApplyTexture();
		}

	#endregion

	#region UnityEntry

		protected void OnValidate()
		{
			CurrentTerrainResultSize = TerrainResultSize;
		}

		protected void Start() { Run(); }
		protected void Reset() { CleanGenerate(); }
		protected void OnDestroy() { CleanGenerate(); }

		[ContextMenu("Run")]
		void Run()
		{
			OnGenerate(out var texture_result_size, out var result_rgb, out var alpha);
			ApplyResultToTexture(texture_result_size, result_rgb, alpha);
			CleanGenerate();
		}

		[ContextMenu("RandRun")]
		void RandRun()
		{
			rand_seed = Random.CreateFromIndex(rand_seed).NextUInt();
			Run();
		}

	#endregion
	}
}