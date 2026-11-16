using UnityEngine;

namespace VRTK;

public class VRTK_ControllerTooltips : MonoBehaviour
{
	public enum TooltipButtons
	{
		None,
		TriggerTooltip,
		GripTooltip,
		TouchpadTooltip,
		ButtonOneTooltip,
		ButtonTwoTooltip,
		StartMenuTooltip
	}

	[Header("Button Text Settings")]
	[Tooltip("The text to display for the trigger button action.")]
	public string triggerText;

	[Tooltip("The text to display for the grip button action.")]
	public string gripText;

	[Tooltip("The text to display for the touchpad action.")]
	public string touchpadText;

	[Tooltip("The text to display for button one action.")]
	public string buttonOneText;

	[Tooltip("The text to display for button two action.")]
	public string buttonTwoText;

	[Tooltip("The text to display for the start menu action.")]
	public string startMenuText;

	[Header("Tooltip Colour Settings")]
	[Tooltip("The colour to use for the tooltip background container.")]
	public Color tipBackgroundColor = Color.black;

	[Tooltip("The colour to use for the text within the tooltip.")]
	public Color tipTextColor = Color.white;

	[Tooltip("The colour to use for the line between the tooltip and the relevant controller button.")]
	public Color tipLineColor = Color.black;

	[Header("Button Transform Settings")]
	[Tooltip("The transform for the position of the trigger button on the controller.")]
	public Transform trigger;

	[Tooltip("The transform for the position of the grip button on the controller.")]
	public Transform grip;

	[Tooltip("The transform for the position of the touchpad button on the controller.")]
	public Transform touchpad;

	[Tooltip("The transform for the position of button one on the controller.")]
	public Transform buttonOne;

	[Tooltip("The transform for the position of button two on the controller.")]
	public Transform buttonTwo;

	[Tooltip("The transform for the position of the start menu on the controller.")]
	public Transform startMenu;

	[Header("Custom Settings")]
	[Tooltip("The controller to read the controller events from. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.")]
	public VRTK_ControllerEvents controllerEvents;

	[Tooltip("The headset controller aware script to use to see if the headset is looking at the controller. If this is blank then it will attempt to get a controller events script from the same or parent GameObject.")]
	public VRTK_HeadsetControllerAware headsetControllerAware;

	[Tooltip("If this is checked then the tooltips will be hidden when the headset is not looking at the controller.")]
	public bool hideWhenNotInView = true;

	[Tooltip("The total number of initialisation attempts to make when waiting for the button transforms to initialise.")]
	public int retryInitMaxTries = 10;

	[Tooltip("The amount of seconds to wait before re-attempting to initialise the controller tooltips if the button transforms have not been initialised yet.")]
	public float retryInitCounter = 0.1f;

	protected TooltipButtons[] availableButtons;

	protected VRTK_ObjectTooltip[] buttonTooltips;

	protected bool[] tooltipStates;

	protected int retryInitCurrentTries;

	public virtual void ResetTooltip()
	{
		InitialiseTips();
	}

	public virtual void UpdateText(TooltipButtons element, string newText)
	{
		switch (element)
		{
		case TooltipButtons.ButtonOneTooltip:
			buttonOneText = newText;
			break;
		case TooltipButtons.ButtonTwoTooltip:
			buttonTwoText = newText;
			break;
		case TooltipButtons.StartMenuTooltip:
			startMenuText = newText;
			break;
		case TooltipButtons.GripTooltip:
			gripText = newText;
			break;
		case TooltipButtons.TouchpadTooltip:
			touchpadText = newText;
			break;
		case TooltipButtons.TriggerTooltip:
			triggerText = newText;
			break;
		}
		ResetTooltip();
	}

	public virtual void ToggleTips(bool state, TooltipButtons element = TooltipButtons.None)
	{
		if (element == TooltipButtons.None)
		{
			for (int i = 1; i < buttonTooltips.Length; i++)
			{
				if (buttonTooltips[i].displayText.Length > 0)
				{
					buttonTooltips[i].gameObject.SetActive(state);
				}
			}
		}
		else if (buttonTooltips[(int)element].displayText.Length > 0)
		{
			buttonTooltips[(int)element].gameObject.SetActive(state);
		}
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
		InitButtonsArray();
	}

	protected virtual void OnEnable()
	{
		controllerEvents = ((!(controllerEvents != null)) ? GetComponentInParent<VRTK_ControllerEvents>() : controllerEvents);
		InitButtonsArray();
		InitListeners();
		ResetTooltip();
	}

