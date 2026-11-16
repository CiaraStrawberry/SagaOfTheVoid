using UnityEngine;

namespace VRTK;

[RequireComponent(typeof(VRTK_RadialMenu))]
public class VRTK_RadialMenuController : MonoBehaviour
{
	[Tooltip("The controller to listen to the controller events on.")]
	public VRTK_ControllerEvents events;

	protected VRTK_RadialMenu menu;

	protected float currentAngle;

	protected bool touchpadTouched;

	protected virtual void Awake()
	{
		menu = GetComponent<VRTK_RadialMenu>();
		Initialize();
	}

	protected virtual void Initialize()
	{
		if (events == null)
		{
			events = GetComponentInParent<VRTK_ControllerEvents>();
		}
	}

	protected virtual void OnEnable()
	{
		if (events == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.REQUIRED_COMPONENT_MISSING_NOT_INJECTED, "RadialMenuController", "VRTK_ControllerEvents", "events", "the parent"));
		}
		else
		{
			events.TouchpadPressed += DoTouchpadClicked;
			events.TouchpadReleased += DoTouchpadUnclicked;
			events.TouchpadTouchStart += DoTouchpadTouched;
			events.TouchpadTouchEnd += DoTouchpadUntouched;
			events.TouchpadAxisChanged += DoTouchpadAxisChanged;
			menu.FireHapticPulse += AttemptHapticPulse;
		}
	}

	protected virtual void OnDisable()
	{
		events.TouchpadPressed -= DoTouchpadClicked;
		events.TouchpadReleased -= DoTouchpadUnclicked;
		events.TouchpadTouchStart -= DoTouchpadTouched;
		events.TouchpadTouchEnd -= DoTouchpadUntouched;
		events.TouchpadAxisChanged -= DoTouchpadAxisChanged;
		menu.FireHapticPulse -= AttemptHapticPulse;
	}

	protected virtual void DoClickButton(object sender = null)
	{
		menu.ClickButton(currentAngle);
	}

	protected virtual void DoUnClickButton(object sender = null)
	{
		menu.UnClickButton(currentAngle);
	}

	protected virtual void DoShowMenu(float initialAngle, object sender = null)
	{
		menu.ShowMenu();
		DoChangeAngle(initialAngle);
	}

	protected virtual void DoHideMenu(bool force, object sender = null)
	{
		menu.StopTouching();
		menu.HideMenu(force);
	}

	protected virtual void DoChangeAngle(float angle, object sender = null)
	{
		currentAngle = angle;
		menu.HoverButton(currentAngle);
	}

	protected virtual void AttemptHapticPulse(float strength)
	{
		if ((bool)events)
		{
			VRTK_ControllerHaptics.TriggerHapticPulse(VRTK_ControllerReference.GetControllerReference(events.gameObject), strength);
		}
	}

	protected virtual void DoTouchpadClicked(object sender, ControllerInteractionEventArgs e)
	{
		DoClickButton();
	}

	protected virtual void DoTouchpadUnclicked(object sender, ControllerInteractionEventArgs e)
	{
		DoUnClickButton();
	}

	protected virtual void DoTouchpadTouched(object sender, ControllerInteractionEventArgs e)
	{
		touchpadTouched = true;
		DoShowMenu(CalculateAngle(e));
	}

	protected virtual void DoTouchpadUntouched(object sender, ControllerInteractionEventArgs e)
	{
		touchpadTouched = false;
		DoHideMenu(force: false);
	}

	protected virtual void DoTouchpadAxisChanged(object sender, ControllerInteractionEventArgs e)
	{
		if (touchpadTouched)
		{
			DoChangeAngle(CalculateAngle(e));
		}
	}

	protected virtual float CalculateAngle(ControllerInteractionEventArgs e)
	{
		return 360f - e.touchpadAngle;
	}
}
