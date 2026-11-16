using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Devdog.SciFiDesign.UI;

public class SelectableUIHelper : MonoBehaviour, ISelectHandler, IDeselectHandler, IEventSystemHandler
{
	public UnityEvent onSelect;

	public UnityEvent onDeselect;

	public void OnSelect(BaseEventData eventData)
	{
		onSelect.Invoke();
	}

	public void OnDeselect(BaseEventData eventData)
	{
		onDeselect.Invoke();
	}
}
