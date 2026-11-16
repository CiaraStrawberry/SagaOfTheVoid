using System.Collections.Generic;
using Apex.Steering;
using UnityEngine;
using Valve.VR;

public class RightHand_TriggerGuardMove : MonoBehaviour
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

	public GameObject MenuObj;

	public Material linemat;

	private GameObject ConeBig;

	private GameObject ConeSmall;

	public bool touchpadDown;

	private int selectioncount;

	public bool triggerdown;

	public bool touchpaddownonce;

	private bool inside;

	private GameObject moveobj;

	private TestCaseController testcontrol;

	private Resizescript resizing;

	public float test;

	private Vector3 startloc;

	private Quaternion startorien;

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
		MenuObj = transform.Find("MoveMenuMenu").gameObject;
		moveobj = MenuObj.transform.Find("ControllerMoveMenu").Find("Effects").Find("Menu")
			.gameObject;
		startloc = MenuObj.transform.localPosition;
		startorien = MenuObj.transform.localRotation;
		MenuObj.SetActive(value: false);
		ConeBig = transform.Find("ConeBig").gameObject;
		ConeBig.SetActive(value: false);
		ConeSmall = transform.Find("ConeSmall").gameObject;
		ConeSmall.SetActive(value: false);
		selectAll();
	}

	private void Update()
	{
		if (Vector3.Distance(transform.position, moveobj.transform.position) < 0.05f)
		{
			inside = true;
		}
		else
		{
			inside = false;
		}
		test = Vector3.Distance(transform.position, moveobj.transform.position);
		if (controller.GetPressDown(touchpad))
		{
			Touchpaddown();
		}
		if (controller.GetPressUp(touchpad))
		{
			Touchpadup();
		}
		if (controller.GetPressDown(MenuButton))
		{
			Menu();
		}
		if (controller.GetPressDown(triggerButton) && !touchpadDown)
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
		Menu();
		triggerdown = true;
	}

	private void TriggerUp()
	{
		menuDisable();
		lineren.enabled = false;
		triggerdown = false;
	}

	private void grip()
	{
	}

	private void Menu()
	{
		Debug.Log("guard");
		MenuObj.SetActive(value: true);
		MenuObj.transform.localPosition = startloc;
		MenuObj.transform.localRotation = startorien;
		MenuObj.transform.SetParent(null);
	}

	public void menuDisable()
	{
		MenuObj.SetActive(value: false);
		MenuObj.transform.SetParent(transform);
		if (inside)
		{
			Startmove(MenuObj.transform.position);
		}
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
