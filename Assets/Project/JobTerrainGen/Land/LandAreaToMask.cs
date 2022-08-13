using Utils.JobUtil.Template;
namespace JobTerrainGen.Land
{
	public struct LandAreaToMask : IJobForRunner
	{
		public (int ExecuteLen, int InnerLoopBatchCount) ScheduleParam { get; }
		public void Execute(int i_pixel) {}
	}
}