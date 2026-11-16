using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

[SDK_Description(typeof(SDK_OculusSystem), 0)]
public class SDK_OculusController : SDK_BaseController
{
	protected SDK_OculusBoundaries cachedBoundariesSDK;

	protected VRTK_TrackedController cachedLeftController;

	protected VRTK_TrackedController cachedRightController;

	protected OVRInput.Controller[] touchControllers = new OVRInput.Controller[2]
	{
		OVRInput.Controller.LTouch,
		OVRInput.Controller.RTouch
	};

	protected OVRInput.RawAxis2D[] touchpads = new OVRInput.RawAxis2D[2]
	{
		OVRInput.RawAxis2D.LThumbstick,
		OVRInput.RawAxis2D.RThumbstick
	};

	protected OVRInput.RawAxis1D[] triggers = new OVRInput.RawAxis1D[2]
	{
		OVRInput.RawAxis1D.LIndexTrigger,
		OVRInput.RawAxis1D.RIndexTrigger
	};

	protected OVRInput.RawAxis1D[] grips = new OVRInput.RawAxis1D[2]
	{
		OVRInput.RawAxis1D.LHandTrigger,
		OVRInput.RawAxis1D.RHandTrigger
	};

	protected Quaternion[] previousControllerRotations = new Quaternion[2];

	protected Quaternion[] currentControllerRotations = new Quaternion[2];

	protected bool[] previousHairTriggerState = new bool[2];

	protected bool[] currentHairTriggerState = new bool[2];

	protected bool[] previousHairGripState = new bool[2];

	protected bool[] currentHairGripState = new bool[2];

	protected float[] hairTriggerLimit = new float[2];

	protected float[] hairGripLimit = new float[2];

	protected OVRHapticsClip hapticsProceduralClipLeft;

	protected OVRHapticsClip hapticsProceduralClipRight;

	public override void OnAfterSetupLoad(VRTK_SDKSetup setup)
	{
		base.OnAfterSetupLoad(setup);
		if (hapticsProceduralClipLeft == null && hapticsProceduralClipRight == null)
		{
			OVRHaptics.Config.Load();
			hapticsProceduralClipLeft = new OVRHapticsClip();
			hapticsProceduralClipRight = new OVRHapticsClip();
		}
	}

