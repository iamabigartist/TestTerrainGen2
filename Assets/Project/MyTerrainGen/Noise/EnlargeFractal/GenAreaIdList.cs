using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
namespace MyTerrainGen.Noise.EnlargeFractal
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenAreaIdList : IJobFor
	{
		[ReadOnly] NativeArray<int> data;
		public void Execute(int i_pixel) {}
	}
}