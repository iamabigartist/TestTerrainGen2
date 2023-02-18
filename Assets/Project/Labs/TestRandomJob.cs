using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Random = Unity.Mathematics.Random;
namespace Labs
{
	[BurstCompile(DisableSafetyChecks = false)]
	public struct TestRandomJob : IJobFor
	{
		Random rand;
		[WriteOnly] NativeArray<int> dst;

		public void Execute(int index)
		{
			dst[index] = rand.NextInt() % 100;
			Debug.Log($"{rand.NextInt(100)}");
		}

		public static JobHandle ScheduleParallel(Random rand, NativeArray<int> dst)
		{
			var job = new TestRandomJob()
			{
				rand = rand,
				dst = dst
			};
			return job.ScheduleParallel(dst.Length, 1024, default);
		}
	}
}