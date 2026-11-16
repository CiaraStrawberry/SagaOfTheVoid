using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Righthand_touchpadthentrigger : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObj;

	private EVRButtonId gripButton = EVRButtonId.k_EButton_Grip;

	private EVRButtonId triggerButton = EVRButtonId.k_EButton_Axis1;

	private EVRButtonId touchpad = EVRButtonId.k_EButton_Axis0;

	private EVRButtonId MenuButton = EVRButtonId.k_EButton_ApplicationMenu;

	private EVRButtonId Abutton = EVRButtonId.k_EButton_A;

	private LineRenderer lineren;

	[HideInInspector]
	public List<GameObject> selectedships = new List<GameObject>();

	private GameObject MenuObj;

	public Material linemat;

	private GameObject ConeBig;

	private GameObject ConeSmall;

	public bool touchpadDown;

	private int selectioncount;

	public bool triggerdown;

	public bool touchpaddownonce;

	private TestCaseController testcontrol;

	private Resizescript resizing;

	private SteamVR_Controller.Device controller => SteamVR_Controller.Input((int)trackedObj.index);

	private void Start()
	{
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		lineren = gameObject.AddComponent<LineRenderer>();
		resizing = transform.parent.parent.GetComponent<Resizescript>();
		testcontrol = transform.parent.parent.GetComponent<TestCaseController>();
		lineren.startWidth = 0.01f;
		lineren.endWidth = 0.01f;
		lineren.enabled = false;
		lineren.material = linemat;
		MenuObj = transform.Find("ControllerMenuMinimap").gameObject;
		MenuObj.SetActive(value: false);
		ConeBig = transform.Find("ConeBig").gameObject;
		ConeBig.SetActive(value: false);
		ConeSmall = transform.Find("ConeSmall").gameObject;
		ConeSmall.SetActive(value: false);
	}

	private void Update()
	{
		if (lineren.enabled)
		{
			lineren.SetPosition(0, transform.position);
			lineren.SetPosition(1, transform.position + transform.forward * 1000f);
		}
		if (controller.GetPressDown(touchpad))
		{
			Touchpaddown();
		}
		if (controller.GetPressUp(touchpad))
		{
			Touchpadup();
		}
		if (controller.GetPressDown(triggerButton))
		{
			Trigger();
		}
		if (controller.GetPressUp(triggerButton))
		{
			TriggerUp();
		}
		if (controller.GetPressDown(gripButton))
		{
			grip();
		}
		if (touchpadDown && controller.GetPressDown(triggerButton))
		{
			triggerwhiletouchpad();
		}
		touchpaddownonce = controller.GetPressDown(touchpad);
	}

	private void Touchpaddown()
	{
		touchpadDown = true;
	}

	private void Touchpadup()
	{
		touchpadDown = false;
	}

	private void triggerwhiletouchpad()
	{
	}

	private void Trigger()
	{
		if (touchpadDown)
		{
			selectAll();
			Debug.Log("move");
		}
		triggerdown = true;
	}

	private void TriggerUp()
	{
		triggerdown = false;
	}

	private void grip()
	{
	}

	private void selectAll()
	{
		GameObject gameObject = GameObject.Find("World").transform.Find("Objects").Find("Working").gameObject;
		foreach (Transform item in gameObject.transform)
		{
			addtolist(item.gameObject);
		}
	}

	private void addtolist(GameObject col)
	{
		if (col.transform.parent != null && !selectedships.Contains(col) && col.transform.parent.name == "Working")
		{
			selectedships.Add(col);
			Debug.Log(col);
			col.GetComponent<Selection_Controller>().enable();
		}
	}
}
