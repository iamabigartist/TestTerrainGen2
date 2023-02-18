using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEditor;
using UnityEngine;
using Random = Unity.Mathematics.Random;
namespace Labs
{
	/// <summary>
	///     <para>结论</para>
	///     <para>1. 使用外带的自动管理的rand 不能保证每次执行结果一致，因为job execute index的顺序每次是随机的</para>
	///     <para>
	///         2. Unity.Mathematics.Random
	///         所带有的State字段，并非完全等于一般意义上随机的seed，因为测试中，临近数字state得出的结果相近，因此在使用index初始化rand的时候，需要用CreateFromIndex来hash index。
	///     </para>
	/// </summary>
	[BurstCompile(DisableSafetyChecks = true)]
	public struct JobWithRefRand : IJobFor
	{
		[ReadOnly] uint rand_seed;
		[ReadOnly] NativeArray<int> numbers;
		[WriteOnly] NativeArray<int> array;

		int RandSelectedNumberRef(ref Random cur_rand)
		{
			return numbers[cur_rand.NextInt(numbers.Length)];
		}
		int RandSelectedNumber(Random cur_rand)
		{
			return numbers[cur_rand.NextInt(numbers.Length)];
		}
		public void Execute(int i)
		{
			var rand = Random.CreateFromIndex(rand_seed * (uint)i); //和直接set state区别在于什么，更加不同吗？
			// var rand = new Random(rand_seed * (uint)i); //和直接set state区别在于什么，更加不同吗？
			int ref_num = RandSelectedNumberRef(ref rand);
			int value_num = RandSelectedNumber(rand);
			array[i] = value_num * ref_num;
		}

		public static JobHandle Plan(int[] selections, int length, uint rand_seed, out NativeArray<int> array, JobHandle deps = default)
		{
			var numbers = new NativeArray<int>(selections, Allocator.TempJob);
			array = new(length, Allocator.TempJob);
			var job = new JobWithRefRand()
			{
				rand_seed = rand_seed,
				numbers = numbers,
				array = array
			};
			var jh = job.ScheduleParallel(length, 1, deps);
			return numbers.Dispose(jh);
		}
	}

	public class TestRefRandJob : EditorWindow
	{
		[MenuItem("Labs/TestRefRandJob")]
		static void ShowWindow()
		{
			var window = GetWindow<TestRefRandJob>();
			window.titleContent = new("TestRefRandJob");
			window.Show();
		}

		uint rand_seed;
		int length;

		void Run()
		{
			var jh = JobWithRefRand.Plan(new[] { 1, 3, 5, 7 }, length, rand_seed, out var array);
			jh.Complete();
			Debug.Log(string.Join(",", array));
			array.Dispose();
		}

		void OnGUI()
		{
			rand_seed = (uint)EditorGUILayout.IntField("rand_seed", (int)rand_seed);
			length = EditorGUILayout.IntField("array_len", length);
			if (GUILayout.Button("Run")) { Run(); }
		}

	}
}