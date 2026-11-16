using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

public class VRTK_ControllerHaptics : MonoBehaviour
{
	protected static VRTK_ControllerHaptics instance;

	protected Dictionary<VRTK_ControllerReference, Coroutine> hapticLoopCoroutines = new Dictionary<VRTK_ControllerReference, Coroutine>();

	public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalTriggerHapticPulse(controllerReference, strength);
		}
	}

	public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float pulseInterval)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalTriggerHapticPulse(controllerReference, strength, duration, pulseInterval);
		}
	}

	public static void TriggerHapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalTriggerHapticPulse(controllerReference, clip);
		}
	}

	public static void CancelHapticPulse(VRTK_ControllerReference controllerReference)
	{
		SetupInstance();
		if (instance != null)
		{
			instance.InternalCancelHapticPulse(controllerReference);
		}
	}

	protected virtual void OnDisable()
	{
		StopAllCoroutines();
		hapticLoopCoroutines.Clear();
	}

	protected static void SetupInstance()
	{
		if (instance == null && VRTK_SDKManager.instance != null)
		{
			instance = VRTK_SDKManager.instance.gameObject.AddComponent<VRTK_ControllerHaptics>();
		}
	}

	protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength)
	{
		InternalCancelHapticPulse(controllerReference);
		float strength2 = Mathf.Clamp(strength, 0f, 1f);
		VRTK_SDK_Bridge.HapticPulse(controllerReference, strength2);
	}

	protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, float strength, float duration, float pulseInterval)
	{
		InternalCancelHapticPulse(controllerReference);
		float hapticPulseStrength = Mathf.Clamp(strength, 0f, 1f);
		SDK_ControllerHapticModifiers hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
		Coroutine value = StartCoroutine(SimpleHapticPulseRoutine(controllerReference, duration * hapticModifiers.durationModifier, hapticPulseStrength, pulseInterval * hapticModifiers.intervalModifier));
		if (!hapticLoopCoroutines.ContainsKey(controllerReference))
		{
			hapticLoopCoroutines.Add(controllerReference, value);
		}
	}

	protected virtual void InternalTriggerHapticPulse(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		InternalCancelHapticPulse(controllerReference);
		if (!VRTK_SDK_Bridge.HapticPulse(controllerReference, clip))
		{
			Coroutine value = StartCoroutine(AudioClipHapticsRoutine(controllerReference, clip));
			if (!hapticLoopCoroutines.ContainsKey(controllerReference))
			{
				hapticLoopCoroutines.Add(controllerReference, value);
			}
		}
	}

	protected virtual void InternalCancelHapticPulse(VRTK_ControllerReference controllerReference)
	{
		if (hapticLoopCoroutines.ContainsKey(controllerReference) && hapticLoopCoroutines[controllerReference] != null)
		{
			StopCoroutine(hapticLoopCoroutines[controllerReference]);
			hapticLoopCoroutines.Remove(controllerReference);
		}
	}

	protected virtual IEnumerator SimpleHapticPulseRoutine(VRTK_ControllerReference controllerReference, float duration, float hapticPulseStrength, float pulseInterval)
	{
		if (!(pulseInterval <= 0f))
		{
			while (duration > 0f)
			{
				VRTK_SDK_Bridge.HapticPulse(controllerReference, hapticPulseStrength);
				yield return new WaitForSeconds(pulseInterval);
				duration -= pulseInterval;
			}
		}
	}

	protected virtual IEnumerator AudioClipHapticsRoutine(VRTK_ControllerReference controllerReference, AudioClip clip)
	{
		SDK_ControllerHapticModifiers hapticModifiers = VRTK_SDK_Bridge.GetHapticModifiers();
		float hapticScalar = (int)hapticModifiers.maxHapticVibration;
		float[] audioData = new float[hapticModifiers.hapticsBufferSize];
		int sampleOffset = -hapticModifiers.hapticsBufferSize;
		float startTime = Time.time;
		float length = clip.length / 1f;
		float endTime = startTime + length;
		float sampleRate = clip.samples;
		while (Time.time <= endTime)
		{
			float lerpVal = (Time.time - startTime) / length;
			int sampleIndex = (int)(sampleRate * lerpVal);
			if (sampleIndex >= sampleOffset + hapticModifiers.hapticsBufferSize)
			{
				clip.GetData(audioData, sampleIndex);
				sampleOffset = sampleIndex;
			}
			float currentSample = Mathf.Abs(audioData[sampleIndex - sampleOffset]);
			ushort hapticStrength = (ushort)(hapticScalar * currentSample);
			VRTK_SDK_Bridge.HapticPulse(controllerReference, (int)hapticStrength);
			yield return null;
		}
	}
}
