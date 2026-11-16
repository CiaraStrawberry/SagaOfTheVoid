using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

[SDK_Description(typeof(SDK_FallbackSystem), 0)]
public class SDK_FallbackController : SDK_BaseController
{
	public override void ProcessUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
	}

	public override void ProcessFixedUpdate(VRTK_ControllerReference controllerReference, Dictionary<string, object> options)
	{
	}

	public override string GetControllerDefaultColliderPath(ControllerHand hand)
	{
		return string.Empty;
	}

	public override string GetControllerElementPath(ControllerElements element, ControllerHand hand, bool fullPath = false)
	{
		return string.Empty;
	}

	public override uint GetControllerIndex(GameObject controller)
	{
		return uint.MaxValue;
	}

	public override GameObject GetControllerByIndex(uint index, bool actual = false)
	{
		return null;
	}

	public override Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
	{
		return null;
	}

	public override Transform GenerateControllerPointerOrigin(GameObject parent)
	{
		return null;
	}

	public override GameObject GetControllerLeftHand(bool actual = false)
	{
		return null;
	}

	public override GameObject GetControllerRightHand(bool actual = false)
	{
		return null;
	}

	public override bool IsControllerLeftHand(GameObject controller)
	{
		return false;
	}

	public override bool IsControllerRightHand(GameObject controller)
	{
		return false;
	}

	public override bool IsControllerLeftHand(GameObject controller, bool actual)
	{
		return false;
	}

	public override bool IsControllerRightHand(GameObject controller, bool actual)
	{
		return false;
	}

	public override GameObject GetControllerModel(GameObject controller)
	{
		return null;
	}

	public override GameObject GetControllerModel(ControllerHand hand)
	{
		return null;
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
	}

	public override bool HapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		return true;
	}

	public override SDK_ControllerHapticModifiers GetHapticModifiers()
	{
		return new SDK_ControllerHapticModifiers();
	}

	public override Vector3 GetVelocity(VRTK_ControllerReference controllerReference)
	{
		return Vector3.zero;
	}

	public override Vector3 GetAngularVelocity(VRTK_ControllerReference controllerReference)
	{
		return Vector3.zero;
	}

	public override bool IsTouchpadStatic(bool isTouched, Vector2 currentAxisValues, Vector2 previousAxisValues, int compareFidelity)
	{
		return !isTouched || VRTK_SharedMethods.Vector2ShallowCompare(currentAxisValues, previousAxisValues, compareFidelity);
	}

	public override Vector2 GetButtonAxis(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		return Vector2.zero;
	}

	public override float GetButtonHairlineDelta(ButtonTypes buttonType, VRTK_ControllerReference controllerReference)
	{
		return 0f;
	}

	public override bool GetControllerButtonState(ButtonTypes buttonType, ButtonPressTypes pressType, VRTK_ControllerReference controllerReference)
	{
		return false;
	}
}
