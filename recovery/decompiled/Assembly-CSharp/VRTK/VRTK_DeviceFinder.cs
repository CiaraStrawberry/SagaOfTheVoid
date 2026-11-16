using System;
using UnityEngine;
using UnityEngine.VR;

namespace VRTK;

public static class VRTK_DeviceFinder
{
	public enum Devices
	{
		Headset,
		LeftController,
		RightController
	}

	public enum Headsets
	{
		Unknown,
		OculusRift,
		OculusRiftCV1,
		Vive,
		ViveMV,
		ViveDVT
	}

	public static uint GetControllerIndex(GameObject controller)
	{
		return VRTK_SDK_Bridge.GetControllerIndex(controller);
	}

	public static GameObject GetControllerByIndex(uint index, bool getActual)
	{
		return VRTK_SDK_Bridge.GetControllerByIndex(index, getActual);
	}

	[Obsolete("`VRTK_DeviceFinder.GetControllerOrigin(controller)` has been replaced with `VRTK_DeviceFinder.GetControllerOrigin(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public static Transform GetControllerOrigin(GameObject controller)
	{
		return GetControllerOrigin(VRTK_ControllerReference.GetControllerReference(controller));
	}

	public static Transform GetControllerOrigin(VRTK_ControllerReference controllerReference)
	{
		return VRTK_SDK_Bridge.GetControllerOrigin(controllerReference);
	}

	public static Transform DeviceTransform(Devices device)
	{
		return device switch
		{
			Devices.Headset => HeadsetTransform(), 
			Devices.LeftController => GetControllerLeftHand().transform, 
			Devices.RightController => GetControllerRightHand().transform, 
			_ => null, 
		};
	}

	public static SDK_BaseController.ControllerHand GetControllerHandType(string hand)
	{
		return hand.ToLower() switch
		{
			"left" => SDK_BaseController.ControllerHand.Left, 
			"right" => SDK_BaseController.ControllerHand.Right, 
			_ => SDK_BaseController.ControllerHand.None, 
		};
	}

	public static SDK_BaseController.ControllerHand GetControllerHand(GameObject controller)
	{
		if (VRTK_SDK_Bridge.IsControllerLeftHand(controller))
		{
			return SDK_BaseController.ControllerHand.Left;
		}
		if (VRTK_SDK_Bridge.IsControllerRightHand(controller))
		{
			return SDK_BaseController.ControllerHand.Right;
		}
		return SDK_BaseController.ControllerHand.None;
	}

	public static GameObject GetControllerLeftHand(bool getActual = false)
	{
		return VRTK_SDK_Bridge.GetControllerLeftHand(getActual);
	}

	public static GameObject GetControllerRightHand(bool getActual = false)
	{
		return VRTK_SDK_Bridge.GetControllerRightHand(getActual);
	}

	public static bool IsControllerOfHand(GameObject checkController, SDK_BaseController.ControllerHand hand)
	{
		return hand switch
		{
			SDK_BaseController.ControllerHand.Left => IsControllerLeftHand(checkController), 
			SDK_BaseController.ControllerHand.Right => IsControllerRightHand(checkController), 
			_ => false, 
		};
	}

	public static bool IsControllerLeftHand(GameObject checkController)
	{
		return VRTK_SDK_Bridge.IsControllerLeftHand(checkController);
	}

	public static bool IsControllerRightHand(GameObject checkController)
	{
		return VRTK_SDK_Bridge.IsControllerRightHand(checkController);
	}

	public static GameObject GetActualController(GameObject givenController)
	{
		if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, actual: true) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, actual: true))
		{
			return givenController;
		}
		if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, actual: false))
		{
			return VRTK_SDK_Bridge.GetControllerLeftHand(actual: true);
		}
		if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, actual: false))
		{
			return VRTK_SDK_Bridge.GetControllerRightHand(actual: true);
		}
		return null;
	}

	public static GameObject GetScriptAliasController(GameObject givenController)
	{
		if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, actual: false) || VRTK_SDK_Bridge.IsControllerRightHand(givenController, actual: false))
		{
			return givenController;
		}
		if (VRTK_SDK_Bridge.IsControllerLeftHand(givenController, actual: true))
		{
			return VRTK_SDK_Bridge.GetControllerLeftHand(actual: false);
		}
		if (VRTK_SDK_Bridge.IsControllerRightHand(givenController, actual: true))
		{
			return VRTK_SDK_Bridge.GetControllerRightHand(actual: false);
		}
		return null;
	}

	public static GameObject GetModelAliasController(GameObject givenController)
	{
		return VRTK_SDK_Bridge.GetControllerModel(givenController);
	}

	public static SDK_BaseController.ControllerHand GetModelAliasControllerHand(GameObject givenObject)
	{
		if (GetModelAliasController(GetControllerLeftHand()) == givenObject)
		{
			return SDK_BaseController.ControllerHand.Left;
		}
		if (GetModelAliasController(GetControllerRightHand()) == givenObject)
		{
			return SDK_BaseController.ControllerHand.Right;
		}
		return SDK_BaseController.ControllerHand.None;
	}

	[Obsolete("`VRTK_DeviceFinder.GetControllerVelocity(givenController)` has been replaced with `VRTK_DeviceFinder.GetControllerVelocity(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public static Vector3 GetControllerVelocity(GameObject givenController)
	{
		return GetControllerVelocity(VRTK_ControllerReference.GetControllerReference(givenController));
	}

	public static Vector3 GetControllerVelocity(VRTK_ControllerReference controllerReference)
	{
		return VRTK_SDK_Bridge.GetControllerVelocity(controllerReference);
	}

	[Obsolete("`VRTK_DeviceFinder.GetControllerAngularVelocity(givenController)` has been replaced with `VRTK_DeviceFinder.GetControllerAngularVelocity(controllerReference)`. This method will be removed in a future version of VRTK.")]
	public static Vector3 GetControllerAngularVelocity(GameObject givenController)
	{
		return GetControllerAngularVelocity(VRTK_ControllerReference.GetControllerReference(givenController));
	}

	public static Vector3 GetControllerAngularVelocity(VRTK_ControllerReference controllerReference)
	{
		return VRTK_SDK_Bridge.GetControllerAngularVelocity(controllerReference);
	}

	public static Vector3 GetHeadsetVelocity()
	{
		return VRTK_SDK_Bridge.GetHeadsetVelocity();
	}

	public static Vector3 GetHeadsetAngularVelocity()
	{
		return VRTK_SDK_Bridge.GetHeadsetAngularVelocity();
	}

	public static Transform HeadsetTransform()
	{
		return VRTK_SDK_Bridge.GetHeadset();
	}

	public static Transform HeadsetCamera()
	{
		return VRTK_SDK_Bridge.GetHeadsetCamera();
	}

	public static Headsets GetHeadsetType(bool summary = false)
	{
		Headsets headsets = Headsets.Unknown;
		string model = VRDevice.model;
		switch (model)
		{
		case "Oculus Rift CV1":
			headsets = (summary ? Headsets.OculusRift : Headsets.OculusRiftCV1);
			break;
		case "Vive MV":
			headsets = ((!summary) ? Headsets.ViveMV : Headsets.Vive);
			break;
		case "Vive DVT":
			headsets = ((!summary) ? Headsets.ViveDVT : Headsets.Vive);
			break;
		}
		if (headsets == Headsets.Unknown)
		{
			VRTK_Logger.Warn(string.Format("Your headset is of type '{0}' which VRTK doesn't know about yet. Please report this headset type to the maintainers of VRTK." + ((!summary) ? string.Empty : " Falling back to a slower check to summarize the headset type now."), model));
			if (summary)
			{
				model = model.ToLowerInvariant();
				if (model.Contains("rift"))
				{
					return Headsets.OculusRift;
				}
				if (model.Contains("vive"))
				{
					return Headsets.Vive;
				}
			}
		}
		return headsets;
	}

	public static Transform PlayAreaTransform()
	{
		return VRTK_SDK_Bridge.GetPlayArea();
	}
}
