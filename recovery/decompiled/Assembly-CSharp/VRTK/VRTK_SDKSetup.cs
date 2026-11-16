using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace VRTK;

public sealed class VRTK_SDKSetup : MonoBehaviour
{
	public delegate void LoadEventHandler(VRTK_SDKManager sender, VRTK_SDKSetup setup);

	[Tooltip("Determines whether the SDK object references are automatically set to the objects of the selected SDKs. If this is true populating is done whenever the selected SDKs change.")]
	public bool autoPopulateObjectReferences = true;

	[Tooltip("A reference to the GameObject that is the user's boundary or play area, most likely provided by the SDK's Camera Rig.")]
	public GameObject actualBoundaries;

	[Tooltip("A reference to the GameObject that contains the VR camera, most likely provided by the SDK's Camera Rig Headset.")]
	public GameObject actualHeadset;

	[Tooltip("A reference to the GameObject that contains the SDK Left Hand Controller.")]
	public GameObject actualLeftController;

	[Tooltip("A reference to the GameObject that contains the SDK Right Hand Controller.")]
	public GameObject actualRightController;

	[Tooltip("A reference to the GameObject that models for the Left Hand Controller.")]
	public GameObject modelAliasLeftController;

	[Tooltip("A reference to the GameObject that models for the Right Hand Controller.")]
	public GameObject modelAliasRightController;

	[SerializeField]
	private VRTK_SDKInfo cachedSystemSDKInfo = VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>()[0];

	[SerializeField]
	private VRTK_SDKInfo cachedBoundariesSDKInfo = VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>()[0];

	[SerializeField]
	private VRTK_SDKInfo cachedHeadsetSDKInfo = VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>()[0];

	[SerializeField]
	private VRTK_SDKInfo cachedControllerSDKInfo = VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>()[0];

	private SDK_BaseSystem cachedSystemSDK;

	private SDK_BaseBoundaries cachedBoundariesSDK;

	private SDK_BaseHeadset cachedHeadsetSDK;

	private SDK_BaseController cachedControllerSDK;

	public VRTK_SDKInfo systemSDKInfo
	{
		get
		{
			return cachedSystemSDKInfo;
		}
		set
		{
			value = value ?? VRTK_SDKInfo.Create<SDK_BaseSystem, SDK_FallbackSystem, SDK_FallbackSystem>()[0];
			if (!(cachedSystemSDKInfo == value))
			{
				UnityEngine.Object.Destroy(cachedSystemSDK);
				cachedSystemSDK = null;
				cachedSystemSDKInfo = new VRTK_SDKInfo(value);
				PopulateObjectReferences(force: false);
			}
		}
	}

	public VRTK_SDKInfo boundariesSDKInfo
	{
		get
		{
			return cachedBoundariesSDKInfo;
		}
		set
		{
			value = value ?? VRTK_SDKInfo.Create<SDK_BaseBoundaries, SDK_FallbackBoundaries, SDK_FallbackBoundaries>()[0];
			if (!(cachedBoundariesSDKInfo == value))
			{
				UnityEngine.Object.Destroy(cachedBoundariesSDK);
				cachedBoundariesSDK = null;
				cachedBoundariesSDKInfo = new VRTK_SDKInfo(value);
				PopulateObjectReferences(force: false);
			}
		}
	}

	public VRTK_SDKInfo headsetSDKInfo
	{
		get
		{
			return cachedHeadsetSDKInfo;
		}
		set
		{
			value = value ?? VRTK_SDKInfo.Create<SDK_BaseHeadset, SDK_FallbackHeadset, SDK_FallbackHeadset>()[0];
			if (!(cachedHeadsetSDKInfo == value))
			{
				UnityEngine.Object.Destroy(cachedHeadsetSDK);
				cachedHeadsetSDK = null;
				cachedHeadsetSDKInfo = new VRTK_SDKInfo(value);
				PopulateObjectReferences(force: false);
			}
		}
	}

	public VRTK_SDKInfo controllerSDKInfo
	{
		get
		{
			return cachedControllerSDKInfo;
		}
		set
		{
			value = value ?? VRTK_SDKInfo.Create<SDK_BaseController, SDK_FallbackController, SDK_FallbackController>()[0];
			if (!(cachedControllerSDKInfo == value))
			{
				UnityEngine.Object.Destroy(cachedControllerSDK);
				cachedControllerSDK = null;
				cachedControllerSDKInfo = new VRTK_SDKInfo(value);
				PopulateObjectReferences(force: false);
			}
		}
	}

