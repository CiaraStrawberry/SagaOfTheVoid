using System.Collections.Generic;
using EasyEditor;
using Forge3D;
using TrueSync;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using VRTK.Highlighters;

public class _Ship : TrueSyncBehaviour
{
	public enum eShipColor
	{
		Green,
		Blue,
		White,
		Yellow,
		Grey,
		Red
	}

	public class storedorder
	{
		public TSVector WorldTarget;

		public FP averagespeed;

		public TSTransform shiptargettstransform;

		public storedorder(TSVector WorldTargetin, FP averagespeedin, GameObject Targetin = null, TSTransform transforminput = null)
		{
			WorldTarget = WorldTargetin;
			averagespeed = averagespeedin;
			if (Targetin != null)
			{
				shiptargettstransform = Targetin.GetComponent<TSTransform>();
			}
			else
			{
				shiptargettstransform = transforminput;
			}
		}
	}

	[Inspector(group = "StartupandDebugstuff")]
	[SerializeField]
	private GameObject HealthBar;

	public GameObject HealthBarinit;

	public Image healthbarinterface;

	public Transform startmovepos;

	private float _ActualMaxSpeed;

	[SerializeField]
	private TSVector MoveTarget;

	private PhotonTransformView transformview;

	public bool isbot;

	public int spawnnum;

	public bool iscloacked;

	public bool isTracker;

	public List<Vector3> targetmesh = new List<Vector3>();

	public List<Vector3> startmeshpos = new List<Vector3>();

	private F3DPoolManager poolmanager;

	public TSTransform shiptarget;

	public List<_Weapon> weaponry = new List<_Weapon>();

	public GameObject objectsholder;

	public GameObject Healthbaronly;

	public Image Healthbaronlyinterface;

	public Sprite RedHealthBar;

	public Sprite GreenHealthBar;

	public Sprite YellowSprite;

	public Sprite WhiteSprite;

	public Sprite GreySprite;

	public Sprite BlueSprite;

	private Vector3 healthbarstartscale;

	public VRTK_OutlineObjectCopyHighlighter outline;

	public int startshield;

	private TSVector shiptargetposlast;

	private TSTransform shiptargettstransform;

	[Inspector(displayHeader = true, group = "Engine Settings", groupDescription = "Settings for the engine", order = 1)]
	[SerializeField]
	private GameObject _ShipObject;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private GameObject _ShieldObject;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private GameObject _WeaponsObject;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private GameObject _EquipmentObject;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private GameObject _EnginesObject;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private Canvas _UIObject;

	[Inspector(displayHeader = true, group = "Hull Settings", groupDescription = "Basic settings for the hull, these don't need to change", order = 2)]
	[SerializeField]
	private eHullType _HullType;

	[Inspector(group = "Hull Settings")]
	[SerializeField]
	private eShipClass _ShipClass;

	[Inspector(group = "Hull Settings")]
	[SerializeField]
	private int _PointsToDeploy;

	[Inspector(displayHeader = true, group = "Basic Stats", groupDescription = "Stats regarding hit points and maneuverability", order = 3)]
	[SerializeField]
	private int _Armor;

	[Inspector(group = "Basic Stats")]
	[SerializeField]
	private int _Shield;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The maximum speed, regardless of movement form.")]
	[SerializeField]
	private FP _MaxSpeed;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The maximum angular acceleration rate of the unit (rads / s^2), i.e. how fast can the unit reach its desired turn speed.")]
	[SerializeField]
	private int _MaxAngularAcceleration;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The maximum turn speed of the unit (rads / s), i.e. how fast can the unit turn.")]
	[SerializeField]
	private FP _MaxAngularSpeed;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The maximum acceleration rate of the unit (m / s^2), i.e. how fast can the unit reach its desired speed.")]
	[SerializeField]
	private int _MaxAcceleration;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The maximum deceleration rate of the unit (m / s^2), i.e. how fast can the unit slow down.")]
	[SerializeField]
	private int _MaxDeceleration;

	[Inspector(group = "Basic Stats")]
	[Tooltip("The minimum speed ever, regardless of movement form. Any speed below this will mean a stop.")]
	[SerializeField]
	private int _MinimumSpeed;

	[Inspector(group = "Basic Stats")]
	[SerializeField]
	private _Weapon[] _Weapons;

	[Inspector(displayHeader = true, group = "Model Settings", groupDescription = "Model settings", order = 4)]
	[SerializeField]
	private eShipColor _ShipColor = eShipColor.Blue;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Black;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Blue;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Green;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Grey;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Red;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _White;

	[Inspector(group = "Model Settings")]
	[SerializeField]
	private Material _Yellow;

	internal string ShipMesh = string.Empty;

	internal string ShipMaterialRoot = string.Empty;

	internal string ShieldMaterial = string.Empty;

	public List<GameObject> meshes = new List<GameObject>();

	public List<MeshRenderCloakController> meshesscripts = new List<MeshRenderCloakController>();

	public Material shieldmaterial;

	private Vector3 correctPlayerPos;

	private Quaternion correctPlayerRot;

	private float startarmour;

	public CustomPathfinding fac;

	public GameObject World2;

	public GameObject Cannon_Projectile;

	public GameObject Missile_Projectile;

	public GameObject Lazer_Shot;

	public GameObject MiniGun_Projectile;

	public GameObject shipDestruction;

	public Forcefield forcefield;

	private eShipColor lasthitby;

	public bool updatedtest;

	public bool isTrackingimproved;

	public float timepassed1;

	private bool shown;

	public TSTransform WeaponTarget;

	public int storedordercount;

	private GameObject LeftHandMenu;

	private UnitMovementcommandcontroller unittargetcontrol;

	public GameObject engines;

	private float startcombined;

	public FP averagespeed;

	public List<storedorder> StoredOrders = new List<storedorder>();

	public int runnext;

	public int timepassedforrun;

	public int a;

	public TSTransform[] Reptemp;

	public bool started;

	private Vector3 forcedhealthscale;

	private GameObject World;

	private int i;

	public TSTransform hitdebug;

	public GameObject ShipObject
	{
		get
		{
			return _ShipObject;
		}
		set
		{
			_ShipObject = value;
		}
	}

	public GameObject ShieldObject
	{
		get
		{
			return _ShieldObject;
		}
		set
		{
			_ShieldObject = value;
		}
	}

