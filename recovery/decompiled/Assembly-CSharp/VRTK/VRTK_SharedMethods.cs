using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.VR;

namespace VRTK;

public static class VRTK_SharedMethods
{
	public static Bounds GetBounds(Transform transform, Transform excludeRotation = null, Transform excludeTransform = null)
	{
		Quaternion rotation = Quaternion.identity;
		if ((bool)excludeRotation)
		{
			rotation = excludeRotation.rotation;
			excludeRotation.rotation = Quaternion.identity;
		}
		bool flag = false;
		Bounds result = new Bounds(transform.position, Vector3.zero);
		Renderer[] componentsInChildren = transform.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (!(excludeTransform != null) || !renderer.transform.IsChildOf(excludeTransform))
			{
				if (!flag)
				{
					result = new Bounds(renderer.transform.position, Vector3.zero);
					flag = true;
				}
				result.Encapsulate(renderer.bounds);
			}
		}
		if (result.size.magnitude == 0f)
		{
			BoxCollider[] componentsInChildren2 = transform.GetComponentsInChildren<BoxCollider>();
			BoxCollider[] array2 = componentsInChildren2;
			foreach (BoxCollider boxCollider in array2)
			{
				if (!(excludeTransform != null) || !boxCollider.transform.IsChildOf(excludeTransform))
				{
					if (!flag)
					{
						result = new Bounds(boxCollider.transform.position, Vector3.zero);
						flag = true;
					}
					result.Encapsulate(boxCollider.bounds);
				}
			}
		}
		if ((bool)excludeRotation)
		{
			excludeRotation.rotation = rotation;
		}
		return result;
	}

	public static bool IsLowest(float value, float[] others)
	{
		foreach (float num in others)
		{
			if (num <= value)
			{
				return false;
			}
		}
		return true;
	}

	public static Transform AddCameraFade()
	{
		Transform transform = VRTK_DeviceFinder.HeadsetCamera();
		VRTK_SDK_Bridge.AddHeadsetFade(transform);
		return transform;
	}

	public static void CreateColliders(GameObject obj)
	{
		Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>();
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			if (!renderer.gameObject.GetComponent<Collider>())
			{
				renderer.gameObject.AddComponent<BoxCollider>();
			}
		}
	}

	public static Component CloneComponent(Component source, GameObject destination, bool copyProperties = false)
	{
		Component component = destination.gameObject.AddComponent(source.GetType());
		if (copyProperties)
		{
			PropertyInfo[] properties = source.GetType().GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (propertyInfo.CanWrite)
				{
					propertyInfo.SetValue(component, propertyInfo.GetValue(source, null), null);
				}
			}
		}
		FieldInfo[] fields = source.GetType().GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			fieldInfo.SetValue(component, fieldInfo.GetValue(source));
		}
		return component;
	}

	public static Color ColorDarken(Color color, float percent)
	{
		return new Color(NumberPercent(color.r, percent), NumberPercent(color.g, percent), NumberPercent(color.b, percent), color.a);
	}

	public static float RoundFloat(float givenFloat, int decimalPlaces, bool rawFidelity = false)
	{
		float num = ((!rawFidelity) ? Mathf.Pow(10f, decimalPlaces) : ((float)decimalPlaces));
		return Mathf.Round(givenFloat * num) / num;
	}

	public static bool IsEditTime()
	{
		return false;
	}

	[Obsolete("`VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength)` has been replaced with `VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength)`. This method will be removed in a future version of VRTK.")]
	public static void TriggerHapticPulse(uint controllerIndex, float strength)
	{
		VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex), strength);
	}

	[Obsolete("`VRTK_SharedMethods.TriggerHapticPulse(controllerIndex, strength, duration, pulseInterval)` has been replaced with `VRTK_ControllerHaptics.TriggerHapticPulse(controllerReference, strength, duration, pulseInterval)`. This method will be removed in a future version of VRTK.")]
	public static void TriggerHapticPulse(uint controllerIndex, float strength, float duration, float pulseInterval)
	{
		VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex), strength, duration, pulseInterval);
	}

	[Obsolete("`VRTK_SharedMethods.CancelHapticPulse(controllerIndex)` has been replaced with `VRTK_SharedMethods.CancelHapticPulse(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public static void CancelHapticPulse(uint controllerIndex)
	{
		VRTK_ControllerHaptics.CancelHapticPulse(VRTK_ControllerReference.GetControllerReference(controllerIndex));
	}

	[Obsolete("`VRTK_SharedMethods.SetOpacity(model, alpha, transitionDuration)` has been replaced with `VRTK_ObjectAppearance.SetOpacity(model, alpha, transitionDuration)`. This method will be removed in a future version of VRTK.")]
	public static void SetOpacity(GameObject model, float alpha, float transitionDuration = 0f)
	{
		VRTK_ObjectAppearance.SetOpacity(model, alpha, transitionDuration);
	}

	[Obsolete("`VRTK_SharedMethods.SetRendererVisible(model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.SetRendererVisible(model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
	public static void SetRendererVisible(GameObject model, GameObject ignoredModel = null)
	{
		VRTK_ObjectAppearance.SetRendererVisible(model, ignoredModel);
	}

	[Obsolete("`VRTK_SharedMethods.SetRendererHidden(model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.SetRendererHidden(model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
	public static void SetRendererHidden(GameObject model, GameObject ignoredModel = null)
	{
		VRTK_ObjectAppearance.SetRendererHidden(model, ignoredModel);
	}

	[Obsolete("`VRTK_SharedMethods.ToggleRenderer(state, model, ignoredModel)` has been replaced with `VRTK_ObjectAppearance.ToggleRenderer(state, model, ignoredModel)`. This method will be removed in a future version of VRTK.")]
	public static void ToggleRenderer(bool state, GameObject model, GameObject ignoredModel = null)
	{
		VRTK_ObjectAppearance.ToggleRenderer(state, model, ignoredModel);
	}

	[Obsolete("`VRTK_SharedMethods.HighlightObject(model, highlightColor, fadeDuration)` has been replaced with `VRTK_ObjectAppearance.HighlightObject(model, highlightColor, fadeDuration)`. This method will be removed in a future version of VRTK.")]
	public static void HighlightObject(GameObject model, Color? highlightColor, float fadeDuration = 0f)
	{
		VRTK_ObjectAppearance.HighlightObject(model, highlightColor, fadeDuration);
	}

	[Obsolete("`VRTK_SharedMethods.UnhighlightObject(model)` has been replaced with `VRTK_ObjectAppearance.UnhighlightObject(model)`. This method will be removed in a future version of VRTK.")]
	public static void UnhighlightObject(GameObject model)
	{
		VRTK_ObjectAppearance.UnhighlightObject(model);
	}

	public static float Mod(float a, float b)
	{
		return a - b * Mathf.Floor(a / b);
	}

	public static GameObject FindEvenInactiveGameObject<T>(string gameObjectName = null) where T : Component
	{
		if (string.IsNullOrEmpty(gameObjectName))
		{
			T val = FindEvenInactiveComponent<T>();
			return (!(val == null)) ? val.gameObject : null;
		}
		Scene activeScene = SceneManager.GetActiveScene();
		IEnumerable<GameObject> source = from component in Resources.FindObjectsOfTypeAll<T>()
			select component.gameObject into gameObject
			where gameObject.scene == activeScene
			select gameObject;
		return source.Select((GameObject gameObject) =>
		{
			Transform transform = gameObject.transform.Find(gameObjectName);
			return (!(transform == null)) ? transform.gameObject : null;
		}).FirstOrDefault((GameObject gameObject) => gameObject != null);
	}

	public static T[] FindEvenInactiveComponents<T>() where T : Component
	{
		Scene activeScene = SceneManager.GetActiveScene();
		return (from @object in Resources.FindObjectsOfTypeAll<T>()
			where @object.gameObject.scene == activeScene
			select @object).ToArray();
	}

	public static T FindEvenInactiveComponent<T>() where T : Component
	{
		Scene activeScene = SceneManager.GetActiveScene();
		return (from @object in Resources.FindObjectsOfTypeAll<T>()
			where @object.gameObject.scene == activeScene
			select @object).FirstOrDefault();
	}

	public static string GenerateVRTKObjectName(bool autoGen, params object[] replacements)
	{
		string text = "[VRTK]";
		if (autoGen)
		{
			text += "[AUTOGEN]";
		}
		for (int i = 0; i < replacements.Length; i++)
		{
			string text2 = text;
			text = text2 + "[{" + i + "}]";
		}
		return string.Format(text, replacements);
	}

	public static float GetGPUTimeLastFrame()
	{
		if (VRStats.TryGetGPUTimeLastFrame(out var gpuTimeLastFrame))
		{
			return gpuTimeLastFrame;
		}
		return 0f;
	}

	public static bool Vector2ShallowCompare(Vector2 vectorA, Vector2 vectorB, int compareFidelity)
	{
		Vector2 vector = vectorA - vectorB;
		return Math.Round(Mathf.Abs(vector.x), compareFidelity, MidpointRounding.AwayFromZero) < 1.401298464324817E-45 && Math.Round(Mathf.Abs(vector.y), compareFidelity, MidpointRounding.AwayFromZero) < 1.401298464324817E-45;
	}

	public static float NumberPercent(float value, float percent)
	{
		percent = Mathf.Clamp(percent, 0f, 100f);
		return (percent != 0f) ? (value - percent / 100f) : value;
	}
}
