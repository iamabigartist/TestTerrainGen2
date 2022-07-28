using Unity.Mathematics;
namespace Utils
{
	/// <summary>
	///     是否有必要使用来简化代码？
	/// </summary>
	public struct IndexRandGenerator
	{
		public uint seed;
		public IndexRandGenerator(uint seed)
		{
			this.seed = seed;
		}
		uint ResultIndex_Multiply(uint i)
		{
			return seed ^ i;
		}
		public void GenRand(int i, out Random rand) { rand = Random.CreateFromIndex(ResultIndex_Multiply((uint)i)); }
	}
}