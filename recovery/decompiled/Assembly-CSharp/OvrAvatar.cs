using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Oculus.Avatar;
using UnityEngine;
using UnityEngine.Events;

public class OvrAvatar : MonoBehaviour
{
	public class PacketEventArgs : EventArgs
	{
		public readonly OvrAvatarPacket Packet;

		public PacketEventArgs(OvrAvatarPacket packet)
		{
			Packet = packet;
		}
	}

	public OvrAvatarDriver Driver;

	public OvrAvatarBase Base;

	public OvrAvatarBody Body;

	public OvrAvatarTouchController ControllerLeft;

	public OvrAvatarTouchController ControllerRight;

	public OvrAvatarHand HandLeft;

	public OvrAvatarHand HandRight;

	public bool RecordPackets;

	public bool StartWithControllers;

	public AvatarLayer FirstPersonLayer;

	public AvatarLayer ThirdPersonLayer;

	public bool ShowFirstPerson = true;

	public bool ShowThirdPerson;

	public ovrAvatarCapabilities Capabilities = ovrAvatarCapabilities.All;

	private const float PacketDurationSeconds = 1f / 45f;

	private OvrAvatarPacket currentPacket;

	private int renderPartCount;

	private bool showLeftController;

	private bool showRightController;

	private List<float[]> voiceUpdates = new List<float[]>();

	public ulong oculusUserID;

	private IntPtr sdkAvatar = IntPtr.Zero;

	private HashSet<ulong> assetLoadingIds = new HashSet<ulong>();

	private Dictionary<string, OvrAvatarComponent> trackedComponents = new Dictionary<string, OvrAvatarComponent>();

	public UnityEvent AssetsDoneLoading = new UnityEvent();

	private bool assetsFinishedLoading;

	public Transform LeftHandCustomPose;

	public Transform RightHandCustomPose;

	private Transform cachedLeftHandCustomPose;

	private Transform[] cachedLeftHandJoints;

	private ovrAvatarTransform[] cachedLeftHandTransforms;

	private Transform cachedRightHandCustomPose;

	private Transform[] cachedRightHandJoints;

	private ovrAvatarTransform[] cachedRightHandTransforms;

	public EventHandler<PacketEventArgs> PacketRecorded;

	public void AssetLoadedCallback(OvrAvatarAsset asset)
	{
		assetLoadingIds.Remove(asset.assetID);
	}

