using System.Linq;
using Project.JobTerrainGen.Area;
using Project.JobTerrainGen.DataDefinition;
using Project.JobTerrainGen.Pipeline;
using Project.JobTerrainGen.Seed;
using Project.JobTerrainGen.Transform;
using Project.JobTerrainGen.Utils;
using Project.JobTerrainGen.Utils.JobUtil.Template;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Project.JobTerrainGen.Utils.JobUtil.NativeContainerUtils;
namespace Project.JobTerrainGen.Land
{
	public class LandData : TerrainData
	{

	#region Config

		public float land_ratio;

	#endregion

	#region Data

		public NativeArray<int> seed_data;
		public NativeArray<int> area_terrain;
		public NativeArray<int> area_ids;
		public NativeHashMap<int, bool> land_by_area_id;

	#endregion

		public LandData(float LandRatio)
		{
			land_ratio = LandRatio;
		}

		public override void Generate(int2 InitSize, TerrainGenStage[] GenStages, uint RandSeed)
		{
			size = TerrainGenStage.GetResultSize(InitSize, GenStages);
			var jh = new JobHandle();
			JobFor<GenSeedWithAroundOcean>.Plan(new(out seed_data, InitSize), ref jh);
			ProcessUtil.PlanEnlarge(seed_data, InitSize, out var area_results, GenStages, RandSeed, ref jh);
			var enlarge_result = area_results.Last();
			var shift = new int2(1, 1) * ((size / InitSize).x / 2);
			JobFor<RotateShift>.Plan(new(enlarge_result, size, shift, out var area_data), ref jh);
			JobFor<GenAreaIdArray>.Plan(new(area_data, out var area_ids_set), ref jh);
			jh.Complete();
			AreaIdSetToLandIdArray.Run(area_ids_set, out area_ids);
			AreaToOceanLandRandomSelect.Run(area_ids, land_ratio, RandSeed, out land_by_area_id);
			jh.Complete();

			Dispose(area_results, area_data);
		}
	}
}