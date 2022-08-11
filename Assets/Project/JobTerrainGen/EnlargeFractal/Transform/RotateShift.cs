using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.EnlargeFractal.Transform
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct RotateShift : IJobFor
	{
		[ReadOnly] int2 shift;
		[ReadOnly] Index2D i;
		[ReadOnly] NativeArray<int> data;
		[WriteOnly] NativeArray<int> shifted_data;
		public void Execute(int i_pixel)
		{
			var shifted_pos = i.RepeatWrap(i[i_pixel] + shift);
			shifted_data[i[shifted_pos]] = data[i_pixel];
		}
		public static void Plan(NativeArray<int> data, int2 size, int2 shift, out NativeArray<int> shifted_data, ref JobHandle jh)
		{
			shifted_data = new(data.Length, Allocator.TempJob);
			var job = new RotateShift()
			{
				shift = shift,
				i = new(size),
				data = data,
				shifted_data = shifted_data
			};
			jh = job.ScheduleParallel(data.Length, 1024, jh);
		}
	}
}