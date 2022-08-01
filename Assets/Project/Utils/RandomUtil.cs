using Unity.Collections;
using Unity.Mathematics;
namespace Utils
{
	public static class RandomUtil
	{
		// public static T SelectWithProbability<T>(this Random rand, NativeArray<T> options, NativeArray<float> probabilities) where T : struct
		// {
		// 	
		// }
		public static T SelectArray<T>(this Random rand, NativeArray<T> options) where T : struct
		{
			var select_index = rand.NextInt(options.Length);
			return options[select_index];
		}

		public static T Select2<T>(this Random rand, T a, T b) where T : struct
		{
			return rand.NextBool() ? a : b;
		}

		public static T Select3<T>(this Random rand, T a, T b, T c) where T : struct
		{
			var select_index = rand.NextInt(3);
			return select_index switch
			{
				0 => a,
				1 => b,
				2 => c
			};
		}
		
		public static T Select4<T>(this Random rand, T a, T b, T c, T d) where T : struct
		{
			var select_index = rand.NextInt(4);
			return select_index switch
			{
				0 => a,
				1 => b,
				2 => c,
				3 => d
			};
		}


		public static void SelectMultiple(this Random rand, int select_count, int total_count, out NativeArray<int> selection)
		{
			var index_list = new NativeList<int>(total_count, Allocator.Temp);
			selection = new(select_count, Allocator.Temp);
			for (int i = 0; i < total_count; i++)
			{
				index_list.Add(i);
			}
			for (int i_select = 0; i_select < select_count; i_select++)
			{
				var cur_list_len = index_list.Length;
				var cur_selected_index_index = rand.NextInt(cur_list_len);
				var cur_selected_index = index_list[cur_selected_index_index];
				index_list.RemoveAt(cur_selected_index_index);
				selection[i_select] = cur_selected_index;
			}
		}
	}
}