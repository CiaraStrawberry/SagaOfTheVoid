using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Valve.VR;

namespace VRTK;

[SDK_Description(typeof(SDK_SteamVRSystem), 0)]
public class SDK_SteamVRController : SDK_BaseController
{
	protected SteamVR_TrackedObject cachedLeftTrackedObject;

	protected SteamVR_TrackedObject cachedRightTrackedObject;

	protected Dictionary<GameObject, SteamVR_TrackedObject> cachedTrackedObjectsByGameObject = new Dictionary<GameObject, SteamVR_TrackedObject>();

	protected Dictionary<uint, SteamVR_TrackedObject> cachedTrackedObjectsByIndex = new Dictionary<uint, SteamVR_TrackedObject>();

	protected ushort maxHapticVibration = 3999;

	public override void OnAfterSetupUnload(VRTK_SDKSetup setup)
	{
		base.OnAfterSetupUnload(setup);
		SteamVR_ControllerManager component = setup.actualLeftController.transform.parent.GetComponent<SteamVR_ControllerManager>();
		FieldInfo field = typeof(SteamVR_ControllerManager).GetField("connected", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null)
		{
			bool[] array = (bool[])field.GetValue(component);
			Array.Clear(array, 0, array.Length);
			field.SetValue(component, array);
		}
	}

