using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Biome
{
	public struct SpreadShallowOceanFromDeepOcean : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam => (source_ocean_data.Length, 8);

		IndexRandGenerator rand_gen;
		Index2D i;
		NativeArray<int> source_ocean_data;
		NativeArray<int> result_ocean_data;

		public void Execute(int i_seed)
		{
			var seed_pos = i[i_seed];
			var seed_ocean = source_ocean_data[i_seed];
			int land_count = 0;
			int shallow_ocean_count = 0;
			for (int y = -1; y <= 1; y++)
			{
				for (int x = -1; x <= 1; x++)
				{
					if (x == 0 && y == 0) { continue; }
					var cur_neighbour_pos = seed_pos + new int2(x, y);
					if (i.OutOfRange(cur_neighbour_pos)) { continue; }
					if (source_ocean_data[i[cur_neighbour_pos]] == 0) { land_count++; }
					if (source_ocean_data[i[cur_neighbour_pos]] == 2) { shallow_ocean_count++; }
				}
			}

			int seed_result = seed_ocean;
			if (seed_ocean == 1)
			{
				if (land_count > 0) { seed_result = 2; }
				else if (shallow_ocean_count > 0)
				{
					rand_gen.Gen(i_seed, out var rand);
					if (rand.NextFloat(1f) < shallow_ocean_count / 8f) { seed_result = 2; }
				}
			}

			result_ocean_data[i_seed] = seed_result;
		}

		public SpreadShallowOceanFromDeepOcean(NativeArray<int> SourceOceanData, int2 Size, uint RandSeed, out NativeArray<int> ResultOceanData)
		{
			rand_gen = new(RandSeed);
			i = new(Size);
			source_ocean_data = SourceOceanData;
			result_ocean_data = new(source_ocean_data.Length, Allocator.TempJob);

			ResultOceanData = result_ocean_data;
		}
	}
}