	public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
	}

	public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
		CalculateAngularVelocity(controllerReference);
	}

	public override string GetControllerDefaultColliderPath(ControllerHand hand)
	{
		if (HasAvatar())
		{
			return "ControllerColliders/OculusTouch_" + hand;
		}
		return "ControllerColliders/Fallback";
	}

	public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
	{
		if (GetAvatar() != null)
		{
			string text = ((!fullPath) ? string.Empty : string.Empty);
			string text2 = "controller_" + ((hand != ControllerHand.Left) ? "right" : "left") + "_renderPart_0";
			string text3 = ((hand != ControllerHand.Left) ? "r" : "l") + "ctrl:";
			string text4 = text3 + ((hand != ControllerHand.Left) ? "right" : "left") + "_touch_controller_world";
			string text5 = text2 + "/" + text4 + "/" + text3 + "b_";
			switch (element)
			{
			case ControllerElements.AttachPoint:
				return null;
			case ControllerElements.Trigger:
				return text5 + "trigger" + text;
			case ControllerElements.GripLeft:
				return text5 + "hold" + text;
			case ControllerElements.GripRight:
				return text5 + "hold" + text;
			case ControllerElements.Touchpad:
				return text5 + "stick/" + text3 + "b_stick_IGNORE" + text;
			case ControllerElements.ButtonOne:
				return text5 + "button01" + text;
			case ControllerElements.ButtonTwo:
				return text5 + "button02" + text;
			case ControllerElements.SystemMenu:
				return text5 + "button03" + text;
			case ControllerElements.StartMenu:
				return text5 + "button03" + text;
			case ControllerElements.Body:
				return text2;
			}
		}
		return null;
	}

	public override uint GetControllerIndex(GameObject controller)
	{
		VRTK_TrackedController trackedObject = GetTrackedObject(controller);
		return (!(trackedObject != null)) ? uint.MaxValue : trackedObject.index;
	}

	public override GameObject GetControllerByIndex(uint index, bool actual = false)
	{
		SetTrackedControllerCaches();
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			if (cachedLeftController != null && cachedLeftController.index == index)
			{
				return (!actual) ? instance.scriptAliasLeftController : instance.loadedSetup.actualLeftController;
			}
			if (cachedRightController != null && cachedRightController.index == index)
			{
				return (!actual) ? instance.scriptAliasRightController : instance.loadedSetup.actualRightController;
			}
		}
		return null;
	}

	public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
	{
		return VRTK_SDK_Bridge.GetPlayArea();
	}

	public override Transform GenerateControllerPointerOrigin(GameObject parent)
	{
		return null;
	}

	public override GameObject GetControllerLeftHand(bool actual = false)
	{
		GameObject gameObject = GetSDKManagerControllerLeftHand(actual);
		if (gameObject == null && actual)
		{
			gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/LeftHandAnchor");
		}
		return gameObject;
	}

	public override GameObject GetControllerRightHand(bool actual = false)
	{
		GameObject gameObject = GetSDKManagerControllerRightHand(actual);
		if (gameObject == null && actual)
		{
			gameObject = VRTK_SharedMethods.FindEvenInactiveGameObject<OVRCameraRig>("TrackingSpace/RightHandAnchor");
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
			GameObject avatar = GetAvatar();
			switch (hand)
			{
			case ControllerHand.Left:
				if (avatar != null)
				{
					gameObject = avatar.transform.Find("controller_left").gameObject;
					break;
				}
				gameObject = GetControllerLeftHand(actual: true);
				gameObject = ((!(gameObject != null) || gameObject.transform.childCount <= 0) ? null : gameObject.transform.GetChild(0).gameObject);
				break;
			case ControllerHand.Right:
				if (avatar != null)
				{
					gameObject = avatar.transform.Find("controller_right").gameObject;
					break;
				}
				gameObject = GetControllerRightHand(actual: true);
				gameObject = ((!(gameObject != null) || gameObject.transform.childCount <= 0) ? null : gameObject.transform.GetChild(0).gameObject);
				break;
			}
		}
		return gameObject;
	}

	public override GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference)
	{
		return null;
	}

	public override void SetControllerRenderModelWheel(GameObject renderModel, bool state)
	{
	}

	public override void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f)
	{
		if (VRTK_ControllerReference.IsValid(controllerReference))
		{
			uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
			GameObject controllerByIndex = GetControllerByIndex(realIndex);
			if (IsControllerLeftHand(controllerByIndex))
			{
				hapticsProceduralClipLeft.Reset();
				hapticsProceduralClipLeft.WriteSample((byte)(strength * 255f));
				OVRHaptics.LeftChannel.Preempt(hapticsProceduralClipLeft);
			}
			else if (IsControllerRightHand(controllerByIndex))
			{
				hapticsProceduralClipRight.Reset();
				hapticsProceduralClipRight.WriteSample((byte)(strength * 255f));
				OVRHaptics.RightChannel.Preempt(hapticsProceduralClipRight);
			}
		}
	}

	public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		if (VRTK_ControllerReference.IsValid(controllerReference))
		{
			uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
			GameObject controllerByIndex = GetControllerByIndex(realIndex);
			if (IsControllerLeftHand(controllerByIndex))
			{
				OVRHaptics.LeftChannel.Preempt(new OVRHapticsClip(clip));
			}
			else if (IsControllerRightHand(controllerByIndex))
			{
				OVRHaptics.RightChannel.Preempt(new OVRHapticsClip(clip));
			}
		}
		return true;
	}

	public override SDK_ControllerHapticModifiers GetHapticModifiers()
	{
		SDK_ControllerHapticModifiers sDK_ControllerHapticModifiers = new SDK_ControllerHapticModifiers();
		sDK_ControllerHapticModifiers.durationModifier = 0.8f;
		sDK_ControllerHapticModifiers.intervalModifier = 1f;
		return sDK_ControllerHapticModifiers;
	}

	public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return Vector3.zero;
		}
		VRTK_TrackedController trackedObject = GetTrackedObject(controllerReference.actual);
		return OVRInput.GetLocalControllerVelocity(touchControllers[trackedObject.index]);
	}

	public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return Vector3.zero;
		}
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		Quaternion quaternion = currentControllerRotations[realIndex] * Quaternion.Inverse(previousControllerRotations[realIndex]);
		return new Vector3(Mathf.DeltaAngle(0f, quaternion.eulerAngles.x), Mathf.DeltaAngle(0f, quaternion.eulerAngles.y), Mathf.DeltaAngle(0f, quaternion.eulerAngles.z));
	}

	public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
	{
		return VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity);
	}

	public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return Vector2.zero;
		}
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		VRTK_TrackedController trackedObject = GetTrackedObject(controllerReference.actual);
		if (trackedObject != null)
		{
			switch (buttonType)
			{
			case ButtonTypes.Touchpad:
				return OVRInput.Get(touchpads[realIndex], touchControllers[realIndex]);
			case ButtonTypes.Trigger:
				return new Vector2(OVRInput.Get(triggers[realIndex], touchControllers[realIndex]), 0f);
			case ButtonTypes.Grip:
				return new Vector2(OVRInput.Get(grips[realIndex], touchControllers[realIndex]), 0f);
			}
		}
		return Vector2.zero;
	}

	public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		return (!VRTK_ControllerReference.IsValid(controllerReference)) ? 0f : 0.1f;
	}

	public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return false;
		}
		uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		switch (buttonType)
		{
		case ButtonTypes.Trigger:
			switch (pressType)
			{
			case ButtonPressTypes.Press:
			case ButtonPressTypes.PressDown:
			case ButtonPressTypes.PressUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Button.PrimaryIndexTrigger);
			case ButtonPressTypes.Touch:
			case ButtonPressTypes.TouchDown:
			case ButtonPressTypes.TouchUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Touch.PrimaryIndexTrigger);
			}
			break;
		case ButtonTypes.TriggerHairline:
			switch (pressType)
			{
			case ButtonPressTypes.PressDown:
				return currentHairTriggerState[realIndex] && !previousHairTriggerState[realIndex];
			case ButtonPressTypes.PressUp:
				return !currentHairTriggerState[realIndex] && previousHairTriggerState[realIndex];
			}
			break;
		case ButtonTypes.Grip:
			return IsButtonPressed(realIndex, pressType, OVRInput.Button.PrimaryHandTrigger);
		case ButtonTypes.GripHairline:
			switch (pressType)
			{
			case ButtonPressTypes.PressDown:
				return currentHairGripState[realIndex] && !previousHairGripState[realIndex];
			case ButtonPressTypes.PressUp:
				return !currentHairGripState[realIndex] && previousHairGripState[realIndex];
			}
			break;
		case ButtonTypes.Touchpad:
			switch (pressType)
			{
			case ButtonPressTypes.Press:
			case ButtonPressTypes.PressDown:
			case ButtonPressTypes.PressUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Button.PrimaryThumbstick);
			case ButtonPressTypes.Touch:
			case ButtonPressTypes.TouchDown:
			case ButtonPressTypes.TouchUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Touch.PrimaryThumbstick);
			}
			break;
		case ButtonTypes.ButtonOne:
			switch (pressType)
			{
			case ButtonPressTypes.Press:
			case ButtonPressTypes.PressDown:
			case ButtonPressTypes.PressUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Button.One);
			case ButtonPressTypes.Touch:
			case ButtonPressTypes.TouchDown:
			case ButtonPressTypes.TouchUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Touch.One);
			}
			break;
		case ButtonTypes.ButtonTwo:
			switch (pressType)
			{
			case ButtonPressTypes.Press:
			case ButtonPressTypes.PressDown:
			case ButtonPressTypes.PressUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Button.Two);
			case ButtonPressTypes.Touch:
			case ButtonPressTypes.TouchDown:
			case ButtonPressTypes.TouchUp:
				return IsButtonPressed(realIndex, pressType, OVRInput.Touch.Two);
			}
			break;
		case ButtonTypes.StartMenu:
			return IsButtonPressed(realIndex, pressType, OVRInput.Button.Start);
		}
		return false;
	}

	protected virtual void CalculateAngularVelocity(VRTK_ControllerReference controllerReference)
	{
		if (VRTK_ControllerReference.IsValid(controllerReference))
		{
			uint realIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
			VRTK_TrackedController trackedObject = GetTrackedObject(controllerReference.actual);
			if (!(trackedObject == null))
			{
				ref Quaternion reference = ref previousControllerRotations[realIndex];
				reference = currentControllerRotations[realIndex];
				ref Quaternion reference2 = ref currentControllerRotations[realIndex];
				reference2 = trackedObject.transform.rotation;
				UpdateHairValues(realIndex, GetButtonAxis(ButtonTypes.Trigger, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Trigger, controllerReference), ref previousHairTriggerState[realIndex], ref currentHairTriggerState[realIndex], ref hairTriggerLimit[realIndex]);
				UpdateHairValues(realIndex, GetButtonAxis(ButtonTypes.Grip, controllerReference).x, GetButtonHairlineDelta(ButtonTypes.Grip, controllerReference), ref previousHairGripState[realIndex], ref currentHairGripState[realIndex], ref hairGripLimit[realIndex]);
			}
		}
	}

	protected virtual void SetTrackedControllerCaches(bool forceRefresh = false)
	{
		if (forceRefresh)
		{
			cachedLeftController = null;
			cachedRightController = null;
		}
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (!(instance != null))
		{
			return;
		}
		if (cachedLeftController == null && (bool)instance.loadedSetup.actualLeftController)
		{
			cachedLeftController = instance.loadedSetup.actualLeftController.GetComponent<VRTK_TrackedController>();
			if (cachedLeftController != null)
			{
				cachedLeftController.index = 0u;
			}
		}
		if (cachedRightController == null && (bool)instance.loadedSetup.actualRightController)
		{
			cachedRightController = instance.loadedSetup.actualRightController.GetComponent<VRTK_TrackedController>();
			if (cachedRightController != null)
			{
				cachedRightController.index = 1u;
			}
		}
	}

	protected virtual VRTK_TrackedController GetTrackedObject(GameObject controller)
	{
		SetTrackedControllerCaches();
		VRTK_TrackedController result = null;
		if (IsControllerLeftHand(controller))
		{
			result = cachedLeftController;
		}
		else if (IsControllerRightHand(controller))
		{
			result = cachedRightController;
		}
		return result;
	}

	protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Button button)
	{
		if (index >= uint.MaxValue)
		{
			return false;
		}
		VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
		if (trackedObject != null)
		{
			OVRInput.Controller controllerMask = touchControllers[index];
			switch (type)
			{
			case ButtonPressTypes.Press:
				return OVRInput.Get(button, controllerMask);
			case ButtonPressTypes.PressDown:
				return OVRInput.GetDown(button, controllerMask);
			case ButtonPressTypes.PressUp:
				return OVRInput.GetUp(button, controllerMask);
			}
		}
		return false;
	}

	protected virtual bool IsButtonPressed(uint index, ButtonPressTypes type, OVRInput.Touch button)
	{
		if (index >= uint.MaxValue)
		{
			return false;
		}
		VRTK_TrackedController trackedObject = GetTrackedObject(GetControllerByIndex(index));
		if (trackedObject != null)
		{
			OVRInput.Controller controllerMask = touchControllers[index];
			switch (type)
			{
			case ButtonPressTypes.Touch:
				return OVRInput.Get(button, controllerMask);
			case ButtonPressTypes.TouchDown:
				return OVRInput.GetDown(button, controllerMask);
			case ButtonPressTypes.TouchUp:
				return OVRInput.GetUp(button, controllerMask);
			}
		}
		return false;
	}

	protected virtual void UpdateHairValues(uint index, float axisValue, float hairDelta, ref bool previousState, ref bool currentState, ref float hairLimit)
	{
		previousState = currentState;
		if (currentState)
		{
			if (axisValue < hairLimit - hairDelta || axisValue <= 0f)
			{
				currentState = false;
			}
		}
		else if (axisValue > hairLimit + hairDelta || axisValue >= 1f)
		{
			currentState = true;
		}
		hairLimit = ((!currentState) ? Mathf.Min(hairLimit, axisValue) : Mathf.Max(hairLimit, axisValue));
	}

	protected virtual SDK_OculusBoundaries GetBoundariesSDK()
	{
		if (cachedBoundariesSDK == null)
		{
			cachedBoundariesSDK = ((!VRTK_SDKManager.instance) ? ScriptableObject.CreateInstance<SDK_OculusBoundaries>() : VRTK_SDKManager.instance.loadedSetup.boundariesSDK) as SDK_OculusBoundaries;
		}
		return cachedBoundariesSDK;
	}

	protected virtual bool HasAvatar(bool controllersAreVisible = true)
	{
		GetBoundariesSDK();
		return false;
	}

	protected virtual GameObject GetAvatar()
	{
		GetBoundariesSDK();
		return null;
	}
}
