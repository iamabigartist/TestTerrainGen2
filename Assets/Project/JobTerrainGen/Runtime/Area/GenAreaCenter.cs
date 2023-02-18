using Project.JobTerrainGen.Utils;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Area
{
	[BurstCompile(
		DisableSafetyChecks = true, OptimizeFor = OptimizeFor.Performance,
		CompileSynchronously = true)]
	public struct GenAreaCenter : IJobFor
	{
		[ReadOnly] Index2D i;
		[ReadOnly] NativeArray<int> area_terrain;
		[ReadOnly] NativeArray<int> area_ids;
		[WriteOnly] NativeArray<int2> area_centers;

		public void Execute(int i_area)
		{
			var cur_id = area_ids[i_area];
			var min_pos = new int2(0, 0);
			for (int i_pixel = 0; i_pixel < area_terrain.Length; i_pixel++)
			{
				if (area_terrain[i_pixel] == cur_id) { min_pos = i[i_pixel]; }
			}

			var max_pos = new int2(0, 0);
			for (int i_pixel = area_terrain.Length - 1; 0 <= i_pixel; i_pixel--)
			{
				if (area_terrain[i_pixel] == cur_id) { max_pos = i[i_pixel]; }
			}

			area_centers[i_area] = (min_pos + max_pos) / 2;
		}


	}
}