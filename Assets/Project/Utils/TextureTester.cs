using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
namespace Utils
{
	public class TextureTester : MonoBehaviour
	{
		
	#region Reference

		public Renderer mRenderer;

	#endregion

	#region Config

		public int2 TextureSize;
		public int PixelCount => TextureSize.area();

	#endregion

	#region Data

		Texture2D mTexture;

	#endregion

	#region Process

	#endregion

	#region Interface

		public void InitTexture(int2 textureSize)
		{
			TextureSize = textureSize;
			mTexture = new(TextureSize.x, TextureSize.y, TextureFormat.RGBAFloat, false) { filterMode = FilterMode.Point };
			mRenderer.material.mainTexture = mTexture;
		}
		
		public void GetTextureSlice<TSliceStride>(out NativeSlice<TSliceStride> Slice, int float_offset_count) where TSliceStride : struct
		{
			Slice = mTexture.GetRawTextureData<float4>().Slice().
				SliceWithStride<TSliceStride>(sizeof(float) * float_offset_count);
		}

		public void ApplyTexture()
		{
			mTexture.Apply();
		}

	#endregion
	}
}