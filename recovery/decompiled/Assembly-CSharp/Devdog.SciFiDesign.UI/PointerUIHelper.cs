using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class PointerUIHelper : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IEventSystemHandler
{
	public UnityEvent onPointerEnter = new UnityEvent();

	public UnityEvent onPointerExit = new UnityEvent();

	public UnityEvent onPointerDown = new UnityEvent();

	public UnityEvent onPointerUp = new UnityEvent();

	public UnityEvent onPointerClick = new UnityEvent();

	public void OnPointerEnter(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if ((component != null && component.IsInteractable()) || component == null)
		{
			onPointerEnter.Invoke();
		}
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if ((component != null && component.IsInteractable()) || component == null)
		{
			onPointerExit.Invoke();
		}
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if ((component != null && component.IsInteractable()) || component == null)
		{
			onPointerDown.Invoke();
		}
	}

	public void OnPointerUp(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if ((component != null && component.IsInteractable()) || component == null)
		{
			onPointerUp.Invoke();
		}
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		Selectable component = GetComponent<Selectable>();
		if ((component != null && component.IsInteractable()) || component == null)
		{
			onPointerClick.Invoke();
		}
	}
}
