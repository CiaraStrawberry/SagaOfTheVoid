using System;
using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Interactions/VRTK_InteractHaptics")]
public class VRTK_InteractHaptics : MonoBehaviour
{
	[Header("Haptics On Touch")]
	[Tooltip("Denotes the audio clip to use to rumble the controller on touch.")]
	public AudioClip clipOnTouch;

	[Tooltip("Denotes how strong the rumble in the controller will be on touch.")]
	[Range(0f, 1f)]
	public float strengthOnTouch;

	[Tooltip("Denotes how long the rumble in the controller will last on touch.")]
	public float durationOnTouch;

	[Tooltip("Denotes interval betweens rumble in the controller on touch.")]
	public float intervalOnTouch = 0.05f;

	[Header("Haptics On Grab")]
	[Tooltip("Denotes the audio clip to use to rumble the controller on grab.")]
	public AudioClip clipOnGrab;

	[Tooltip("Denotes how strong the rumble in the controller will be on grab.")]
	[Range(0f, 1f)]
	public float strengthOnGrab;

	[Tooltip("Denotes how long the rumble in the controller will last on grab.")]
	public float durationOnGrab;

	[Tooltip("Denotes interval betweens rumble in the controller on grab.")]
	public float intervalOnGrab = 0.05f;

	[Header("Haptics On Use")]
	[Tooltip("Denotes the audio clip to use to rumble the controller on use.")]
	public AudioClip clipOnUse;

	[Tooltip("Denotes how strong the rumble in the controller will be on use.")]
	[Range(0f, 1f)]
	public float strengthOnUse;

	[Tooltip("Denotes how long the rumble in the controller will last on use.")]
	public float durationOnUse;

	[Tooltip("Denotes interval betweens rumble in the controller on use.")]
	public float intervalOnUse = 0.05f;

	protected const float minInterval = 0.05f;

	[Obsolete("`VRTK_InteractHaptics.HapticsOnTouch(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnTouch(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public virtual void HapticsOnTouch(uint controllerIndex)
	{
		HapticsOnTouch(VRTK_ControllerReference.GetControllerReference(controllerIndex));
	}

	public virtual void HapticsOnTouch(VRTK_ControllerReference controllerReference)
	{
		if (clipOnTouch != null)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnTouch);
		}
		else if (strengthOnTouch > 0f && durationOnTouch > 0f)
		{
			TriggerHapticPulse(controllerReference, strengthOnTouch, durationOnTouch, intervalOnTouch);
		}
	}

	[Obsolete("`VRTK_InteractHaptics.HapticsOnGrab(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnGrab(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public virtual void HapticsOnGrab(uint controllerIndex)
	{
		HapticsOnGrab(VRTK_ControllerReference.GetControllerReference(controllerIndex));
	}

	public virtual void HapticsOnGrab(VRTK_ControllerReference controllerReference)
	{
		if (clipOnGrab != null)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnGrab);
		}
		else if (strengthOnGrab > 0f && durationOnGrab > 0f)
		{
			TriggerHapticPulse(controllerReference, strengthOnGrab, durationOnGrab, intervalOnGrab);
		}
	}

	[Obsolete("`VRTK_InteractHaptics.HapticsOnUse(controllerIndex)` has been replaced with `VRTK_InteractHaptics.HapticsOnUse(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public virtual void HapticsOnUse(uint controllerIndex)
	{
		HapticsOnUse(VRTK_ControllerReference.GetControllerReference(controllerIndex));
	}

	public virtual void HapticsOnUse(VRTK_ControllerReference controllerReference)
	{
		if (clipOnUse != null)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, clipOnUse);
		}
		else if (strengthOnUse > 0f && durationOnUse > 0f)
		{
			TriggerHapticPulse(controllerReference, strengthOnUse, durationOnUse, intervalOnUse);
		}
	}

	protected virtual void OnEnable()
	{
		if (!GetComponent<VRTK_InteractableObject>())
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "VRTK_InteractHaptics", "VRTK_InteractableObject", "the same"));
		}
	}

	protected virtual void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float interval)
	{
		VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength, duration, (!(interval >= 0.05f)) ? 0.05f : interval);
	}
}
