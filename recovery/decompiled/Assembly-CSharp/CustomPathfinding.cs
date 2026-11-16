using System.Collections.Generic;
using System.Linq;
using TrueSync;
using UnityEngine;

public class CustomPathfinding : TrueSyncBehaviour
{
	[SerializeField]
	public TSVector Target;

	public GameObject World;

	public FP turnspeed;

	[AddTracking]
	public FP speed = 0;

	public FP maxspeed;

	public int acceleration;

	public _Ship shipscript;

	public bool moving = true;

	private bool accelerating;

	private int mindecdistance = 300;

	[SerializeField]
	private FP distancetotarget;

	public TSVector localtarget;

	private PhotonTransformView transformview;

	public bool currentlymoving;

	public TSTransform transformts;

	public bool started;

	public float debug;

	public GameObject debuggam;

	public UnitMovementcommandcontroller unitcom;

	public Vector3 Lastpos;

	public Vector3 thisposdif;

	public List<Vector3> startmeshpos = new List<Vector3>();

	private int waitforpushaway;

	public float timepassedsincespawn;

	public float debugtest;

	private TSQuaternion lastquaternion;

	public TSTransform Closestoverall;

	public List<TSTransform> allshipstemp = new List<TSTransform>();

	public void Start()
	{
		World = GameObject.Find("World");
		shipscript = GetComponent<_Ship>();
		acceleration = 4;
		turnspeed = shipscript.MaxAngularSpeed;
		transformts = GetComponent<TSTransform>();
		unitcom = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
		waitforpushaway = (int)(long)shipscript.MaxAngularSpeed % 5;
		int num = 0;
		foreach (Transform item in base.transform)
		{
			if (item.name == "Ship")
			{
				startmeshpos.Add(item.transform.localPosition);
				Vector3 localPosition = item.transform.localPosition;
				localPosition.z = Mathf.Lerp(-3000f, startmeshpos[num].z, 1f);
				item.transform.localPosition = localPosition;
				num++;
			}
		}
	}

	public void assigntarget(TSVector targetin, TSVector localtargetin)
	{
		Target = targetin;
		localtarget = localtargetin;
		Accelerate();
	}

	public void SpecUpdate()
	{
		timepassedsincespawn++;
		if (turnspeed < shipscript.MaxAngularSpeed)
		{
			turnspeed += (FP)0.003;
		}
		else
		{
			turnspeed = shipscript.MaxAngularSpeed;
		}
		thisposdif = Lastpos - transform.position;
		Lastpos = transform.position;
		if (timepassedsincespawn <= 180f)
		{
			int num = 0;
			foreach (GameObject mesh in shipscript.meshes)
			{
				Vector3 localPosition = mesh.transform.localPosition;
				localPosition.z = Mathf.Lerp(-3000f, startmeshpos[num].z, 1f / (180f / timepassedsincespawn));
				debugtest = 1f / (180f / timepassedsincespawn);
				mesh.transform.localPosition = localPosition;
				num++;
			}
		}
		if ((bool)shipscript && timepassedsincespawn > 180f)
		{
			distancetotarget = TSVector.Distance(transformts.position, localtarget);
			if (!started)
			{
				maxspeed = shipscript.MaxSpeed * 30;
				started = true;
			}
			if (shipscript.WeaponTarget != null)
			{
				maxspeed = shipscript.MaxSpeed * 30;
			}
			else
			{
				maxspeed = shipscript.averagespeed * 30;
			}
			if (TrueSyncManager.Time > 0)
			{
				currentlymoving = false;
				if (distancetotarget > 10 || shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette)
				{
					if (distancetotarget < mindecdistance)
					{
						if (speed > 0)
						{
							speed -= (FP)(acceleration / 2);
						}
					}
					else if (accelerating && speed < maxspeed)
					{
						speed += (FP)acceleration;
					}
					else if (speed > maxspeed)
					{
						accelerating = false;
					}
					if (speed > maxspeed)
					{
						speed = maxspeed;
					}
					if (shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette)
					{
						speed = maxspeed;
					}
					if (Target != new TSVector(0, 0, 0) && moving && distancetotarget > 150)
					{
						Look(Target);
					}
					if ((Target != new TSVector(0, 0, 0) && moving) || shipscript.HullType == eHullType.Light || shipscript.HullType == eHullType.Corvette)
					{
						Move(speed);
					}
				}
				else if (shipscript.HullType != eHullType.Light && shipscript.HullType != eHullType.Corvette && speed > 0)
				{
					Move(speed);
				}
				if (distancetotarget < 400)
				{
					shipscript.movetonextstoredorder();
				}
			}
		}
		else
		{
			shipscript = GetComponent<_Ship>();
		}
		waitforpushaway++;
		if ((bool)Closestoverall && shipscript.HullType != eHullType.Light && shipscript.HullType != eHullType.Corvette && TSVector.Distance(Closestoverall.position, transformts.position) < 300 && Closestoverall.shipscript.HullType != eHullType.Light && Closestoverall.shipscript.HullType != eHullType.Corvette)
		{
			TSVector tSVector = transformts.position - Closestoverall.position;
			transformts.position += tSVector * 0.005;
		}
		if (waitforpushaway > 10 && shipscript.HullType != eHullType.Light)
		{
			waitforpushaway = 0;
			TSTransform tSTransform = getclosesttarget();
			if (tSTransform != null)
			{
				Closestoverall = tSTransform;
			}
		}
	}

