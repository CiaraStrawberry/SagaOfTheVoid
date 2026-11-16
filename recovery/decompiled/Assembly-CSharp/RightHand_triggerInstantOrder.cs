using System.Collections.Generic;
using Forge3D;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;
using VRTK;

public class RightHand_triggerInstantOrder : TrueSyncBehaviour
{
	public bool ordernow;

	public LineRenderer lineren;

	public List<GameObject> selectedships = new List<GameObject>();

	public Material linemat;

	private GameObject ConeBig;

	private GameObject ConeSmall;

	public bool touchpadDown;

	private int selectioncount;

	public bool triggerdown;

	public GameObject resizeobj;

	private TestCaseController testcontrol;

	private Resizescript resizing;

	public GameObject photonviewobj;

	private PhotonView photonview;

	public _Ship.eShipColor team;

	private GameObject WorldBase;

	public Camera cameramain;

	public RelayController inputrelay;

	public GameObject pulsewave;

	public GameObject tutorial;

	public VRTK_ControllerEvents vrtkcontrolevents;

	private onTriggerEnterEnemy ontriggerenter;

	public float touchpadamount;

	public GameObject MenuObj;

	private Vector3 startscale;

	private Vector3 startpos;

	public LeftControl_Onehanddrag lefthandscriptl;

	public GameObject oculuscam;

	public GameObject Vivecam;

	public GameObject pulsewavered;

	private UnitMovementcommandcontroller unitcon;

	private MainMenuControllerController maincon;

	private TutorialTextControlScript tutcontrol;

	private CrossLevelVariableHolder crosslevelvarhol;

	private int revertcountdown;

	public Material startmat;

	public Material attackordermat;

	public GameObject Pulserenderwaypoint;

	private int[] agentsPerSide = new int[20];

	private void Awake()
	{
		maincon = GetComponent<MainMenuControllerController>();
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
		lefthandscriptl = GameObject.Find("LeftController").GetComponent<LeftControl_Onehanddrag>();
		WorldBase = GameObject.Find("World").transform.Find("Objects").gameObject;
		startscale = resizeobj.transform.localScale;
		startpos = resizeobj.transform.position;
		resizing = resizeobj.GetComponent<Resizescript>();
		ConeBig = transform.Find("ConeBig").gameObject;
		ConeBig.SetActive(value: false);
		ConeSmall = transform.Find("ConeSmall").gameObject;
		ConeSmall.SetActive(value: false);
		MenuObj.SetActive(value: false);
		vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
		vrtkcontrolevents.TouchpadPressed += Touchpaddown;
		vrtkcontrolevents.TouchpadReleased += Touchpadup;
		vrtkcontrolevents.TriggerPressed += Trigger;
		vrtkcontrolevents.TriggerReleased += TriggerUp;
		vrtkcontrolevents.ButtonTwoPressed += Menu;
		vrtkcontrolevents.GripPressed += grip;
		vrtkcontrolevents.ButtonOnePressed += turnbutton;
		if (GetComponent<LineRenderer>() == null)
		{
			lineren = gameObject.AddComponent<LineRenderer>();
		}
		else
		{
			lineren = GetComponent<LineRenderer>();
		}
		lineren.material = linemat;
		lineren.SetPosition(0, new Vector3(0f, 0f, 0f));
		lineren.SetPosition(1, new Vector3(0f, 0f, 0f));
	}

