using System.Collections.Generic;
using TrueSync;
using UnityEngine;

public class InputRelay : TrueSyncBehaviour
{
	private class shipbuycontainer
	{
		public int spawnship;

		public TSVector spawnpos;

		public int spawnteam;

		public int viewID;

		public shipbuycontainer(int spawnshipin, TSVector spawnposin, int spawnteamin, int viewIDin)
		{
			spawnship = spawnshipin;
			spawnpos = spawnposin;
			spawnteam = spawnteamin;
			viewID = viewIDin;
		}
	}

	private class ordermovecontainer
	{
		public List<GameObject> selectedships;

		public TSVector targetpos;

		public GameObject targethold;

		public FP averagespeed;

		public bool waypoints;

		public ordermovecontainer(List<GameObject> selectedshipsin, TSVector targetposin, GameObject targetholdin, FP averagespeedin, bool waypointsin)
		{
			selectedships = selectedshipsin;
			targethold = targetholdin;
			targetpos = targetposin;
			averagespeed = averagespeedin;
			waypoints = waypointsin;
		}
	}

	private List<shipbuycontainer> shipstobuy = new List<shipbuycontainer>();

	private List<ordermovecontainer> orderstodo = new List<ordermovecontainer>();

	private List<AIController> aicontrollers = new List<AIController>();

	public RelayController manager;

	private GameObject Objects;

	public FP timepassed;

	public bool runsimulationnow;

	public bool running;

	private PhotonView photonview;

	private PhotonView photonviewthis;

	public FP timepassedmain;

	private bool simrunning;

	public FP ActualGameTimePassed;

	private GameObject temptarget;

	private float exacttime;

	private Vector3 pos;

	private Quaternion targetrot;

	private int Armour;

	private int Shields;

	private GameObject unitcon;

	private UnitMovementcommandcontroller unitcomscript;

	public List<_Ship> allships = new List<_Ship>();

	private int[] agentsPerSide = new int[20];

	private void Start()
	{
		Objects = GameObject.Find("Objects");
		if ((bool)GameObject.Find("JoinMultiplayer"))
		{
			photonview = GameObject.Find("JoinMultiplayer").GetComponent<PhotonView>();
		}
		photonviewthis = GetComponent<PhotonView>();
		photonviewthis.viewID = PhotonNetwork.AllocateViewID();
	}

	private void OnJoinedRoom()
	{
	}

	[PunRPC]
	public void runsim()
	{
	}

	private void Update()
	{
		timepassedmain = TrueSyncManager.Time;
		timepassed += (FP)Time.deltaTime;
		if (running)
		{
			ActualGameTimePassed += TrueSyncManager.DeltaTime;
		}
		if (exacttime == 0f || !(ActualGameTimePassed > exacttime))
		{
			return;
		}
		if ((bool)temptarget)
		{
			_Ship component = temptarget.GetComponent<_Ship>();
			if (Quaternion.Angle(component.transform.rotation, targetrot) > 15f)
			{
				component.transform.rotation = targetrot;
			}
			if (Vector3.Distance(component.GetComponent<TSTransform>().position.ToVector(), pos) > 200f)
			{
				component.GetComponent<TSTransform>().position = pos.ToTSVector();
			}
			component.Armor = Armour;
			component.Shield = Shields;
			Debug.Log("check");
		}
		exacttime = 0f;
	}

	public void ordermove(List<GameObject> selectedshipsin, TSVector positionin, GameObject gaminput, FP speedin, bool waypoints)
	{
		orderstodo.Add(new ordermovecontainer(selectedshipsin, positionin, gaminput, speedin, waypoints));
	}

	public void ordershipspawn(int spawnshipin, TSVector spawnposin, int spawnteamin, int ViewIDin)
	{
		shipstobuy.Add(new shipbuycontainer(spawnshipin, spawnposin, spawnteamin, ViewIDin));
		Debug.Log(spawnshipin);
	}

	public void CheckValues()
	{
		foreach (Transform item in Objects.transform)
		{
			if (item.name != "Working" && item.name != "Engines")
			{
				_Ship component = item.GetComponent<_Ship>();
				photonviewthis.RpcSecure("CheckValuesClient", PhotonTargets.AllViaServer, true, item.GetComponent<PhotonView>().viewID, ActualGameTimePassed.AsFloat() + 0.5f, item.transform.GetComponent<TSTransform>().position.ToVector(), item.transform.rotation, component.Armor, component.Shield);
			}
		}
	}

	public float addstuff(float a, float b)
	{
		return a + b;
	}

	private void managerstart()
	{
		TSRandom.instance = TSRandom.New(5);
		unitcon = GameObject.Find("Controllers");
		unitcomscript = unitcon.GetComponent<UnitMovementcommandcontroller>();
		unitcomscript.StartMain();
		foreach (Transform item in unitcon.transform)
		{
			if (item.GetComponent<AIController>() != null)
			{
				aicontrollers.Add(item.GetComponent<AIController>());
			}
		}
		foreach (Transform item2 in Objects.transform)
		{
			if (item2.name != "Working" && item2.name != "Engines")
			{
				item2.GetComponent<_Ship>().StartMain();
			}
		}
	}

