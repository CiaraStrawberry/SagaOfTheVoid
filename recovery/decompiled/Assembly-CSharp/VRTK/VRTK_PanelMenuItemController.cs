using UnityEngine;

namespace VRTK;

public class VRTK_PanelMenuItemController : MonoBehaviour
{
	public event PanelMenuItemControllerEventHandler PanelMenuItemShowing;

	public event PanelMenuItemControllerEventHandler PanelMenuItemHiding;

	public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeLeft;

	public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeRight;

	public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeTop;

	public event PanelMenuItemControllerEventHandler PanelMenuItemSwipeBottom;

	public event PanelMenuItemControllerEventHandler PanelMenuItemTriggerPressed;

	public virtual void OnPanelMenuItemShowing(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemShowing != null)
		{
			PanelMenuItemShowing(this, e);
		}
	}

	public virtual void OnPanelMenuItemHiding(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemHiding != null)
		{
			PanelMenuItemHiding(this, e);
		}
	}

	public virtual void OnPanelMenuItemSwipeLeft(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemSwipeLeft != null)
		{
			PanelMenuItemSwipeLeft(this, e);
		}
	}

	public virtual void OnPanelMenuItemSwipeRight(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemSwipeRight != null)
		{
			PanelMenuItemSwipeRight(this, e);
		}
	}

	public virtual void OnPanelMenuItemSwipeTop(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemSwipeTop != null)
		{
			PanelMenuItemSwipeTop(this, e);
		}
	}

	public virtual void OnPanelMenuItemSwipeBottom(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemSwipeBottom != null)
		{
			PanelMenuItemSwipeBottom(this, e);
		}
	}

	public virtual PanelMenuItemControllerEventArgs SetPanelMenuItemEvent(GameObject interactableObject)
	{
		PanelMenuItemControllerEventArgs result = default;
		result.interactableObject = interactableObject;
		return result;
	}

	public virtual void Show(GameObject interactableObject)
	{
		gameObject.SetActive(value: true);
		OnPanelMenuItemShowing(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void Hide(GameObject interactableObject)
	{
		gameObject.SetActive(value: false);
		OnPanelMenuItemHiding(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void SwipeLeft(GameObject interactableObject)
	{
		OnPanelMenuItemSwipeLeft(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void SwipeRight(GameObject interactableObject)
	{
		OnPanelMenuItemSwipeRight(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void SwipeTop(GameObject interactableObject)
	{
		OnPanelMenuItemSwipeTop(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void SwipeBottom(GameObject interactableObject)
	{
		OnPanelMenuItemSwipeBottom(SetPanelMenuItemEvent(interactableObject));
	}

	public virtual void TriggerPressed(GameObject interactableObject)
	{
		OnPanelMenuItemTriggerPressed(SetPanelMenuItemEvent(interactableObject));
	}

	protected virtual void OnPanelMenuItemTriggerPressed(PanelMenuItemControllerEventArgs e)
	{
		if (PanelMenuItemTriggerPressed != null)
		{
			PanelMenuItemTriggerPressed(this, e);
		}
	}
}
