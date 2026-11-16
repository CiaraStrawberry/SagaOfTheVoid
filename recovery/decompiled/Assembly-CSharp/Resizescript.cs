using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;

public class Resizescript : MonoBehaviour
{
	public Vector3 scale1;

	private SteamVR_TrackedObject trackedObj;

	private int Tempsize;

	private Transform parent1;

	private Vector3 originscale = new Vector3(5f, 5f, 5f);

	public Vector3 size2;

	private Vector3 startposition;

	private Quaternion startrotation;

	public GameObject lefthand;

	public GameObject righthand;

	public float prevnumber;

	public GameObject cam;

	public GameObject centerBridge;

	private GameObject parent;

	public Vector3 lastposscale;

	public GameObject World;

	public GameObject WorldBase;

	public GameObject DragTarget;

	private Vector3 lastpostarget = new Vector3(0f, 0f, 0f);

	public GameObject WaspPos;

	public GameObject startparent;

	public GameObject Joystick;

	private UnitMovementcommandcontroller unitcom;

	private _Ship.eShipColor team;

	public GameObject CapitalScaleBase;

	private CrossLevelVariableHolder crosslevelhol;

	private int amountin;

	private GameObject objects;

	public GameObject camtestobj;

	public VignetteAndChromaticAberration fovlimiter;

	private bool waitforshiptobeready;

	private Quaternion curangle;

	private Vector3 quat;

	private Vector3 last;

	private Quaternion start;

	public float speed;

	public float minworldscale = 0.1f;

	public int maxworldscale = 4000;

	public float minplayerscale = 0.12f;

	private bool zoomedin;

	private Vector3 LastDistancefrom;

	private bool tempscaleset;

	public int countup;

	private Vector3 startlocal;

	private Vector3 startworld;

	private Vector3 lastpos;

	private bool noclue;

	private Vector3 tempscale;

	public Quaternion startquatcontr;

	public string debug;

	private void Awake()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	private GameObject getCapitalShip()
	{
		GameObject result = null;
		objects = GameObject.Find("Objects");
		foreach (Transform item in objects.transform)
		{
			if (item.name == "Super-Capital(Clone)" && item.GetComponent<_Ship>().ShipColor == team)
			{
				result = item.gameObject.transform.Find("bridge1piece").Find("CamCenter").gameObject;
			}
		}
		return result;
	}