	protected virtual void OnDisable()
	{
		if (controllerEvents != null)
		{
			controllerEvents.ControllerEnabled -= DoControllerEnabled;
			controllerEvents.ControllerVisible -= DoControllerVisible;
			controllerEvents.ControllerHidden -= DoControllerInvisible;
		}
		if (headsetControllerAware != null)
		{
			headsetControllerAware.ControllerGlanceEnter -= DoGlanceEnterController;
			headsetControllerAware.ControllerGlanceExit -= DoGlanceExitController;
		}
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void InitButtonsArray()
	{
		availableButtons = new TooltipButtons[7]
		{
			TooltipButtons.None,
			TooltipButtons.TriggerTooltip,
			TooltipButtons.GripTooltip,
			TooltipButtons.TouchpadTooltip,
			TooltipButtons.ButtonOneTooltip,
			TooltipButtons.ButtonTwoTooltip,
			TooltipButtons.StartMenuTooltip
		};
		buttonTooltips = new VRTK_ObjectTooltip[availableButtons.Length];
		tooltipStates = new bool[availableButtons.Length];
		for (int i = 1; i < availableButtons.Length; i++)
		{
			buttonTooltips[i] = transform.Find(availableButtons[i].ToString()).GetComponent<VRTK_ObjectTooltip>();
		}
		retryInitCurrentTries = retryInitMaxTries;
	}

	protected virtual void InitListeners()
	{
		if (controllerEvents != null)
		{
			controllerEvents.ControllerEnabled += DoControllerEnabled;
			controllerEvents.ControllerVisible += DoControllerVisible;
			controllerEvents.ControllerHidden += DoControllerInvisible;
		}
		headsetControllerAware = ((!(headsetControllerAware != null)) ? Object.FindObjectOfType<VRTK_HeadsetControllerAware>() : headsetControllerAware);
		if (headsetControllerAware != null)
		{
			headsetControllerAware.ControllerGlanceEnter += DoGlanceEnterController;
			headsetControllerAware.ControllerGlanceExit += DoGlanceExitController;
			ToggleTips(state: false);
		}
	}

	protected virtual void DoControllerEnabled(object sender, ControllerInteractionEventArgs e)
	{
		if (controllerEvents != null)
		{
			GameObject actualController = VRTK_DeviceFinder.GetActualController(controllerEvents.gameObject);
			if (actualController != null && actualController.activeInHierarchy)
			{
				ResetTooltip();
			}
		}
	}

	protected virtual void DoControllerVisible(object sender, ControllerInteractionEventArgs e)
	{
		for (int i = 0; i < availableButtons.Length; i++)
		{
			ToggleTips(tooltipStates[i], availableButtons[i]);
		}
	}

	protected virtual void DoControllerInvisible(object sender, ControllerInteractionEventArgs e)
	{
		for (int i = 1; i < buttonTooltips.Length; i++)
		{
			tooltipStates[i] = buttonTooltips[i].gameObject.activeSelf;
		}
		ToggleTips(state: false);
	}

	protected virtual void DoGlanceEnterController(object sender, HeadsetControllerAwareEventArgs e)
	{
		if (controllerEvents != null && hideWhenNotInView)
		{
			VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject);
			if (controllerReference == e.controllerReference)
			{
				ToggleTips(state: true);
			}
		}
	}

	protected virtual void DoGlanceExitController(object sender, HeadsetControllerAwareEventArgs e)
	{
		if (controllerEvents != null && hideWhenNotInView)
		{
			VRTK_ControllerReference controllerReference = VRTK_ControllerReference.GetControllerReference(controllerEvents.gameObject);
			if (controllerReference == e.controllerReference)
			{
				ToggleTips(state: false);
			}
		}
	}

	protected virtual void InitialiseTips()
	{
		bool flag = false;
		VRTK_ObjectTooltip[] componentsInChildren = GetComponentsInChildren<VRTK_ObjectTooltip>(includeInactive: true);
		foreach (VRTK_ObjectTooltip vRTK_ObjectTooltip in componentsInChildren)
		{
			string empty = string.Empty;
			Transform transform = null;
			switch (vRTK_ObjectTooltip.name.Replace("Tooltip", string.Empty).ToLower())
			{
			case "trigger":
				empty = triggerText;
				transform = GetTransform(trigger, SDK_BaseController.ControllerElements.Trigger);
				break;
			case "grip":
				empty = gripText;
				transform = GetTransform(grip, SDK_BaseController.ControllerElements.GripLeft);
				break;
			case "touchpad":
				empty = touchpadText;
				transform = GetTransform(touchpad, SDK_BaseController.ControllerElements.Touchpad);
				break;
			case "buttonone":
				empty = buttonOneText;
				transform = GetTransform(buttonOne, SDK_BaseController.ControllerElements.ButtonOne);
				break;
			case "buttontwo":
				empty = buttonTwoText;
				transform = GetTransform(buttonTwo, SDK_BaseController.ControllerElements.ButtonTwo);
				break;
			case "startmenu":
				empty = startMenuText;
				transform = GetTransform(startMenu, SDK_BaseController.ControllerElements.StartMenu);
				break;
			}
			flag = transform != null;
			vRTK_ObjectTooltip.displayText = empty;
			vRTK_ObjectTooltip.drawLineTo = transform;
			vRTK_ObjectTooltip.containerColor = tipBackgroundColor;
			vRTK_ObjectTooltip.fontColor = tipTextColor;
			vRTK_ObjectTooltip.lineColor = tipLineColor;
			vRTK_ObjectTooltip.ResetTooltip();
			if (transform == null || empty.Trim().Length == 0)
			{
				vRTK_ObjectTooltip.gameObject.SetActive(value: false);
			}
		}
		if (!flag && retryInitCurrentTries > 0)
		{
			retryInitCurrentTries--;
			Invoke("ResetTooltip", retryInitCounter);
		}
		else if (!hideWhenNotInView)
		{
			ToggleTips(state: true);
		}
	}

	protected virtual Transform GetTransform(Transform setTransform, SDK_BaseController.ControllerElements findElement)
	{
		Transform result = null;
		if (setTransform != null)
		{
			result = setTransform;
		}
		else if (controllerEvents != null)
		{
			GameObject modelAliasController = VRTK_DeviceFinder.GetModelAliasController(controllerEvents.gameObject);
			if (modelAliasController != null && modelAliasController.activeInHierarchy)
			{
				SDK_BaseController.ControllerHand controllerHand = VRTK_DeviceFinder.GetControllerHand(controllerEvents.gameObject);
				string controllerElementPath = VRTK_SDK_Bridge.GetControllerElementPath(findElement, controllerHand, fullPath: true);
				result = ((controllerElementPath == null) ? null : modelAliasController.transform.Find(controllerElementPath));
			}
		}
		return result;
	}
}
