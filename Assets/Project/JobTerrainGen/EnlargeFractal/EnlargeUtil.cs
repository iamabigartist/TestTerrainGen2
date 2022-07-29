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

		public static void EnlargePlan(NativeArray<int> data, int2 data_size, out NativeArray<int>[] Results, Stage[] EnlargeStages, uint plan_rand_seed, ref JobHandle deps)
		{
			var stage_count = EnlargeStages.Length;
			Results = new NativeArray<int>[stage_count];
			var rand = Random.CreateFromIndex(plan_rand_seed);
			
			for (int i = 0; i < stage_count; i++)
			{
				var stage_rand_seed = rand.NextUInt();
				switch (EnlargeStages[i])
				{
					case Stage.Normal:
						{
							Enlarge2X2<Compare11Sampler>.Plan(data, data_size, out data, new(), stage_rand_seed, ref deps);
						}
						break;
					case Stage.Sawtooth:
						{
							Enlarge2X2<Rand11Sampler>.Plan(data, data_size, out data, new(), stage_rand_seed, ref deps);
						}
						break;
					default: throw new();
				}
				data_size *= 2;
				Results[i] = data;
			}
		}
	}
}