	public SDK_BaseSystem systemSDK
	{
		get
		{
			if (cachedSystemSDK == null)
			{
				HandleSDKGetter<SDK_BaseSystem>("System", systemSDKInfo, VRTK_SDKManager.InstalledSystemSDKInfos);
				cachedSystemSDK = (SDK_BaseSystem)ScriptableObject.CreateInstance(systemSDKInfo.type);
			}
			return cachedSystemSDK;
		}
	}

	public SDK_BaseBoundaries boundariesSDK
	{
		get
		{
			if (cachedBoundariesSDK == null)
			{
				HandleSDKGetter<SDK_BaseBoundaries>("Boundaries", boundariesSDKInfo, VRTK_SDKManager.InstalledBoundariesSDKInfos);
				cachedBoundariesSDK = (SDK_BaseBoundaries)ScriptableObject.CreateInstance(boundariesSDKInfo.type);
			}
			return cachedBoundariesSDK;
		}
	}

	public SDK_BaseHeadset headsetSDK
	{
		get
		{
			if (cachedHeadsetSDK == null)
			{
				HandleSDKGetter<SDK_BaseHeadset>("Headset", headsetSDKInfo, VRTK_SDKManager.InstalledHeadsetSDKInfos);
				cachedHeadsetSDK = (SDK_BaseHeadset)ScriptableObject.CreateInstance(headsetSDKInfo.type);
			}
			return cachedHeadsetSDK;
		}
	}

	public SDK_BaseController controllerSDK
	{
		get
		{
			if (cachedControllerSDK == null)
			{
				HandleSDKGetter<SDK_BaseController>("Controller", controllerSDKInfo, VRTK_SDKManager.InstalledControllerSDKInfos);
				cachedControllerSDK = (SDK_BaseController)ScriptableObject.CreateInstance(controllerSDKInfo.type);
			}
			return cachedControllerSDK;
		}
	}

	public string[] usedVRDeviceNames
	{
		get
		{
			VRTK_SDKInfo[] source = new VRTK_SDKInfo[4] { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo };
			return source.Select((VRTK_SDKInfo info) => info.description.vrDeviceName).Distinct().ToArray();
		}
	}

	public bool isValid => GetSimplifiedErrorDescriptions().Length == 0;

	public event LoadEventHandler Loaded;

	public event LoadEventHandler Unloaded;

