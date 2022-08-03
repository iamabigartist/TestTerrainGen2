using JobTerrainGen.EnlargeFractal.Samplers;
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
	public struct Enlarge2X2<TEnlargeSampler> : IJobFor
		where TEnlargeSampler : struct, IEnlargeSampler
	{
		[ReadOnly] IndexRandGenerator rand_gen;
		[ReadOnly] Index2D i;
		[ReadOnly] Index2D i_2x2;
		[ReadOnly] NativeArray<int> data;
		[ReadOnly] TEnlargeSampler sampler;
		[WriteOnly] NativeArray<int> data_2x2;

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
			// bool out10 = i.OutOfRange(seed_pos10);
			// bool out01 = i.OutOfRange(seed_pos01);
			// var seed10 = out10 ? 0 : data[i[seed_pos10]];
			// var seed01 = out01 ? 0 : data[i[seed_pos01]];
			// var seed11 = out10 || out01 ? 0 : data[i[seed_pos11]];
			var seed10 = data[i[i.RepeatWrap(seed_pos10)]];
			var seed01 = data[i[i.RepeatWrap(seed_pos01)]];
			var seed11 = data[i[i.RepeatWrap(seed_pos11)]];

			rand_gen.GenRand(i_pixel, out var rand);
			sampler.Sample(rand,
				seed00, seed10, seed01, seed11,
				out var result00, out var result10, out var result01, out var result11);
			data_2x2[i_2x2[result_pos00]] = result00;
			data_2x2[i_2x2[result_pos10]] = result10;
			data_2x2[i_2x2[result_pos01]] = result01;
			data_2x2[i_2x2[result_pos11]] = result11;
		}

		public static void Plan(NativeArray<int> data, int2 size, out NativeArray<int> data_2x2, TEnlargeSampler sampler, uint rand_seed, ref JobHandle deps)
		{
			var size_2x2 = size * 2;
			data_2x2 = new(size_2x2.area(), Allocator.TempJob);
			var job = new Enlarge2X2<TEnlargeSampler>()
			{
				rand_gen = new(rand_seed),
				i = new(size),
				i_2x2 = new(size * 2),
				data = data,
				sampler = sampler,
				data_2x2 = data_2x2
			};
			deps = job.ScheduleParallel(size.area(), 1, deps);
		}
	}
}