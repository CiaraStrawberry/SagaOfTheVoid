using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(Button))]
public class OnSelectTriggerButtonUse : MonoBehaviour, ISelectHandler, IEventSystemHandler
{
	public void OnSelect(BaseEventData eventData)
	{
		GetComponent<Button>().OnPointerClick(new PointerEventData(EventSystem.current));
	}
}