	public void PopulateObjectReferences(bool force)
	{
		if (force || autoPopulateObjectReferences)
		{
			VRTK_SDK_Bridge.InvalidateCaches();
			actualBoundaries = null;
			actualHeadset = null;
			actualLeftController = null;
			actualRightController = null;
			modelAliasLeftController = null;
			modelAliasRightController = null;
			Transform playArea = boundariesSDK.GetPlayArea();
			Transform headset = headsetSDK.GetHeadset();
			actualBoundaries = ((!(playArea == null)) ? playArea.gameObject : null);
			actualHeadset = ((!(headset == null)) ? headset.gameObject : null);
			actualLeftController = controllerSDK.GetControllerLeftHand(actual: true);
			actualRightController = controllerSDK.GetControllerRightHand(actual: true);
			modelAliasLeftController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Left);
			modelAliasRightController = controllerSDK.GetControllerModel(SDK_BaseController.ControllerHand.Right);
		}
	}

	public string[] GetSimplifiedErrorDescriptions()
	{
		List<string> list = new List<string>();
		ReadOnlyCollection<VRTK_SDKInfo>[] array = new ReadOnlyCollection<VRTK_SDKInfo>[4]
		{
			VRTK_SDKManager.InstalledSystemSDKInfos,
			VRTK_SDKManager.InstalledBoundariesSDKInfos,
			VRTK_SDKManager.InstalledHeadsetSDKInfos,
			VRTK_SDKManager.InstalledControllerSDKInfos
		};
		VRTK_SDKInfo[] array2 = new VRTK_SDKInfo[4] { systemSDKInfo, boundariesSDKInfo, headsetSDKInfo, controllerSDKInfo };
		for (int i = 0; i < array.Length; i++)
		{
			ReadOnlyCollection<VRTK_SDKInfo> readOnlyCollection = array[i];
			VRTK_SDKInfo vRTK_SDKInfo = array2[i];
			Type baseType = vRTK_SDKInfo.type.BaseType;
			if (baseType != null)
			{
				if (vRTK_SDKInfo.originalTypeNameWhenFallbackIsUsed != null)
				{
					list.Add($"The SDK '{vRTK_SDKInfo.originalTypeNameWhenFallbackIsUsed}' doesn't exist anymore.");
				}
				else if (vRTK_SDKInfo.description.describesFallbackSDK)
				{
					list.Add("A fallback SDK is used. Make sure to set a real SDK.");
				}
				else if (!readOnlyCollection.Contains(vRTK_SDKInfo))
				{
					list.Add($"The vendor SDK for '{vRTK_SDKInfo.description.prettyName}' is not installed.");
				}
			}
		}
		if (usedVRDeviceNames.Except(new string[1] { "None" }).Count() > 1)
		{
			list.Add("The current SDK selection uses multiple VR Devices. It's not possible to use more than one VR Device at the same time.");
		}
		return list.Distinct().ToArray();
	}

	public void OnLoaded(VRTK_SDKManager sender)
	{
		List<SDK_Base> list = new SDK_Base[4] { systemSDK, boundariesSDK, headsetSDK, controllerSDK }.ToList();
		list.ForEach((SDK_Base sdkBase) =>
		{
			sdkBase.OnBeforeSetupLoad(this);
		});
		gameObject.SetActive(value: true);
		VRTK_SDK_Bridge.InvalidateCaches();
		SetupHeadset();
		SetupControllers();
		boundariesSDK.InitBoundaries();
		list.ForEach((SDK_Base sdkBase) =>
		{
			sdkBase.OnAfterSetupLoad(this);
		});
		Loaded?.Invoke(sender, this);
	}

	public void OnUnloaded(VRTK_SDKManager sender)
	{
		List<SDK_Base> list = new SDK_Base[4] { systemSDK, boundariesSDK, headsetSDK, controllerSDK }.ToList();
		list.ForEach((SDK_Base sdkBase) =>
		{
			sdkBase.OnBeforeSetupUnload(this);
		});
		gameObject.SetActive(value: false);
		list.ForEach((SDK_Base sdkBase) =>
		{
			sdkBase.OnAfterSetupUnload(this);
		});
		Unloaded?.Invoke(sender, this);
	}

	private static void HandleSDKGetter<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
	{
		if (!VRTK_SharedMethods.IsEditTime())
		{
			string sDKErrorDescription = GetSDKErrorDescription<BaseType>(prettyName, info, installedInfos);
			if (!string.IsNullOrEmpty(sDKErrorDescription))
			{
				VRTK_Logger.Error(sDKErrorDescription);
			}
		}
	}

	private static string GetSDKErrorDescription<BaseType>(string prettyName, VRTK_SDKInfo info, IEnumerable<VRTK_SDKInfo> installedInfos) where BaseType : SDK_Base
	{
		Type type = info.type;
		Type typeFromHandle = typeof(BaseType);
		Type type2 = VRTK_SDKManager.SDKFallbackTypesByBaseType[typeFromHandle];
		if (type == type2)
		{
			return string.Format("The fallback {0} SDK is being used because there is no other {0} SDK set in the SDK Setup.", prettyName);
		}
		if (!typeFromHandle.IsAssignableFrom(type) || type2.IsAssignableFrom(type))
		{
			string text = $"The fallback {prettyName} SDK is being used despite being set to '{type.Name}'.";
			if (installedInfos.Select((VRTK_SDKInfo installedInfo) => installedInfo.type).Contains(type))
			{
				return text + " Its needed scripting define symbols are not added. You can click the GameObject with the `VRTK_SDKManager` script attached to it in Edit Mode and choose to automatically let the manager handle the scripting define symbols.";
			}
			return text + " The needed vendor SDK isn't installed.";
		}
		return null;
	}

	private void SetupHeadset()
	{
		if ((bool)actualHeadset && !actualHeadset.GetComponent<VRTK_TrackedHeadset>())
		{
			actualHeadset.AddComponent<VRTK_TrackedHeadset>();
		}
	}

	private void SetupControllers()
	{
		Action<GameObject, GameObject> action = (GameObject scriptAliasGameObject, GameObject actualGameObject) =>
		{
			if (!(scriptAliasGameObject == null))
			{
				Transform transform = scriptAliasGameObject.transform;
				Transform transform2 = actualGameObject.transform;
				if (transform.parent != transform2)
				{
					Vector3 localScale = transform.localScale;
					transform.SetParent(transform2);
					transform.localScale = localScale;
				}
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		};
		if (actualLeftController != null)
		{
			action(VRTK_SDKManager.instance.scriptAliasLeftController, actualLeftController);
			if (actualLeftController.GetComponent<VRTK_TrackedController>() == null)
			{
				actualLeftController.AddComponent<VRTK_TrackedController>();
			}
		}
		if (actualRightController != null)
		{
			action(VRTK_SDKManager.instance.scriptAliasRightController, actualRightController);
			if (actualRightController.GetComponent<VRTK_TrackedController>() == null)
			{
				actualRightController.AddComponent<VRTK_TrackedController>();
			}
		}
	}
}
