namespace Project.JobTerrainGen.Pipeline
{
	public abstract class PipelineNodePort {}

	public class PipelineNodePort<TNativeContainer> : PipelineNodePort
	{
		TNativeContainer Container;
	}
}