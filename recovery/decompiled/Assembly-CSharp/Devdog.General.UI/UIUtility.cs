using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.General.UI;

public static class UIUtility
{
	private static Dictionary<Type, MethodInfo> _reflectionCache = new Dictionary<Type, MethodInfo>();

	public static bool isFocusedOnInput
	{
		get
		{
			if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<InputField>() != null)
			{
				return true;
			}
			return false;
		}
	}

	public static GameObject currentlySelectedGameObject
	{
		get
		{
			if (EventSystem.current == null || EventSystem.current.currentSelectedGameObject == null)
			{
				return null;
			}
			return EventSystem.current.currentSelectedGameObject;
		}
	}

	public static bool isHoveringUIElement
	{
		get
		{
			if (EventSystem.current == null || EventSystem.current.currentInputModule == null)
			{
				return false;
			}
			Type type = EventSystem.current.currentInputModule.GetType();
			_reflectionCache.TryGetValue(type, out var value);
			if (value == null)
			{
				value = type.GetMethod("GetLastPointerEventData", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
				_reflectionCache[type] = value;
			}
			PointerEventData pointerEventData = (PointerEventData)value.Invoke(EventSystem.current.currentInputModule, new object[1] { -1 });
			if (pointerEventData != null && (bool)pointerEventData.pointerEnter)
			{
				return pointerEventData.pointerEnter.layer == 5;
			}
			return false;
		}
	}

	public static bool IsPointerOverUIObject(Canvas canvas, Vector2 screenPosition)
	{
		return PointerOverUIObject(canvas, screenPosition).Count > 0;
	}

	public static List<RaycastResult> PointerOverUIObject(Canvas canvas, Vector2 screenPosition)
	{
		PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
		pointerEventData.position = screenPosition;
		GraphicRaycaster component = canvas.gameObject.GetComponent<GraphicRaycaster>();
		List<RaycastResult> list = new List<RaycastResult>();
		component.Raycast(pointerEventData, list);
		return list;
	}

	public static void ResetTransform(Transform transform)
	{
		transform.ResetTRSRect();
	}

	public static void InheritParentSize(Transform transform)
	{
		RectTransform component = transform.GetComponent<RectTransform>();
		component.anchorMin = Vector2.zero;
		component.anchorMax = Vector2.one;
		component.sizeDelta = Vector2.one;
		component.pivot = new Vector2(0.5f, 0.5f);
	}
}
