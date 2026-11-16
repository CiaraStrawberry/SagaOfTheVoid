using UnityEngine;
using UnityEngine.UI;

namespace VRTK.Examples.PanelMenu;

public class PanelMenuUISlider : MonoBehaviour
{
	private Slider slider;

	private void Start()
	{
		slider = GetComponent<Slider>();
		if (slider == null)
		{
			VRTK_Logger.Warn(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_FROM_GAMEOBJECT, "PanelMenuUISlider", "Slider", "the same"));
		}
		else
		{
			GetComponentInParent<VRTK_PanelMenuItemController>().PanelMenuItemSwipeLeft += OnPanelMenuItemSwipeLeft;
			GetComponentInParent<VRTK_PanelMenuItemController>().PanelMenuItemSwipeRight += OnPanelMenuItemSwipeRight;
		}
	}

	private void OnPanelMenuItemSwipeLeft(object sender, PanelMenuItemControllerEventArgs e)
	{
		slider.value--;
		SendMessageToInteractableObject(e.interactableObject);
	}

	private void OnPanelMenuItemSwipeRight(object sender, PanelMenuItemControllerEventArgs e)
	{
		slider.value++;
		SendMessageToInteractableObject(e.interactableObject);
	}

	private void SendMessageToInteractableObject(GameObject interactableObject)
	{
		interactableObject.SendMessage("UpdateSliderValue", slider.value);
	}
}
