using System.Collections.Generic;
using Apex.Steering;
using UnityEngine;
using Valve.VR;

public class RightHand_TriggerDownTwice : MonoBehaviour
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

	private float timesince;

	private bool aloud;

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
		selectAll();
	}

	private void Update()
	{
		timesince += Time.deltaTime;
		if (timesince <= 1f)
		{
			aloud = true;
		}
		else
		{
			aloud = false;
		}
		if (controller.GetPressDown(touchpad))
		{
			Touchpaddown();
		}
		if (controller.GetPressUp(touchpad))
		{
			Touchpadup();
		}
		if (controller.GetPressDown(triggerButton) && !touchpadDown)
		{
			Trigger();
		}
		if (touchpadDown && controller.GetPressDown(triggerButton))
		{
			triggerwhiletouchpad();
		}
		touchpaddownonce = controller.GetPressDown(touchpad);
	}

	private void Touchpaddown()
	{
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
		if (aloud)
		{
			Startmove(transform.position);
		}
		touchpadDown = true;
		timesince = 0f;
	}

	private void Startmove(Vector3 movepoint)
	{
		foreach (GameObject selectedship in selectedships)
		{
			IMovable component = selectedship.GetComponent<IMovable>();
			component.MoveTo(movepoint, append: true);
			Debug.Log("move");
		}
	}

	private void selectAll()
	{
		GameObject gameObject = GameObject.Find("World").transform.Find("Objects").Find("Working").gameObject;
		foreach (Transform item in gameObject.transform)
		{
			if (item.name == "Ship 1")
			{
				addtolist(item.gameObject);
			}
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