	private void Start()
	{
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		countup = 0;
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			World = GameObject.Find("WorldScaleBase");
			WorldBase = GameObject.Find("World");
			startparent = transform.parent.gameObject;
			unitcom = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			team = unitcom.team;
			CapitalScaleBase = getCapitalShip();
			crosslevelhol = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
			crosslevelhol.campaignnextlevelset = false;
			WaspPos = GameObject.Find("Camera Position Socket");
			Joystick = GameObject.Find("steering");
			WaspPos.transform.parent.gameObject.SetActive(value: false);
		}
		countup = 0;
		waitforshiptobeready = false;
	}

	private void Update()
	{
		countup++;
		if (this != null && gameObject != null && SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
		{
			if (!waitforshiptobeready && unitcom.teammembersout(unitcom.team).Count > 0)
			{
				waitforshiptobeready = true;
				World.transform.localScale = new Vector3(200f, 200f, 200f);
				transform.position = unitcom.teammembersout(unitcom.team)[0].transform.position;
			}
			if (amountin < 305)
			{
				amountin++;
			}
			if (amountin == 5 && (bool)WaspPos)
			{
				WaspPos.transform.parent.gameObject.SetActive(value: false);
			}
			if (amountin == 300)
			{
				CapitalScaleBase = getCapitalShip();
			}
			lastpos = transform.position - cam.transform.position;
			if ((bool)centerBridge && transform.parent != null && Vector3.Distance(transform.position, centerBridge.transform.position) > 5f)
			{
				transform.position = centerBridge.transform.position;
			}
			if ((bool)DragTarget)
			{
				transform.position -= DragTarget.GetComponent<CustomPathfinding>().thisposdif;
			}
			if ((bool)WaspPos && WaspPos.transform.parent.gameObject.GetActive())
			{
				WaspPos.transform.parent.localPosition = WaspPos.transform.parent.localPosition + WaspPos.transform.parent.forward * speed;
			}
		}
		zoomedin = false;
		if ((!WaspPos || !WaspPos.GetActive()) && (!CapitalScaleBase || !(transform.parent == CapitalScaleBase.transform)) && transform.localScale.x < 0.05f)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
	}

	public void scaleup()
	{
		cam.transform.localPosition = new Vector3(0f, 0f, 0f);
		transform.localScale += new Vector3(20f, 20f, 20f);
	}

	public void scaledown()
	{
		cam.transform.localPosition = new Vector3(0f, 0f, 0f);
		transform.localScale -= new Vector3(20f, 20f, 20f);
	}

	public void positiondown()
	{
		transform.localPosition -= new Vector3(0f, 300f, 0f);
	}

	public void positionup()
	{
		transform.localPosition += new Vector3(0f, 300f, 0f);
	}

	public void change(Vector3 midpoint, float scalealter)
	{
		if (((bool)WaspPos && WaspPos.GetActive()) || ((bool)CapitalScaleBase && transform.parent == CapitalScaleBase.transform))
		{
			scalealter = 1f;
		}
		else if (transform.localScale.x < 0.05f)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
		}
		if (scalealter != 1f && !zoomedin)
		{
			zoomedin = true;
		}
		if (!noclue && (scalealter < 1f || ((bool)DragTarget && World.transform.localScale.x < 40f) || DragTarget == null || ((bool)DragTarget && DragTarget.GetComponent<_Ship>().HullType == eHullType.Light)))
		{
			if ((World.transform.localScale.x < (float)maxworldscale || scalealter < 1f) && (World.transform.localScale.x > minworldscale || scalealter > 1f) && World.transform.localScale.x < (float)maxworldscale)
			{
				World.transform.localScale = new Vector3(World.transform.localScale.x * scalealter, World.transform.localScale.y * scalealter, World.transform.localScale.z * scalealter);
			}
			if (World.transform.localScale.x > (float)maxworldscale && (transform.localScale.x > minplayerscale || scalealter < 1f) && (transform.localScale.x < 1f || scalealter > 1f))
			{
				transform.localScale = new Vector3(transform.localScale.x * (1f / scalealter), transform.localScale.y * (1f / scalealter), transform.localScale.z * (1f / scalealter));
				if (transform.localScale.x > 1f && scalealter < 1f)
				{
					World.transform.localScale = new Vector3(maxworldscale - 10, maxworldscale - 10, maxworldscale - 10);
				}
			}
			if (World.transform.localScale.x < (float)maxworldscale)
			{
				Vector3 position = WorldBase.transform.position;
				World.transform.position = midpoint;
				WorldBase.transform.position = position;
			}
			else
			{
				Vector3 position2 = cam.transform.position;
				transform.position = midpoint;
				cam.transform.position = position2;
			}
		}
		else
		{
			disable();
		}
		if (righthand.transform.localPosition != new Vector3(0f, 0f, 0f) && lefthand.transform.localPosition != new Vector3(0f, 0f, 0f))
		{
			quat = (righthand.transform.localPosition - lefthand.transform.localPosition).normalized;
			if (quat != new Vector3(0f, 0f, 0f))
			{
				start = Quaternion.LookRotation(quat);
			}
			noclue = false;
			last = quat;
		}
		if (tempscaleset)
		{
			transform.localScale = new Vector3(1f, 1f, 1f);
			tempscaleset = false;
		}
	}

	public void move(float moveamount)
	{
		Vector3 vector = righthand.transform.localPosition - lastposscale;
		transform.position += vector * transform.lossyScale.x / 10f;
	}

	public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
	{
		return x * Vector3.Normalize(B - A) + A;
	}

	public void translate(Vector3 moveaboutpoint)
	{
		if (countup > 50)
		{
			Vector3 position = cam.transform.position;
			startlocal = transform.position;
			if (!noclue)
			{
				transform.position = moveaboutpoint;
				cam.transform.position = position;
				transform.position = startlocal;
			}
			noclue = false;
		}
		else
		{
			transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 0f);
		}
	}

	private void LateUpdate()
	{
		if (DragTarget != null)
		{
			DragTarget.GetComponent<_Ship>().enablehealthbar();
		}
	}

	public void starttranslate(Vector3 moveaboutpoint)
	{
		DragTarget = null;
		Vector3 position = cam.transform.position;
		transform.position = moveaboutpoint;
		cam.transform.position = position;
		startlocal = transform.position;
		noclue = true;
		lastposscale = righthand.transform.localPosition;
	}

	public void disable()
	{
	}

	public void rotateright()
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 40f, transform.eulerAngles.z);
	}

	public void rotateleft()
	{
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 40f, transform.eulerAngles.z);
	}

	public void SwitchToWasp()
	{
		if (!(this != null) || !(gameObject != null) || SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 5 || SceneManager.GetActiveScene().buildIndex == 6)
		{
			return;
		}
		if ((bool)crosslevelhol && (crosslevelhol.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelhol.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll))
		{
			if ((bool)WaspPos && !WaspPos.GetActive())
			{
				World.transform.localScale = new Vector3(2000f, 2000f, 2000f);
				WaspPos.transform.parent.transform.position = transform.position;
				WaspPos.transform.parent.gameObject.SetActive(value: true);
				WaspPos.transform.parent.rotation = transform.rotation;
				transform.parent = WaspPos.transform;
				cam.transform.localPosition = new Vector3(0f, 0f, 0f);
				iTween.ScaleTo(gameObject, new Vector3(4f, 4f, 4f), 2f);
				transform.localPosition = new Vector3(0f, 0f, 0f);
				transform.localScale = new Vector3(1f, 1f, 1f);
				tempscale = World.transform.localScale;
				if ((bool)transform.GetChild(0).Find("OculusSDK"))
				{
					transform.GetChild(0).Find("OculusSDK").localPosition = new Vector3(0f, 0f, 0f);
				}
				if ((bool)transform.GetChild(0).Find("ViveSDK"))
				{
					transform.GetChild(0).Find("ViveSDK").localPosition = new Vector3(0f, 0f, 0f);
				}
			}
			else if ((bool)WaspPos)
			{
				WaspPos.transform.parent.gameObject.SetActive(value: false);
				transform.parent = startparent.transform;
				transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				transform.localScale = new Vector3(1f, 1f, 1f);
				World.transform.localScale = tempscale;
				WaspPos.transform.parent.rotation = new Quaternion(0f, 0f, 0f, 0f);
			}
		}
		else
		{
			if (crosslevelhol.Gamemode != CrossLevelVariableHolder.gamemode.CapitalShip)
			{
				return;
			}
			if (DragTarget != null)
			{
				DragTarget = null;
			}
			if (CapitalScaleBase == null)
			{
				CapitalScaleBase = getCapitalShip();
			}
			if (CapitalScaleBase != null && transform.parent != CapitalScaleBase.transform)
			{
				World.transform.localScale = new Vector3(6000f, 6000f, 6000f);
				CapitalScaleBase.transform.parent.gameObject.SetActive(value: true);
				transform.parent = CapitalScaleBase.transform;
				transform.localEulerAngles = new Vector3(90f, 0f, 0f);
				cam.transform.localPosition = new Vector3(0f, 0f, 0f);
				transform.localPosition = new Vector3(0f, 0f, 0f);
				transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
				tempscale = World.transform.localScale;
				if ((bool)transform.GetChild(0).Find("OculusSDK"))
				{
					transform.GetChild(0).Find("OculusSDK").localPosition = new Vector3(0f, 0f, 0f);
				}
				if ((bool)transform.GetChild(0).Find("ViveSDK"))
				{
					transform.GetChild(0).Find("ViveSDK").localPosition = new Vector3(0f, 0f, 0f);
				}
			}
			else if (CapitalScaleBase != null)
			{
				if (DragTarget != null)
				{
					DragTarget = null;
				}
				transform.parent = startparent.transform;
				transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
				World.transform.localScale = new Vector3(4f, 4f, 4f);
				transform.position = CapitalScaleBase.transform.parent.position;
				transform.localScale = new Vector3(1f, 1f, 1f);
				tempscaleset = true;
			}
		}
	}

	public void forcereturntonormal()
	{
		if (transform.parent.name != "VRTK_SDK")
		{
			SwitchToWasp();
		}
	}

	public void startclick(Quaternion input)
	{
		startquatcontr = input;
	}

	public void StickInput(Quaternion input)
	{
		if ((bool)WaspPos && WaspPos.transform.parent.gameObject.GetActive())
		{
			Quaternion quaternion = Quaternion.Inverse(startquatcontr) * input;
			quaternion = Quaternion.Slerp(WaspPos.transform.parent.rotation, WaspPos.transform.parent.rotation * quaternion, 0.015f);
			WaspPos.transform.parent.rotation = quaternion;
			Joystick.transform.localRotation = Quaternion.Inverse(startquatcontr) * input;
		}
	}
}
