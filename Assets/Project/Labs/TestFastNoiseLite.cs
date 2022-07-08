using UnityEngine;
namespace Labs
{
	public class TestFastNoiseLite : MonoBehaviour
	{
		void OnEnable()
		{
			// Create and configure FastNoise object
			FastNoiseLite noise = new FastNoiseLite();
			noise.SetNoiseType(FastNoiseLite.NoiseType.Cellular);
			noise.SetCellularReturnType();
// Gather noise data
			float[] noiseData = new float[128 * 128];
			int index = 0;

			for (int y = 0; y < 128; y++)
			{
				for (int x = 0; x < 128; x++)
				{
					noiseData[index++] = noise.GetNoise(x, y);
				}
			}
		}
	}
}