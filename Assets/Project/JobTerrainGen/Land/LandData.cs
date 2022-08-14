using System.Linq;
using JobTerrainGen.Area;
using JobTerrainGen.DataDefinition;
using JobTerrainGen.Pipeline;
using JobTerrainGen.Seed;
using JobTerrainGen.Transform;
using JobTerrainGen.Util;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Utils.JobUtil.Template;
using static Utils.JobUtil.NativeContainerUtils;
namespace JobTerrainGen.Land
{
	public class LandData : TerrainData
	{

	#region Config

		public float land_ratio;

	#endregion

	#region Data

		public NativeArray<int> seed_data;
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
			PlaneUtil.EnlargePlan(seed_data, InitSize, out var area_results, GenStages, RandSeed, ref jh);
			var enlarge_result = area_results.Last();
			var shift = new int2(1, 1) * ((size / InitSize).x / 2);
			JobFor<RotateShift>.Plan(new(enlarge_result, size, shift, out var area_data), ref jh);
			JobFor<GenAreaIdArray>.Plan(new(area_data, out var area_ids_set), ref jh);
			jh.Complete();
			AreaIdSetToLandIdArray.Run(area_ids_set, out area_ids);
			AreaToOceanLandRandomSelect.Run(area_ids, land_ratio, RandSeed, out land_by_area_id);
			jh.Complete();

			Dispose(seed_data, area_results, area_data, area_ids, land_by_area_id);
		}
	}
}