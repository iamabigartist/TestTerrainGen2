using Unity.Collections;
using UnityEditor;
namespace Labs
{
	public class TestRandom : EditorWindow
	{
		[MenuItem("Labs/TestRandom")]
		static void ShowWindow()
		{
			var window = GetWindow<TestRandom>();
			window.titleContent = new("TestRandom");
			window.Show();
		}

		string array_string;

		void OnEnable()
		{
			var dst = new NativeArray<int>(10, Allocator.Persistent);
			var test_rand_job_handle = TestRandomJob.ScheduleParallel(new(100), dst);
			test_rand_job_handle.Complete();
			array_string = string.Join(",", dst);
			dst.Dispose();
		}

		void OnGUI()
		{
			EditorGUILayout.LabelField(array_string);
		}
	}
}