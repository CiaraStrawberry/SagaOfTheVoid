using System;
using System.Collections.Generic;
using Forge3D;
using TrueSync;
using UnityEngine;

public class _Weapon : TrueSyncBehaviour
{
	public enum guntype
	{
		Cannon,
		Missile,
		MiniGun,
		ShieldGenerator,
		Tracker,
		Cloak,
		HPUpgrade,
		EngineUpgrade,
		WeaponUpgrade,
		FighterGun,
		BomberGun,
		Dual_Role,
		CapitalMainGun,
		Lazer
	}

	public class FiredContainer
	{
		public FP timestart = 0;

		public FP timesince;

		public FP damage;

		public GameObject Target;

		public FiredContainer(FP timesincein, FP damagein, GameObject Targetin)
		{
			timestart = timesincein;
			damage = damagein;
			Target = Targetin;
		}
	}

	private UnitMovementcommandcontroller unittargetcontrol;

	private _Ship parentscript;

	public _Ship.eShipColor team;

	public GameObject verticalmovechild;

	public Material shieldmatgam;

	public GameObject horrizontalmovething;

	public FP chanceout;

	private F3DPoolManager poolmanager;

	public AudioClip FireSound;

	public AudioSource audiosource;

	public GameObject WorldBase;

	public GameObject lastfired;

	private int startcount;

	public FP debughitchance;

	public List<FiredContainer> timepassedonships = new List<FiredContainer>();

	public FP burstfirewait;

	public int shotsperburst;

	public int damage;

	public GameObject MuzzleFlash;

	public GameObject MuzzlePos;

	[SerializeField]
	private GameObject m_Cannon_Projectile;

	[SerializeField]
	private GameObject m_Missile_Projectile;

	[SerializeField]
	private GameObject m_MiniGun_Projectile;

	[SerializeField]
	private guntype m_guntypemain;

	[SerializeField]
	private int m_Range;

	[SerializeField]
	private int m_OptimalRange;

	[SerializeField]
	private double m_DecayRate;

	[SerializeField]
	private FP m_Accuracy;

	private TSRandom customran;

	private bool started;

	private int indexWeapon;

	private int turnspeed = 50;

	private FP timepassing;

	public FP timebetweenshots = 1;

	public GameObject Lazer_Proectile;

	public float debugwaittime;

	public int runnext;

	public int timepassedforrun;

	public int a;

	public string teststring;

	public FP timetilnextshot;

	public int shotsfiredinthisburst;

	public FP timepassed;

	public int debugfiredcontainercount;

	public GameObject test;

	private int i;

	public GameObject Cannon_Projectile
	{
		get
		{
			return m_Cannon_Projectile;
		}
		set
		{
			m_Cannon_Projectile = value;
		}
	}

	public GameObject Missile_Projectile
	{
		get
		{
			return m_Missile_Projectile;
		}
		set
		{
			m_Missile_Projectile = value;
		}
	}

	public GameObject MiniGun_Projectile
	{
		get
		{
			return m_MiniGun_Projectile;
		}
		set
		{
			m_MiniGun_Projectile = value;
		}
	}

	public guntype guntypemain
	{
		get
		{
			return m_guntypemain;
		}
		set
		{
			m_guntypemain = value;
		}
	}

	public eWeaponType WeaponType { get; set; }

	public eHullType MinimumHull { get; set; }

	public eShipClass SupportedShipClass { get; set; }

	public int RoundInMagazine { get; set; }

	public int RoundsPerSalvo { get; set; }

	public TimeSpan ReloadTime { get; set; }

	public FP TimeBetweenSalvos { get; set; }

	public int DamagePerRound { get; set; }

	public int TrackingSpeed { get; set; }

	public int Range
	{
		get
		{
			return m_Range;
		}
		set
		{
			m_Range = value;
		}
	}

	public int OptimalRange
	{
		get
		{
			return m_OptimalRange;
		}
		set
		{
			m_OptimalRange = value;
		}
	}

	public double DecayRate
	{
		get
		{
			return m_DecayRate;
		}
		set
		{
			m_DecayRate = value;
		}
	}

	public FP Accuracy
	{
		get
		{
			return m_Accuracy;
		}
		set
		{
			m_Accuracy = value;
		}
	}

	private void Awake()
	{
	}

