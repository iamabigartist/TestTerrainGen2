using System;
using Unity.Mathematics;
namespace Project.JobTerrainGen.Pipeline
{
	[Serializable]
	public abstract class TerrainGenStage
	{
		public static int2 GetResultSize(int2 InitSize, TerrainGenStage[] Stages)
		{
			var result_size = InitSize;
			foreach (var stage in Stages)
			{
				stage?.ModifySize(ref result_size);
			}
			return result_size;
		}
		protected abstract void ModifySize(ref int2 size);

	}
}