	public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
	}

	public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
	}

	public override string GetControllerDefaultColliderPath(ControllerHand hand)
	{
		string result = "ControllerColliders/Fallback";
		switch (VRTK_DeviceFinder.GetHeadsetType(summary: true))
		{
		case VRTK_DeviceFinder.Headsets.OculusRift:
			result = ((hand != ControllerHand.Left) ? "ControllerColliders/SteamVROculusTouch_Right" : "ControllerColliders/SteamVROculusTouch_Left");
			break;
		case VRTK_DeviceFinder.Headsets.Vive:
			result = "ControllerColliders/HTCVive";
			break;
		}
		return result;
	}

	public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
	{
		string text = ((!fullPath) ? string.Empty : "/attach");
		return element switch
		{
			ControllerElements.AttachPoint => "tip/attach", 
			ControllerElements.Trigger => "trigger" + text, 
			ControllerElements.GripLeft => GetControllerGripPath(hand, text, ControllerHand.Left), 
			ControllerElements.GripRight => GetControllerGripPath(hand, text, ControllerHand.Right), 
			ControllerElements.Touchpad => GetControllerTouchpadPath(hand, text), 
			ControllerElements.ButtonOne => GetControllerButtonOnePath(hand, text), 
			ControllerElements.ButtonTwo => GetControllerButtonTwoPath(hand, text), 
			ControllerElements.SystemMenu => GetControllerSystemMenuPath(hand, text), 
			ControllerElements.StartMenu => GetControllerStartMenuPath(hand, text), 
			ControllerElements.Body => "body", 
			_ => null, 
		};
	}

	public override uint GetControllerIndex(GameObject controller)
	{
		SteamVR_TrackedObject trackedObject = GetTrackedObject(controller);
		return (uint)((!(trackedObject != null)) ? SteamVR_TrackedObject.EIndex.None : trackedObject.index);
	}

	public override GameObject GetControllerByIndex(uint index, bool actual = false)
	{
		SetTrackedControllerCaches();
		if (index < uint.MaxValue)
		{
			VRTK_SDKManager instance = VRTK_SDKManager.instance;
			if (instance != null)
			{
				if (cachedLeftTrackedObject != null && cachedLeftTrackedObject.index == (SteamVR_TrackedObject.EIndex)index)
				{
					return (!actual) ? instance.scriptAliasLeftController : instance.loadedSetup.actualLeftController;
				}
				if (cachedRightTrackedObject != null && cachedRightTrackedObject.index == (SteamVR_TrackedObject.EIndex)index)
				{
					return (!actual) ? instance.scriptAliasRightController : instance.loadedSetup.actualRightController;
				}
			}
			if (cachedTrackedObjectsByIndex.ContainsKey(index) && cachedTrackedObjectsByIndex[index] != null)
			{
				return cachedTrackedObjectsByIndex[index].gameObject;
			}
		}
		return null;
	}

	public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
	{
		SteamVR_TrackedObject trackedObject = GetTrackedObject(controllerReference.actual);
		if (trackedObject != null)
		{
			return (!(trackedObject.origin != null)) ? trackedObject.transform.parent : trackedObject.origin;
		}
		return null;
	}

	public override Transform GenerateControllerPointerOrigin(GameObject parent)
	{
		VRTK_DeviceFinder.Headsets headsetType = VRTK_DeviceFinder.GetHeadsetType(summary: true);
		if (headsetType == VRTK_DeviceFinder.Headsets.OculusRift && (IsControllerLeftHand(parent) || IsControllerRightHand(parent)))
		{
			GameObject gameObject = new GameObject(parent.name + " _CustomPointerOrigin");
			gameObject.transform.SetParent(parent.transform);
			gameObject.transform.localEulerAngles = new Vector3(40f, 0f, 0f);
			gameObject.transform.localPosition = new Vector3((!IsControllerLeftHand(parent)) ? (-0.0081f) : 0.0081f, -0.0273f, -0.0311f);
			return gameObject.transform;
		}
		return null;
	}

	public override GameObject GetControllerLeftHand(bool actual = false)
	{
		GameObject gameObject = GetSDKManagerControllerLeftHand(actual);
		if (gameObject == null && actual)
		{
			gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (left)");
		}
		return gameObject;
	}

	public override GameObject GetControllerRightHand(bool actual = false)
	{
		GameObject gameObject = GetSDKManagerControllerRightHand(actual);
		if (gameObject == null && actual)
		{
			gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<SteamVR_ControllerManager>("Controller (right)");
		}
		return gameObject;
	}

	public override bool IsControllerLeftHand(GameObject controller)
	{
		return CheckActualOrScriptAliasControllerIsLeftHand(controller);
	}

	public override bool IsControllerRightHand(GameObject controller)
	{
		return CheckActualOrScriptAliasControllerIsRightHand(controller);
	}

	public override bool IsControllerLeftHand(GameObject controller, bool actual)
	{
		return CheckControllerLeftHand(controller, actual);
	}

	public override bool IsControllerRightHand(GameObject controller, bool actual)
	{
		return CheckControllerRightHand(controller, actual);
	}

	public override GameObject GetControllerModel(GameObject controller)
	{
		return GetControllerModelFromController(controller);
	}

	public override GameObject GetControllerModel(ControllerHand hand)
	{
		GameObject gameObject = GetSDKManagerControllerModelForHand(hand);
		if (gameObject == null)
		{
			GameObject gameObject2 = null;
			switch (hand)
			{
			case ControllerHand.Left:
				gameObject2 = GetControllerLeftHand(actual: true);
				break;
			case ControllerHand.Right:
				gameObject2 = GetControllerRightHand(actual: true);
				break;
			}
			if (gameObject2 != null)
			{
				gameObject = gameObject2.transform.Find("Model").gameObject;
			}
		}
		return gameObject;
	}

	public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
	{
		SteamVR_RenderModel componentInChildren = controllerReference.actual.GetComponentInChildren<SteamVR_RenderModel>();
		return (!(componentInChildren != null)) ? null : componentInChildren.gameObject;
	}

	public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
	{
		SteamVR_RenderModel component = renderModel.GetComponent<SteamVR_RenderModel>();
		if (component != null)
		{
			component.controllerModeState.bScrollWheelVisible = state;
		}
	}

	public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex < uint.MaxValue)
		{
			float num = (float)(int)maxHapticVibration * strength;
			SteamVR_Controller.Device device = SteamVR_Controller.Input((int)realIndex);
			device.TriggerHapticPulse((ushort)num);
		}
	}

	public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		return false;
	}

	public override SDK_ControllerHapticModifiers GetHapticModifiers()
	{
		SDK_ControllerHapticModifiers sDK_ControllerHapticModifiers = new SDK_ControllerHapticModifiers();
		sDK_ControllerHapticModifiers.maxHapticVibration = maxHapticVibration;
		return sDK_ControllerHapticModifiers;
	}

	public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex == 0 || realIndex >= uint.MaxValue)
		{
			return Vector3.zero;
		}
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)realIndex);
		return device.velocity;
	}

	public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex == 0 || realIndex >= uint.MaxValue)
		{
			return Vector3.zero;
		}
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)realIndex);
		return device.angularVelocity;
	}

	public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
	{
		return !isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity);
	}

	public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex >= uint.MaxValue)
		{
			return Vector2.zero;
		}
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)realIndex);
		return buttonType switch
		{
			ButtonTypes.Touchpad => device.GetAxis(), 
			ButtonTypes.Trigger => device.GetAxis(EVRButtonId.k_EButton_Axis1), 
			_ => Vector2.zero, 
		};
	}

	public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex >= uint.MaxValue)
		{
			return 0f;
		}
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)realIndex);
		return (buttonType != ButtonTypes.Trigger && buttonType != ButtonTypes.TriggerHairline) ? 0f : device.hairTriggerDelta;
	}

	public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
	{
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		if (realIndex >= uint.MaxValue)
		{
			return false;
		}
		switch (buttonType)
		{
		case ButtonTypes.Trigger:
			return IsButtonPressed(realIndex, pressType, 8589934592uL);
		case ButtonTypes.TriggerHairline:
			switch (pressType)
			{
			case ButtonPressTypes.PressDown:
				return SteamVR_Controller.Input((int)realIndex).GetHairTriggerDown();
			case ButtonPressTypes.PressUp:
				return SteamVR_Controller.Input((int)realIndex).GetHairTriggerUp();
			}
			break;
		case ButtonTypes.Grip:
			return IsButtonPressed(realIndex, pressType, 4uL);
		case ButtonTypes.Touchpad:
			return IsButtonPressed(realIndex, pressType, 4294967296uL);
		case ButtonTypes.ButtonOne:
			return IsButtonPressed(realIndex, pressType, 128uL);
		case ButtonTypes.ButtonTwo:
			return IsButtonPressed(realIndex, pressType, 2uL);
		case ButtonTypes.StartMenu:
			return IsButtonPressed(realIndex, pressType, 1uL);
		}
		return false;
	}

	protected virtual void Awake()
	{
		SteamVR_Events.System(EVREventType.VREvent_TrackedDeviceRoleChanged).Listen(OnTrackedDeviceRoleChanged);
		SetTrackedControllerCaches(forceRefresh: true);
	}

	protected virtual void OnTrackedDeviceRoleChanged<T>(T ignoredArgument)
	{
		SetTrackedControllerCaches(forceRefresh: true);
	}

	protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
	{
		if (forceRefresh)
		{
			cachedLeftTrackedObject = null;
			cachedRightTrackedObject = null;
			cachedTrackedObjectsByGameObject.Clear();
			cachedTrackedObjectsByIndex.Clear();
		}
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			if (cachedLeftTrackedObject == null && (bool)instance.loadedSetup.actualLeftController)
			{
				cachedLeftTrackedObject = instance.loadedSetup.actualLeftController.GetComponent<SteamVR_TrackedObject>();
			}
			if (cachedRightTrackedObject == null && (bool)instance.loadedSetup.actualRightController)
			{
				cachedRightTrackedObject = instance.loadedSetup.actualRightController.GetComponent<SteamVR_TrackedObject>();
			}
		}
	}

	protected virtual SteamVR_TrackedObject GetTrackedObject(GameObject controller)
	{
		SetTrackedControllerCaches();
		if (IsControllerLeftHand(controller))
		{
			return cachedLeftTrackedObject;
		}
		if (IsControllerRightHand(controller))
		{
			return cachedRightTrackedObject;
		}
		if (controller == null)
		{
			return null;
		}
		if (cachedTrackedObjectsByGameObject.ContainsKey(controller) && cachedTrackedObjectsByGameObject[controller] != null)
		{
			return cachedTrackedObjectsByGameObject[controller];
		}
		SteamVR_TrackedObject component = controller.GetComponent<SteamVR_TrackedObject>();
		if (component != null)
		{
			cachedTrackedObjectsByGameObject.Add(controller, component);
			cachedTrackedObjectsByIndex.Add((uint)component.index, component);
		}
		return component;
	}

	protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, ulong button)
	{
		if (index >= uint.MaxValue)
		{
			return false;
		}
		SteamVR_Controller.Device device = SteamVR_Controller.Input((int)index);
		return type switch
		{
			ButtonPressTypes.Press => device.GetPress(button), 
			ButtonPressTypes.PressDown => device.GetPressDown(button), 
			ButtonPressTypes.PressUp => device.GetPressUp(button), 
			ButtonPressTypes.Touch => device.GetTouch(button), 
			ButtonPressTypes.TouchDown => device.GetTouchDown(button), 
			ButtonPressTypes.TouchUp => device.GetTouchUp(button), 
			_ => false, 
		};
	}

	protected virtual string GetControllerGripPath(ControllerHand hand, string suffix, ControllerHand forceHand)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => ((forceHand != ControllerHand.Left) ? "rgrip" : "lgrip") + suffix, 
			VRTK_DeviceFinder.Headsets.OculusRift => "grip" + suffix, 
			_ => null, 
		};
	}

	protected virtual string GetControllerTouchpadPath(ControllerHand hand, string suffix)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => "trackpad" + suffix, 
			VRTK_DeviceFinder.Headsets.OculusRift => "thumbstick" + suffix, 
			_ => null, 
		};
	}

	protected virtual string GetControllerButtonOnePath(ControllerHand hand, string suffix)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => null, 
			VRTK_DeviceFinder.Headsets.OculusRift => ((hand != ControllerHand.Left) ? "a_button" : "x_button") + suffix, 
			_ => null, 
		};
	}

	protected virtual string GetControllerButtonTwoPath(ControllerHand hand, string suffix)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => "button" + suffix, 
			VRTK_DeviceFinder.Headsets.OculusRift => ((hand != ControllerHand.Left) ? "b_button" : "y_button") + suffix, 
			_ => null, 
		};
	}

	protected virtual string GetControllerSystemMenuPath(ControllerHand hand, string suffix)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => "sys_button" + suffix, 
			VRTK_DeviceFinder.Headsets.OculusRift => ((hand != ControllerHand.Left) ? "home_button" : "enter_button") + suffix, 
			_ => null, 
		};
	}

	protected virtual string GetControllerStartMenuPath(ControllerHand hand, string suffix)
	{
		return VRTK_DeviceFinder.GetHeadsetType(summary: true) switch
		{
			VRTK_DeviceFinder.Headsets.Vive => null, 
			VRTK_DeviceFinder.Headsets.OculusRift => ((hand != ControllerHand.Left) ? "home_button" : "enter_button") + suffix, 
			_ => null, 
		};
	}
}
