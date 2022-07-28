using JobTerrainGen.EnlargeFractal.Samplers;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
namespace JobTerrainGen.EnlargeFractal
{
	public static class EnlargeUtil
	{
		public enum Stage
		{
			Normal,
			Sawtooth
		}

		public static JobHandle EnlargePlan(NativeArray<int> data, int2 data_size, out NativeArray<int>[] Results, Stage[] EnlargeStages, uint rand_seed, JobHandle deps)
		{
			var stage_count = EnlargeStages.Length;
			Results = new NativeArray<int>[stage_count];

			var cur_data = data;
			NativeArray<int> cur_result;
			var cur_data_size = data_size;
			JobHandle cur_jh = deps;

			for (int i = 0; i < stage_count; i++)
			{
				switch (EnlargeStages[i])
				{
					case Stage.Normal:
						{
							cur_jh = Enlarge2X2<Compare11Sampler>.Plan(cur_data, cur_data_size, out cur_result, new(), rand_seed, cur_jh);
						}
						break;
					case Stage.Sawtooth:
						{
							cur_jh = Enlarge2X2<Rand11Sampler>.Plan(cur_data, cur_data_size, out cur_result, new(), rand_seed, cur_jh);
						}
						break;
					default: throw new();
				}
				cur_data = cur_result;
				cur_data_size *= 2;
				Results[i] = cur_result;
			}
			return cur_jh;
		}
	}
}