using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
namespace Labs
{
	public struct TestPerlinNoise : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (result.Length, 1024);
		float2 offset;
		float2 scale;
		float2 repetition;
		Index2D i;
		NativeArray<float> result;
		public void Execute(int i_seed)
		{
			float2 ori_pos = i[i_seed];
			var pos = scale * ori_pos + offset;
			var sample_result = noise.pnoise(pos, repetition);
			result[i_seed] = sample_result;
		}
		public TestPerlinNoise(float2 Offset, float2 Scale, float2 Repetition, int2 Size, out NativeArray<float> Result)
		{
			offset = Offset;
			scale = Scale;
			repetition = Repetition;
			i = new(Size);
			result = new(Size.area(), Allocator.TempJob);
			Result = result;
		}
	}


	public class TestMathematicsNoise : MonoBehaviour
	{
	#region Reference

		public TextureTester MyTextureTester;

	#endregion

	#region Config

		public float2 offset;
		public float2 scale;
		public float2 repetition;
		public int texture_size;

		public int2 Size => new int2(1, 1) * texture_size;

	#endregion

	#region Process

		[ContextMenu("Run")]
		void Run()
		{
			var jh = new JobHandle();
			JobFor<TestPerlinNoise>.Plan(new(offset, scale, repetition, Size, out var result), ref jh);
			JobHandle.ScheduleBatchedJobs();
			MyTextureTester.InitTexture(Size);
			jh.Complete();
			MyTextureTester.SetTextureSlice(result, 0);
			MyTextureTester.SetTextureSlice(result, 1);
			MyTextureTester.SetTextureSlice(result, 2);
			MyTextureTester.ApplyTexture();

			result.Dispose();
		}

	#endregion

	#region UnityEntry

	#endregion

	}
}