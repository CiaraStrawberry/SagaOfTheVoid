using System;
using Oculus.Avatar;
using UnityEngine;

public class OvrAvatarAssetTexture : OvrAvatarAsset
{
	public Texture2D texture;

	public OvrAvatarAssetTexture(ulong _assetId, IntPtr asset)
	{
		assetID = _assetId;
		ovrAvatarTextureAssetData ovrAvatarTextureAssetData2 = CAPI.ovrAvatarAsset_GetTextureData(asset);
		texture = new Texture2D(format: ovrAvatarTextureAssetData2.format switch
		{
			ovrAvatarTextureFormat.RGB24 => TextureFormat.RGB24, 
			ovrAvatarTextureFormat.DXT1 => TextureFormat.DXT1, 
			ovrAvatarTextureFormat.DXT5 => TextureFormat.DXT5, 
			_ => throw new NotImplementedException($"Unsupported texture format {ovrAvatarTextureAssetData2.format.ToString()}"), 
		}, width: (int)ovrAvatarTextureAssetData2.sizeX, height: (int)ovrAvatarTextureAssetData2.sizeY, mipmap: ovrAvatarTextureAssetData2.mipCount > 1, linear: false);
		texture.LoadRawTextureData(ovrAvatarTextureAssetData2.textureData, (int)ovrAvatarTextureAssetData2.textureDataSize);
		texture.Apply(updateMipmaps: true, makeNoLongerReadable: true);
	}
}