	public GameObject WeaponsObject
	{
		get
		{
			return _WeaponsObject;
		}
		set
		{
			_WeaponsObject = value;
		}
	}

	public GameObject EquipmentObject
	{
		get
		{
			return _EquipmentObject;
		}
		set
		{
			_EquipmentObject = value;
		}
	}

	public GameObject EnginesObject
	{
		get
		{
			return _EnginesObject;
		}
		set
		{
			_EnginesObject = value;
		}
	}

	public Canvas UIObject
	{
		get
		{
			return _UIObject;
		}
		set
		{
			_UIObject = value;
		}
	}

	public eHullType HullType
	{
		get
		{
			return _HullType;
		}
		set
		{
			_HullType = value;
			AdjustStats();
		}
	}

	public eShipClass ShipClass
	{
		get
		{
			return _ShipClass;
		}
		set
		{
			_ShipClass = value;
			AdjustStats();
		}
	}

	public int PointsToDeploy => _PointsToDeploy;

	public int Armor
	{
		get
		{
			return _Armor;
		}
		set
		{
			_Armor = value;
		}
	}

	public int Shield
	{
		get
		{
			return _Shield;
		}
		set
		{
			_Shield = value;
		}
	}

	public FP MaxSpeed
	{
		get
		{
			return _MaxSpeed;
		}
		set
		{
			_MaxSpeed = value;
		}
	}

	public int MaxAngularAcceleration
	{
		get
		{
			return _MaxAngularAcceleration;
		}
		set
		{
			_MaxAngularAcceleration = value;
		}
	}

	public FP MaxAngularSpeed
	{
		get
		{
			return _MaxAngularSpeed;
		}
		set
		{
			_MaxAngularSpeed = value;
		}
	}

	public int MaxAcceleration
	{
		get
		{
			return _MaxAcceleration;
		}
		set
		{
			_MaxAcceleration = value;
		}
	}

	public int MaxDeceleration
	{
		get
		{
			return _MaxDeceleration;
		}
		set
		{
			_MaxDeceleration = value;
		}
	}

	public int MinimumSpeed
	{
		get
		{
			return _MinimumSpeed;
		}
		set
		{
			_MinimumSpeed = value;
		}
	}

	public _Weapon[] Weapons
	{
		get
		{
			return _Weapons;
		}
		set
		{
			_Weapons = value;
		}
	}

	public virtual eShipColor ShipColor
	{
		get
		{
			return _ShipColor;
		}
		set
		{
			_ShipColor = value;
			foreach (Transform item in base.transform)
			{
				if (!(item.name == "Ship"))
				{
					continue;
				}
				MeshRenderer component = item.GetComponent<MeshRenderer>();
				if (ColorBlack != null)
				{
					switch (_ShipColor)
					{
					case eShipColor.Blue:
						component.material = ColorBlue;
						break;
					case eShipColor.Green:
						component.material = ColorGreen;
						break;
					case eShipColor.Grey:
						component.material = ColorGrey;
						break;
					case eShipColor.Red:
						component.material = ColorRed;
						break;
					case eShipColor.White:
						component.material = ColorWhite;
						break;
					case eShipColor.Yellow:
						component.material = ColorYellow;
						break;
					}
				}
			}
		}
	}

	public Material ColorBlack
	{
		get
		{
			return _Black;
		}
		set
		{
			_Black = value;
		}
	}

	public Material ColorBlue
	{
		get
		{
			return _Blue;
		}
		set
		{
			_Blue = value;
		}
	}

	public Material ColorGreen
	{
		get
		{
			return _Green;
		}
		set
		{
			_Green = value;
		}
	}

	public Material ColorGrey
	{
		get
		{
			return _Grey;
		}
		set
		{
			_Grey = value;
		}
	}

	public Material ColorRed
	{
		get
		{
			return _Red;
		}
		set
		{
			_Red = value;
		}
	}

	public Material ColorWhite
	{
		get
		{
			return _White;
		}
		set
		{
			_White = value;
		}
	}

	public Material ColorYellow
	{
		get
		{
			return _Yellow;
		}
		set
		{
			_Yellow = value;
		}
	}

