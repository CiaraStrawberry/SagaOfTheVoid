using System;
using System.Runtime.InteropServices;

namespace Oculus.Avatar;

public class CAPI
{
	private const string LibFile = "libovravatar";

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_Initialize(string appID);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_Shutdown();

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr ovrAvatarMessage_Pop();

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern ovrAvatarMessageType ovrAvatarMessage_GetType(IntPtr msg);

	public static ovrAvatarMessage_AvatarSpecification ovrAvatarMessage_GetAvatarSpecification(IntPtr msg)
	{
		IntPtr ptr = ovrAvatarMessage_GetAvatarSpecification_Native(msg);
		return (ovrAvatarMessage_AvatarSpecification)Marshal.PtrToStructure(ptr, typeof(ovrAvatarMessage_AvatarSpecification));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarMessage_GetAvatarSpecification")]
	private static extern IntPtr ovrAvatarMessage_GetAvatarSpecification_Native(IntPtr msg);

	public static ovrAvatarMessage_AssetLoaded ovrAvatarMessage_GetAssetLoaded(IntPtr msg)
	{
		IntPtr ptr = ovrAvatarMessage_GetAssetLoaded_Native(msg);
		return (ovrAvatarMessage_AssetLoaded)Marshal.PtrToStructure(ptr, typeof(ovrAvatarMessage_AssetLoaded));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarMessage_GetAssetLoaded")]
	private static extern IntPtr ovrAvatarMessage_GetAssetLoaded_Native(IntPtr msg);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatarMessage_Free(IntPtr msg);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_RequestAvatarSpecification(ulong userID);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern IntPtr ovrAvatar_Create(IntPtr avatarSpecification, ovrAvatarCapabilities capabilities);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatarPose_UpdateBody(IntPtr avatar, ovrAvatarTransform headPose);

	public static void ovrAvatarPose_UpdateVoiceVisualization(IntPtr avatar, float[] pcmData)
	{
		ovrAvatarPose_UpdateVoiceVisualization_Native(avatar, (uint)pcmData.Length, pcmData);
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_UpdateVoiceVisualization")]
	private static extern void ovrAvatarPose_UpdateVoiceVisualization_Native(IntPtr avatar, uint pcmDataSize, [In] float[] pcmData);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatarPose_UpdateHands(IntPtr avatar, ovrAvatarHandInputState inputStateLeft, ovrAvatarHandInputState inputStateRight);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatarPose_Finalize(IntPtr avatar, float elapsedSeconds);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetLeftControllerVisibility(IntPtr avatar, bool show);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetRightControllerVisibility(IntPtr avatar, bool show);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern ulong ovrAvatarComponent_Count(IntPtr avatar);

	public static ovrAvatarComponent ovrAvatarComponent_Get(IntPtr avatar, ulong index)
	{
		IntPtr ptr = ovrAvatarComponent_Get_Native(avatar, index);
		return (ovrAvatarComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarComponent_Get")]
	public static extern IntPtr ovrAvatarComponent_Get_Native(IntPtr avatar, ulong index);

	public static ovrAvatarBaseComponent ovrAvatarPose_GetBaseComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetBaseComponent_Native(avatar);
		return (ovrAvatarBaseComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarBaseComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetBaseComponent")]
	private static extern IntPtr ovrAvatarPose_GetBaseComponent_Native(IntPtr avatar);

	public static ovrAvatarBodyComponent ovrAvatarPose_GetBodyComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetBodyComponent_Native(avatar);
		return (ovrAvatarBodyComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarBodyComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetBodyComponent")]
	private static extern IntPtr ovrAvatarPose_GetBodyComponent_Native(IntPtr avatar);

	public static ovrAvatarControllerComponent ovrAvatarPose_GetLeftControllerComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetLeftControllerComponent_Native(avatar);
		return (ovrAvatarControllerComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarControllerComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetLeftControllerComponent")]
	private static extern IntPtr ovrAvatarPose_GetLeftControllerComponent_Native(IntPtr avatar);

	public static ovrAvatarControllerComponent ovrAvatarPose_GetRightControllerComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetRightControllerComponent_Native(avatar);
		return (ovrAvatarControllerComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarControllerComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetRightControllerComponent")]
	private static extern IntPtr ovrAvatarPose_GetRightControllerComponent_Native(IntPtr avatar);

	public static ovrAvatarHandComponent ovrAvatarPose_GetLeftHandComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetLeftHandComponent_Native(avatar);
		return (ovrAvatarHandComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarHandComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetLeftHandComponent")]
	private static extern IntPtr ovrAvatarPose_GetLeftHandComponent_Native(IntPtr avatar);

	public static ovrAvatarHandComponent ovrAvatarPose_GetRightHandComponent(IntPtr avatar)
	{
		IntPtr ptr = ovrAvatarPose_GetRightHandComponent_Native(avatar);
		return (ovrAvatarHandComponent)Marshal.PtrToStructure(ptr, typeof(ovrAvatarHandComponent));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarPose_GetRightHandComponent")]
	private static extern IntPtr ovrAvatarPose_GetRightHandComponent_Native(IntPtr avatar);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatarAsset_BeginLoading(ulong assetID);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern ovrAvatarAssetType ovrAvatarAsset_GetType(IntPtr assetHandle);

	public static ovrAvatarMeshAssetData ovrAvatarAsset_GetMeshData(IntPtr assetPtr)
	{
		IntPtr ptr = ovrAvatarAsset_GetMeshData_Native(assetPtr);
		return (ovrAvatarMeshAssetData)Marshal.PtrToStructure(ptr, typeof(ovrAvatarMeshAssetData));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarAsset_GetMeshData")]
	private static extern IntPtr ovrAvatarAsset_GetMeshData_Native(IntPtr assetPtr);

	public static ovrAvatarTextureAssetData ovrAvatarAsset_GetTextureData(IntPtr assetPtr)
	{
		IntPtr ptr = ovrAvatarAsset_GetTextureData_Native(assetPtr);
		return (ovrAvatarTextureAssetData)Marshal.PtrToStructure(ptr, typeof(ovrAvatarTextureAssetData));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarAsset_GetTextureData")]
	private static extern IntPtr ovrAvatarAsset_GetTextureData_Native(IntPtr assetPtr);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern ovrAvatarRenderPartType ovrAvatarRenderPart_GetType(IntPtr renderPart);

	public static ovrAvatarRenderPart_SkinnedMeshRender ovrAvatarRenderPart_GetSkinnedMeshRender(IntPtr renderPart)
	{
		IntPtr ptr = ovrAvatarRenderPart_GetSkinnedMeshRender_Native(renderPart);
		return (ovrAvatarRenderPart_SkinnedMeshRender)Marshal.PtrToStructure(ptr, typeof(ovrAvatarRenderPart_SkinnedMeshRender));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarRenderPart_GetSkinnedMeshRender")]
	private static extern IntPtr ovrAvatarRenderPart_GetSkinnedMeshRender_Native(IntPtr renderPart);

	public static ovrAvatarRenderPart_SkinnedMeshRenderPBS ovrAvatarRenderPart_GetSkinnedMeshRenderPBS(IntPtr renderPart)
	{
		IntPtr ptr = ovrAvatarRenderPart_GetSkinnedMeshRenderPBS_Native(renderPart);
		return (ovrAvatarRenderPart_SkinnedMeshRenderPBS)Marshal.PtrToStructure(ptr, typeof(ovrAvatarRenderPart_SkinnedMeshRenderPBS));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarRenderPart_GetSkinnedMeshRenderPBS")]
	private static extern IntPtr ovrAvatarRenderPart_GetSkinnedMeshRenderPBS_Native(IntPtr renderPart);

	public static ovrAvatarRenderPart_ProjectorRender ovrAvatarRenderPart_GetProjectorRender(IntPtr renderPart)
	{
		IntPtr ptr = ovrAvatarRenderPart_GetProjectorRender_Native(renderPart);
		return (ovrAvatarRenderPart_ProjectorRender)Marshal.PtrToStructure(ptr, typeof(ovrAvatarRenderPart_ProjectorRender));
	}

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl, EntryPoint = "ovrAvatarRenderPart_GetProjectorRender")]
	private static extern IntPtr ovrAvatarRenderPart_GetProjectorRender_Native(IntPtr renderPart);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern uint ovrAvatar_GetReferencedAssetCount(IntPtr avatar);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern ulong ovrAvatar_GetReferencedAsset(IntPtr avatar, uint index);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetLeftHandGesture(IntPtr avatar, ovrAvatarHandGesture gesture);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetRightHandGesture(IntPtr avatar, ovrAvatarHandGesture gesture);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetLeftHandCustomGesture(IntPtr avatar, uint jointCount, [In] ovrAvatarTransform[] customJointTransforms);

	[DllImport("libovravatar", CallingConvention = CallingConvention.Cdecl)]
	public static extern void ovrAvatar_SetRightHandCustomGesture(IntPtr avatar, uint jointCount, [In] ovrAvatarTransform[] customJointTransforms);
}
