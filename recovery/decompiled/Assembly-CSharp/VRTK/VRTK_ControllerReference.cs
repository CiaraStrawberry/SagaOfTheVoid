using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

public class VRTK_ControllerReference : IEquatable<VRTK_ControllerReference>
{
	public static Dictionary<uint, VRTK_ControllerReference> controllerReferences = new Dictionary<uint, VRTK_ControllerReference>();

	protected uint storedControllerIndex = uint.MaxValue;

	public uint index => storedControllerIndex;

	public GameObject scriptAlias => VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, actual: false);

	public GameObject actual => VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, actual: true);

	public GameObject model => VRTK_SDK_Bridge.GetControllerModel(GetValidObjectFromIndex());

	public SDK_BaseController.ControllerHand hand => GetControllerHand(GetValidObjectFromIndex());

	public VRTK_ControllerReference(uint controllerIndex)
	{
		if (VRTK_SDK_Bridge.GetControllerByIndex(controllerIndex, actual: true) != null)
		{
			storedControllerIndex = controllerIndex;
			AddToCache();
		}
	}

	public VRTK_ControllerReference(GameObject controllerObject)
		: this(GetControllerHand(controllerObject))
	{
	}

	public VRTK_ControllerReference(SDK_BaseController.ControllerHand controllerHand)
	{
		storedControllerIndex = VRTK_SDK_Bridge.GetControllerIndex(GetValidObjectFromHand(controllerHand));
		AddToCache();
	}

	public static VRTK_ControllerReference GetControllerReference(uint controllerIndex)
	{
		if (controllerIndex < uint.MaxValue)
		{
			if (controllerReferences.ContainsKey(controllerIndex))
			{
				return controllerReferences[controllerIndex];
			}
			return new VRTK_ControllerReference(controllerIndex);
		}
		return null;
	}

	public static VRTK_ControllerReference GetControllerReference(GameObject controllerObject)
	{
		uint controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(controllerObject);
		if (controllerIndex >= uint.MaxValue)
		{
			controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(GetValidObjectFromHand(VRTK_SDK_Bridge.GetControllerModelHand(controllerObject)));
		}
		if (controllerReferences.ContainsKey(controllerIndex))
		{
			return controllerReferences[controllerIndex];
		}
		return new VRTK_ControllerReference(controllerIndex);
	}

	public static VRTK_ControllerReference GetControllerReference(SDK_BaseController.ControllerHand controllerHand)
	{
		GameObject validObjectFromHand = GetValidObjectFromHand(controllerHand);
		uint controllerIndex = VRTK_SDK_Bridge.GetControllerIndex(validObjectFromHand);
		if (controllerReferences.ContainsKey(controllerIndex))
		{
			return controllerReferences[controllerIndex];
		}
		return new VRTK_ControllerReference(validObjectFromHand);
	}

	public static bool IsValid(VRTK_ControllerReference controllerReference)
	{
		return controllerReference?.IsValid() ?? false;
	}

	public static uint GetRealIndex(VRTK_ControllerReference controllerReference)
	{
		return (!IsValid(controllerReference)) ? uint.MaxValue : controllerReference.index;
	}

	public bool IsValid()
	{
		return index < uint.MaxValue;
	}

	public override string ToString()
	{
		return string.Concat(base.ToString(), " --> INDEX[", index, "] - ACTUAL[", actual, "] - SCRIPT_ALIAS[", scriptAlias, "] - MODEL[", model, "] - HAND[", hand, "]");
	}

	public override int GetHashCode()
	{
		return (int)index;
	}

	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		if (!(obj is VRTK_ControllerReference other))
		{
			return false;
		}
		return Equals(other);
	}

	public bool Equals(VRTK_ControllerReference other)
	{
		if (other == null)
		{
			return false;
		}
		return index.Equals(other.index);
	}

	protected virtual GameObject GetValidObjectFromIndex()
	{
		GameObject controllerByIndex = VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, actual: false);
		return (!(controllerByIndex == null)) ? controllerByIndex : VRTK_SDK_Bridge.GetControllerByIndex(storedControllerIndex, actual: true);
	}

	protected virtual void AddToCache()
	{
		if (IsValid() && controllerReferences.ContainsKey(storedControllerIndex))
		{
			controllerReferences.Remove(storedControllerIndex);
		}
		if (IsValid() && !controllerReferences.ContainsKey(storedControllerIndex))
		{
			controllerReferences.Add(storedControllerIndex, this);
		}
	}

	private static GameObject GetValidObjectFromHand(SDK_BaseController.ControllerHand controllerHand)
	{
		return controllerHand switch
		{
			SDK_BaseController.ControllerHand.Left => (!VRTK_SDK_Bridge.GetControllerLeftHand(actual: false)) ? VRTK_SDK_Bridge.GetControllerLeftHand(actual: true) : VRTK_SDK_Bridge.GetControllerLeftHand(actual: false), 
			SDK_BaseController.ControllerHand.Right => (!VRTK_SDK_Bridge.GetControllerRightHand(actual: false)) ? VRTK_SDK_Bridge.GetControllerRightHand(actual: true) : VRTK_SDK_Bridge.GetControllerRightHand(actual: false), 
			_ => null, 
		};
	}

	private static SDK_BaseController.ControllerHand GetControllerHand(GameObject controllerObject)
	{
		if (VRTK_SDK_Bridge.IsControllerLeftHand(controllerObject, actual: false) || VRTK_SDK_Bridge.IsControllerLeftHand(controllerObject, actual: true))
		{
			return SDK_BaseController.ControllerHand.Left;
		}
		if (VRTK_SDK_Bridge.IsControllerRightHand(controllerObject, actual: false) || VRTK_SDK_Bridge.IsControllerRightHand(controllerObject, actual: true))
		{
			return SDK_BaseController.ControllerHand.Right;
		}
		return VRTK_SDK_Bridge.GetControllerModelHand(controllerObject);
	}
}
