using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class RightControllerUI : MonoBehaviour
{
	public SceneField SceneTransfer;

	private SteamVR_TrackedObject trackedObj;

	private EVRButtonId gripButton = EVRButtonId.k_EButton_Grip;

	private EVRButtonId triggerButton = EVRButtonId.k_EButton_Axis1;

	private EVRButtonId touchpad = EVRButtonId.k_EButton_Axis0;

	private EVRButtonId MenuButton = EVRButtonId.k_EButton_ApplicationMenu;

	private EVRButtonId Abutton = EVRButtonId.k_EButton_A;

	private LineRenderer lineren;

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
		lineren.enabled = true;
		touchpadDown = true;
	}

	private void Touchpadup()
	{
		lineren.enabled = false;
		touchpadDown = false;
		ConeSmall.SetActive(value: false);
		ConeBig.SetActive(value: false);
		selectioncount = 0;
	}

	private void triggerwhiletouchpad()
	{
		if (selectioncount == 0)
		{
			ConeSmall.SetActive(value: true);
			lineren.enabled = false;
		}
		else if (selectioncount >= 1)
		{
			ConeSmall.SetActive(value: false);
			ConeBig.SetActive(value: true);
		}
		selectioncount++;
	}

	private void Trigger()
	{
		Debug.Log("move");
		triggerdown = true;
	}

	private void TriggerUp()
	{
		triggerdown = false;
	}

	private void grip()
	{
		SceneManager.LoadSceneAsync(SceneTransfer);
	}

	private void Menu()
	{
		Debug.Log("menu");
		if (MenuObj.activeSelf)
		{
			MenuObj.SetActive(value: false);
		}
		else
		{
			MenuObj.SetActive(value: true);
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
