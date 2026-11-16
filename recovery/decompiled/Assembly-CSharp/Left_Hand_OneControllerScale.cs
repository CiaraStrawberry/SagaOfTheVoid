using UnityEngine;
using Valve.VR;

public class Left_Hand_OneControllerScale : MonoBehaviour
{
	private SteamVR_TrackedObject trackedObj;

	private EVRButtonId gripButton = EVRButtonId.k_EButton_Grip;

	private EVRButtonId triggerButton = EVRButtonId.k_EButton_Axis1;

	private EVRButtonId touchpad = EVRButtonId.k_EButton_Axis0;

	private EVRButtonId MenuButton = EVRButtonId.k_EButton_ApplicationMenu;

	private EVRButtonId Abutton = EVRButtonId.k_EButton_A;

	private Resizescript resizing;

	private RightControllerUI righthandscript;

	private LineRenderer lineren;

	private CrossLevelVariableHolder crossvar;

	public bool touchpadDown;

	private bool gripdown;

	private bool triggerbuttondown;

	public bool teleport;

	public bool movenormal;

	public float touchpadamount;

	public Material linemat;

	private GameObject floor;

	private TutorialTextControlScript tutcontrol;

	private SteamVR_Controller.Device controller => SteamVR_Controller.Input((int)trackedObj.index);

	private void Start()
	{
		floor = GameObject.Find("Floor");
		floor.SetActive(value: false);
		resizing = transform.parent.parent.GetComponent<Resizescript>();
		trackedObj = GetComponent<SteamVR_TrackedObject>();
		lineren = gameObject.AddComponent<LineRenderer>();
		lineren.startWidth = 0.01f;
		lineren.endWidth = 0.01f;
		lineren.enabled = false;
		lineren.material = linemat;
		righthandscript = transform.parent.Find("Controller (right)").GetComponent<RightControllerUI>();
	}

	public Vector3 getposition()
	{
		Vector3 vector = new Vector3(0f, 0f, 0f);
		return Vector3.Lerp(righthandscript.gameObject.transform.position, transform.position, 0.5f);
	}

	private void Update()
	{
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
		if (controller.GetPressDown(triggerButton))
		{
			Trigger();
		}
		if (controller.GetPressUp(triggerButton))
		{
			Triggerup();
		}
		if (controller.GetPressDown(gripButton))
		{
			gripdownin();
		}
		if (controller.GetPressUp(gripButton))
		{
			gripupin();
		}
		floor.transform.position = transform.parent.position;
		if (triggerbuttondown)
		{
			resizing.change(getposition(), scalealterout());
		}
		touchpadamount = controller.GetAxis()[1];
	}

	public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
	{
		return x * Vector3.Normalize(B - A) + A;
	}

	private void Touchpaddown()
	{
		touchpadDown = true;
	}

	private void Touchpadup()
	{
		touchpadDown = false;
		teleport = false;
		movenormal = false;
	}

	private void Menu()
	{
	}

	private void Trigger()
	{
		triggerbuttondown = true;
		resizing.disable();
	}

	private void Triggerup()
	{
		triggerbuttondown = false;
		resizing.disable();
	}

	private void gripdownin()
	{
		gripdown = true;
	}

	private void gripupin()
	{
		gripdown = false;
	}

	public float scalealterout()
	{
		float prevnumber = resizing.prevnumber;
		float num = Vector3.Distance(righthandscript.transform.localPosition, transform.localPosition);
		float num2 = num - resizing.prevnumber;
		prevnumber = resizing.prevnumber / num;
		resizing.prevnumber = num;
		return prevnumber;
	}
}