	public void StartMain(int inputint)
	{
		started = true;
		i = inputint;
		startcount = 0;
		customran = TSRandom.New(base.transform.parent.GetComponent<PhotonView>().viewID + inputint);
		parentscript = base.transform.parent.GetComponent<_Ship>();
		if (guntypemain == guntype.EngineUpgrade)
		{
			parentscript.MaxSpeed *= 1.15f;
		}
		if (guntypemain == guntype.HPUpgrade)
		{
			parentscript.Armor = Mathf.RoundToInt((float)parentscript.Armor * 1.15f);
		}
		if (guntypemain == guntype.WeaponUpgrade)
		{
			foreach (Transform item in base.transform.parent)
			{
				if (item.tag == "Pickup")
				{
					_Weapon component = item.GetComponent<_Weapon>();
					component.timebetweenshots *= (FP)1.15;
				}
			}
		}
		if (guntypemain == guntype.Cloak)
		{
			parentscript.iscloacked = true;
		}
		if (guntypemain == guntype.Tracker)
		{
			parentscript.isTracker = true;
		}
		unittargetcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
		poolmanager = GameObject.Find("F3dPoolManager").GetComponent<F3DPoolManager>();
		team = parentscript.ShipColor;
		if (verticalmovechild == null && transform.childCount > 0 && transform.GetChild(0).childCount > 0)
		{
			verticalmovechild = transform.GetChild(0).GetChild(0).gameObject;
		}
		_Ship component2 = transform.parent.GetComponent<_Ship>();
		if (Cannon_Projectile != null)
		{
			component2.Cannon_Projectile = Cannon_Projectile;
		}
		if (Missile_Projectile != null)
		{
			component2.Missile_Projectile = Missile_Projectile;
		}
		if (MiniGun_Projectile != null)
		{
			component2.MiniGun_Projectile = MiniGun_Projectile;
		}
		if (Lazer_Proectile != null)
		{
			component2.Lazer_Shot = Lazer_Proectile;
		}
		component2.shieldmaterial = shieldmatgam;
		damage = Calculate_damage();
		audiosource = GetComponent<AudioSource>();
		WorldBase = GameObject.Find("WorldScaleBase");
		timepassed = 0;
	}

	public int Calculate_damage()
	{
		int result = 0;
		if (guntypemain == guntype.Cannon)
		{
			result = 800;
		}
		if (guntypemain == guntype.Cannon && parentscript.HullType == eHullType.Capital)
		{
			result = 275;
		}
		if (guntypemain == guntype.Missile)
		{
			result = 300;
		}
		if (guntypemain == guntype.MiniGun)
		{
			result = 20;
		}
		if (guntypemain == guntype.ShieldGenerator)
		{
			result = 300;
		}
		if (guntypemain == guntype.FighterGun)
		{
			result = 20;
		}
		if (guntypemain == guntype.BomberGun)
		{
			result = 200;
		}
		if (guntypemain == guntype.Dual_Role)
		{
			result = 50;
		}
		if (guntypemain == guntype.CapitalMainGun)
		{
			result = 2000;
		}
		if (guntypemain == guntype.Lazer)
		{
			result = 3000;
		}
		return result;
	}

