using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace Labs
{
	public class TestNativeListRef : EditorWindow
	{
		struct TestJob1 : IJob
		{
			public NativeList<int> list;
			public void Execute()
			{
				list.Resize(99, NativeArrayOptions.ClearMemory); //分配内粗选项影响结果吗
			}
		}
		struct TestJob2 : IJob
		{
			public NativeList<int> list;
			public NativeArray<int> parameters;
			public void Execute()
			{
				parameters[0] = list.Length;
			}
		}
		[MenuItem("Labs/TestNativeListRef")]
		static void ShowWindow()
		{
			var window = GetWindow<TestNativeListRef>();
			window.titleContent = new("TestNativeListRef");
			window.Show();
		}

		static void Plan(out NativeList<int> list, out NativeArray<int> parameters, ref JobHandle jh)
		{

			list = new(Allocator.TempJob);
			parameters = new(1, Allocator.TempJob);
			var job1 = new TestJob1 { list = list };
			var job2 = new TestJob2 { list = list, parameters = parameters };
			jh = job1.Schedule(jh);
			jh = job2.Schedule(jh);
		}

		void OnEnable()
		{
			var jh = new JobHandle();
			Plan(out var list, out var parameters, ref jh);
			jh.Complete();
			Debug.Log(parameters[0]);
		}
	}
}