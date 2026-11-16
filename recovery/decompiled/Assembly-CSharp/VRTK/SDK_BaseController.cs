using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

public abstract class SDK_BaseController : SDK_Base
{
	public enum ButtonTypes
	{
		ButtonOne,
		ButtonTwo,
		Grip,
		GripHairline,
		StartMenu,
		Trigger,
		TriggerHairline,
		Touchpad
	}

	public enum ButtonPressTypes
	{
		Press,
		PressDown,
		PressUp,
		Touch,
		TouchDown,
		TouchUp
	}

	public enum ControllerElements
	{
		AttachPoint,
		Trigger,
		GripLeft,
		GripRight,
		Touchpad,
		ButtonOne,
		ButtonTwo,
		SystemMenu,
		Body,
		StartMenu
	}

	public enum ControllerHand
	{
		None,
		Left,
		Right
	}

	public abstract void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

	public abstract void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options);

	public abstract string GetControllerDefaultColliderPath(ControllerHand hand);

	public abstract string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false);

	public abstract uint GetControllerIndex(GameObject controller);

	public abstract GameObject GetControllerByIndex(uint index, bool actual = false);

	public abstract Transform GetControllerOrigin(VRTK_ControllerReference controllerReference);

	public abstract Transform GenerateControllerPointerOrigin(GameObject parent);

	public abstract GameObject GetControllerLeftHand(bool actual = false);

	public abstract GameObject GetControllerRightHand(bool actual = false);

	public abstract bool IsControllerLeftHand(GameObject controller);

	public abstract bool IsControllerRightHand(GameObject controller);

	public abstract bool IsControllerLeftHand(GameObject controller, bool actual);

	public abstract bool IsControllerRightHand(GameObject controller, bool actual);

	public abstract GameObject GetControllerModel(GameObject controller);

	public abstract GameObject GetControllerModel(ControllerHand hand);

	public virtual ControllerHand GetControllerModelHand(GameObject controllerModel)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null && instance.loadedSetup != null)
		{
			if (controllerModel == instance.loadedSetup.modelAliasLeftController)
			{
				return ControllerHand.Left;
			}
			if (controllerModel == instance.loadedSetup.modelAliasRightController)
			{
				return ControllerHand.Right;
			}
		}
		return ControllerHand.None;
	}

	public abstract GameObject GetControllerRenderModel(VRTK_ControllerReference controllerReference);

	public abstract void SetControllerRenderModelWheel(GameObject renderModel, bool state);

	public abstract void HapticPulse(VRTK_ControllerReference controllerReference, float strength = 0.5f);

	public abstract bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip);

	public abstract SDK_ControllerHapticModifiers GetHapticModifiers();

	public abstract Vector3 GetVelocity(VRTK_ControllerReference controllerReference);

	public abstract Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference);

	public abstract bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity);

	public abstract Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);

	public abstract float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference);

	public abstract bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference);

	protected virtual GameObject GetSDKManagerControllerLeftHand(bool actual = false)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			return (!actual) ? instance.scriptAliasLeftController : instance.loadedSetup.actualLeftController;
		}
		return null;
	}

	protected virtual GameObject GetSDKManagerControllerRightHand(bool actual = false)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			return (!actual) ? instance.scriptAliasRightController : instance.loadedSetup.actualRightController;
		}
		return null;
	}

	protected virtual bool CheckActualOrScriptAliasControllerIsLeftHand(GameObject controller)
	{
		return IsControllerLeftHand(controller, actual: true) || IsControllerLeftHand(controller, actual: false);
	}

	protected virtual bool CheckActualOrScriptAliasControllerIsRightHand(GameObject controller)
	{
		return IsControllerRightHand(controller, actual: true) || IsControllerRightHand(controller, actual: false);
	}

	protected virtual bool CheckControllerLeftHand(GameObject controller, bool actual)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null && controller != null)
		{
			return (!actual) ? controller.Equals(instance.scriptAliasLeftController) : controller.Equals(instance.loadedSetup.actualLeftController);
		}
		return false;
	}

	protected virtual bool CheckControllerRightHand(GameObject controller, bool actual)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null && controller != null)
		{
			return (!actual) ? controller.Equals(instance.scriptAliasRightController) : controller.Equals(instance.loadedSetup.actualRightController);
		}
		return false;
	}

	protected virtual GameObject GetControllerModelFromController(GameObject controller)
	{
		return GetControllerModel(VRTK_DeviceFinder.GetControllerHand(controller));
	}

	protected virtual GameObject GetSDKManagerControllerModelForHand(ControllerHand hand)
	{
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			switch (hand)
			{
			case ControllerHand.Left:
				return instance.loadedSetup.modelAliasLeftController;
			case ControllerHand.Right:
				return instance.loadedSetup.modelAliasRightController;
			}
		}
		return null;
	}

	protected virtual GameObject GetActualController(GameObject controller)
	{
		GameObject result = null;
		VRTK_SDKManager instance = VRTK_SDKManager.instance;
		if (instance != null)
		{
			if (IsControllerLeftHand(controller))
			{
				result = instance.loadedSetup.actualLeftController;
			}
			else if (IsControllerRightHand(controller))
			{
				result = instance.loadedSetup.actualRightController;
			}
		}
		return result;
	}
}