	private void fireweapon(Vector3 targetpos)
	{
		if (MuzzlePos == null)
		{
			MuzzlePos = gameObject;
		}
		bool flag = ishit(parentscript.WeaponTarget.shipscript);
		GameObject id = parentscript.WeaponTarget.gameObject;
		if (guntypemain == guntype.Cannon)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.Cannon, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.Missile)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.Missile, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.MiniGun)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.ShieldGenerator)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, hit: true, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.ShieldGenerator, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.FighterGun)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.BomberGun)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.BomberGun, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.Dual_Role)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.MiniGun, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.CapitalMainGun)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.Cannon, MuzzlePos, guntypemain);
		}
		if (guntypemain == guntype.Lazer)
		{
			lastfired = parentscript.FireWeaponNetwork(targetpos, flag, getshotstartpos(), id, damage, getshotstartposlocal(), guntype.Lazer, MuzzlePos, guntypemain);
		}
		if ((bool)audiosource && (bool)FireSound && gameObject.GetActive())
		{
			audiosource.clip = FireSound;
			audiosource.PlayOneShot(FireSound, audiosource.volume);
		}
		if (flag && guntypemain != guntype.ShieldGenerator)
		{
			timepassedonships.Add(new FiredContainer(hittime(parentscript.fac.transformts.position, parentscript.WeaponTarget.position, 440), damage, parentscript.WeaponTarget.gameObject));
		}
		if ((bool)MuzzleFlash)
		{
			Transform transform = F3DPoolManager.Pools["GeneratedPool"].Spawn(MuzzleFlash.transform);
			if ((bool)transform)
			{
				transform.transform.parent = WorldBase.transform.Find("World").Find("Objects").Find("Working");
				transform.transform.localScale = new Vector3(100f, 100f, 100f);
				transform.transform.position = base.transform.position;
			}
		}
	}

	public static bool AngleDir(Transform start, Vector3 target)
	{
		Vector3 vector = start.InverseTransformPoint(target);
		if ((double)vector.z < 0.0)
		{
			return false;
		}
		if ((double)vector.z > 0.0)
		{
			return true;
		}
		return true;
	}

	private Vector3 getshotstartpos()
	{
		if (parentscript.HullType == eHullType.Light || parentscript.HullType == eHullType.Corvette)
		{
			Vector3 position = transform.position;
			return parentscript.meshes[Mathf.RoundToInt(UnityEngine.Random.Range(0, parentscript.meshes.Count - 1))].transform.position;
		}
		return transform.position;
	}

	private Vector3 getshotstartposlocal()
	{
		if (parentscript.HullType == eHullType.Light || parentscript.HullType == eHullType.Corvette)
		{
			Vector3 localPosition = transform.localPosition;
			return parentscript.meshes[Mathf.RoundToInt(UnityEngine.Random.Range(0, parentscript.meshes.Count - 1))].transform.localPosition;
		}
		return transform.localPosition;
	}

	public FP hittime(TSVector start, TSVector end, FP speed)
	{
		FP fP = 0;
		fP = TSVector.Distance(start, end) / speed;
		if (guntypemain != guntype.Lazer)
		{
			return fP + 0.5;
		}
		return 3;
	}

	private bool ishit(_Ship inputship)
	{
		bool flag = false;
		if (guntypemain == guntype.Missile && inputship.HullType != eHullType.Light)
		{
			flag = true;
		}
		else
		{
			FP fP = (chanceout = TSMath.Round(100 * getmodifyer(inputship.HullType)));
			if (parentscript.isTrackingimproved)
			{
				fP *= (FP)2;
			}
			FP fP2 = customran.Next(0, 100);
			flag = ((fP > fP2) ? true : false);
			teststring = fP.ToString() + " is more? than " + fP2.ToString() + " so it is a hit: " + flag;
		}
		return flag;
	}

	public FP getmodifyer(eHullType input)
	{
		FP result = 1;
		if (guntypemain == guntype.Cannon || guntypemain == guntype.CapitalMainGun)
		{
			if (input == eHullType.Light)
			{
				result = 0.02;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.05;
			}
			if (input == eHullType.Frigate)
			{
				result = 0.8;
			}
			if (input == eHullType.Cruiser)
			{
				result = 0.8;
			}
			if (input == eHullType.Destroyer)
			{
				result = 0.8;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.MiniGun)
		{
			if (input == eHullType.Light)
			{
				result = 0.1;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.1;
			}
			if (input == eHullType.Frigate)
			{
				result = 0.8;
			}
			if (input == eHullType.Cruiser)
			{
				result = 0.8;
			}
			if (input == eHullType.Destroyer)
			{
				result = 0.8;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.Missile)
		{
			if (input == eHullType.Light)
			{
				result = 0.05;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.5;
			}
			if (input == eHullType.Frigate)
			{
				result = 1;
			}
			if (input == eHullType.Cruiser)
			{
				result = 1;
			}
			if (input == eHullType.Destroyer)
			{
				result = 1;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.FighterGun)
		{
			if (input == eHullType.Light)
			{
				result = 0.1;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.5;
			}
			if (input == eHullType.Frigate)
			{
				result = 1;
			}
			if (input == eHullType.Cruiser)
			{
				result = 1;
			}
			if (input == eHullType.Destroyer)
			{
				result = 1;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.BomberGun)
		{
			if (input == eHullType.Light)
			{
				result = 0.05;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.5;
			}
			if (input == eHullType.Frigate)
			{
				result = 1;
			}
			if (input == eHullType.Cruiser)
			{
				result = 1;
			}
			if (input == eHullType.Destroyer)
			{
				result = 1;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.Dual_Role)
		{
			if (input == eHullType.Light)
			{
				result = 0.01;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.5;
			}
			if (input == eHullType.Frigate)
			{
				result = 0.5;
			}
			if (input == eHullType.Cruiser)
			{
				result = 0.5;
			}
			if (input == eHullType.Destroyer)
			{
				result = 1;
			}
			if (input == eHullType.Capital)
			{
				result = 1;
			}
		}
		if (guntypemain == guntype.Lazer)
		{
			if (input == eHullType.Light)
			{
				result = 0.01;
			}
			if (input == eHullType.Corvette)
			{
				result = 0.02;
			}
			if (input == eHullType.Frigate)
			{
				result = 0.6;
			}
			if (input == eHullType.Cruiser)
			{
				result = 0.9;
			}
			if (input == eHullType.Destroyer)
			{
				result = 0.9;
			}
			if (input == eHullType.Capital)
			{
				result = 0.9;
			}
		}
		if (guntypemain == guntype.ShieldGenerator)
		{
			result = 1;
		}
		debughitchance = result;
		return result;
	}

	public void UpdateSpec()
	{
		debugfiredcontainercount = timepassedonships.Count;
		List<FiredContainer> list = new List<FiredContainer>();
		foreach (FiredContainer timepassedonship in timepassedonships)
		{
			timepassedonship.timestart += TrueSyncManager.DeltaTime;
			if (timepassedonship.timestart >= timepassedonship.timesince)
			{
				if (timepassedonship.Target.gameObject != null)
				{
					timepassedonship.Target.GetComponent<_Ship>().takeDamage(damage, parentscript.ShipColor, parentscript.HullType);
				}
				list.Add(timepassedonship);
			}
		}
		foreach (FiredContainer item in list)
		{
			timepassedonships.Remove(item);
		}
		if (startcount == 0)
		{
			team = parentscript.ShipColor;
		}
		startcount++;
		if (this != null && TrueSyncManager.Time > 0)
		{
			if (verticalmovechild == null && transform.childCount > 0 && (bool)transform.GetChild(0) && transform.GetChild(0).childCount > 0 && (bool)transform.GetChild(0).GetChild(0))
			{
				verticalmovechild = transform.GetChild(0).GetChild(0).gameObject;
			}
			if ((bool)parentscript.WeaponTarget && parentscript.WeaponTarget.gameObject == null)
			{
				parentscript.WeaponTarget = null;
			}
			if ((bool)parentscript.WeaponTarget && guntypemain != guntype.CapitalMainGun)
			{
				look(parentscript.WeaponTarget.transform.position);
			}
			timepassing += TrueSyncManager.DeltaTime;
			if (timepassing >= timetilnextshot && (bool)parentscript.WeaponTarget)
			{
				timepassing = 0;
				if (guntypemain != guntype.ShieldGenerator)
				{
					fireweaponchecknormal();
				}
				else
				{
					fireweaponcheckshield();
				}
				shotsfiredinthisburst++;
				if (shotsfiredinthisburst == shotsperburst)
				{
					timetilnextshot = burstfirewait;
					shotsfiredinthisburst = 0;
				}
				else
				{
					timetilnextshot = timebetweenshots;
				}
			}
		}
		if ((bool)audiosource)
		{
			audiosource.minDistance = 0.003f * WorldBase.transform.localScale.x;
			audiosource.maxDistance = 0.02f * WorldBase.transform.localScale.x;
		}
	}

	private void fireweaponchecknormal()
	{
		if (TSVector.Distance(parentscript.WeaponTarget.GetComponent<TSTransform>().position, parentscript.fac.transformts.position) < Range && UnitMovementcommandcontroller.getteam(parentscript.WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) != UnitMovementcommandcontroller.getteam(parentscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode))
		{
			fireweapon(parentscript.WeaponTarget.transform.position);
		}
		else
		{
			parentscript.WeaponTarget = null;
		}
	}

	private void fireweaponcheckshield()
	{
		if (TSVector.Distance(parentscript.WeaponTarget.GetComponent<TSTransform>().position, parentscript.fac.transformts.position) < Range && UnitMovementcommandcontroller.getteam(parentscript.WeaponTarget.shipscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(parentscript.ShipColor, unittargetcontrol.crosslevelholder.Gamemode))
		{
			fireweapon(parentscript.WeaponTarget.transform.position);
		}
		else
		{
			parentscript.WeaponTarget = null;
		}
	}

	public void look(Vector3 target)
	{
		if (target != new Vector3(0f, 0f, 0f))
		{
			Quaternion b = Quaternion.LookRotation(target - transform.position);
			Quaternion rotation = Quaternion.Lerp(transform.rotation, b, Time.deltaTime * (float)turnspeed);
			if ((bool)horrizontalmovething)
			{
				horrizontalmovething.transform.rotation = rotation;
				horrizontalmovething.transform.localEulerAngles = new Vector3(0f, horrizontalmovething.transform.localEulerAngles.y, 0f);
			}
			if ((bool)verticalmovechild)
			{
				verticalmovechild.transform.rotation = rotation;
				verticalmovechild.transform.localEulerAngles = new Vector3(verticalmovechild.transform.localEulerAngles.x, 0f, 0f);
			}
		}
	}
}