	public virtual void Reset()
	{
		if (GetComponent<CapsuleCollider>() == null)
		{
			base.gameObject.AddComponent<CapsuleCollider>();
		}
		if (GetComponent<PhotonView>() == null)
		{
			base.gameObject.AddComponent<PhotonView>();
		}
		if (GetComponent<PhotonTransformView>() == null)
		{
			base.gameObject.AddComponent<PhotonTransformView>();
		}
		if (transform.Find("Ship") == null)
		{
			GameObject gameObject = new GameObject("Ship");
			gameObject.layer = LayerMask.NameToLayer("UI");
			gameObject.AddComponent<MeshFilter>();
			gameObject.AddComponent<MeshRenderer>();
			gameObject.AddComponent<MeshCollider>();
			SetChildTransformTo(gameObject, transform);
			ShipObject = gameObject;
		}
		if (transform.Find("Weapons") == null)
		{
			GameObject gameObject2 = new GameObject("Weapons");
			SetChildTransformTo(gameObject2, transform);
			WeaponsObject = gameObject2;
		}
		if (transform.Find("Engines") == null)
		{
			GameObject gameObject3 = new GameObject("Engines");
			SetChildTransformTo(gameObject3, transform);
			EnginesObject = gameObject3;
		}
		if (transform.Find("Equipment") == null)
		{
			GameObject gameObject4 = new GameObject("Equipment");
			SetChildTransformTo(gameObject4, transform);
			EquipmentObject = gameObject4;
		}
		if (transform.Find("HealthBar") == null)
		{
			HealthBar = Object.Instantiate(HealthBarinit, transform.position, new Quaternion(0f, 0f, 0f, 0f));
			HealthBar.transform.parent = transform;
			HealthBar.transform.localPosition = new Vector3(0f, 0f, 0f);
		}
		else
		{
			HealthBar = transform.Find("HealthBar").gameObject;
		}
		if (transform.Find("UI") == null)
		{
			GameObject gameObject5 = new GameObject("UI");
			gameObject5.layer = LayerMask.NameToLayer("UI");
			gameObject5.AddComponent<Canvas>();
			gameObject5.AddComponent<CanvasScaler>();
			SetChildTransformTo(gameObject5, transform);
			RectTransform component = gameObject5.GetComponent<RectTransform>();
			component.localPosition = Vector3.zero;
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.Euler(90f, 0f, 0f);
			component.localScale = new Vector3(0.3f, 0.3f, 0.3f);
			GameObject gameObject6 = new GameObject("Bar");
			Image image = gameObject6.AddComponent<Image>();
			image.color = new Color32(163, 231, byte.MaxValue, 68);
			image.raycastTarget = false;
			gameObject6.transform.SetParent(gameObject5.transform);
			component = gameObject6.GetComponent<RectTransform>();
			component.localPosition = new Vector3(0f, 0f, -20.75f);
			component.sizeDelta = new Vector2(1f, 25f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.Euler(-90f, 0f, 0f);
			component.localScale = Vector3.one;
			GameObject gameObject7 = new GameObject("Bar");
			image = gameObject7.AddComponent<Image>();
			image.color = new Color32(163, 231, byte.MaxValue, 68);
			image.raycastTarget = false;
			gameObject7.transform.SetParent(gameObject6.transform);
			component = gameObject7.GetComponent<RectTransform>();
			component.localPosition = new Vector3(0f, 0f, 0f);
			component.sizeDelta = new Vector2(1f, 25f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.Euler(0f, 90f, 0f);
			component.localScale = Vector3.one;
			GameObject gameObject8 = new GameObject("Banner");
			gameObject8.AddComponent<RectTransform>();
			gameObject8.AddComponent<BillBoardMe>();
			gameObject8.transform.SetParent(gameObject5.transform);
			component = gameObject8.GetComponent<RectTransform>();
			component.localPosition = new Vector3(0f, 0f, -35.13f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.Euler(-60f, 0f, 0f);
			component.localScale = Vector3.one;
			GameObject gameObject9 = new GameObject("Hull");
			gameObject9.transform.SetParent(gameObject8.transform);
			image = gameObject9.AddComponent<Image>();
			image.type = Image.Type.Simple;
			Shadow shadow = gameObject9.AddComponent<Shadow>();
			shadow.effectDistance = new Vector2(0.1f, -0.1f);
			shadow.useGraphicAlpha = true;
			component = gameObject9.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(5f, 5f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
			component.localPosition = new Vector3(-2.5f, 0f, 0.12f);
			GameObject gameObject10 = new GameObject("Class");
			gameObject10.transform.SetParent(gameObject9.transform);
			image = gameObject10.AddComponent<Image>();
			image.type = Image.Type.Simple;
			shadow = gameObject10.AddComponent<Shadow>();
			shadow.effectDistance = new Vector2(0.1f, -0.1f);
			shadow.useGraphicAlpha = true;
			component = gameObject10.GetComponent<RectTransform>();
			component.sizeDelta = new Vector2(5f, 5f);
			component.pivot = new Vector2(0.5f, 0.5f);
			component.localRotation = Quaternion.identity;
			component.localScale = Vector3.one;
			component.localPosition = new Vector3(5.05f, 0f, 0f);
			UIObject = gameObject5.GetComponent<Canvas>();
			AdjustStats();
		}
	}

	public void takeDamage(int damage, eShipColor oriteam, eHullType inputhull)
	{
		if (!unittargetcontrol.crosslevelholder.tutorial)
		{
			lasthitby = oriteam;
			if (Shield > 0)
			{
				Shield -= damage;
			}
			else if (HullType == eHullType.Corvette)
			{
				if ((double)Armor > (double)startarmour * 0.6)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.5f);
				}
				else if ((double)Armor > (double)startarmour * 0.3)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.2f);
				}
				else
				{
					Armor = -5;
				}
			}
			else if (HullType == eHullType.Light)
			{
				if ((double)Armor > (double)startarmour * 0.8)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.7f);
				}
				else if ((double)Armor > (double)startarmour * 0.6)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.5f);
				}
				else if ((double)Armor > (double)startarmour * 0.4)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.3f);
				}
				else if ((double)Armor > (double)startarmour * 0.2)
				{
					Armor = Mathf.RoundToInt(startarmour * 0.1f);
				}
				else
				{
					Armor = -5;
				}
			}
			else
			{
				Armor -= damage;
			}
			if (Shield < 0)
			{
				Shield = -1;
			}
		}
		if (inputhull == eHullType.Light && HullType == eHullType.Light)
		{
			fac.turnspeed -= 0.5;
		}
	}

	public bool isshieldfull()
	{
		if (Shield >= startshield)
		{
			return true;
		}
		return false;
	}

	public void addShield(int damage)
	{
		if (HullType != eHullType.Light)
		{
			Shield += damage;
		}
		if (Shield > startshield)
		{
			Shield = startshield;
		}
	}

	public void enablehealthbar()
	{
		if ((bool)HealthBar && !Healthbaronly.GetActive())
		{
			Healthbaronly.SetActive(value: true);
		}
	}

	public void disablehealthbar()
	{
		if (Healthbaronly == null)
		{
			Healthbaronly = transform.Find("HealthBarOnly").gameObject;
		}
		if (Healthbaronly != null && Healthbaronly.GetActive() && LeftHandMenu != null && !LeftHandMenu.GetActive())
		{
			Healthbaronly.SetActive(value: false);
		}
	}

	private void Awake()
	{
		if ((bool)GameObject.Find("LeftController"))
		{
			LeftHandMenu = GameObject.Find("LeftController").GetComponent<LeftControl_Onehanddrag>().MenuObj;
		}
		if ((bool)base.transform.Find("WarpJumpOut_green_gamma") && (bool)base.transform.Find("WarpJumpOut_green_gamma").Find("WarpJumpFX").Find("WarpJumpDistortion"))
		{
			Object.Destroy(base.transform.Find("WarpJumpOut_green_gamma").Find("WarpJumpFX").Find("WarpJumpDistortion")
				.gameObject);
			}
			foreach (Transform item in base.transform)
			{
				if (item.GetComponent<_Weapon>() != null)
				{
					item.gameObject.SetActive(value: false);
				}
			}
		}

		private GameObject spawnshot(GameObject objecttospawn)
		{
			GameObject result = null;
			Transform transform = F3DPoolManager.Pools["GeneratedPool"].Spawn(objecttospawn.transform);
			if ((bool)transform)
			{
				result = transform.gameObject;
			}
			return result;
		}

		public GameObject FireWeaponNetwork(Vector3 target, bool hit, Vector3 source, GameObject id, int damage, Vector3 sourcelocal, _Weapon.guntype Type, GameObject sourceobj, _Weapon.guntype actualtype)
		{
			GameObject gameObject = null;
			if (Type == _Weapon.guntype.Cannon || Type == _Weapon.guntype.CapitalMainGun)
			{
				gameObject = spawnshot(Cannon_Projectile);
			}
			if (Type == _Weapon.guntype.Missile)
			{
				gameObject = spawnshot(Missile_Projectile);
			}
			if (Type == _Weapon.guntype.MiniGun)
			{
				gameObject = spawnshot(MiniGun_Projectile);
			}
			if (Type == _Weapon.guntype.ShieldGenerator || Type == _Weapon.guntype.Lazer)
			{
				gameObject = spawnshot(Lazer_Shot);
			}
			if (Type == _Weapon.guntype.BomberGun)
			{
				gameObject = spawnshot(MiniGun_Projectile);
			}
			if (gameObject == null)
			{
				return null;
			}
			gameObject.transform.position = source;
			gameObject.transform.parent = transform.parent.Find("Working");
			if (Type == _Weapon.guntype.Cannon)
			{
				gameObject.transform.localScale = new Vector3(20f, 20f, 20f);
			}
			if (Type == _Weapon.guntype.Cannon && HullType == eHullType.Capital)
			{
				gameObject.transform.localScale = new Vector3(10f, 10f, 10f);
			}
			if (Type == _Weapon.guntype.MiniGun)
			{
				gameObject.transform.localScale = new Vector3(3f, 3f, 15f);
			}
			if (Type == _Weapon.guntype.BomberGun)
			{
				gameObject.transform.localScale = new Vector3(6f, 6f, 6f);
			}
			if (Type == _Weapon.guntype.Missile)
			{
				gameObject.transform.localScale = new Vector3(15f, 15f, 15f);
			}
			if (Type == _Weapon.guntype.CapitalMainGun)
			{
				gameObject.transform.localScale = new Vector3(40f, 40f, 80f);
			}
			if (hit && (Type == _Weapon.guntype.Cannon || Type == _Weapon.guntype.MiniGun))
			{
				gameObject.transform.LookAt(target);
			}
			if (Type == _Weapon.guntype.Missile)
			{
				gameObject.transform.forward = sourceobj.transform.up;
			}
			if (Type == _Weapon.guntype.Missile || Type == _Weapon.guntype.BomberGun)
			{
				gameObject.GetComponent<ProjectileMovementCom>().isMissile();
			}
			if (!hit)
			{
				gameObject.transform.LookAt(target);
				gameObject.transform.eulerAngles = gameObject.transform.eulerAngles + new Vector3(Random.Range(-5, 5), Random.Range(-5, 5), Random.Range(-5, 5));
			}
			if (id != null && Type != _Weapon.guntype.ShieldGenerator)
			{
				ProjectileMovementCom component = gameObject.GetComponent<ProjectileMovementCom>();
				component.targetgam = id;
				component.origin = sourcelocal;
				component.damage = damage;
				component.ishit = hit;
				component.guntype = actualtype;
			}
			if (Type == _Weapon.guntype.ShieldGenerator)
			{
				_Ship component2 = id.GetComponent<_Ship>();
				Debug.Log(component2.isshieldfull());
				if (!component2.isshieldfull())
				{
					ProjectileMovementCom component3 = gameObject.GetComponent<ProjectileMovementCom>();
					component3.startlineren(sourceobj, id, shieldmaterial, 1f, Type, sourceobj);
					component2.addShield(damage);
					component3.origin = sourcelocal;
					component3.damage = damage;
					component3.ishit = hit;
					component3.guntype = actualtype;
				}
				else
				{
					WeaponTarget = null;
					F3DPoolManager.Pools["GeneratedPool"].Despawn(gameObject.transform);
				}
			}
			if (Type == _Weapon.guntype.Lazer)
			{
				ProjectileMovementCom component4 = gameObject.GetComponent<ProjectileMovementCom>();
				_Ship component5 = id.GetComponent<_Ship>();
				component4.startlineren(sourceobj, id, shieldmaterial, 3f, Type, sourceobj);
				component5.takeDamage(damage, ShipColor, HullType);
			}
			return gameObject;
		}

		public void assigncloackfog(bool cloak)
		{
			foreach (Transform item in base.transform)
			{
				if (item.name == "Ship")
				{
					MeshRenderCloakController component = item.GetComponent<MeshRenderCloakController>();
					if (cloak)
					{
						component.cloack();
					}
					else
					{
						component.uncloack();
					}
					component.distancecloacked = cloak;
				}
			}
		}

		private void cloakrep()
		{
			if (iscloacked)
			{
				float num = 10000f;
				if (HullType == eHullType.Capital)
				{
					num = 1500f;
				}
				if (HullType == eHullType.Destroyer)
				{
					num = 1000f;
				}
				if (HullType == eHullType.Cruiser)
				{
					num = 800f;
				}
				if (HullType == eHullType.Frigate)
				{
					num = 1500f;
				}
				List<GameObject> list = unittargetcontrol.teammembersout(ShipColor);
				foreach (GameObject item in list)
				{
					if (!item || !(item.gameObject != null))
					{
						continue;
					}
					foreach (Transform item2 in item.transform)
					{
						if (item2.name == "Ship")
						{
							item2.GetComponent<MeshRenderCloakController>().cloaked = false;
						}
					}
				}
				foreach (GameObject item3 in list)
				{
					int num2 = 0;
					if (!item3 || !(item3.gameObject != null) || !(Vector3.Distance(transform.localPosition, item3.transform.localPosition) < num) || num2 >= 3)
					{
						continue;
					}
					num2++;
					foreach (Transform item4 in item3.transform)
					{
						if (item4.name == "Ship" && item4.GetComponent<MeshRenderCloakController>().asigncloacked())
						{
							Transform transform3 = F3DPoolManager.Pools["GeneratedPool"].Spawn(Lazer_Shot.transform);
							if ((bool)transform3)
							{
								GameObject gameObject = transform3.gameObject;
								ProjectileMovementCom component = gameObject.GetComponent<ProjectileMovementCom>();
								gameObject.transform.parent = transform.parent.Find("Working");
								component.startlineren(base.gameObject, item3, shieldmaterial, 1f, _Weapon.guntype.ShieldGenerator, base.gameObject);
								component.ishit = true;
							}
						}
					}
				}
			}
			if ((HullType == eHullType.Light || HullType == eHullType.Corvette) && fac.started)
			{
				for (int i = 0; i < targetmesh.Count; i++)
				{
					targetmesh[i] = new Vector3((0.5f - Random.value) * 100f, (0.5f - Random.value) * 100f, (0.5f - Random.value) * 100f);
				}
			}
		}

		private void asignordertopathfinding(storedorder input)
		{
			if (input.shiptargettstransform != null && input.shiptargettstransform.gameObject != null)
			{
				TSVector targetin = input.shiptargettstransform.transform.position.ToTSVector();
				fac.assigntarget(targetin, input.shiptargettstransform.position);
				shiptargetposlast = input.shiptargettstransform.position;
			}
			else
			{
				TSVector targetin2 = World2.transform.Find("Objects").TransformPoint(input.WorldTarget.ToVector()).ToTSVector();
				fac.assigntarget(targetin2, input.WorldTarget);
			}
		}

		public void Rep()
		{
			if (WeaponTarget != null && unittargetcontrol != null && weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator && UnitMovementcommandcontroller.getteam(WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode))
			{
				WeaponTarget = null;
			}
			if (WeaponTarget == null)
			{
				WeaponTarget = FindTarget();
			}
		}

		private float getthicknesstoset()
		{
			float num = World.transform.localScale.x / 1000f;
			if (num < 0.03f)
			{
				num = 0.03f;
			}
			return num;
		}

		public void Update()
		{
			storedordercount = StoredOrders.Count;
			if ((bool)HealthBar && healthbarstartscale != new Vector3(0f, 0f, 0f) && (HealthBar.GetActive() || Healthbaronly.GetActive()))
			{
				if (World.transform.localScale.x > 60f)
				{
					HealthBar.transform.localScale = healthbarstartscale;
				}
				if (World.transform.localScale.x < 40f && World.transform.localScale.x > 12f)
				{
					HealthBar.transform.localScale = 1f / (World.transform.localScale.x / 40f) * forcedhealthscale * 2f;
				}
				else if (World.transform.localScale.x < 12f)
				{
					HealthBar.transform.localScale = 8f * forcedhealthscale * 0.7f;
				}
				if (World.transform.localScale.x > 60f)
				{
					Healthbaronly.transform.localScale = healthbarstartscale;
				}
				if (World.transform.localScale.x < 40f && World.transform.localScale.x > 12f)
				{
					Healthbaronly.transform.localScale = 1f / (World.transform.localScale.x / 40f) * forcedhealthscale * 2f;
				}
				else if (World.transform.localScale.x < 12f)
				{
					Healthbaronly.transform.localScale = 8f * forcedhealthscale * 0.7f;
				}
			}
			timepassedforrun++;
			if (runnext != 0 && timepassedforrun >= runnext)
			{
				Rep();
				runnext = 0;
			}
			timepassed1 += Time.deltaTime;
			if ((bool)World && (bool)outline && outline.active)
			{
				outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
				for (int i = 0; i < outline.listofcopiedmodels.Count; i++)
				{
					if (outline.listoforiginalmodels[i] != null && outline.listoforiginalmodels[i].gameObject != null)
					{
						outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
						continue;
					}
					GameObject item = outline.listoforiginalmodels[i];
					GameObject gameObject = outline.listofcopiedmodels[i];
					outline.listoforiginalmodels.Remove(item);
					outline.listofcopiedmodels.Remove(gameObject);
					Object.Destroy(gameObject);
				}
			}
			if (timepassed1 > 3f && !shown)
			{
				shown = true;
				foreach (GameObject mesh in meshes)
				{
					if (!mesh.GetActive())
					{
						mesh.SetActive(value: true);
					}
				}
				foreach (Transform item2 in base.transform)
				{
					if (item2.GetComponent<_Weapon>() != null && !item2.gameObject.GetActive() && meshes[0].GetActive())
					{
						item2.gameObject.SetActive(value: true);
					}
					if (item2.name == "Engines" && !item2.gameObject.GetActive())
					{
						item2.gameObject.SetActive(value: true);
					}
				}
				if ((bool)transform.Find("WarpJumpOut_green_gamma"))
				{
					Object.Destroy(transform.Find("WarpJumpOut_green_gamma").gameObject);
				}
				if ((bool)transform.Find("PAR_SEBImpactA_1a_0000 (1)"))
				{
					Object.Destroy(transform.Find("PAR_SEBImpactA_1a_0000 (1)").gameObject);
				}
				HealthBar.SetActive(value: true);
				Healthbaronly.SetActive(value: true);
				transform.Find("HealthBar").Find("GameObject").Find("barPanel")
					.Find("Healthbar")
					.GetComponent<Image>()
					.sprite = getsprite();
				transform.Find("HealthBarOnly").Find("GameObject").Find("barPanel")
					.Find("Healthbar")
					.GetComponent<Image>()
					.sprite = getsprite();
				HealthBar.SetActive(value: false);
				Healthbaronly.SetActive(value: false);
			}
			else
			{
				foreach (Transform item3 in transform)
				{
					if (item3.name == "Engines" && !item3.gameObject.GetActive())
					{
						item3.gameObject.SetActive(value: false);
					}
				}
			}
			if ((bool)fac)
			{
				updatedtest = true;
				fac.SpecUpdate();
				foreach (_Weapon item4 in weaponry)
				{
					if (timepassed1 > 3f)
					{
						item4.UpdateSpec();
					}
				}
				foreach (MeshRenderCloakController meshesscript in meshesscripts)
				{
					if (meshesscript != null)
					{
						meshesscript.specupdate();
					}
				}
				if (World2 == null)
				{
					World2 = GameObject.Find("World");
				}
				if (TrueSyncManager.Time > 0)
				{
					if ((bool)shiptarget && shiptarget.gameObject == null)
					{
						shiptarget = null;
						shiptargettstransform = null;
						MoveTarget = shiptargetposlast;
					}
					if (StoredOrders.Count == 0)
					{
						asignordertopathfinding(new storedorder(MoveTarget, averagespeed, null, shiptargettstransform));
					}
					else
					{
						asignordertopathfinding(StoredOrders[0]);
					}
					if (HealthBar.GetActive())
					{
						healthbarinterface.fillAmount = (float)(Armor + Shield) / startcombined;
					}
					if (Healthbaronlyinterface.gameObject.GetActive())
					{
						Healthbaronlyinterface.fillAmount = (float)(Armor + Shield) / startcombined;
					}
					if (Armor < 0)
					{
						if (!unittargetcontrol.crosslevelholder.campaign && unittargetcontrol.team == lasthitby)
						{
							unittargetcontrol.addmoney(PointsToDeploy / 3);
						}
						foreach (AIController aicontroller in unittargetcontrol.aicontrollers)
						{
							if (aicontroller.team == lasthitby && !aicontroller.ismission)
							{
								aicontroller.addmoney(PointsToDeploy / 3);
							}
						}
						if (HullType != eHullType.Light && HullType != eHullType.Corvette)
						{
							Transform transform3 = F3DPoolManager.Pools["GeneratedPool"].Spawn(shipDestruction.transform);
							if ((bool)transform3)
							{
								GameObject gameObject2 = transform3.gameObject;
								gameObject2.transform.parent = GameObject.Find("Working").transform;
								gameObject2.transform.position = transform.position;
								gameObject2.transform.localScale = new Vector3(10f, 10f, 10f) * hulltypemoderator(HullType);
							}
						}
						Object.Destroy(base.gameObject);
					}
					if ((bool)engines)
					{
						if (fac.currentlymoving)
						{
							if (timepassed1 > 3f)
							{
								setenginesactive(input: true);
							}
						}
						else
						{
							setenginesactive(input: false);
						}
					}
					if (HullType == eHullType.Light || HullType == eHullType.Corvette)
					{
						for (int j = 0; j < meshes.Count; j++)
						{
							if (meshes[j].gameObject != null)
							{
								meshes[j].transform.localPosition = Vector3.Lerp(meshes[j].transform.localPosition, targetmesh[j], Time.deltaTime * 0.01f);
							}
						}
						if (HullType == eHullType.Light)
						{
							if (meshes.Count == 5 && (double)Armor < (double)startarmour * 0.8)
							{
								removefromfighters(meshes[4]);
							}
							if (meshes.Count == 4 && (double)Armor < (double)startarmour * 0.6)
							{
								removefromfighters(meshes[3]);
							}
							if (meshes.Count == 3 && (double)Armor < (double)startarmour * 0.4)
							{
								removefromfighters(meshes[2]);
							}
							if (meshes.Count == 2 && (double)Armor < (double)startarmour * 0.2)
							{
								removefromfighters(meshes[1]);
							}
						}
						if (HullType == eHullType.Corvette)
						{
							if (meshes.Count == 5 && (double)Armor < (double)startarmour * 0.66)
							{
								removefromfighters(meshes[2]);
							}
							if (meshes.Count == 4 && (double)Armor < (double)startarmour * 0.33)
							{
								removefromfighters(meshes[1]);
							}
						}
					}
				}
			}
			disablehealthbar();
			if ((bool)LeftHandMenu && LeftHandMenu.GetActive() && !Healthbaronly.GetActive())
			{
				Healthbaronly.SetActive(value: true);
			}
		}

		private void setenginesactive(bool input)
		{
			if (!input)
			{
				if (engines.GetActive())
				{
					engines.SetActive(value: false);
				}
			}
			else if (!engines.GetActive())
			{
				engines.SetActive(value: true);
			}
		}

		private void removefromfighters(GameObject input)
		{
			meshes.Remove(input);
			Object.Destroy(input);
		}

		public void asignMoveOrder(TSVector WorldTarget, GameObject Target, FP averagespeedin, bool addtoorderlist)
		{
			if (!addtoorderlist)
			{
				asignsinglemoveorder(WorldTarget, Target, averagespeedin);
			}
			else
			{
				StoredOrders.Add(new storedorder(WorldTarget, averagespeedin, Target));
			}
		}

		private void asignsinglemoveorder(TSVector WorldTarget, GameObject Target, FP averagespeedin)
		{
			StoredOrders.Clear();
			MoveTarget = WorldTarget;
			if (Target != null)
			{
				shiptarget = Target.GetComponent<TSTransform>();
			}
			if (Target != null)
			{
				shiptargettstransform = Target.GetComponent<TSTransform>();
			}
			else
			{
				shiptarget = null;
				shiptargettstransform = null;
			}
			averagespeed = averagespeedin;
		}

		public void movetonextstoredorder()
		{
			if (StoredOrders.Count > 0)
			{
				StoredOrders.RemoveAt(0);
			}
		}

		private float hulltypemoderator(eHullType input)
		{
			float result = 1f;
			if (input == eHullType.Capital)
			{
				result = 3f;
			}
			if (input == eHullType.Destroyer)
			{
				result = 2.5f;
			}
			if (input == eHullType.Cruiser)
			{
				result = 2f;
			}
			if (input == eHullType.Frigate)
			{
				result = 1.5f;
			}
			if (input == eHullType.Corvette)
			{
				result = 1f;
			}
			if (input == eHullType.Light)
			{
				result = 0.7f;
			}
			return result;
		}

		public void callallreps()
		{
			List<TSTransform> list = new List<TSTransform>();
			list = unittargetcontrol.targetsout(ShipColor);
			if (list.Count != 0)
			{
				if (weaponry.Count > 0 && weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator)
				{
					Reptemp = list.ToArray();
				}
				else
				{
					Reptemp = unittargetcontrol.alliesoutcustom(ShipColor).ToArray();
				}
			}
			timepassedforrun = 0;
			runnext = transform.GetComponent<PhotonView>().viewID % 1000 / 2;
		}

		public virtual void StartMain()
		{
			started = true;
			int num = 0;
			averagespeed = MaxSpeed * 30;
			foreach (Transform item in base.transform)
			{
				if (item.GetComponent<_Weapon>() != null)
				{
					weaponry.Add(item.GetComponent<_Weapon>());
					item.GetComponent<_Weapon>().StartMain(num);
					num++;
					item.gameObject.SetActive(value: false);
				}
			}
			if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
			{
				InvokeRepeating("cloakrep", TSRandom.RangeFP(0f, 1f).AsFloat(), 3f);
			}
			objectsholder = GameObject.Find("Objects");
			unittargetcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
			startshield = Shield;
			if ((bool)ShieldObject)
			{
				forcefield = ShieldObject.GetComponent<Forcefield>();
			}
			shipDestruction = Resources.Load("Explosion_001") as GameObject;
			startarmour = Armor;
			startcombined = Armor + Shield;
			if ((bool)transform.Find("Engines"))
			{
				engines = transform.Find("Engines").gameObject;
			}
			Healthbaronly = transform.Find("HealthBarOnly").gameObject;
			foreach (Transform item2 in transform)
			{
				if (item2.name == "Ship")
				{
					meshes.Add(item2.gameObject);
					meshesscripts.Add(item2.GetComponent<MeshRenderCloakController>());
					targetmesh.Add(new Vector3(0f, 0f, 0f));
					startmeshpos.Add(item2.localPosition);
				}
				if (item2.name == "Engines")
				{
					item2.gameObject.SetActive(value: false);
				}
			}
			outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
			if ((bool)outline)
			{
				outline.Initialise(Color.green);
			}
			transformview = GetComponent<PhotonTransformView>();
			if (SceneManager.GetActiveScene().name != "MenuMain" && HullType != eHullType.Destroyer)
			{
				transform.localScale = new Vector3(8f, 8f, 8f);
			}
			else if (SceneManager.GetActiveScene().name != "MenuMain")
			{
				transform.localScale = new Vector3(12f, 12f, 12f);
			}
			World = GameObject.Find("WorldScaleBase");
			World2 = GameObject.Find("World");
			if (SceneManager.GetActiveScene().buildIndex == 2 || SceneManager.GetActiveScene().buildIndex == 4)
			{
				GetComponent<TSTransform>().position = transform.localPosition.ToTSVector();
			}
			fac = GetComponent<CustomPathfinding>();
			asignsinglemoveorder(GetComponent<TSTransform>().position, null, MaxSpeed);
			HealthBar = transform.Find("HealthBar").gameObject;
			healthbarinterface = transform.Find("HealthBar").Find("GameObject").Find("barPanel")
				.Find("Healthbar")
				.GetComponent<Image>();
			if ((bool)Healthbaronly)
			{
				Healthbaronlyinterface = transform.Find("HealthBarOnly").Find("GameObject").Find("barPanel")
					.Find("Healthbar")
					.GetComponent<Image>();
			}
			if ((bool)Healthbaronly)
			{
				Healthbaronly.SetActive(value: false);
			}
			if ((bool)HealthBar)
			{
				HealthBar.SetActive(value: false);
			}
			if ((bool)GameObject.Find("RightController") && GameObject.Find("RightController").GetComponent<RightHand_triggerInstantOrder>().selectedships.Contains(gameObject))
			{
				HealthBar.SetActive(value: true);
			}
			ShipColor = getteambynumber(spawnnum);
			foreach (GameObject mesh in meshes)
			{
				mesh.GetComponent<MeshRenderCloakController>().StartMain();
			}
			healthbarstartscale = HealthBar.transform.localScale;
			forcedhealthscale = new Vector3(0.02f, 0.02f, 0.02f);
		}

		public static eShipColor getteambynumber(int input)
		{
			eShipColor result = eShipColor.Blue;
			if (input == 1)
			{
				result = eShipColor.Green;
			}
			if (input == 2)
			{
				result = eShipColor.Blue;
			}
			if (input == 3)
			{
				result = eShipColor.White;
			}
			if (input == 4)
			{
				result = eShipColor.Yellow;
			}
			if (input == 5)
			{
				result = eShipColor.Grey;
			}
			if (input == 6)
			{
				result = eShipColor.Red;
			}
			return result;
		}

		private Sprite getsprite()
		{
			if (unittargetcontrol.crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || unittargetcontrol.crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
			{
				if (UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(unittargetcontrol.team, unittargetcontrol.crosslevelholder.Gamemode))
				{
					if (ShipColor == unittargetcontrol.team)
					{
						return GreenHealthBar;
					}
					return BlueSprite;
				}
				return RedHealthBar;
			}
			if (ShipColor == eShipColor.Blue)
			{
				return BlueSprite;
			}
			if (ShipColor == eShipColor.Green)
			{
				return GreenHealthBar;
			}
			if (ShipColor == eShipColor.Grey)
			{
				return GreySprite;
			}
			if (ShipColor == eShipColor.Red)
			{
				return RedHealthBar;
			}
			if (ShipColor == eShipColor.White)
			{
				return WhiteSprite;
			}
			return YellowSprite;
		}

		private void AdjustStats()
		{
		}

		private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
		{
			if (stream.isWriting)
			{
				stream.SendNext(transform.position);
				stream.SendNext(transform.rotation);
			}
		}

		private void SetChildTransformTo(GameObject child, Transform parent)
		{
			child.transform.SetParent(parent.transform);
			child.transform.localPosition = Vector3.zero;
			child.transform.localRotation = Quaternion.identity;
			child.transform.localScale = Vector3.one;
		}

		public void enable()
		{
			if (outline == null)
			{
				outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
			}
			outline.Highlight(Color.green);
			outline.active = true;
			outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
			for (int i = 0; i < outline.listofcopiedmodels.Count; i++)
			{
				outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
			}
		}

		public void Disable()
		{
			if (outline == null)
			{
				outline = GetComponent<VRTK_OutlineObjectCopyHighlighter>();
			}
			outline.stencilOutline.SetFloat("_Thickness", getthicknesstoset());
			for (int i = 0; i < outline.listofcopiedmodels.Count; i++)
			{
				outline.listofcopiedmodels[i].transform.localPosition = outline.listoforiginalmodels[i].transform.localPosition;
			}
			outline.Unhighlight();
		}

		public void SpawnWeapons(string wep1, string wep2, string wep3, string wep4, string wep5, Vector3 wep1pos, Vector3 wep2pos, Vector3 wep3pos, Vector3 wep4pos, Vector3 wep5pos, Vector3 wep1scale, Vector3 wep2scale, Vector3 wep3scale, Vector3 wep4scale, Vector3 wep5scale, int spawnpos, bool bot, Vector3 spawnposmain)
		{
			isbot = bot;
			spawnnum = spawnpos;
			if (wep1 != null && wep1 != string.Empty)
			{
				GameObject original = Resources.Load("Turrets/" + wep1 + "InGame") as GameObject;
				GameObject gameObject = Object.Instantiate(original);
				gameObject.transform.parent = transform;
				gameObject.transform.localScale = wep1scale;
				gameObject.transform.localPosition = wep1pos;
				gameObject.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			if (wep2 != null && wep2 != string.Empty)
			{
				GameObject original2 = Resources.Load("Turrets/" + wep2 + "InGame") as GameObject;
				Debug.Log(wep2);
				GameObject gameObject2 = Object.Instantiate(original2);
				gameObject2.transform.parent = transform;
				gameObject2.transform.localScale = wep2scale;
				gameObject2.transform.localPosition = wep2pos;
				gameObject2.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			if (wep3 != null && wep3 != string.Empty)
			{
				GameObject original3 = Resources.Load("Turrets/" + wep3 + "InGame") as GameObject;
				GameObject gameObject3 = Object.Instantiate(original3);
				gameObject3.transform.parent = transform;
				gameObject3.transform.localScale = wep3scale;
				gameObject3.transform.localPosition = wep3pos;
				gameObject3.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			if (wep4 != null && wep4 != string.Empty)
			{
				GameObject original4 = Resources.Load("Turrets/" + wep4 + "InGame") as GameObject;
				GameObject gameObject4 = Object.Instantiate(original4);
				gameObject4.transform.parent = transform;
				gameObject4.transform.localScale = wep4scale;
				gameObject4.transform.localPosition = wep4pos;
				gameObject4.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			if (wep5 != null && wep5 != string.Empty)
			{
				GameObject original5 = Resources.Load("Turrets/" + wep5 + "InGame") as GameObject;
				GameObject gameObject5 = Object.Instantiate(original5);
				gameObject5.transform.parent = transform;
				gameObject5.transform.localScale = wep5scale;
				gameObject5.transform.localPosition = wep5pos;
				gameObject5.transform.localEulerAngles = new Vector3(0f, 0f, 0f);
			}
			GetComponent<TSTransform>().position = spawnposmain.ToTSVector();
		}

		public static eHullType getpreferance(_Weapon.guntype source)
		{
			return source switch
			{
				_Weapon.guntype.FighterGun => eHullType.Light, 
				_Weapon.guntype.BomberGun => eHullType.Capital, 
				_Weapon.guntype.Dual_Role => eHullType.Frigate, 
				_Weapon.guntype.MiniGun => eHullType.Light, 
				_Weapon.guntype.Cannon => eHullType.Capital, 
				_Weapon.guntype.Lazer => eHullType.Capital, 
				_Weapon.guntype.Missile => eHullType.Frigate, 
				_Weapon.guntype.CapitalMainGun => eHullType.Frigate, 
				_ => eHullType.Frigate, 
			};
		}

		public TSTransform FindTarget()
		{
			if (weaponry.Count > 0)
			{
				this.i++;
				if (weaponry.Count > 0 && weaponry[0].guntypemain == _Weapon.guntype.ShieldGenerator && (bool)WeaponTarget && WeaponTarget.GetComponent<_Ship>().isshieldfull())
				{
					WeaponTarget = null;
				}
				if (this.i == 5)
				{
					WeaponTarget = null;
				}
				if ((bool)WeaponTarget && TSVector.Distance(WeaponTarget.GetComponent<TSTransform>().position, fac.transformts.position) > weaponry[0].Range)
				{
					WeaponTarget = null;
				}
				FP fP = 100000;
				if (weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator)
				{
					if (shiptarget != null && UnitMovementcommandcontroller.getteam(shiptarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) != UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) && (weaponry[0].guntypemain != _Weapon.guntype.BomberGun || shiptarget.shipscript.HullType != eHullType.Light) && (weaponry[0].guntypemain != _Weapon.guntype.Missile || shiptarget.shipscript.HullType != eHullType.Light))
					{
						WeaponTarget = shiptarget.GetComponent<TSTransform>();
					}
				}
				else if (shiptarget != null && UnitMovementcommandcontroller.getteam(shiptarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(ShipColor, unittargetcontrol.crosslevelholder.Gamemode) && shiptarget != fac.transformts)
				{
					WeaponTarget = shiptarget.GetComponent<TSTransform>();
				}
				TSTransform[] reptemp = Reptemp;
				eHullType eHullType2 = getpreferance(weaponry[0].guntypemain);
				if (weaponry[0].guntypemain != _Weapon.guntype.ShieldGenerator)
				{
					if (WeaponTarget == null)
					{
						for (int i = 0; i < reptemp.Length; i++)
						{
							if (!(reptemp[i] != null) || !(reptemp[i].gameObject != null))
							{
								continue;
							}
							FP fP2 = TSVector.Distance(reptemp[i].position, fac.transformts.position);
							if (fP > fP2 && reptemp[i] != transform.gameObject && reptemp[i].shipscript.HullType == eHullType2)
							{
								hitdebug = reptemp[i];
								fP = fP2;
								if (fP < weaponry[0].Range)
								{
									WeaponTarget = reptemp[i];
								}
							}
						}
					}
					if (WeaponTarget == null)
					{
						for (int j = 0; j < reptemp.Length; j++)
						{
							if (!(reptemp[j] != null) || !(reptemp[j].gameObject != null))
							{
								continue;
							}
							FP fP3 = TSVector.Distance(reptemp[j].position, fac.transformts.position);
							if (fP > fP3 && reptemp[j] != transform.gameObject && (weaponry[0].guntypemain != _Weapon.guntype.BomberGun || reptemp[j].shipscript.HullType != eHullType.Light) && (weaponry[0].guntypemain != _Weapon.guntype.Missile || reptemp[j].shipscript.HullType != eHullType.Light))
							{
								hitdebug = reptemp[j];
								fP = fP3;
								if (fP < weaponry[0].Range)
								{
									WeaponTarget = reptemp[j];
								}
							}
						}
					}
				}
				else if (WeaponTarget == null)
				{
					for (int k = 0; k < reptemp.Length; k++)
					{
						if (!(reptemp[k] != null) || !(reptemp[k].gameObject != null))
						{
							continue;
						}
						FP fP4 = TSVector.Distance(reptemp[k].position, fac.transformts.position);
						if (fP > fP4 && reptemp[k] != transform.gameObject && !reptemp[k].shipscript.isshieldfull())
						{
							hitdebug = reptemp[k];
							fP = fP4;
							if (fP < weaponry[0].Range)
							{
								WeaponTarget = reptemp[k];
							}
						}
					}
				}
				return WeaponTarget;
			}
			return null;
		}
	}
