using UnityEngine;
using UnityEngine.EventSystems;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/UI/VRTK_UIDropZone")]
public class VRTK_UIDropZone : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IEventSystemHandler
{
	protected VRTK_UIDraggableItem droppableItem;

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if ((bool)eventData.pointerDrag)
		{
			VRTK_UIDraggableItem component = eventData.pointerDrag.GetComponent<VRTK_UIDraggableItem>();
			if ((bool)component && component.restrictToDropZone)
			{
				component.validDropZone = gameObject;
				droppableItem = component;
			}
		}
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if ((bool)droppableItem)
		{
			droppableItem.validDropZone = null;
		}
		droppableItem = null;
	}
}
