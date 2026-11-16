using System;
using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Interactions/VRTK_ControllerEvents")]
public class VRTK_ControllerEvents : MonoBehaviour
{
	public enum ButtonAlias
	{
		Undefined,
		TriggerHairline,
		TriggerTouch,
		TriggerPress,
		TriggerClick,
		GripHairline,
		GripTouch,
		GripPress,
		GripClick,
		TouchpadTouch,
		TouchpadPress,
		ButtonOneTouch,
		ButtonOnePress,
		ButtonTwoTouch,
		ButtonTwoPress,
		StartMenuPress
	}

	[Header("Action Alias Buttons")]
	[Tooltip("**OBSOLETE [use VRTK_Pointer.activationButton]** The button to use for the action of turning a laser pointer on / off.")]
	[Obsolete("`VRTK_ControllerEvents.pointerToggleButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.activationButton` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias pointerToggleButton = ButtonAlias.TouchpadPress;

	[Tooltip("**OBSOLETE [use VRTK_Pointer.selectionButton]** The button to use for the action of setting a destination marker from the cursor position of the pointer.")]
	[Obsolete("`VRTK_ControllerEvents.pointerSetButton` is no longer used in the new `VRTK_Pointer` class, use `VRTK_Pointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias pointerSetButton = ButtonAlias.TouchpadPress;

	[Tooltip("**OBSOLETE [use VRTK_InteractGrab.grabButton]** The button to use for the action of grabbing game objects.")]
	[Obsolete("`VRTK_ControllerEvents.grabToggleButton` is no longer used in the `VRTK_InteractGrab` class, use `VRTK_InteractGrab.grabButton` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias grabToggleButton = ButtonAlias.GripPress;

	[Tooltip("**OBSOLETE [use VRTK_InteractUse.useButton]** The button to use for the action of using game objects.")]
	[Obsolete("`VRTK_ControllerEvents.useToggleButton` is no longer used in the `VRTK_InteractUse` class, use `VRTK_InteractUse.useButton` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias useToggleButton = ButtonAlias.TriggerPress;

	[Tooltip("**OBSOLETE [use VRTK_UIPointer.selectionButton]** The button to use for the action of clicking a UI element.")]
	[Obsolete("`VRTK_ControllerEvents.uiClickButton` is no longer used in the `VRTK_UIPointer` class, use `VRTK_UIPointer.selectionButton` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias uiClickButton = ButtonAlias.TriggerPress;

	[Tooltip("**OBSOLETE [use VRTK_ControllerEvents.buttonTwoPressed]** The button to use for the action of bringing up an in-game menu.")]
	[Obsolete("`VRTK_ControllerEvents.menuToggleButton` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
	public ButtonAlias menuToggleButton = ButtonAlias.ButtonTwoPress;

	[Header("Axis Refinement")]
	[Tooltip("The amount of fidelity in the changes on the axis, which is defaulted to 1. Any number higher than 2 will probably give too sensitive results.")]
	public int axisFidelity = 1;

	[Tooltip("The level on the trigger axis to reach before a click is registered.")]
	public float triggerClickThreshold = 1f;

	[Tooltip("The level on the trigger axis to reach before the axis is forced to 0f.")]
	public float triggerForceZeroThreshold = 0.01f;

	[Tooltip("If this is checked then the trigger axis will be forced to 0f when the trigger button reports an untouch event.")]
	public bool triggerAxisZeroOnUntouch;

	[Tooltip("The level on the grip axis to reach before a click is registered.")]
	public float gripClickThreshold = 1f;

	[Tooltip("The level on the grip axis to reach before the axis is forced to 0f.")]
	public float gripForceZeroThreshold = 0.01f;

	[Tooltip("If this is checked then the grip axis will be forced to 0f when the grip button reports an untouch event.")]
	public bool gripAxisZeroOnUntouch;

	[HideInInspector]
	public bool triggerPressed;

	[HideInInspector]
	public bool triggerTouched;

	[HideInInspector]
	public bool triggerHairlinePressed;

	[HideInInspector]
	public bool triggerClicked;

	[HideInInspector]
	public bool triggerAxisChanged;

	[HideInInspector]
	public bool gripPressed;

	[HideInInspector]
	public bool gripTouched;

	[HideInInspector]
	public bool gripHairlinePressed;

	[HideInInspector]
	public bool gripClicked;

	[HideInInspector]
	public bool gripAxisChanged;

	[HideInInspector]
	public bool touchpadPressed;

	[HideInInspector]
	public bool touchpadTouched;

	[HideInInspector]
	public bool touchpadAxisChanged;

	[HideInInspector]
	public bool buttonOnePressed;

	[HideInInspector]
	public bool buttonOneTouched;

	[HideInInspector]
	public bool buttonTwoPressed;

	[HideInInspector]
	public bool buttonTwoTouched;

	[HideInInspector]
	public bool startMenuPressed;

	[HideInInspector]
	[Obsolete("`VRTK_ControllerEvents.pointerPressed` is no longer used, use `VRTK_Pointer.IsActivationButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
	public bool pointerPressed;

	[HideInInspector]
	[Obsolete("`VRTK_ControllerEvents.grabPressed` is no longer used, use `VRTK_InteractGrab.IsGrabButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
	public bool grabPressed;

	[HideInInspector]
	[Obsolete("`VRTK_ControllerEvents.usePressed` is no longer used, use `VRTK_InteractUse.IsUseButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
	public bool usePressed;

	[HideInInspector]
	[Obsolete("`VRTK_ControllerEvents.uiClickPressed` is no longer used, use `VRTK_UIPointer.IsSelectionButtonPressed()` instead. This parameter will be removed in a future version of VRTK.")]
	public bool uiClickPressed;

	[HideInInspector]
	[Obsolete("`VRTK_ControllerEvents.menuPressed` is no longer used, use `VRTK_ControllerEvents.buttonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
	public bool menuPressed;

	[HideInInspector]
	public bool controllerVisible = true;

	protected Vector2 touchpadAxis = Vector2.zero;

	protected Vector2 triggerAxis = Vector2.zero;

	protected Vector2 gripAxis = Vector2.zero;

	protected float hairTriggerDelta;

	protected float hairGripDelta;

	public event ControllerInteractionEventHandler TriggerPressed;

	public event ControllerInteractionEventHandler TriggerReleased;

	public event ControllerInteractionEventHandler TriggerTouchStart;

	public event ControllerInteractionEventHandler TriggerTouchEnd;

	public event ControllerInteractionEventHandler TriggerHairlineStart;

	public event ControllerInteractionEventHandler TriggerHairlineEnd;

	public event ControllerInteractionEventHandler TriggerClicked;

	public event ControllerInteractionEventHandler TriggerUnclicked;

	public event ControllerInteractionEventHandler TriggerAxisChanged;

	public event ControllerInteractionEventHandler GripPressed;

	public event ControllerInteractionEventHandler GripReleased;

	public event ControllerInteractionEventHandler GripTouchStart;

	public event ControllerInteractionEventHandler GripTouchEnd;

	public event ControllerInteractionEventHandler GripHairlineStart;

	public event ControllerInteractionEventHandler GripHairlineEnd;

	public event ControllerInteractionEventHandler GripClicked;

	public event ControllerInteractionEventHandler GripUnclicked;

	public event ControllerInteractionEventHandler GripAxisChanged;

	public event ControllerInteractionEventHandler TouchpadPressed;

	public event ControllerInteractionEventHandler TouchpadReleased;

	public event ControllerInteractionEventHandler TouchpadTouchStart;

	public event ControllerInteractionEventHandler TouchpadTouchEnd;

	public event ControllerInteractionEventHandler TouchpadAxisChanged;

	public event ControllerInteractionEventHandler ButtonOneTouchStart;

	public event ControllerInteractionEventHandler ButtonOneTouchEnd;

	public event ControllerInteractionEventHandler ButtonOnePressed;

	public event ControllerInteractionEventHandler ButtonOneReleased;

	public event ControllerInteractionEventHandler ButtonTwoTouchStart;

	public event ControllerInteractionEventHandler ButtonTwoTouchEnd;

	public event ControllerInteractionEventHandler ButtonTwoPressed;

	public event ControllerInteractionEventHandler ButtonTwoReleased;

	public event ControllerInteractionEventHandler StartMenuPressed;

	public event ControllerInteractionEventHandler StartMenuReleased;

	[Obsolete("`VRTK_ControllerEvents.AliasPointerOn` has been replaced with `VRTK_Pointer.ActivationButtonPressed`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasPointerOn;

	[Obsolete("`VRTK_ControllerEvents.AliasPointerOff` has been replaced with `VRTK_Pointer.ActivationButtonReleased`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasPointerOff;

	[Obsolete("`VRTK_ControllerEvents.AliasPointerSet` has been replaced with `VRTK_Pointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasPointerSet;

	[Obsolete("`VRTK_ControllerEvents.AliasGrabOn` has been replaced with `VRTK_InteractGrab.GrabButtonPressed`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasGrabOn;

	[Obsolete("`VRTK_ControllerEvents.AliasGrabOff` has been replaced with `VRTK_InteractGrab.GrabButtonReleased`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasGrabOff;

	[Obsolete("`VRTK_ControllerEvents.AliasUseOn` has been replaced with `VRTK_InteractUse.UseButtonPressed`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasUseOn;

	[Obsolete("`VRTK_ControllerEvents.AliasUseOff` has been replaced with `VRTK_InteractUse.UseButtonReleased`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasUseOff;

	[Obsolete("`VRTK_ControllerEvents.AliasMenuOn` is no longer used, use `VRTK_ControllerEvents.ButtonTwoPressed` instead. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasMenuOn;

	[Obsolete("`VRTK_ControllerEvents.AliasMenuOff` is no longer used, use `VRTK_ControllerEvents.ButtonTwoReleased` instead. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasMenuOff;

	[Obsolete("`VRTK_ControllerEvents.AliasUIClickOn` has been replaced with `VRTK_UIPointer.SelectionButtonPressed`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasUIClickOn;

	[Obsolete("`VRTK_ControllerEvents.AliasUIClickOff` has been replaced with `VRTK_UIPointer.SelectionButtonReleased`. This parameter will be removed in a future version of VRTK.")]
	public event ControllerInteractionEventHandler AliasUIClickOff;

	public event ControllerInteractionEventHandler ControllerEnabled;

	public event ControllerInteractionEventHandler ControllerDisabled;

	public event ControllerInteractionEventHandler ControllerIndexChanged;

	public event ControllerInteractionEventHandler ControllerVisible;

	public event ControllerInteractionEventHandler ControllerHidden;

	public virtual void OnTriggerPressed(ControllerInteractionEventArgs e)
	{
		if (TriggerPressed != null)
		{
			TriggerPressed(this, e);
		}
	}

	public virtual void OnTriggerReleased(ControllerInteractionEventArgs e)
	{
		if (TriggerReleased != null)
		{
			TriggerReleased(this, e);
		}
	}

	public virtual void OnTriggerTouchStart(ControllerInteractionEventArgs e)
	{
		if (TriggerTouchStart != null)
		{
			TriggerTouchStart(this, e);
		}
	}

	public virtual void OnTriggerTouchEnd(ControllerInteractionEventArgs e)
	{
		if (TriggerTouchEnd != null)
		{
			TriggerTouchEnd(this, e);
		}
	}

	public virtual void OnTriggerHairlineStart(ControllerInteractionEventArgs e)
	{
		if (TriggerHairlineStart != null)
		{
			TriggerHairlineStart(this, e);
		}
	}

	public virtual void OnTriggerHairlineEnd(ControllerInteractionEventArgs e)
	{
		if (TriggerHairlineEnd != null)
		{
			TriggerHairlineEnd(this, e);
		}
	}

	public virtual void OnTriggerClicked(ControllerInteractionEventArgs e)
	{
		if (TriggerClicked != null)
		{
			TriggerClicked(this, e);
		}
	}

	public virtual void OnTriggerUnclicked(ControllerInteractionEventArgs e)
	{
		if (TriggerUnclicked != null)
		{
			TriggerUnclicked(this, e);
		}
	}

	public virtual void OnTriggerAxisChanged(ControllerInteractionEventArgs e)
	{
		if (TriggerAxisChanged != null)
		{
			TriggerAxisChanged(this, e);
		}
	}

	public virtual void OnGripPressed(ControllerInteractionEventArgs e)
	{
		if (GripPressed != null)
		{
			GripPressed(this, e);
		}
	}

	public virtual void OnGripReleased(ControllerInteractionEventArgs e)
	{
		if (GripReleased != null)
		{
			GripReleased(this, e);
		}
	}

	public virtual void OnGripTouchStart(ControllerInteractionEventArgs e)
	{
		if (GripTouchStart != null)
		{
			GripTouchStart(this, e);
		}
	}

	public virtual void OnGripTouchEnd(ControllerInteractionEventArgs e)
	{
		if (GripTouchEnd != null)
		{
			GripTouchEnd(this, e);
		}
	}

	public virtual void OnGripHairlineStart(ControllerInteractionEventArgs e)
	{
		if (GripHairlineStart != null)
		{
			GripHairlineStart(this, e);
		}
	}

	public virtual void OnGripHairlineEnd(ControllerInteractionEventArgs e)
	{
		if (GripHairlineEnd != null)
		{
			GripHairlineEnd(this, e);
		}
	}

	public virtual void OnGripClicked(ControllerInteractionEventArgs e)
	{
		if (GripClicked != null)
		{
			GripClicked(this, e);
		}
	}

	public virtual void OnGripUnclicked(ControllerInteractionEventArgs e)
	{
		if (GripUnclicked != null)
		{
			GripUnclicked(this, e);
		}
	}

	public virtual void OnGripAxisChanged(ControllerInteractionEventArgs e)
	{
		if (GripAxisChanged != null)
		{
			GripAxisChanged(this, e);
		}
	}

	public virtual void OnTouchpadPressed(ControllerInteractionEventArgs e)
	{
		if (TouchpadPressed != null)
		{
			TouchpadPressed(this, e);
		}
	}

	public virtual void OnTouchpadReleased(ControllerInteractionEventArgs e)
	{
		if (TouchpadReleased != null)
		{
			TouchpadReleased(this, e);
		}
	}

	public virtual void OnTouchpadTouchStart(ControllerInteractionEventArgs e)
	{
		if (TouchpadTouchStart != null)
		{
			TouchpadTouchStart(this, e);
		}
	}

	public virtual void OnTouchpadTouchEnd(ControllerInteractionEventArgs e)
	{
		if (TouchpadTouchEnd != null)
		{
			TouchpadTouchEnd(this, e);
		}
	}

	public virtual void OnTouchpadAxisChanged(ControllerInteractionEventArgs e)
	{
		if (TouchpadAxisChanged != null)
		{
			TouchpadAxisChanged(this, e);
		}
	}

	public virtual void OnButtonOneTouchStart(ControllerInteractionEventArgs e)
	{
		if (ButtonOneTouchStart != null)
		{
			ButtonOneTouchStart(this, e);
		}
	}

	public virtual void OnButtonOneTouchEnd(ControllerInteractionEventArgs e)
	{
		if (ButtonOneTouchEnd != null)
		{
			ButtonOneTouchEnd(this, e);
		}
	}

	public virtual void OnButtonOnePressed(ControllerInteractionEventArgs e)
	{
		if (ButtonOnePressed != null)
		{
			ButtonOnePressed(this, e);
		}
	}

	public virtual void OnButtonOneReleased(ControllerInteractionEventArgs e)
	{
		if (ButtonOneReleased != null)
		{
			ButtonOneReleased(this, e);
		}
	}

	public virtual void OnButtonTwoTouchStart(ControllerInteractionEventArgs e)
	{
		if (ButtonTwoTouchStart != null)
		{
			ButtonTwoTouchStart(this, e);
		}
	}

	public virtual void OnButtonTwoTouchEnd(ControllerInteractionEventArgs e)
	{
		if (ButtonTwoTouchEnd != null)
		{
			ButtonTwoTouchEnd(this, e);
		}
	}

	public virtual void OnButtonTwoPressed(ControllerInteractionEventArgs e)
	{
		if (ButtonTwoPressed != null)
		{
			ButtonTwoPressed(this, e);
		}
	}

	public virtual void OnButtonTwoReleased(ControllerInteractionEventArgs e)
	{
		if (ButtonTwoReleased != null)
		{
			ButtonTwoReleased(this, e);
		}
	}

	public virtual void OnStartMenuPressed(ControllerInteractionEventArgs e)
	{
		if (StartMenuPressed != null)
		{
			StartMenuPressed(this, e);
		}
	}

	public virtual void OnStartMenuReleased(ControllerInteractionEventArgs e)
	{
		if (StartMenuReleased != null)
		{
			StartMenuReleased(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasPointerOn` has been replaced with `VRTK_Pointer.OnActivationButtonPressed`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasPointerOn(ControllerInteractionEventArgs e)
	{
		if (AliasPointerOn != null)
		{
			AliasPointerOn(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasPointerOff` has been replaced with `VRTK_Pointer.OnActivationButtonReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasPointerOff(ControllerInteractionEventArgs e)
	{
		if (AliasPointerOff != null)
		{
			AliasPointerOff(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasPointerSet` has been replaced with `VRTK_Pointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasPointerSet(ControllerInteractionEventArgs e)
	{
		if (AliasPointerSet != null)
		{
			AliasPointerSet(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasGrabOn` has been replaced with `VRTK_InteractGrab.OnGrabButtonPressed`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasGrabOn(ControllerInteractionEventArgs e)
	{
		if (AliasGrabOn != null)
		{
			AliasGrabOn(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasGrabOff` has been replaced with `VRTK_InteractGrab.OnGrabButtonReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasGrabOff(ControllerInteractionEventArgs e)
	{
		if (AliasGrabOff != null)
		{
			AliasGrabOff(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasUseOn` has been replaced with `VRTK_InteractUse.OnUseButtonPressed`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasUseOn(ControllerInteractionEventArgs e)
	{
		if (AliasUseOn != null)
		{
			AliasUseOn(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasUseOff` has been replaced with `VRTK_InteractUse.OnUseButtonReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasUseOff(ControllerInteractionEventArgs e)
	{
		if (AliasUseOff != null)
		{
			AliasUseOff(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOn` has been replaced with `VRTK_UIPointer.OnSelectionButtonPressed`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasUIClickOn(ControllerInteractionEventArgs e)
	{
		if (AliasUIClickOn != null)
		{
			AliasUIClickOn(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasUIClickOff` has been replaced with `VRTK_UIPointer.OnSelectionButtonReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasUIClickOff(ControllerInteractionEventArgs e)
	{
		if (AliasUIClickOff != null)
		{
			AliasUIClickOff(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasMenuOn` has been replaced with `VRTK_ControllerEvents.OnButtonTwoPressed`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasMenuOn(ControllerInteractionEventArgs e)
	{
		if (AliasMenuOn != null)
		{
			AliasMenuOn(this, e);
		}
	}

	[Obsolete("`VRTK_ControllerEvents.OnAliasMenuOff` has been replaced with `VRTK_ControllerEvents.OnButtonTwoReleased`. This method will be removed in a future version of VRTK.")]
	public virtual void OnAliasMenuOff(ControllerInteractionEventArgs e)
	{
		if (AliasMenuOff != null)
		{
			AliasMenuOff(this, e);
		}
	}

	public virtual void OnControllerEnabled(ControllerInteractionEventArgs e)
	{
		if (ControllerEnabled != null)
		{
			ControllerEnabled(this, e);
		}
	}

	public virtual void OnControllerDisabled(ControllerInteractionEventArgs e)
	{
		if (ControllerDisabled != null)
		{
			ControllerDisabled(this, e);
		}
	}

	public virtual void OnControllerIndexChanged(ControllerInteractionEventArgs e)
	{
		if (ControllerIndexChanged != null)
		{
			ControllerIndexChanged(this, e);
		}
	}

	public virtual void OnControllerVisible(ControllerInteractionEventArgs e)
	{
		controllerVisible = true;
		if (ControllerVisible != null)
		{
			ControllerVisible(this, e);
		}
	}

	public virtual void OnControllerHidden(ControllerInteractionEventArgs e)
	{
		controllerVisible = false;
		if (ControllerHidden != null)
		{
			ControllerHidden(this, e);
		}
	}

	public virtual ControllerInteractionEventArgs SetControllerEvent()
	{
		bool buttonBool = false;
		return SetControllerEvent(ref buttonBool);
	}

	public virtual ControllerInteractionEventArgs SetControllerEvent(ref bool buttonBool, bool value = false, float buttonPressure = 0f)
	{
		VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
		buttonBool = value;
		ControllerInteractionEventArgs result = default;
		result.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		result.controllerReference = controllerReference;
		result.buttonPressure = buttonPressure;
		result.touchpadAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
		result.touchpadAngle = CalculateTouchpadAxisAngle(result.touchpadAxis);
		return result;
	}

	[Obsolete("`VRTK_ControllerEvents.GetVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
	public virtual Vector3 GetVelocity()
	{
		return VRTK_DeviceFinder.GetControllerVelocity(gameObject);
	}

	[Obsolete("`VRTK_ControllerEvents.GetAngularVelocity()` has been replaced with `VRTK_DeviceFinder.GetControllerAngularVelocity(givenController)`. This method will be removed in a future version of VRTK.")]
	public virtual Vector3 GetAngularVelocity()
	{
		return VRTK_DeviceFinder.GetControllerAngularVelocity(gameObject);
	}

	public virtual Vector2 GetTouchpadAxis()
	{
		return touchpadAxis;
	}

	public virtual float GetTouchpadAxisAngle()
	{
		return CalculateTouchpadAxisAngle(touchpadAxis);
	}

	public virtual float GetTriggerAxis()
	{
		return triggerAxis.x;
	}

	public virtual float GetGripAxis()
	{
		return gripAxis.x;
	}

	public virtual float GetHairTriggerDelta()
	{
		return hairTriggerDelta;
	}

	public virtual float GetHairGripDelta()
	{
		return hairGripDelta;
	}

	public virtual bool AnyButtonPressed()
	{
		return triggerPressed || gripPressed || touchpadPressed || buttonOnePressed || buttonTwoPressed || startMenuPressed;
	}

	public virtual bool IsButtonPressed(ButtonAlias button)
	{
		return button switch
		{
			ButtonAlias.TriggerHairline => triggerHairlinePressed, 
			ButtonAlias.TriggerTouch => triggerTouched, 
			ButtonAlias.TriggerPress => triggerPressed, 
			ButtonAlias.TriggerClick => triggerClicked, 
			ButtonAlias.GripHairline => gripHairlinePressed, 
			ButtonAlias.GripTouch => gripTouched, 
			ButtonAlias.GripPress => gripPressed, 
			ButtonAlias.GripClick => gripClicked, 
			ButtonAlias.TouchpadTouch => touchpadTouched, 
			ButtonAlias.TouchpadPress => touchpadPressed, 
			ButtonAlias.ButtonOnePress => buttonOnePressed, 
			ButtonAlias.ButtonOneTouch => buttonOneTouched, 
			ButtonAlias.ButtonTwoPress => buttonTwoPressed, 
			ButtonAlias.ButtonTwoTouch => buttonTwoTouched, 
			ButtonAlias.StartMenuPress => startMenuPressed, 
			_ => false, 
		};
	}

	public virtual void SubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
	{
		ButtonAliasEventSubscription(subscribe: true, givenButton, startEvent, callbackMethod);
	}

	public virtual void UnsubscribeToButtonAliasEvent(ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
	{
		ButtonAliasEventSubscription(subscribe: false, givenButton, startEvent, callbackMethod);
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		GameObject actualController = VRTK_DeviceFinder.GetActualController(gameObject);
		if ((bool)actualController)
		{
			VRTK_TrackedController component = actualController.GetComponent<VRTK_TrackedController>();
			if ((bool)component)
			{
				component.ControllerEnabled += TrackedControllerEnabled;
				component.ControllerDisabled += TrackedControllerDisabled;
				component.ControllerIndexChanged += TrackedControllerIndexChanged;
			}
		}
	}

	protected virtual void OnDisable()
	{
		Invoke("DisableEvents", 0f);
		GameObject actualController = VRTK_DeviceFinder.GetActualController(gameObject);
		if ((bool)actualController)
		{
			VRTK_TrackedController component = actualController.GetComponent<VRTK_TrackedController>();
			if ((bool)component)
			{
				component.ControllerEnabled -= TrackedControllerEnabled;
				component.ControllerDisabled -= TrackedControllerDisabled;
			}
		}
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
		if (VRTK_ControllerReference.IsValid(controllerReference))
		{
			Vector2 controllerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
			Vector2 controllerAxis2 = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
			Vector2 controllerAxis3 = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
			{
				OnTriggerTouchStart(SetControllerEvent(ref triggerTouched, value: true, controllerAxis.x));
				EmitAlias(ButtonAlias.TriggerTouch, touchDown: true, controllerAxis.x, ref triggerTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnTriggerHairlineStart(SetControllerEvent(ref triggerHairlinePressed, value: true, controllerAxis.x));
				EmitAlias(ButtonAlias.TriggerHairline, touchDown: true, controllerAxis.x, ref triggerHairlinePressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnTriggerPressed(SetControllerEvent(ref triggerPressed, value: true, controllerAxis.x));
				EmitAlias(ButtonAlias.TriggerPress, touchDown: true, controllerAxis.x, ref triggerPressed);
			}
			if (!triggerClicked && controllerAxis.x >= triggerClickThreshold)
			{
				OnTriggerClicked(SetControllerEvent(ref triggerClicked, value: true, controllerAxis.x));
				EmitAlias(ButtonAlias.TriggerClick, touchDown: true, controllerAxis.x, ref triggerClicked);
			}
			else if (triggerClicked && controllerAxis.x < triggerClickThreshold)
			{
				OnTriggerUnclicked(SetControllerEvent(ref triggerClicked));
				EmitAlias(ButtonAlias.TriggerClick, touchDown: false, 0f, ref triggerClicked);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnTriggerReleased(SetControllerEvent(ref triggerPressed));
				EmitAlias(ButtonAlias.TriggerPress, touchDown: false, 0f, ref triggerPressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.TriggerHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed));
				EmitAlias(ButtonAlias.TriggerHairline, touchDown: false, 0f, ref triggerHairlinePressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Trigger, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
			{
				OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched));
				EmitAlias(ButtonAlias.TriggerTouch, touchDown: false, 0f, ref triggerTouched);
			}
			controllerAxis.x = (((triggerTouched || !triggerAxisZeroOnUntouch) && !(controllerAxis.x < triggerForceZeroThreshold)) ? controllerAxis.x : 0f);
			if (VRTK_SharedMethods.Vector2ShallowCompare(triggerAxis, controllerAxis, axisFidelity))
			{
				triggerAxisChanged = false;
			}
			else
			{
				OnTriggerAxisChanged(SetControllerEvent(ref triggerAxisChanged, value: true, controllerAxis.x));
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
			{
				OnGripTouchStart(SetControllerEvent(ref gripTouched, value: true, controllerAxis2.x));
				EmitAlias(ButtonAlias.GripTouch, touchDown: true, controllerAxis2.x, ref gripTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnGripHairlineStart(SetControllerEvent(ref gripHairlinePressed, value: true, controllerAxis2.x));
				EmitAlias(ButtonAlias.GripHairline, touchDown: true, controllerAxis2.x, ref gripHairlinePressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnGripPressed(SetControllerEvent(ref gripPressed, value: true, controllerAxis2.x));
				EmitAlias(ButtonAlias.GripPress, touchDown: true, controllerAxis2.x, ref gripPressed);
			}
			if (!gripClicked && controllerAxis2.x >= gripClickThreshold)
			{
				OnGripClicked(SetControllerEvent(ref gripClicked, value: true, controllerAxis2.x));
				EmitAlias(ButtonAlias.GripClick, touchDown: true, controllerAxis2.x, ref gripClicked);
			}
			else if (gripClicked && controllerAxis2.x < gripClickThreshold)
			{
				OnGripUnclicked(SetControllerEvent(ref gripClicked));
				EmitAlias(ButtonAlias.GripClick, touchDown: false, 0f, ref gripClicked);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnGripReleased(SetControllerEvent(ref gripPressed));
				EmitAlias(ButtonAlias.GripPress, touchDown: false, 0f, ref gripPressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.GripHairline, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed));
				EmitAlias(ButtonAlias.GripHairline, touchDown: false, 0f, ref gripHairlinePressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Grip, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
			{
				OnGripTouchEnd(SetControllerEvent(ref gripTouched));
				EmitAlias(ButtonAlias.GripTouch, touchDown: false, 0f, ref gripTouched);
			}
			controllerAxis2.x = (((gripTouched || !gripAxisZeroOnUntouch) && !(controllerAxis2.x < gripForceZeroThreshold)) ? controllerAxis2.x : 0f);
			if (VRTK_SharedMethods.Vector2ShallowCompare(gripAxis, controllerAxis2, axisFidelity))
			{
				gripAxisChanged = false;
			}
			else
			{
				OnGripAxisChanged(SetControllerEvent(ref gripAxisChanged, value: true, controllerAxis2.x));
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
			{
				OnTouchpadTouchStart(SetControllerEvent(ref touchpadTouched, value: true, 1f));
				EmitAlias(ButtonAlias.TouchpadTouch, touchDown: true, 1f, ref touchpadTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnTouchpadPressed(SetControllerEvent(ref touchpadPressed, value: true, 1f));
				EmitAlias(ButtonAlias.TouchpadPress, touchDown: true, 1f, ref touchpadPressed);
			}
			else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnTouchpadReleased(SetControllerEvent(ref touchpadPressed));
				EmitAlias(ButtonAlias.TouchpadPress, touchDown: false, 0f, ref touchpadPressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.Touchpad, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
			{
				OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched));
				EmitAlias(ButtonAlias.TouchpadTouch, touchDown: false, 0f, ref touchpadTouched);
				touchpadAxis = Vector2.zero;
			}
			if (VRTK_SDK_Bridge.IsTouchpadStatic(touchpadTouched, touchpadAxis, controllerAxis3, axisFidelity))
			{
				touchpadAxisChanged = false;
			}
			else
			{
				OnTouchpadAxisChanged(SetControllerEvent(ref touchpadAxisChanged, value: true, 1f));
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
			{
				OnButtonOneTouchStart(SetControllerEvent(ref buttonOneTouched, value: true, 1f));
				EmitAlias(ButtonAlias.ButtonOneTouch, touchDown: true, 1f, ref buttonOneTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnButtonOnePressed(SetControllerEvent(ref buttonOnePressed, value: true, 1f));
				EmitAlias(ButtonAlias.ButtonOnePress, touchDown: true, 1f, ref buttonOnePressed);
			}
			else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed));
				EmitAlias(ButtonAlias.ButtonOnePress, touchDown: false, 0f, ref buttonOnePressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonOne, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
			{
				OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched));
				EmitAlias(ButtonAlias.ButtonOneTouch, touchDown: false, 0f, ref buttonOneTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchDown, controllerReference))
			{
				OnButtonTwoTouchStart(SetControllerEvent(ref buttonTwoTouched, value: true, 1f));
				EmitAlias(ButtonAlias.ButtonTwoTouch, touchDown: true, 1f, ref buttonTwoTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnButtonTwoPressed(SetControllerEvent(ref buttonTwoPressed, value: true, 1f));
				EmitAlias(ButtonAlias.ButtonTwoPress, touchDown: true, 1f, ref buttonTwoPressed);
			}
			else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed));
				EmitAlias(ButtonAlias.ButtonTwoPress, touchDown: false, 0f, ref buttonTwoPressed);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.ButtonTwo, SDK_BaseController.ButtonPressTypes.TouchUp, controllerReference))
			{
				OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched));
				EmitAlias(ButtonAlias.ButtonTwoTouch, touchDown: false, 0f, ref buttonTwoTouched);
			}
			if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressDown, controllerReference))
			{
				OnStartMenuPressed(SetControllerEvent(ref startMenuPressed, value: true, 1f));
				EmitAlias(ButtonAlias.StartMenuPress, touchDown: true, 1f, ref startMenuPressed);
			}
			else if (VRTK_SDK_Bridge.GetControllerButtonState(SDK_BaseController.ButtonTypes.StartMenu, SDK_BaseController.ButtonPressTypes.PressUp, controllerReference))
			{
				OnStartMenuReleased(SetControllerEvent(ref startMenuPressed));
				EmitAlias(ButtonAlias.StartMenuPress, touchDown: false, 0f, ref startMenuPressed);
			}
			touchpadAxis = ((!touchpadAxisChanged) ? touchpadAxis : new Vector2(controllerAxis3.x, controllerAxis3.y));
			triggerAxis = ((!triggerAxisChanged) ? triggerAxis : new Vector2(controllerAxis.x, controllerAxis.y));
			gripAxis = ((!gripAxisChanged) ? gripAxis : new Vector2(controllerAxis2.x, controllerAxis2.y));
			hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
			hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);
		}
	}

	protected virtual void ButtonAliasEventSubscription(bool subscribe, ButtonAlias givenButton, bool startEvent, ControllerInteractionEventHandler callbackMethod)
	{
		switch (givenButton)
		{
		case ButtonAlias.TriggerClick:
			if (subscribe)
			{
				if (startEvent)
				{
					TriggerClicked += callbackMethod;
				}
				else
				{
					TriggerUnclicked += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TriggerClicked -= callbackMethod;
			}
			else
			{
				TriggerUnclicked -= callbackMethod;
			}
			break;
		case ButtonAlias.TriggerHairline:
			if (subscribe)
			{
				if (startEvent)
				{
					TriggerHairlineStart += callbackMethod;
				}
				else
				{
					TriggerHairlineEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TriggerHairlineStart -= callbackMethod;
			}
			else
			{
				TriggerHairlineEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.TriggerPress:
			if (subscribe)
			{
				if (startEvent)
				{
					TriggerPressed += callbackMethod;
				}
				else
				{
					TriggerReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TriggerPressed -= callbackMethod;
			}
			else
			{
				TriggerReleased -= callbackMethod;
			}
			break;
		case ButtonAlias.TriggerTouch:
			if (subscribe)
			{
				if (startEvent)
				{
					TriggerTouchStart += callbackMethod;
				}
				else
				{
					TriggerTouchEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TriggerTouchStart -= callbackMethod;
			}
			else
			{
				TriggerTouchEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.GripClick:
			if (subscribe)
			{
				if (startEvent)
				{
					GripClicked += callbackMethod;
				}
				else
				{
					GripUnclicked += callbackMethod;
				}
			}
			else if (startEvent)
			{
				GripClicked -= callbackMethod;
			}
			else
			{
				GripUnclicked -= callbackMethod;
			}
			break;
		case ButtonAlias.GripHairline:
			if (subscribe)
			{
				if (startEvent)
				{
					GripHairlineStart += callbackMethod;
				}
				else
				{
					GripHairlineEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				GripHairlineStart -= callbackMethod;
			}
			else
			{
				GripHairlineEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.GripPress:
			if (subscribe)
			{
				if (startEvent)
				{
					GripPressed += callbackMethod;
				}
				else
				{
					GripReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				GripPressed -= callbackMethod;
			}
			else
			{
				GripReleased -= callbackMethod;
			}
			break;
		case ButtonAlias.GripTouch:
			if (subscribe)
			{
				if (startEvent)
				{
					GripTouchStart += callbackMethod;
				}
				else
				{
					GripTouchEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				GripTouchStart -= callbackMethod;
			}
			else
			{
				GripTouchEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.TouchpadPress:
			if (subscribe)
			{
				if (startEvent)
				{
					TouchpadPressed += callbackMethod;
				}
				else
				{
					TouchpadReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TouchpadPressed -= callbackMethod;
			}
			else
			{
				TouchpadReleased -= callbackMethod;
			}
			break;
		case ButtonAlias.TouchpadTouch:
			if (subscribe)
			{
				if (startEvent)
				{
					TouchpadTouchStart += callbackMethod;
				}
				else
				{
					TouchpadTouchEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				TouchpadTouchStart -= callbackMethod;
			}
			else
			{
				TouchpadTouchEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.ButtonOnePress:
			if (subscribe)
			{
				if (startEvent)
				{
					ButtonOnePressed += callbackMethod;
				}
				else
				{
					ButtonOneReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				ButtonOnePressed -= callbackMethod;
			}
			else
			{
				ButtonOneReleased -= callbackMethod;
			}
			break;
		case ButtonAlias.ButtonOneTouch:
			if (subscribe)
			{
				if (startEvent)
				{
					ButtonOneTouchStart += callbackMethod;
				}
				else
				{
					ButtonOneTouchEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				ButtonOneTouchStart -= callbackMethod;
			}
			else
			{
				ButtonOneTouchEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.ButtonTwoPress:
			if (subscribe)
			{
				if (startEvent)
				{
					ButtonTwoPressed += callbackMethod;
				}
				else
				{
					ButtonTwoReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				ButtonTwoPressed -= callbackMethod;
			}
			else
			{
				ButtonTwoReleased -= callbackMethod;
			}
			break;
		case ButtonAlias.ButtonTwoTouch:
			if (subscribe)
			{
				if (startEvent)
				{
					ButtonTwoTouchStart += callbackMethod;
				}
				else
				{
					ButtonTwoTouchEnd += callbackMethod;
				}
			}
			else if (startEvent)
			{
				ButtonTwoTouchStart -= callbackMethod;
			}
			else
			{
				ButtonTwoTouchEnd -= callbackMethod;
			}
			break;
		case ButtonAlias.StartMenuPress:
			if (subscribe)
			{
				if (startEvent)
				{
					StartMenuPressed += callbackMethod;
				}
				else
				{
					StartMenuReleased += callbackMethod;
				}
			}
			else if (startEvent)
			{
				StartMenuPressed -= callbackMethod;
			}
			else
			{
				StartMenuReleased -= callbackMethod;
			}
			break;
		}
	}

	protected virtual void TrackedControllerEnabled(object sender, VRTKTrackedControllerEventArgs e)
	{
		OnControllerEnabled(SetControllerEvent());
	}

	protected virtual void TrackedControllerDisabled(object sender, VRTKTrackedControllerEventArgs e)
	{
		DisableEvents();
		OnControllerDisabled(SetControllerEvent());
	}

	protected virtual void TrackedControllerIndexChanged(object sender, VRTKTrackedControllerEventArgs e)
	{
		OnControllerIndexChanged(SetControllerEvent());
	}

	protected virtual float CalculateTouchpadAxisAngle(Vector2 axis)
	{
		float num = Mathf.Atan2(axis.y, axis.x) * 57.29578f;
		num = 90f - num;
		if (num < 0f)
		{
			num += 360f;
		}
		return num;
	}

	protected virtual void EmitAlias(ButtonAlias type, bool touchDown, float buttonPressure, ref bool buttonBool)
	{
		if (pointerToggleButton == type)
		{
			if (touchDown)
			{
				pointerPressed = true;
				OnAliasPointerOn(SetControllerEvent(ref buttonBool, value: true, buttonPressure));
			}
			else
			{
				pointerPressed = false;
				OnAliasPointerOff(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
			}
		}
		if (pointerSetButton == type && !touchDown)
		{
			OnAliasPointerSet(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
		}
		if (grabToggleButton == type)
		{
			if (touchDown)
			{
				grabPressed = true;
				OnAliasGrabOn(SetControllerEvent(ref buttonBool, value: true, buttonPressure));
			}
			else
			{
				grabPressed = false;
				OnAliasGrabOff(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
			}
		}
		if (useToggleButton == type)
		{
			if (touchDown)
			{
				usePressed = true;
				OnAliasUseOn(SetControllerEvent(ref buttonBool, value: true, buttonPressure));
			}
			else
			{
				usePressed = false;
				OnAliasUseOff(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
			}
		}
		if (uiClickButton == type)
		{
			if (touchDown)
			{
				uiClickPressed = true;
				OnAliasUIClickOn(SetControllerEvent(ref buttonBool, value: true, buttonPressure));
			}
			else
			{
				uiClickPressed = false;
				OnAliasUIClickOff(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
			}
		}
		if (menuToggleButton == type)
		{
			if (touchDown)
			{
				menuPressed = true;
				OnAliasMenuOn(SetControllerEvent(ref buttonBool, value: true, buttonPressure));
			}
			else
			{
				menuPressed = false;
				OnAliasMenuOff(SetControllerEvent(ref buttonBool, value: false, buttonPressure));
			}
		}
	}

	protected virtual void DisableEvents()
	{
		if (triggerPressed)
		{
			OnTriggerReleased(SetControllerEvent(ref triggerPressed));
			EmitAlias(ButtonAlias.TriggerPress, touchDown: false, 0f, ref triggerPressed);
		}
		if (triggerTouched)
		{
			OnTriggerTouchEnd(SetControllerEvent(ref triggerTouched));
			EmitAlias(ButtonAlias.TriggerTouch, touchDown: false, 0f, ref triggerTouched);
		}
		if (triggerHairlinePressed)
		{
			OnTriggerHairlineEnd(SetControllerEvent(ref triggerHairlinePressed));
			EmitAlias(ButtonAlias.TriggerHairline, touchDown: false, 0f, ref triggerHairlinePressed);
		}
		if (triggerClicked)
		{
			OnTriggerUnclicked(SetControllerEvent(ref triggerClicked));
			EmitAlias(ButtonAlias.TriggerClick, touchDown: false, 0f, ref triggerClicked);
		}
		if (gripPressed)
		{
			OnGripReleased(SetControllerEvent(ref gripPressed));
			EmitAlias(ButtonAlias.GripPress, touchDown: false, 0f, ref gripPressed);
		}
		if (gripTouched)
		{
			OnGripTouchEnd(SetControllerEvent(ref gripTouched));
			EmitAlias(ButtonAlias.GripTouch, touchDown: false, 0f, ref gripTouched);
		}
		if (gripHairlinePressed)
		{
			OnGripHairlineEnd(SetControllerEvent(ref gripHairlinePressed));
			EmitAlias(ButtonAlias.GripHairline, touchDown: false, 0f, ref gripHairlinePressed);
		}
		if (gripClicked)
		{
			OnGripUnclicked(SetControllerEvent(ref gripClicked));
			EmitAlias(ButtonAlias.GripClick, touchDown: false, 0f, ref gripClicked);
		}
		if (touchpadPressed)
		{
			OnTouchpadReleased(SetControllerEvent(ref touchpadPressed));
			EmitAlias(ButtonAlias.TouchpadPress, touchDown: false, 0f, ref touchpadPressed);
		}
		if (touchpadTouched)
		{
			OnTouchpadTouchEnd(SetControllerEvent(ref touchpadTouched));
			EmitAlias(ButtonAlias.TouchpadTouch, touchDown: false, 0f, ref touchpadTouched);
		}
		if (buttonOnePressed)
		{
			OnButtonOneReleased(SetControllerEvent(ref buttonOnePressed));
			EmitAlias(ButtonAlias.ButtonOnePress, touchDown: false, 0f, ref buttonOnePressed);
		}
		if (buttonOneTouched)
		{
			OnButtonOneTouchEnd(SetControllerEvent(ref buttonOneTouched));
			EmitAlias(ButtonAlias.ButtonOneTouch, touchDown: false, 0f, ref buttonOneTouched);
		}
		if (buttonTwoPressed)
		{
			OnButtonTwoReleased(SetControllerEvent(ref buttonTwoPressed));
			EmitAlias(ButtonAlias.ButtonTwoPress, touchDown: false, 0f, ref buttonTwoPressed);
		}
		if (buttonTwoTouched)
		{
			OnButtonTwoTouchEnd(SetControllerEvent(ref buttonTwoTouched));
			EmitAlias(ButtonAlias.ButtonTwoTouch, touchDown: false, 0f, ref buttonTwoTouched);
		}
		if (startMenuPressed)
		{
			OnStartMenuReleased(SetControllerEvent(ref startMenuPressed));
			EmitAlias(ButtonAlias.StartMenuPress, touchDown: false, 0f, ref startMenuPressed);
		}
		triggerAxisChanged = false;
		gripAxisChanged = false;
		touchpadAxisChanged = false;
		VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(gameObject);
		if (VRTK_ControllerReference.IsValid(controllerReference))
		{
			Vector2 controllerAxis = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Trigger, controllerReference);
			Vector2 controllerAxis2 = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Grip, controllerReference);
			Vector2 controllerAxis3 = VRTK_SDK_Bridge.GetControllerAxis(SDK_BaseController.ButtonTypes.Touchpad, controllerReference);
			touchpadAxis = new Vector2(controllerAxis3.x, controllerAxis3.y);
			triggerAxis = new Vector2(controllerAxis.x, controllerAxis.y);
			gripAxis = new Vector2(controllerAxis2.x, controllerAxis2.y);
			hairTriggerDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.TriggerHairline, controllerReference);
			hairGripDelta = VRTK_SDK_Bridge.GetControllerHairlineDelta(SDK_BaseController.ButtonTypes.GripHairline, controllerReference);
		}
	}
}
