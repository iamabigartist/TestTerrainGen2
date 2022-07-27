using System.Linq;
using UnityEngine;
using Utils;
namespace Labs
{
	public class TestTextureTester : MonoBehaviour
	{
		void Start()
		{
			var tester = GetComponent<TextureTester>();
			tester.GetTextureSlice<float>(out var slice, 1);
			slice.CopyFrom(Enumerable.Repeat<float>(1, tester.PixelCount).ToArray());
			tester.ApplyTexture();
		}
	}
}