	private void AddAvatarComponent(GameObject componentObject, ovrAvatarComponent component)
	{
		OvrAvatarComponent ovrAvatarComponent2 = componentObject.AddComponent<OvrAvatarComponent>();
		trackedComponents.Add(component.name, ovrAvatarComponent2);
		for (uint num = 0u; num < component.renderPartCount; num++)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = GetRenderPartName(component, num);
			gameObject.transform.SetParent(componentObject.transform);
			IntPtr renderPart = GetRenderPart(component, num);
			ovrAvatarRenderPartType ovrAvatarRenderPartType2 = CAPI.ovrAvatarRenderPart_GetType(renderPart);
			OvrAvatarRenderComponent item = ovrAvatarRenderPartType2 switch
			{
				ovrAvatarRenderPartType.SkinnedMeshRender => (OvrAvatarRenderComponent)AddSkinnedMeshRenderComponent(gameObject, CAPI.ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart)), 
				ovrAvatarRenderPartType.SkinnedMeshRenderPBS => AddSkinnedMeshRenderPBSComponent(gameObject, CAPI.ovrAvatarRenderPart_GetSkinnedMeshRenderPBS(renderPart)), 
				ovrAvatarRenderPartType.ProjectorRender => AddProjectorRenderComponent(gameObject, CAPI.ovrAvatarRenderPart_GetProjectorRender(renderPart)), 
				_ => throw new NotImplementedException($"Unsupported render part type: {ovrAvatarRenderPartType2.ToString()}"), 
			};
			ovrAvatarComponent2.RenderParts.Add(item);
		}
	}

	private OvrAvatarSkinnedMeshRenderComponent AddSkinnedMeshRenderComponent(GameObject gameObject, ovrAvatarRenderPart_SkinnedMeshRender skinnedMeshRender)
	{
		OvrAvatarSkinnedMeshRenderComponent ovrAvatarSkinnedMeshRenderComponent = gameObject.AddComponent<OvrAvatarSkinnedMeshRenderComponent>();
		ovrAvatarSkinnedMeshRenderComponent.Initialize(skinnedMeshRender, ThirdPersonLayer.layerIndex, FirstPersonLayer.layerIndex, renderPartCount++);
		return ovrAvatarSkinnedMeshRenderComponent;
	}

	private OvrAvatarSkinnedMeshRenderPBSComponent AddSkinnedMeshRenderPBSComponent(GameObject gameObject, ovrAvatarRenderPart_SkinnedMeshRenderPBS skinnedMeshRenderPBS)
	{
		OvrAvatarSkinnedMeshRenderPBSComponent ovrAvatarSkinnedMeshRenderPBSComponent = gameObject.AddComponent<OvrAvatarSkinnedMeshRenderPBSComponent>();
		ovrAvatarSkinnedMeshRenderPBSComponent.Initialize(skinnedMeshRenderPBS, ThirdPersonLayer.layerIndex, FirstPersonLayer.layerIndex, renderPartCount++);
		return ovrAvatarSkinnedMeshRenderPBSComponent;
	}

	private OvrAvatarProjectorRenderComponent AddProjectorRenderComponent(GameObject gameObject, ovrAvatarRenderPart_ProjectorRender projectorRender)
	{
		ovrAvatarComponent ovrAvatarComponent2 = CAPI.ovrAvatarComponent_Get(sdkAvatar, projectorRender.componentIndex);
		if (trackedComponents.TryGetValue(ovrAvatarComponent2.name, out var value) && projectorRender.renderPartIndex < value.RenderParts.Count)
		{
			OvrAvatarRenderComponent target = value.RenderParts[(int)projectorRender.renderPartIndex];
			OvrAvatarProjectorRenderComponent ovrAvatarProjectorRenderComponent = gameObject.AddComponent<OvrAvatarProjectorRenderComponent>();
			ovrAvatarProjectorRenderComponent.InitializeProjectorRender(projectorRender, target);
			return ovrAvatarProjectorRenderComponent;
		}
		return null;
	}

	private IntPtr GetRenderPart(ovrAvatarComponent component, uint renderPartIndex)
	{
		long num = Marshal.SizeOf(typeof(IntPtr)) * renderPartIndex;
		IntPtr ptr = new IntPtr(component.renderParts.ToInt64() + num);
		return (IntPtr)Marshal.PtrToStructure(ptr, typeof(IntPtr));
	}

	private void UpdateAvatarComponent(ovrAvatarComponent component)
	{
		if (!trackedComponents.TryGetValue(component.name, out var value))
		{
			throw new Exception($"trackedComponents didn't have {component.name}");
		}
		ConvertTransform(component.transform, value.transform);
		for (uint num = 0u; num < component.renderPartCount; num++)
		{
			if (value.RenderParts.Count > num)
			{
				OvrAvatarRenderComponent ovrAvatarRenderComponent = value.RenderParts[(int)num];
				IntPtr renderPart = GetRenderPart(component, num);
				ovrAvatarRenderPartType ovrAvatarRenderPartType2 = CAPI.ovrAvatarRenderPart_GetType(renderPart);
				switch (ovrAvatarRenderPartType2)
				{
				case ovrAvatarRenderPartType.SkinnedMeshRender:
					((OvrAvatarSkinnedMeshRenderComponent)ovrAvatarRenderComponent).UpdateSkinnedMeshRender(this, CAPI.ovrAvatarRenderPart_GetSkinnedMeshRender(renderPart));
					break;
				case ovrAvatarRenderPartType.SkinnedMeshRenderPBS:
					((OvrAvatarSkinnedMeshRenderPBSComponent)ovrAvatarRenderComponent).UpdateSkinnedMeshRenderPBS(this, CAPI.ovrAvatarRenderPart_GetSkinnedMeshRenderPBS(renderPart));
					break;
				case ovrAvatarRenderPartType.ProjectorRender:
					((OvrAvatarProjectorRenderComponent)ovrAvatarRenderComponent).UpdateProjectorRender(CAPI.ovrAvatarRenderPart_GetProjectorRender(renderPart));
					break;
				default:
					throw new NotImplementedException($"Unsupported render part type: {ovrAvatarRenderPartType2.ToString()}");
				}
			}
		}
	}

	private static string GetRenderPartName(ovrAvatarComponent component, uint renderPartIndex)
	{
		return component.name + "_renderPart_" + (int)renderPartIndex;
	}

	internal static void ConvertTransform(ovrAvatarTransform transform, Transform target)
	{
		Vector3 position = transform.position;
		position.z = 0f - position.z;
		Quaternion orientation = transform.orientation;
		orientation.x = 0f - orientation.x;
		orientation.y = 0f - orientation.y;
		target.localPosition = position;
		target.localRotation = orientation;
		target.localScale = transform.scale;
	}

	private static ovrAvatarTransform CreateOvrAvatarTransform(Vector3 position, Quaternion orientation)
	{
		return new ovrAvatarTransform
		{
			position = new Vector3(position.x, position.y, 0f - position.z),
			orientation = new Quaternion(0f - orientation.x, 0f - orientation.y, orientation.z, orientation.w),
			scale = Vector3.one
		};
	}

	private void RemoveAvatarComponent(string name)
	{
		trackedComponents.TryGetValue(name, out var value);
		UnityEngine.Object.Destroy(value.gameObject);
		trackedComponents.Remove(name);
	}

	private void UpdateSDKAvatarUnityState()
	{
		ulong num = CAPI.ovrAvatarComponent_Count(sdkAvatar);
		HashSet<string> hashSet = new HashSet<string>();
		for (ulong num2 = 0uL; num2 < num; num2++)
		{
			IntPtr intPtr = CAPI.ovrAvatarComponent_Get_Native(sdkAvatar, num2);
			ovrAvatarComponent component = (ovrAvatarComponent)Marshal.PtrToStructure(intPtr, typeof(ovrAvatarComponent));
			hashSet.Add(component.name);
			if (!trackedComponents.ContainsKey(component.name))
			{
				GameObject gameObject = null;
				Type type = null;
				if (intPtr == CAPI.ovrAvatarPose_GetBaseComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarBase);
					if (Base != null)
					{
						gameObject = Base.gameObject;
					}
				}
				else if (intPtr == CAPI.ovrAvatarPose_GetBodyComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarBody);
					if (Body != null)
					{
						gameObject = Body.gameObject;
					}
				}
				else if (intPtr == CAPI.ovrAvatarPose_GetLeftControllerComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarTouchController);
					if (ControllerLeft != null)
					{
						gameObject = ControllerLeft.gameObject;
					}
				}
				else if (intPtr == CAPI.ovrAvatarPose_GetRightControllerComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarTouchController);
					if (ControllerRight != null)
					{
						gameObject = ControllerRight.gameObject;
					}
				}
				else if (intPtr == CAPI.ovrAvatarPose_GetLeftHandComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarHand);
					if (HandLeft != null)
					{
						gameObject = HandLeft.gameObject;
					}
				}
				else if (intPtr == CAPI.ovrAvatarPose_GetRightHandComponent(sdkAvatar).renderComponent)
				{
					type = typeof(OvrAvatarHand);
					if (HandRight != null)
					{
						gameObject = HandRight.gameObject;
					}
				}
				if (gameObject == null && type == null)
				{
					gameObject = new GameObject();
					gameObject.name = component.name;
					gameObject.transform.SetParent(transform);
				}
				if (gameObject != null)
				{
					AddAvatarComponent(gameObject, component);
					if (type != null && gameObject.GetComponent(type) is IAvatarPart avatarPart)
					{
						avatarPart.OnAssetsLoaded();
					}
				}
			}
			UpdateAvatarComponent(component);
		}
		HashSet<string> hashSet2 = new HashSet<string>(trackedComponents.Keys);
		hashSet2.ExceptWith(hashSet);
		foreach (string item in hashSet2)
		{
			RemoveAvatarComponent(item);
		}
	}

	private void UpdateCustomPoses()
	{
		if (UpdatePoseRoot(LeftHandCustomPose, ref cachedLeftHandCustomPose, ref cachedLeftHandJoints, ref cachedLeftHandTransforms) && cachedLeftHandCustomPose == null && sdkAvatar != IntPtr.Zero)
		{
			CAPI.ovrAvatar_SetLeftHandGesture(sdkAvatar, ovrAvatarHandGesture.Default);
		}
		if (UpdatePoseRoot(RightHandCustomPose, ref cachedRightHandCustomPose, ref cachedRightHandJoints, ref cachedRightHandTransforms) && cachedRightHandCustomPose == null && sdkAvatar != IntPtr.Zero)
		{
			CAPI.ovrAvatar_SetRightHandGesture(sdkAvatar, ovrAvatarHandGesture.Default);
		}
		if (sdkAvatar != IntPtr.Zero)
		{
			if (cachedLeftHandCustomPose != null && UpdateTransforms(cachedLeftHandJoints, cachedLeftHandTransforms))
			{
				CAPI.ovrAvatar_SetLeftHandCustomGesture(sdkAvatar, (uint)cachedLeftHandTransforms.Length, cachedLeftHandTransforms);
			}
			if (cachedRightHandCustomPose != null && UpdateTransforms(cachedRightHandJoints, cachedRightHandTransforms))
			{
				CAPI.ovrAvatar_SetRightHandCustomGesture(sdkAvatar, (uint)cachedRightHandTransforms.Length, cachedRightHandTransforms);
			}
		}
	}

	private static bool UpdatePoseRoot(Transform poseRoot, ref Transform cachedPoseRoot, ref Transform[] cachedPoseJoints, ref ovrAvatarTransform[] transforms)
	{
		if (poseRoot == cachedPoseRoot)
		{
			return false;
		}
		if (!poseRoot)
		{
			cachedPoseRoot = null;
			cachedPoseJoints = null;
			transforms = null;
		}
		else
		{
			List<Transform> list = new List<Transform>();
			OrderJoints(poseRoot, list);
			cachedPoseRoot = poseRoot;
			cachedPoseJoints = list.ToArray();
			transforms = new ovrAvatarTransform[list.Count];
		}
		return true;
	}

	private static bool UpdateTransforms(Transform[] joints, ovrAvatarTransform[] transforms)
	{
		bool result = false;
		for (int i = 0; i < joints.Length; i++)
		{
			Transform transform = joints[i];
			ovrAvatarTransform ovrAvatarTransform2 = CreateOvrAvatarTransform(transform.localPosition, transform.localRotation);
			if (ovrAvatarTransform2.position != transforms[i].position || ovrAvatarTransform2.orientation != transforms[i].orientation)
			{
				transforms[i] = ovrAvatarTransform2;
				result = true;
			}
		}
		return result;
	}

	private static void OrderJoints(Transform transform, List<Transform> joints)
	{
		joints.Add(transform);
		for (int i = 0; i < transform.childCount; i++)
		{
			Transform child = transform.GetChild(i);
			OrderJoints(child, joints);
		}
	}

	private void AvatarSpecificationCallback(IntPtr avatarSpecification)
	{
		sdkAvatar = CAPI.ovrAvatar_Create(avatarSpecification, Capabilities);
		ShowLeftController(showLeftController);
		ShowRightController(showRightController);
		uint num = CAPI.ovrAvatar_GetReferencedAssetCount(sdkAvatar);
		for (uint num2 = 0u; num2 < num; num2++)
		{
			ulong num3 = CAPI.ovrAvatar_GetReferencedAsset(sdkAvatar, num2);
			if (OvrAvatarSDKManager.Instance.GetAsset(num3) == null)
			{
				OvrAvatarSDKManager.Instance.BeginLoadingAsset(num3, AssetLoadedCallback);
				assetLoadingIds.Add(num3);
			}
		}
	}

	private void Start()
	{
		ShowLeftController(StartWithControllers);
		ShowRightController(StartWithControllers);
		OvrAvatarSDKManager.Instance.RequestAvatarSpecification(oculusUserID, AvatarSpecificationCallback);
	}

	private void Update()
	{
		if (Driver != null && Driver.GetCurrentPose(out var pose))
		{
			if (RecordPackets)
			{
				RecordFrame(Time.deltaTime, pose);
			}
			if (ControllerLeft != null)
			{
				ControllerLeft.UpdatePose(pose.controllerLeftPose);
			}
			if (ControllerRight != null)
			{
				ControllerRight.UpdatePose(pose.controllerRightPose);
			}
			if (HandLeft != null)
			{
				HandLeft.UpdatePose(pose.controllerLeftPose);
			}
			if (HandRight != null)
			{
				HandRight.UpdatePose(pose.controllerRightPose);
			}
			if (Body != null)
			{
				Body.UpdatePose(pose.voiceAmplitude);
			}
			if (sdkAvatar != IntPtr.Zero)
			{
				ovrAvatarTransform headPose = CreateOvrAvatarTransform(pose.headPosition, pose.headRotation);
				ovrAvatarHandInputState inputStateLeft = CreateInputState(CreateOvrAvatarTransform(pose.handLeftPosition, pose.handLeftRotation), pose.controllerLeftPose);
				ovrAvatarHandInputState inputStateRight = CreateInputState(CreateOvrAvatarTransform(pose.handRightPosition, pose.handRightRotation), pose.controllerRightPose);
				foreach (float[] voiceUpdate in voiceUpdates)
				{
					CAPI.ovrAvatarPose_UpdateVoiceVisualization(sdkAvatar, voiceUpdate);
				}
				voiceUpdates.Clear();
				CAPI.ovrAvatarPose_UpdateBody(sdkAvatar, headPose);
				CAPI.ovrAvatarPose_UpdateHands(sdkAvatar, inputStateLeft, inputStateRight);
				CAPI.ovrAvatarPose_Finalize(sdkAvatar, Time.deltaTime);
			}
		}
		if (sdkAvatar != IntPtr.Zero && assetLoadingIds.Count == 0)
		{
			UpdateSDKAvatarUnityState();
			UpdateCustomPoses();
			if (!assetsFinishedLoading)
			{
				AssetsDoneLoading.Invoke();
				assetsFinishedLoading = true;
			}
		}
	}

	private ovrAvatarHandInputState CreateInputState(ovrAvatarTransform transform, OvrAvatarDriver.ControllerPose pose)
	{
		return new ovrAvatarHandInputState
		{
			transform = transform,
			buttonMask = pose.buttons,
			touchMask = pose.touches,
			joystickX = pose.joystickPosition.x,
			joystickY = pose.joystickPosition.y,
			indexTrigger = pose.indexTrigger,
			handTrigger = pose.handTrigger,
			isActive = pose.isActive
		};
	}

	public void ShowControllers(bool show)
	{
		ShowLeftController(show);
		ShowRightController(show);
	}

	public void ShowLeftController(bool show)
	{
		if (sdkAvatar != IntPtr.Zero)
		{
			CAPI.ovrAvatar_SetLeftControllerVisibility(sdkAvatar, show);
		}
		showLeftController = show;
	}

	public void ShowRightController(bool show)
	{
		if (sdkAvatar != IntPtr.Zero)
		{
			CAPI.ovrAvatar_SetRightControllerVisibility(sdkAvatar, show);
		}
		showRightController = show;
	}

	public void UpdateVoiceVisualization(float[] voiceSamples)
	{
		voiceUpdates.Add(voiceSamples);
	}

	private void RecordFrame(float deltaSeconds, OvrAvatarDriver.PoseFrame frame)
	{
		if (currentPacket == null)
		{
			currentPacket = new OvrAvatarPacket(frame);
			deltaSeconds = 0f;
		}
		float num = 0f;
		while (num < deltaSeconds)
		{
			float num2 = deltaSeconds - num;
			float num3 = 1f / 45f - currentPacket.Duration;
			if (num2 < num3)
			{
				currentPacket.AddFrame(frame, num2);
				num += num2;
				continue;
			}
			OvrAvatarDriver.PoseFrame finalFrame = currentPacket.FinalFrame;
			float t = num3 / num2;
			OvrAvatarDriver.PoseFrame poseFrame = OvrAvatarDriver.PoseFrame.Interpolate(finalFrame, frame, t);
			currentPacket.AddFrame(poseFrame, num3);
			num += num3;
			if (PacketRecorded != null)
			{
				PacketRecorded(this, new PacketEventArgs(currentPacket));
			}
			currentPacket = new OvrAvatarPacket(poseFrame);
		}
	}
}