	private void resetallpos()
	{
		if (resizeobj.gameObject != null)
		{
			resizeobj.transform.localScale = startscale;
			resizeobj.transform.position = startpos;
			resizeobj.transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private void turnbutton(object sender, ControllerInteractionEventArgs e)
	{
		resizing.rotateright();
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		if (!(this != null) || !(base.gameObject != null))
		{
			return;
		}
		selectedships.Clear();
		if (oculuscam != null && oculuscam.gameObject != null && oculuscam.GetActive())
		{
			cameramain = oculuscam.GetComponent<Camera>();
		}
		if (Vivecam != null && Vivecam.gameObject != null && Vivecam.GetActive())
		{
			cameramain = Vivecam.GetComponent<Camera>();
		}
		if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1)
		{
			resetallpos();
		}
		else
		{
			GameObject.Find("VRTK_SDK").transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		if (this != null && base.gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			GameObject gameObject = GameObject.Find("CrossLevelVariables");
			if ((bool)gameObject)
			{
				crosslevelvarhol = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			}
			if (crosslevelvarhol.tutorial && (bool)GameObject.Find("TutorialPanelHolder"))
			{
				tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
			}
			unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			photonview = GameObject.Find("JoinMultiplayer").GetComponent<PhotonView>();
			selectAll();
			lineren.enabled = false;
			vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
			team = GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().team;
			ontriggerenter = GetComponent<onTriggerEnterEnemy>();
			tutorial = GameObject.Find("TutorialPanelHolder");
			if ((bool)gameObject && !gameObject.GetComponent<CrossLevelVariableHolder>().tutorial)
			{
				tutorial.SetActive(value: false);
				tutorial = null;
			}
			else
			{
				tutorial.SetActive(value: true);
			}
		}
	}

	public override void OnSyncedStart()
	{
		inputrelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
		owner = TrueSyncManager.LocalPlayer;
		ownerIndex = TrueSyncManager.LocalPlayer.Id;
	}

	private void Update()
	{
		if (!(this != null) || !(gameObject != null) || SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6)
		{
			return;
		}
		revertcountdown--;
		if (revertcountdown == 0)
		{
			lineren.material = startmat;
		}
		if (vrtkcontrolevents == null)
		{
			vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
		}
		touchpadamount = vrtkcontrolevents.GetTouchpadAxis().y;
		if (resizing.transform.localScale.x != 1f)
		{
			lineren.startWidth = 0.002f * resizing.transform.lossyScale.x;
			lineren.endWidth = 0.002f * resizing.transform.lossyScale.x;
		}
		resizing.speed = lefthandscriptl.touchpadamount * 5f + 0.5f;
		if (triggerdown)
		{
			resizing.StickInput(transform.parent.localRotation);
		}
		if ((touchpadamount < -0.05f || touchpadamount > 0.05f) && !touchpadDown)
		{
			if (resizing.WaspPos != null && !resizing.WaspPos.GetActive())
			{
				lineren.enabled = true;
			}
		}
		else if (maincon == null || maincon.hitobj == null)
		{
			lineren.enabled = false;
		}
		if (touchpadamount > -0.05f && touchpadamount < 0.05f && ConeSmall.GetActive())
		{
			ConeSmall.SetActive(value: false);
		}
		if (lineren.enabled)
		{
			setlinerenpos();
		}
		if (ConeSmall.GetActive())
		{
			lineren.enabled = false;
		}
	}

	public void setlinerenpos()
	{
		lineren.SetPosition(0, transform.position);
		if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1000f))
		{
			if (crosslevelvarhol.tutorial && hitInfo.transform.gameObject.GetComponent<_Ship>() != null && hitInfo.transform.gameObject.transform.parent != null && !selectedships.Contains(hitInfo.transform.gameObject) && hitInfo.transform.gameObject.GetComponent<_Ship>().ShipColor == team)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTouchpadtouch);
			}
			addtolist(hitInfo.transform.gameObject);
			lineren.SetPosition(1, hitInfo.point);
		}
		else
		{
			lineren.SetPosition(1, transform.position + transform.forward * 1000f);
		}
	}

	private void Touchpaddown(object sender, ControllerInteractionEventArgs e)
	{
		touchpadDown = true;
	}

	private void Touchpadup(object sender, ControllerInteractionEventArgs e)
	{
		touchpadDown = false;
	}

	private void Trigger(object sender, ControllerInteractionEventArgs e)
	{
		resizing.startclick(transform.parent.localRotation);
		if ((MenuObj.GetActive() || lefthandscriptl.MenuObj.GetActive()) && Physics.Raycast(transform.position, transform.forward, out var hitInfo, 10000f) && hitInfo.collider.gameObject.name == "LeaveMatch")
		{
			if (PhotonNetwork.isMasterClient)
			{
				GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().phopview.RPC("HostLeft", PhotonTargets.All);
			}
			else
			{
				PhotonNetwork.LeaveRoom();
				PhotonNetwork.LoadLevel(0);
			}
		}
		if (resizing.WaspPos == null || !resizing.WaspPos.GetActive())
		{
			if (!touchpadDown && selectedships.Count > 0 && !lineren.enabled)
			{
				Startmove(waypoints: false);
			}
			else if (touchpadDown)
			{
				Startmove(waypoints: true);
			}
			if (lineren.enabled && !touchpadDown && selectedships.Count > 0)
			{
				triggerwhiletouchpad();
			}
		}
		if (!touchpadDown)
		{
			triggerdown = true;
		}
	}

	private void TriggerUp(object sender, ControllerInteractionEventArgs e)
	{
		triggerdown = false;
	}

	private void grip(object sender, ControllerInteractionEventArgs e)
	{
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6 && !lineren.enabled)
		{
			team = GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().team;
			selectAll();
		}
	}

	private void triggerwhiletouchpad()
	{
		if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1000f) && hitInfo.transform.gameObject.GetComponent<_Ship>() != null && hitInfo.transform.gameObject.transform.parent != null && hitInfo.transform.gameObject.GetComponent<_Ship>().ShipColor != team)
		{
			if (crosslevelvarhol.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTriggerGiveLongAttackOrder);
			}
			inputrelay.ordermove(selectedships, hitInfo.transform.position.ToTSVector(), hitInfo.transform.gameObject, calculate_average_speed(selectedships.ToArray()), waypoints: false);
			givepulsewaveorder(spawnpulsewave(hitInfo.collider.transform.position, pulsewavered), hitInfo.transform.gameObject);
			lineren.material = attackordermat;
			revertcountdown = 30;
		}
	}

	private void Menu(object sender, ControllerInteractionEventArgs e)
	{
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			if (crosslevelvarhol.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightMenu);
			}
			if (!lefthandscriptl.MenuObj.GetActive())
			{
				Debug.Log(MenuObj.GetActive());
				if (!MenuObj.GetActive())
				{
					turnonmenu(ison: true);
					MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
				}
				else
				{
					turnonmenu(ison: false);
				}
			}
			else
			{
				lefthandscriptl.MenuObj.SetActive(value: false);
				if (!MenuObj.GetActive())
				{
					turnonmenu(ison: true);
					MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
				}
			}
		}
		else
		{
			resizeobj.transform.localPosition = new Vector3(0.119999f, 4.86f, 5.71f);
			resizing.countup = 0;
			resizeobj.transform.Find("DragOBJ").transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private void turnonmenu(bool ison)
	{
		setlinerenpos();
		MenuObj.SetActive(ison);
	}

	private void Startmove(bool waypoints)
	{
		if (Physics.Raycast(transform.position, transform.forward, out var hitInfo, 1000f))
		{
			ButtonPressedMessageSender component = hitInfo.transform.gameObject.GetComponent<ButtonPressedMessageSender>();
			if (component == null)
			{
				startmoveactual(waypoints);
			}
		}
		else
		{
			startmoveactual(waypoints);
		}
	}

	public void startmoveactual(bool waypoints)
	{
		if (waypoints && crosslevelvarhol.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTriggerGiveWayPoint);
		}
		if (ontriggerenter.closest == null)
		{
			if (crosslevelvarhol.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTrigger);
			}
			inputrelay.ordermove(selectedships, transform.parent.Find("MovePos").position.ToTSVector(), null, calculate_average_speed(selectedships.ToArray()), waypoints);
			if (!waypoints)
			{
				spawnpulsewave(transform.parent.Find("MovePos").position, pulsewave);
			}
			else
			{
				spawnpulsewave(transform.parent.Find("MovePos").position, Pulserenderwaypoint);
			}
		}
		else
		{
			if (crosslevelvarhol.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightAttackOrder);
			}
			inputrelay.ordermove(selectedships, ontriggerenter.closest.transform.position.ToTSVector(), ontriggerenter.closest, calculate_average_speed(selectedships.ToArray()), waypoints);
			if (!waypoints)
			{
				givepulsewaveorder(spawnpulsewave(ontriggerenter.closest.transform.position, isalliedtarget(ontriggerenter.closest)), ontriggerenter.closest);
			}
			else
			{
				spawnpulsewave(transform.parent.Find("MovePos").position, Pulserenderwaypoint);
			}
		}
	}

	public static FP calculate_average_speed(GameObject[] ships)
	{
		FP fP = 0;
		foreach (GameObject gameObject in ships)
		{
			if (gameObject != null && gameObject.gameObject != null)
			{
				fP += gameObject.GetComponent<_Ship>().MaxSpeed;
			}
		}
		return fP / ships.Length;
	}

	private GameObject spawnpulsewave(Vector3 startpos, GameObject pulsewavetospawn)
	{
		GameObject gameObject = Object.Instantiate(pulsewavetospawn);
		gameObject.transform.position = startpos;
		gameObject.GetComponent<F3DPulsewave>().ScaleSize = new Vector3(0.01f, 0.0005f, 0.01f) * resizeobj.transform.localScale.x;
		return gameObject;
	}

	private GameObject isalliedtarget(GameObject input)
	{
		if (UnitMovementcommandcontroller.getteam(input.GetComponent<_Ship>().ShipColor, unitcon.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(unitcon.team, unitcon.crosslevelholder.Gamemode))
		{
			return pulsewave;
		}
		return pulsewavered;
	}

	private void givepulsewaveorder(GameObject gam, GameObject target)
	{
		gam.GetComponent<F3DPulsewave>().trackobj = target;
	}

	private void selectAll()
	{
		if (WorldBase == null || WorldBase.gameObject == null)
		{
			WorldBase = GameObject.Find("World").transform.Find("Objects").gameObject;
		}
		if (selectedships.Count > 0)
		{
			foreach (GameObject selectedship in selectedships)
			{
				if (selectedship.gameObject != null)
				{
					selectedship.GetComponent<_Ship>().Disable();
				}
			}
			selectedships.Clear();
			if (crosslevelvarhol.tutorial)
			{
				tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightGripdeselectall);
			}
			return;
		}
		if (crosslevelvarhol.tutorial)
		{
			tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightGripSelectall);
		}
		foreach (Transform item in WorldBase.transform)
		{
			if (item.gameObject.name != "Engines" && item.gameObject.name != "Working" && item.gameObject.GetComponent<_Ship>() != null && item.gameObject.GetComponent<_Ship>().ShipColor == team)
			{
				addtolist(item.gameObject);
			}
		}
	}

	private void addtolist(GameObject col)
	{
		col.GetComponent<_Ship>().enablehealthbar();
		if (col.GetComponent<_Ship>() != null && col.transform.parent != null && !selectedships.Contains(col))
		{
			if (col.GetComponent<_Ship>().ShipColor == team)
			{
				selectedships.Add(col);
				col.GetComponent<_Ship>().enable();
				col.GetComponent<_Ship>().disablehealthbar();
			}
			else
			{
				col.GetComponent<_Ship>().enablehealthbar();
			}
		}
	}

	private TSVector TargetPosition(int index, TSVector sphere, int agentsnum)
	{
		if (agentsnum != 0 && agentsPerSide.Length != 0 && index < agentsPerSide.Length)
		{
			int num = 100;
			agentsPerSide[index] = agentsnum / 3 + ((agentsnum % 3 > 0) ? 1 : 0);
			int num2 = agentsnum * 200;
			int num3 = index % 3;
			float num4 = (float)(index / 3) / (float)agentsPerSide[num3];
			num4 = 1f - (num4 - (float)(int)num4);
			float num5 = (float)(num2 / 2) * Mathf.Sqrt(3f);
			if (index == 0)
			{
				return sphere;
			}
			return sphere + new TSVector(num * ((index % 2 != 0) ? 1 : (-1)) * ((index - 1) / 2 + 1), 0, num * ((index - 1) / 2 + 1));
		}
		return sphere;
	}
}
