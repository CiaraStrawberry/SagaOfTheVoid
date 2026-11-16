using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class LeftControl_Onehanddrag : MonoBehaviour
{
	public Resizescript resizing;

	public RightHand_triggerInstantOrder righthandscript;

	private LineRenderer lineren;

	private GameObject World;

	public bool touchpadDown;

	private bool gripdown;

	private bool triggerbuttondown;

	public bool movenormal;

	public float touchpadamount;

	public Material linemat;

	private GameObject floor;

	public VRTK_ControllerEvents vrtkcontrolevents;

	private onTriggerEnterEnemy ontriggerenter;

	public GameObject MenuObj;

	private CrossLevelVariableHolder crossvar;

	public TutorialTextControlScript tutcontrol;

	private bool insidepulse;

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			if (GameObject.Find("CrossLevelVariables") != null)
			{
				crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			}
			if (crossvar.tutorial && (bool)GameObject.Find("TutorialPanelHolder"))
			{
				tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
			}
			World = GameObject.Find("World");
			floor = GameObject.Find("Floor");
			if ((bool)floor)
			{
				floor.SetActive(value: false);
			}
			ontriggerenter = GetComponent<onTriggerEnterEnemy>();
			lineren = gameObject.GetComponent<LineRenderer>();
			if (lineren == null)
			{
				lineren = gameObject.AddComponent<LineRenderer>();
			}
			lineren.startWidth = 0.01f;
			lineren.endWidth = 0.01f;
			lineren.enabled = false;
			lineren.material = linemat;
		}
	}

	private void Awake()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		righthandscript = GameObject.Find("RightController").GetComponent<RightHand_triggerInstantOrder>();
		MenuObj.SetActive(value: false);
		vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
		vrtkcontrolevents.TouchpadPressed += Touchpaddown;
		vrtkcontrolevents.TouchpadReleased += Touchpadup;
		vrtkcontrolevents.TriggerPressed += Trigger;
		vrtkcontrolevents.TriggerReleased += TriggerUp;
		vrtkcontrolevents.GripPressed += gripdownin;
		vrtkcontrolevents.GripReleased += gripupin;
		vrtkcontrolevents.ButtonTwoPressed += Menu;
		vrtkcontrolevents.ButtonOnePressed += turnbutton;
	}

	private void turnbutton(object sender, ControllerInteractionEventArgs e)
	{
		resizing.rotateleft();
	}

	private void Menu(object sender, ControllerInteractionEventArgs e)
	{
		if (!(this != null) || !(gameObject != null) || SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6)
		{
			return;
		}
		righthandscript.setlinerenpos();
		if (crossvar.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.leftMenuDown);
		}
		if (!righthandscript.MenuObj.GetActive())
		{
			if (!MenuObj.GetActive())
			{
				MenuObj.SetActive(value: true);
				MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
			}
			else
			{
				MenuObj.SetActive(value: false);
				righthandscript.lineren.enabled = false;
			}
		}
		else
		{
			righthandscript.MenuObj.SetActive(value: false);
			MenuObj.SetActive(value: true);
			MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
		}
	}

	private void Update()
	{
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			touchpadamount = vrtkcontrolevents.GetTouchpadAxis().y;
			if ((touchpadamount < -0.1f || touchpadamount > 0.1f) && touchpadDown)
			{
				resizing.change(transform.position, scalealterout());
			}
		}
		if (triggerbuttondown)
		{
			resizing.translate(transform.position);
		}
	}

	private void Touchpaddown(object sender, ControllerInteractionEventArgs e)
	{
		touchpadDown = true;
		if (crossvar.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTouchpadPressed);
		}
	}

	private void Touchpadup(object sender, ControllerInteractionEventArgs e)
	{
		touchpadDown = false;
	}

	private void Trigger(object sender, ControllerInteractionEventArgs e)
	{
		if ((bool)crossvar && (bool)tutcontrol && crossvar.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTriggerDown);
		}
		resizing.starttranslate(transform.position);
		triggerbuttondown = true;
	}

	private void TriggerUp(object sender, ControllerInteractionEventArgs e)
	{
		triggerbuttondown = false;
	}

	private void gripdownin(object sender, ControllerInteractionEventArgs e)
	{
		gripdown = true;
		if (resizing.transform.parent.name == "VRTK_SDK" && crossvar != null)
		{
			if (crossvar.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.leftGripDown);
			}
			if (resizing.DragTarget == null)
			{
				resizing.DragTarget = ontriggerenter.closest;
			}
			else if (resizing.DragTarget != null)
			{
				resizing.DragTarget = null;
			}
		}
		else
		{
			resizing.SwitchToWasp();
		}
	}

	private void gripupin(object sender, ControllerInteractionEventArgs e)
	{
		gripdown = false;
	}

	public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
	{
		return x * Vector3.Normalize(B - A) + A;
	}

	public float scalealterout()
	{
		if (touchpadamount < -0.1f || touchpadamount > 0.1f)
		{
			float prevnumber = resizing.prevnumber;
			return 1f + touchpadamount / 20f;
		}
		return 1f;
	}
}