	private void Look(TSVector targetin)
	{
		TSVector forward = localtarget - transformts.position;
		TSQuaternion to = TSQuaternion.LookRotation(forward);
		TSQuaternion tSQuaternion = TSQuaternion.Slerp(transformts.rotation, to, TrueSyncManager.DeltaTime * turnspeed);
		if (TSQuaternion.Angle(tSQuaternion, lastquaternion) < TrueSyncManager.DeltaTime * turnspeed / 2)
		{
			transformts.rotation = tSQuaternion;
		}
		lastquaternion = tSQuaternion;
	}

	private bool isInFront(TSVector targetpos)
	{
		return TSVector.Dot(transformts.forward, targetpos) > 0;
	}

	private bool FrontTest()
	{
		TSVector forward = transformts.forward;
		TSVector vector = localtarget - transformts.position;
		vector.Normalize();
		FP fP = TSMath.Acos(TSVector.Dot(forward, vector)) * 57.29578f;
		if (fP <= 45f)
		{
			return true;
		}
		return false;
	}

	private void Move(FP speedin)
	{
		if (FrontTest() || shipscript.HullType == eHullType.Light)
		{
			TSVector position = transformts.position;
			TSVector forward = transformts.forward;
			if (moving)
			{
				transformts.position += transformts.forward * (speedin / 60);
			}
			if (moving && speedin > acceleration)
			{
				currentlymoving = true;
			}
			if (transformts.position.x > 20000 || transformts.position.x > 20000 || transformts.position.x > 20000)
			{
				transformts.position = position;
			}
		}
	}

	private void Accelerate()
	{
		accelerating = true;
	}

	private TSTransform getclosesttarget()
	{
		if ((bool)unitcom)
		{
			List<TSTransform> list = new List<TSTransform>();
			list = ((unitcom.allshipststransform.Length == 0) ? allshipstemp : (allshipstemp = unitcom.allshipststransform.ToList()));
			if (list.Contains(transformts))
			{
				list.Remove(transformts);
			}
			if (list.Count != 0)
			{
				List<TSTransform> list2 = new List<TSTransform>();
				foreach (TSTransform item in list)
				{
					if (item == null || item.gameObject == null)
					{
						list2.Add(item);
					}
				}
				foreach (TSTransform item2 in list2)
				{
					list.Remove(item2);
				}
				list = list.OrderBy((TSTransform x) => TSVector.Distance(transformts.position, x.position)).ToList();
				if (list.Count > 0)
				{
					return list[0];
				}
				return null;
			}
			return null;
		}
		return null;
	}
}
