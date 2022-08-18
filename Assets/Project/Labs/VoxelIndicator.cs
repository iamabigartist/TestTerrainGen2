using System.Linq;
using Project.JobTerrainGen.Utils;
using Unity.Mathematics;
using UnityEngine;
namespace Labs
{
	[RequireComponent(typeof(MeshRenderer))]
	public class VoxelIndicator : MonoBehaviour
	{
	#region Reference

		public TextureTester Tester;
		public Color Color;
		Renderer mRenderer;

	#endregion

	#region Data

		Texture2D mTexture;
		// Index2D texture_i;

	#endregion

		void InitPixelIndicatorTexture((int2 texture_size, Texture2D texture) Obj)
		{
			(int2 texture_size, Texture2D texture) = Obj;
			mTexture = new(texture_size.x, texture_size.y, TextureFormat.RGBAFloat, false) { filterMode = FilterMode.Point };
			mRenderer.material.mainTexture = mTexture;
			mTexture.SetPixels(Enumerable.Repeat(new Color(0, 0, 0, 0), texture_size.area()).ToArray());
			mTexture.Apply();

			Debug.Log("asdasd");
		}

		int2 last_pos = new(-1, -1);
		Color last_color;

		void IndicatePixel((Vector3 world_pos, int2 pixel_pos) Obj)
		{
			mTexture.SetPixel(last_pos.x, last_pos.y, last_color);
			(Vector3 world_pos, int2 pixel_pos) = Obj;
			last_color = mTexture.GetPixel(pixel_pos.x, pixel_pos.y);
			last_pos = pixel_pos;

			mTexture.SetPixel(pixel_pos.x, pixel_pos.y, Color);
			mTexture.Apply();
		}


		void Awake()
		{
			mRenderer = GetComponent<Renderer>();
			Tester.OnHoverTexture += IndicatePixel;
			Tester.OnTextureInited += InitPixelIndicatorTexture;
		}



	}
}