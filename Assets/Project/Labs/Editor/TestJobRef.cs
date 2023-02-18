using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
namespace Labs
{
	public interface IRefExecutor
	{
		void Execute(int i, NativeArray<int> array);
	}

	public struct RefExecutor : IRefExecutor
	{
		void TransformInt(ref int source)
		{
			source *= 2;
		}
		public void Execute(int i, NativeArray<int> array)
		{
			int source = i;
			TransformInt(ref source);
			array[i] = source;
		}
	}

	/// <summary>
	///     <para>结论</para>
	///     <para>1. job中可以使用带有ref的函数</para>
	///     <para>2. job中可以使用带有interface的泛型的字段</para>
	///     <para>3. 没有找到比类的静态方法更好的schedule方式，一部分参数比较麻烦也没办法</para>
	/// </summary>
	/// <typeparam name="TExecutor"></typeparam>
	[BurstCompile(DisableSafetyChecks = true)]
	public struct JobWithReference<TExecutor> : IJobFor
		where TExecutor : struct, IRefExecutor
	{
		TExecutor executor;
		[WriteOnly] NativeArray<int> array;

		public void Execute(int index)
		{
			executor.Execute(index, array);
		}

		public static JobHandle Plan(TExecutor executor, int length, out NativeArray<int> array, JobHandle deps = default)
		{
			array = new(length, Allocator.TempJob);
			var job = new JobWithReference<TExecutor>()
			{
				executor = executor,
				array = array
			};
			return job.ScheduleParallel(length, 1, deps);
		}
	}

	public class TestJobRef : EditorWindow
	{
		[MenuItem("Labs/TestJobRef")]
		static void ShowWindow()
		{
			var window = GetWindow<TestJobRef>();
			window.titleContent = new("TestJobRef");
			window.Show();
		}

		void OnEnable()
		{
			var jh = JobWithReference<RefExecutor>.Plan(new(), 5, out var array);
			jh.Complete();
			Debug.Log(string.Join(",", array));
			array.Dispose();
		}

	}
}