using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utils;
namespace JobTerrainGen.EnlargeFractal
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenSeedDataWithAroundOcean : IJobFor
	{
		[ReadOnly] Index2D i;
		[WriteOnly] NativeArray<int> data;
		public void Execute(int i_seed)
		{
			if (i.IsEdge(i[i_seed])) { data[i_seed] = 0; }
			else { data[i_seed] = i_seed + 1; }
		}

		public static void Plan(out NativeArray<int> data, int2 size, ref JobHandle deps)
		{
			data = new(size.area(), Allocator.TempJob);
			var job = new GenSeedDataWithAroundOcean()
			{
				i = new(size),
				data = data
			};
			deps = job.ScheduleParallel(data.Length, 1024, deps);
		}
	}
}