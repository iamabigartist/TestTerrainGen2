namespace Project.JobTerrainGen.Runtime.Pipeline
{
	public abstract class PipelineNodePort {}

	public class PipelineNodePort<TNativeContainer> : PipelineNodePort
	{
		TNativeContainer Container;
	}
}