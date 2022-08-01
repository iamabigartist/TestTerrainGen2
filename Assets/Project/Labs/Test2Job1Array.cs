using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
namespace Labs
{
	public struct TestWriteArray : IJobFor
	{
		[ReadOnly] bool odd;
		[WriteOnly] NativeArray<int> result;
		public void Execute(int i)
		{
			if (!odd && i % 2 == 0) { result[i] = 2; }
			if (odd && i % 2 != 0) { result[i] = 3; }
		}
		public static JobHandle ScheduleParallel(bool odd, int length, NativeArray<int> result)
		{
			var job = new TestWriteArray()
			{
				odd = odd,
				result = result
			};
			return job.ScheduleParallel(length, 1, default);
		}
	}


	public class Test2Job1Array : MonoBehaviour
	{

	#region Config

		public int array_len = 10;

	#endregion

		void OnEnable()
		{
			var result = new NativeArray<int>(array_len, Allocator.TempJob);
			var jh_odd = TestWriteArray.ScheduleParallel(true, array_len, result);
			var jh_even = TestWriteArray.ScheduleParallel(false, array_len, result);


		}

	}
}