	public override void OnSyncedInput()
	{
		if (running)
		{
			foreach (_Ship item in unitcomscript.allshipsscript)
			{
				if ((bool)item && item.gameObject != null)
				{
					item.Update();
				}
			}
		}
		if (shipstobuy.Count == 0 && orderstodo.Count == 0)
		{
			TrueSyncInput.SetInt(0, 10);
		}
		if ((manager == null || (manager.relay == null && unitcomscript != null)) && (bool)unitcomscript)
		{
			manager = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
			unitcomscript.output = true;
			manager.relay = this;
			Debug.Log(manager.relay);
		}
		if ((bool)unitcomscript)
		{
			unitcomscript.output = true;
		}
		else
		{
			unitcomscript = (unitcomscript = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>());
		}
		if (shipstobuy.Count > 0)
		{
			TrueSyncInput.SetInt(0, 1);
			TrueSyncInput.SetInt(1, shipstobuy[0].spawnship);
			TrueSyncInput.SetTSVector(2, shipstobuy[0].spawnpos);
			TrueSyncInput.SetInt(3, shipstobuy[0].spawnteam);
			TrueSyncInput.SetInt(4, shipstobuy[0].viewID);
			Debug.Log(string.Concat(shipstobuy[0].spawnship, " ", shipstobuy[0].spawnpos, " ", shipstobuy[0].viewID));
			shipstobuy.RemoveAt(0);
		}
		else
		{
			TrueSyncInput.SetInt(1, 0);
		}
		if (orderstodo.Count > 0)
		{
			int num = 2;
			TrueSyncInput.SetInt(0, 0);
			foreach (GameObject selectedship in orderstodo[0].selectedships)
			{
				if (orderstodo[0].targethold == null && selectedship.gameObject != null)
				{
					PhotonView component = selectedship.GetComponent<PhotonView>();
					TrueSyncInput.SetInt((byte)num, component.viewID);
					num++;
					TrueSyncInput.SetTSVector((byte)num, TargetPosition(num / 2, GameObject.Find("World").transform.Find("Objects").InverseTransformPoint(orderstodo[0].targetpos.ToVector()).ToTSVector(), orderstodo[0].selectedships.Count));
					num++;
					TrueSyncInput.SetInt((byte)num, 3000);
					num++;
					TrueSyncInput.SetFP((byte)num, orderstodo[0].averagespeed);
					num++;
					TrueSyncInput.SetBool((byte)num, orderstodo[0].waypoints);
					num++;
				}
				else if (selectedship.gameObject != null)
				{
					PhotonView component2 = selectedship.GetComponent<PhotonView>();
					TrueSyncInput.SetInt((byte)num, component2.viewID);
					num++;
					TrueSyncInput.SetTSVector((byte)num, TargetPosition(num / 2, GameObject.Find("World").transform.Find("Objects").InverseTransformPoint(orderstodo[0].targetpos.ToVector()).ToTSVector(), orderstodo[0].selectedships.Count));
					num++;
					TrueSyncInput.SetInt((byte)num, orderstodo[0].targethold.GetComponent<PhotonView>().viewID);
					num++;
					TrueSyncInput.SetFP((byte)num, orderstodo[0].averagespeed);
					num++;
					TrueSyncInput.SetBool((byte)num, orderstodo[0].waypoints);
					num++;
				}
			}
			TrueSyncInput.SetInt(1, num);
			orderstodo.RemoveAt(0);
		}
		if (runsimulationnow)
		{
			TrueSyncInput.SetInt(100, 1);
			runsimulationnow = false;
		}
	}

	public override void OnSyncedUpdate()
	{
		if (running && unitcomscript.output)
		{
			foreach (AIController aicontroller in aicontrollers)
			{
				aicontroller.SpecUpdate();
			}
		}
		if ((bool)unitcomscript)
		{
			unitcomscript.Timeleft -= TrueSyncManager.DeltaTime;
		}
		if (TrueSyncInput.GetInt(100) == 1 && !running)
		{
			Debug.Log("start");
			managerstart();
		}
		int num = 3000;
		TSVector tSVector = new TSVector(0, 0, 0);
		int num2 = 3000;
		FP fP = 1;
		int num3 = TrueSyncInput.GetInt(0);
		bool flag = false;
		switch (num3)
		{
		case 0:
		{
			int num5 = TrueSyncInput.GetInt(1);
			if (num5 <= 0)
			{
				break;
			}
			int num6 = 2;
			while (num6 < num5)
			{
				num = TrueSyncInput.GetInt((byte)num6);
				num6++;
				Debug.Log(num + " out of " + num6);
				tSVector = TrueSyncInput.GetTSVector((byte)num6);
				num6++;
				num2 = TrueSyncInput.GetInt((byte)num6);
				num6++;
				fP = TrueSyncInput.GetFP((byte)num6);
				num6++;
				flag = TrueSyncInput.GetBool((byte)num6);
				num6++;
				if (num != 3000)
				{
					GameObject gameObject = null;
					if ((bool)PhotonView.Find(num))
					{
						gameObject = PhotonView.Find(num).gameObject;
					}
					GameObject target = null;
					if (num2 != 3000 && PhotonView.Find(num2) != null)
					{
						target = PhotonView.Find(num2).gameObject;
					}
					if ((bool)gameObject)
					{
						gameObject.GetComponent<_Ship>().asignMoveOrder(tSVector, target, fP, flag);
					}
				}
			}
			break;
		}
		case 1:
		{
			int num4 = TrueSyncInput.GetInt(1);
			if (num4 != 0)
			{
				spawnshipfun(num4, TrueSyncInput.GetTSVector(2), TrueSyncInput.GetInt(3), TrueSyncInput.GetInt(4));
			}
			break;
		}
		}
	}

	public void spawnshipfun(int a, TSVector pos, int oriplayer, int viewidin)
	{
		GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().spawnship(a, pos, oriplayer, viewidin);
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
