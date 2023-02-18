using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace Labs
{

	public class TestJobInJob : EditorWindow
	{

		[BurstCompile(DisableSafetyChecks = true)]
		struct TestJob : IJobParallelFor
		{
			[BurstCompile(DisableSafetyChecks = true)]
			struct TestJobInit : IJob
			{
				public NativeList<int> source;
				public NativeList<int> result;
				public void Execute()
				{
					result.Resize(source.Length, NativeArrayOptions.UninitializedMemory);
				}
				public TestJobInit(NativeList<int> Source, NativeList<int> Result)
				{
					source = Source;
					result = Result;
				}
			}

			[ReadOnly] public NativeList<int> source;
			[WriteOnly] public NativeArray<int> result;

			public void Execute(int i)
			{
				result[i] = source[i];
			}

			public TestJob(NativeList<int> Source, NativeList<int> Result)
			{
				source = Source;
				result = Result.AsDeferredJobArray();
			}

			public static void Plan(NativeList<int> source, out NativeList<int> result, ref JobHandle jh)
			{
				result = new(Allocator.TempJob);
				var init = new TestJobInit(source, result);
				var execute = new TestJob(source, result);
				var len = source.Length;
				jh = init.Schedule(jh);
				jh = execute.Schedule(len, 1024, jh);
			}
		}

		[MenuItem("Labs/TestJobInJob")]
		static void ShowWindow()
		{
			var window = GetWindow<TestJobInJob>();
			window.titleContent = new("TestJobInJob");
			window.Show();
		}

		void OnEnable()
		{
			var jh = new JobHandle();
			var source = new NativeList<int>(Allocator.TempJob);
			var array = new NativeArray<int>(Enumerable.Repeat(10, 100).ToArray(), Allocator.TempJob);
			source.CopyFrom(array);
			TestJob.Plan(source, out var result, ref jh);
			jh.Complete();
			Debug.Log(string.Join(",", result.ToArray()));
		}

	}
}