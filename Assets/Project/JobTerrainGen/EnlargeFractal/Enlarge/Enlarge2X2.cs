using JobTerrainGen.EnlargeFractal.Samplers;
using Unity.Collections;
using Unity.Mathematics;
using Utils;
using Utils.JobUtil.Template;
namespace JobTerrainGen.EnlargeFractal.Enlarge
{
	public struct Enlarge2X2<TEnlargeSampler> : IJobForRunner
		where TEnlargeSampler : struct, IEnlargeSampler
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam { get; }

		IndexRandGenerator rand_gen;
		Index2D i;
		Index2D i_2;
		NativeArray<int> data;
		TEnlargeSampler sampler;
		NativeArray<int> data_2;

		public void Execute(int i_pixel)
		{
			var seed_pos00 = i[i_pixel];
			var seed_pos10 = seed_pos00 + new int2(1, 0);
			var seed_pos01 = seed_pos00 + new int2(0, 1);
			var seed_pos11 = seed_pos00 + new int2(1, 1);

			var result_pos00 = seed_pos00 * 2;
			var result_pos10 = result_pos00 + new int2(1, 0);
			var result_pos01 = result_pos00 + new int2(0, 1);
			var result_pos11 = result_pos00 + new int2(1, 1);

			var seed00 = data[i_pixel];
			var seed10 = data[i[i.RepeatWrap(seed_pos10)]];
			var seed01 = data[i[i.RepeatWrap(seed_pos01)]];
			var seed11 = data[i[i.RepeatWrap(seed_pos11)]];

			rand_gen.Gen(i_pixel, out var rand);
			sampler.Sample(rand,
				seed00, seed10, seed01, seed11,
				out var result00, out var result10, out var result01, out var result11);
			data_2[i_2[result_pos00]] = result00;
			data_2[i_2[result_pos10]] = result10;
			data_2[i_2[result_pos01]] = result01;
			data_2[i_2[result_pos11]] = result11;
		}

		public Enlarge2X2(NativeArray<int> data, int2 size, out NativeArray<int> data_2, TEnlargeSampler sampler, uint rand_seed)
		{
			ScheduleParam = (size.area(), 1);
			var size_2 = size * 2;
			rand_gen = new(rand_seed);
			i = new(size);
			i_2 = new(size_2);
			this.data = data;
			this.sampler = sampler;
			this.data_2 = new(size_2.area(), Allocator.TempJob);
			data_2 = this.data_2;
		}
	}
}