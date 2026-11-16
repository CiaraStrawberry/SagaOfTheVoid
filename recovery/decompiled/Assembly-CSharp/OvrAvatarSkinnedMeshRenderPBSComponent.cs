using UnityEngine;

public class OvrAvatarSkinnedMeshRenderPBSComponent : OvrAvatarRenderComponent
{
	private SkinnedMeshRenderer mesh;

	private Transform[] bones;

	internal void Initialize(ovrAvatarRenderPart_SkinnedMeshRenderPBS skinnedMeshRenderPBS, int thirdPersonLayer, int firstPersonLayer, int sortOrder)
	{
		mesh = CreateSkinnedMesh(skinnedMeshRenderPBS.meshAssetID, skinnedMeshRenderPBS.visibilityMask, physicallyBasedShader: true, thirdPersonLayer, firstPersonLayer, sortOrder);
		bones = mesh.bones;
	}

	internal void UpdateSkinnedMeshRenderPBS(OvrAvatar avatar, ovrAvatarRenderPart_SkinnedMeshRenderPBS meshRender)
	{
		UpdateSkinnedMesh(avatar, mesh, bones, meshRender.localTransform, meshRender.visibilityMask, meshRender.skinnedPose);
		Material sharedMaterial = mesh.sharedMaterial;
		sharedMaterial.SetTexture("_Albedo", OvrAvatarRenderComponent.GetLoadedTexture(meshRender.albedoTextureAssetID));
		sharedMaterial.SetTexture("_Surface", OvrAvatarRenderComponent.GetLoadedTexture(meshRender.surfaceTextureAssetID));
	}
}
