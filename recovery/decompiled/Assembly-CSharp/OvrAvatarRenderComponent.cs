using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OvrAvatarRenderComponent : MonoBehaviour
{
	private static readonly string[] LayerKeywords = new string[9] { "LAYERS_0", "LAYERS_1", "LAYERS_2", "LAYERS_3", "LAYERS_4", "LAYERS_5", "LAYERS_6", "LAYERS_7", "LAYERS_8" };

	private static readonly string[] LayerSampleModeParameters = new string[8] { "_LayerSampleMode0", "_LayerSampleMode1", "_LayerSampleMode2", "_LayerSampleMode3", "_LayerSampleMode4", "_LayerSampleMode5", "_LayerSampleMode6", "_LayerSampleMode7" };

	private static readonly string[] LayerBlendModeParameters = new string[8] { "_LayerBlendMode0", "_LayerBlendMode1", "_LayerBlendMode2", "_LayerBlendMode3", "_LayerBlendMode4", "_LayerBlendMode5", "_LayerBlendMode6", "_LayerBlendMode7" };

	private static readonly string[] LayerMaskTypeParameters = new string[8] { "_LayerMaskType0", "_LayerMaskType1", "_LayerMaskType2", "_LayerMaskType3", "_LayerMaskType4", "_LayerMaskType5", "_LayerMaskType6", "_LayerMaskType7" };

	private static readonly string[] LayerColorParameters = new string[8] { "_LayerColor0", "_LayerColor1", "_LayerColor2", "_LayerColor3", "_LayerColor4", "_LayerColor5", "_LayerColor6", "_LayerColor7" };

	private static readonly string[] LayerSurfaceParameters = new string[8] { "_LayerSurface0", "_LayerSurface1", "_LayerSurface2", "_LayerSurface3", "_LayerSurface4", "_LayerSurface5", "_LayerSurface6", "_LayerSurface7" };

	private static readonly string[] LayerSampleParametersParameters = new string[8] { "_LayerSampleParameters0", "_LayerSampleParameters1", "_LayerSampleParameters2", "_LayerSampleParameters3", "_LayerSampleParameters4", "_LayerSampleParameters5", "_LayerSampleParameters6", "_LayerSampleParameters7" };

	private static readonly string[] LayerMaskParametersParameters = new string[8] { "_LayerMaskParameters0", "_LayerMaskParameters1", "_LayerMaskParameters2", "_LayerMaskParameters3", "_LayerMaskParameters4", "_LayerMaskParameters5", "_LayerMaskParameters6", "_LayerMaskParameters7" };

	private static readonly string[] LayerMaskAxisParameters = new string[8] { "_LayerMaskAxis0", "_LayerMaskAxis1", "_LayerMaskAxis2", "_LayerMaskAxis3", "_LayerMaskAxis4", "_LayerMaskAxis5", "_LayerMaskAxis6", "_LayerMaskAxis7" };

	private Dictionary<Material, ovrAvatarMaterialState> materialStates = new Dictionary<Material, ovrAvatarMaterialState>();

	protected void UpdateActive(OvrAvatar avatar, ovrAvatarVisibilityFlags mask)
	{
		bool flag = avatar.ShowFirstPerson && (mask & ovrAvatarVisibilityFlags.FirstPerson) != 0;
		flag |= avatar.ShowThirdPerson && (mask & ovrAvatarVisibilityFlags.ThirdPerson) != 0;
		gameObject.SetActive(flag);
	}

	protected SkinnedMeshRenderer CreateSkinnedMesh(ulong assetID, ovrAvatarVisibilityFlags visibilityMask, bool physicallyBasedShader, int thirdPersonLayer, int firstPersonLayer, int sortingOrder)
	{
		OvrAvatarAssetMesh ovrAvatarAssetMesh = (OvrAvatarAssetMesh)OvrAvatarSDKManager.Instance.GetAsset(assetID);
		if (ovrAvatarAssetMesh == null)
		{
			throw new Exception("Couldn't find mesh for asset " + assetID);
		}
		if ((visibilityMask & ovrAvatarVisibilityFlags.ThirdPerson) != 0)
		{
			gameObject.layer = thirdPersonLayer;
		}
		else
		{
			gameObject.layer = firstPersonLayer;
		}
		SkinnedMeshRenderer skinnedMeshRenderer = ovrAvatarAssetMesh.CreateSkinnedMeshRendererOnObject(gameObject);
		skinnedMeshRenderer.sharedMaterial = CreateAvatarMaterial(gameObject.name + "_material", physicallyBasedShader, (visibilityMask & ovrAvatarVisibilityFlags.SelfOccluding) != 0);
		skinnedMeshRenderer.quality = SkinQuality.Bone4;
		skinnedMeshRenderer.sortingOrder = sortingOrder;
		if ((visibilityMask & ovrAvatarVisibilityFlags.SelfOccluding) == 0)
		{
			skinnedMeshRenderer.shadowCastingMode = ShadowCastingMode.Off;
		}
		return skinnedMeshRenderer;
	}

	protected void UpdateSkinnedMesh(OvrAvatar avatar, SkinnedMeshRenderer mesh, Transform[] bones, ovrAvatarTransform localTransform, ovrAvatarVisibilityFlags visibilityMask, ovrAvatarSkinnedMeshPose skinnedPose)
	{
		UpdateActive(avatar, visibilityMask);
		OvrAvatar.ConvertTransform(localTransform, transform);
		for (int i = 0; i < skinnedPose.jointCount; i++)
		{
			Transform target = bones[i];
			OvrAvatar.ConvertTransform(skinnedPose.jointTransform[i], target);
		}
	}

	protected Material CreateAvatarMaterial(string name, bool physicallyBasedShader, bool selfOccluding)
	{
		string text;
		if (physicallyBasedShader)
		{
			text = "OvrAvatar/AvatarSurfaceShaderPBS";
		}
		else
		{
			text = ((!selfOccluding) ? "OvrAvatar/AvatarSurfaceShader" : "OvrAvatar/AvatarSurfaceShaderSelfOccluding");
		}
		Shader shader = Shader.Find(text);
		Material material = new Material(shader);
		material.name = name;
		materialStates.Add(material, default);
		return material;
	}

	protected void UpdateAvatarMaterial(Material mat, ovrAvatarMaterialState matState)
	{
		if (!materialStates.TryGetValue(mat, out var value) || value.Equals(matState))
		{
			return;
		}
		mat.SetColor("_BaseColor", matState.baseColor);
		mat.SetInt("_BaseMaskType", (int)matState.baseMaskType);
		mat.SetVector("_BaseMaskParameters", matState.baseMaskParameters);
		mat.SetVector("_BaseMaskAxis", matState.baseMaskAxis);
		if (matState.alphaMaskTextureID != 0)
		{
			mat.SetTexture("_AlphaMask", GetLoadedTexture(matState.alphaMaskTextureID));
			mat.SetTextureScale("_AlphaMask", new Vector2(matState.alphaMaskScaleOffset.x, matState.alphaMaskScaleOffset.y));
			mat.SetTextureOffset("_AlphaMask", new Vector2(matState.alphaMaskScaleOffset.z, matState.alphaMaskScaleOffset.w));
		}
		if (matState.normalMapTextureID != 0)
		{
			mat.EnableKeyword("NORMAL_MAP_ON");
			mat.SetTexture("_NormalMap", GetLoadedTexture(matState.normalMapTextureID));
			mat.SetTextureScale("_NormalMap", new Vector2(matState.normalMapScaleOffset.x, matState.normalMapScaleOffset.y));
			mat.SetTextureOffset("_NormalMap", new Vector2(matState.normalMapScaleOffset.z, matState.normalMapScaleOffset.w));
		}
		if (matState.parallaxMapTextureID != 0)
		{
			mat.EnableKeyword("PARALLAX_ON");
			mat.SetTexture("_ParallaxMap", GetLoadedTexture(matState.parallaxMapTextureID));
			mat.SetTextureScale("_ParallaxMap", new Vector2(matState.parallaxMapScaleOffset.x, matState.parallaxMapScaleOffset.y));
			mat.SetTextureOffset("_ParallaxMap", new Vector2(matState.parallaxMapScaleOffset.z, matState.parallaxMapScaleOffset.w));
		}
		if (matState.parallaxMapTextureID != 0)
		{
			mat.EnableKeyword("ROUGHNESS_ON");
			mat.SetTexture("_RoughnessMap", GetLoadedTexture(matState.roughnessMapTextureID));
			mat.SetTextureScale("_RoughnessMap", new Vector2(matState.roughnessMapScaleOffset.x, matState.roughnessMapScaleOffset.y));
			mat.SetTextureOffset("_RoughnessMap", new Vector2(matState.roughnessMapScaleOffset.z, matState.roughnessMapScaleOffset.w));
		}
		mat.EnableKeyword(LayerKeywords[matState.layerCount]);
		for (ulong num = 0uL; num < matState.layerCount; num++)
		{
			ovrAvatarMaterialLayerState ovrAvatarMaterialLayerState2 = matState.layers[num];
			mat.SetInt(LayerSampleModeParameters[num], (int)ovrAvatarMaterialLayerState2.sampleMode);
			mat.SetInt(LayerBlendModeParameters[num], (int)ovrAvatarMaterialLayerState2.blendMode);
			mat.SetInt(LayerMaskTypeParameters[num], (int)ovrAvatarMaterialLayerState2.maskType);
			mat.SetColor(LayerColorParameters[num], ovrAvatarMaterialLayerState2.layerColor);
			if (ovrAvatarMaterialLayerState2.sampleMode != ovrAvatarMaterialLayerSampleMode.Color)
			{
				string text = LayerSurfaceParameters[num];
				mat.SetTexture(text, GetLoadedTexture(ovrAvatarMaterialLayerState2.sampleTexture));
				mat.SetTextureScale(text, new Vector2(ovrAvatarMaterialLayerState2.sampleScaleOffset.x, ovrAvatarMaterialLayerState2.sampleScaleOffset.y));
				mat.SetTextureOffset(text, new Vector2(ovrAvatarMaterialLayerState2.sampleScaleOffset.z, ovrAvatarMaterialLayerState2.sampleScaleOffset.w));
			}
			mat.SetColor(LayerSampleParametersParameters[num], ovrAvatarMaterialLayerState2.sampleParameters);
			mat.SetColor(LayerMaskParametersParameters[num], ovrAvatarMaterialLayerState2.maskParameters);
			mat.SetColor(LayerMaskAxisParameters[num], ovrAvatarMaterialLayerState2.maskAxis);
		}
		materialStates[mat] = matState;
	}

	protected static Texture2D GetLoadedTexture(ulong assetId)
	{
		if (assetId == 0)
		{
			return null;
		}
		OvrAvatarAssetTexture ovrAvatarAssetTexture = (OvrAvatarAssetTexture)OvrAvatarSDKManager.Instance.GetAsset(assetId);
		if (ovrAvatarAssetTexture == null)
		{
			throw new Exception("Could not find texture for asset " + assetId);
		}
		return ovrAvatarAssetTexture.texture;
	}
}
