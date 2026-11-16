using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;

public class LeftControl : MonoBehaviour
{
	public SceneField scene;

	private SteamVR_TrackedObject trackedObj;

	private EVRButtonId gripButton = EVRButtonId.k_EButton_Grip;

	private EVRButtonId triggerButton = EVRButtonId.k_EButton_Axis1;

	private EVRButtonId touchpad = EVRButtonId.k_EButton_Axis0;

	private EVRButtonId MenuButton = EVRButtonId.k_EButton_ApplicationMenu;

	private EVRButtonId Abutton = EVRButtonId.k_EButton_A;

	private Resizescript resizing;

	private TestCaseController testcontrol;

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
		testcontrol = transform.parent.parent.GetComponent<TestCaseController>();
		lineren = gameObject.AddComponent<LineRenderer>();
		lineren.startWidth = 0.01f;
		lineren.endWidth = 0.01f;
		lineren.enabled = false;
		lineren.material = linemat;
		righthandscript = transform.parent.Find("Controller (right)").GetComponent<RightControllerUI>();
	}

	public Vector3 getposition()
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (testcontrol.oneControllerTriggerScale || testcontrol.TwoControllerTriggerScale)
		{
			result = Vector3.Lerp(righthandscript.gameObject.transform.position, transform.position, 0.5f);
		}
		if (testcontrol.linearScaleWithTouchpad || testcontrol.controllerpositionscale)
		{
			result = transform.position;
		}
		return result;
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
		if (triggerbuttondown && testcontrol.oneControllerTriggerScale)
		{
			resizing.change(getposition(), scalealterout());
		}
		if (testcontrol.TwoControllerTriggerScale && righthandscript.triggerdown)
		{
			resizing.change(getposition(), scalealterout());
		}
		if (testcontrol.controllerpositionscale && triggerbuttondown)
		{
			resizing.change(getposition(), scalealterout());
		}
		if (testcontrol.linearScaleWithTouchpad)
		{
			resizing.change(getposition(), scalealterout());
		}
		touchpadamount = controller.GetAxis()[1];
		if (touchpadDown && testcontrol.TwoHandDrag && righthandscript.touchpadDown)
		{
			resizing.translate(movepoint());
		}
		if (touchpadDown && testcontrol.OneHandDrag)
		{
			resizing.translate(movepoint());
		}
		if ((testcontrol.raycastjump || teleport) && touchpadDown)
		{
			lineren.SetPosition(0, transform.position);
			if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 10000f))
			{
				lineren.SetPosition(1, hitInfo.point);
			}
			else
			{
				lineren.SetPosition(1, transform.forward * 500f);
			}
			lineren.SetWidth(resizing.gameObject.transform.lossyScale.x / 50f, resizing.transform.lossyScale.x / 50f);
		}
		if (touchpadDown && (testcontrol.accelerateviamovement || movenormal))
		{
			resizing.move(touchpadamount);
		}
		if (touchpadDown && testcontrol.TwoHandDrag && righthandscript.touchpaddownonce)
		{
			resizing.starttranslate(movepoint());
		}
	}

	private Vector3 movepoint()
	{
		Vector3 result = new Vector3(0f, 0f, 0f);
		if (testcontrol.OneHandDrag)
		{
			result = transform.position;
		}
		if (testcontrol.TwoHandDrag)
		{
			result = LerpByDistance(transform.position, righthandscript.transform.position, 0.5f);
		}
		return result;
	}

	public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
	{
		return x * Vector3.Normalize(B - A) + A;
	}

	private void Touchpaddown()
	{
		if (testcontrol.dpadnavigation)
		{
			if (touchpadamount < 0f)
			{
				movenormal = true;
			}
			else
			{
				teleport = true;
			}
		}
		if (testcontrol.incrimenatalScaleWithToucpad)
		{
			if (controller.GetAxis()[1] > 0f)
			{
				resizing.scaleup();
			}
			else
			{
				resizing.scaledown();
			}
		}
		if (testcontrol.OneHandDrag || testcontrol.TwoHandDrag || testcontrol.accelerateviamovement || movenormal)
		{
			resizing.starttranslate(movepoint());
		}
		touchpadDown = true;
		if (testcontrol.raycastjump || teleport)
		{
			floor.SetActive(value: true);
			lineren.enabled = true;
		}
	}

	private void Touchpadup()
	{
		touchpadDown = false;
		if (testcontrol.raycastjump || teleport)
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
			Debug.DrawLine(transform.position, transform.forward * 10000f);
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
		if (testcontrol.oneControllerTriggerScale)
		{
			resizing.disable();
		}
		if (testcontrol.TwoControllerTriggerScale && righthandscript.triggerdown)
		{
			resizing.disable();
		}
		if (testcontrol.controllerpositionscale)
		{
			resizing.lastposscale = righthandscript.gameObject.transform.localPosition;
		}
	}

	private void Triggerup()
	{
		triggerbuttondown = false;
		resizing.disable();
	}

	private void gripdownin()
	{
		gripdown = true;
		SceneManager.LoadSceneAsync(scene);
	}

	private void gripupin()
	{
		gripdown = false;
	}

	public float scalealterout()
	{
		float result = resizing.prevnumber;
		if (testcontrol.oneControllerTriggerScale || testcontrol.TwoControllerTriggerScale)
		{
			float num = Vector3.Distance(righthandscript.transform.localPosition, transform.localPosition);
			float num2 = num - resizing.prevnumber;
			result = resizing.prevnumber / num;
			resizing.prevnumber = num;
		}
		if (testcontrol.linearScaleWithTouchpad)
		{
			result = 1f + touchpadamount / 20f;
		}
		if (testcontrol.controllerpositionscale)
		{
			Vector3 localPosition = righthandscript.transform.localPosition;
			float num3 = 0f - (resizing.lastposscale.y - localPosition.y) / 10f;
			result = 1f + num3;
		}
		return result;
	}
}
