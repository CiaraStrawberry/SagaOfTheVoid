using UnityEngine;
using UnityEngine.SceneManagement;

namespace VRTK.Examples.Utilities;

public class SceneChanger : MonoBehaviour
{
	private bool canPress;

	private VRTK_ControllerReference controllerReference;

	private void Awake()
	{
		canPress = false;
		Invoke("ResetPress", 1f);
		DynamicGI.UpdateEnvironment();
	}

	private bool ForwardPressed()
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return false;
		}
		if (canPress && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference) && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, controllerReference) && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.Press, controllerReference))
		{
			return true;
		}
		return false;
	}

	private bool BackPressed()
	{
		if (!VRTK_ControllerReference.IsValid(controllerReference))
		{
			return false;
		}
		if (canPress && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.Press, controllerReference) && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.Press, controllerReference) && VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.Press, controllerReference))
		{
			return true;
		}
		return false;
	}

	private void ResetPress()
	{
		canPress = true;
	}

	private void Update()
	{
		GameObject controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand(getActual: true);
		controllerReference = VRTK_ControllerReference.GetControllerReference(controllerRightHand);
		if (ForwardPressed() || Input.GetKeyUp(KeyCode.Space))
		{
			int num = SceneManager.GetActiveScene().buildIndex + 1;
			if (num >= SceneManager.sceneCountInBuildSettings)
			{
				num = 0;
			}
			VRTK_SDKManager.instance.UnloadSDKSetup();
			SceneManager.LoadScene(num);
		}
		if (BackPressed() || Input.GetKeyUp(KeyCode.Backspace))
		{
			int num2 = SceneManager.GetActiveScene().buildIndex - 1;
			if (num2 < 0)
			{
				num2 = SceneManager.sceneCountInBuildSettings - 1;
			}
			VRTK_SDKManager.instance.UnloadSDKSetup();
			SceneManager.LoadScene(num2);
		}
	}
}
