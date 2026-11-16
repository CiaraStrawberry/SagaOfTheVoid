using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.General;

public class TriggerInputHandler : TriggerInputHandlerBase
{
	[SerializeField]
	private bool _triggerMouseClick = true;

	[SerializeField]
	private KeyCode _triggerKeyCode;

	public bool useCursorIcon = true;

	[SerializeField]
	private CursorIcon _cursorIcon;

	public override TriggerActionInfo actionInfo => new TriggerActionInfo
	{
		actionName = triggerKeyCode.ToString()
	};

	public virtual bool triggerMouseClick => _triggerMouseClick;

	public virtual KeyCode triggerKeyCode => _triggerKeyCode;

	public virtual CursorIcon cursorIcon => _cursorIcon;

	public override bool AreKeysDown()
	{
		if (_triggerKeyCode == KeyCode.None)
		{
			return false;
		}
		return Input.GetKeyDown(_triggerKeyCode);
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		if (useCursorIcon)
		{
			cursorIcon.Enable();
		}
	}

	public override void OnPointerClick(PointerEventData eventData)
	{
		base.OnPointerClick(eventData);
		if (_triggerMouseClick)
		{
			Use();
		}
	}

	public override string ToString()
	{
		return triggerKeyCode.ToString();
	}
}
