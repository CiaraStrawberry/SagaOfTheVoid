using UnityEngine;
using Valve.VR;

public class LeftHand_DPAD : MonoBehaviour
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

	public bool touchpadDown;

	private bool gripdown;

	private bool triggerbuttondown;

	public bool teleport;

	public bool movenormal;

	public float touchpadamount;

	public Material linemat;

	private GameObject floor;

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
		touchpadamount = controller.GetAxis()[1];
		if (teleport && touchpadDown)
		{
			lineren.SetPosition(0, transform.position);
			if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 10000f))
			{
				lineren.SetPosition(1, hitInfo.point);
			}
			else
			{
				lineren.SetPosition(1, transform.forward * 50f);
			}
			lineren.SetWidth(resizing.gameObject.transform.lossyScale.x / 50f, resizing.transform.lossyScale.x / 50f);
		}
		if (touchpadDown && movenormal)
		{
			resizing.move(touchpadamount);
		}
	}

	private Vector3 movepoint()
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (movenormal)
		{
			return transform.position;
		}
		return result;
	}

	public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
	{
		return x * Vector3.Normalize(B - A) + A;
	}

	private void Touchpaddown()
	{
		if (touchpadamount < 0f)
		{
			movenormal = true;
		}
		else
		{
			teleport = true;
		}
		if (movenormal)
		{
			resizing.starttranslate(movepoint());
		}
		touchpadDown = true;
		if (teleport)
		{
			floor.SetActive(value: true);
			lineren.enabled = true;
		}
	}

	private void Touchpadup()
	{
		touchpadDown = false;
		if (teleport)
		{
			lineren.enabled = false;
			if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 10000f))
			{
				Vector3 position = resizing.gameObject.transform.position;
				position.x = hitInfo.point.x;
				position.z = hitInfo.point.y;
				transform.parent.position = position;
				Debug.Log(hitInfo.transform.gameObject);
			}
			Debug.Log("move");
			floor.SetActive(value: false);
		}
		teleport = false;
		movenormal = false;
	}

	private void Menu()
	{
	}

	private void Trigger()
	{
		triggerbuttondown = true;
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
}
