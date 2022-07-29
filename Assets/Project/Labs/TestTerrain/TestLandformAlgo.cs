using JobTerrainGen.View;
using static JobTerrainGen.EnlargeFractal.EnlargeUtil;
using static JobTerrainGen.EnlargeFractal.EnlargeUtil.Stage;
namespace Labs.TestTerrain
{
	public class TestLandformAlgo : TerrainDataTester
	{
		protected override int enlarge_count { get; }

		public Stage[] stage_list =
		{
			Normal,
			Sawtooth,
			Normal,
			Normal,
			Normal
		};

	}
}