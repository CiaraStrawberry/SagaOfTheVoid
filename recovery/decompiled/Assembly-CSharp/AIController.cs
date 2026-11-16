using System.Collections.Generic;
using System.Linq;
using TrueSync;
using UnityEngine;

public class AIController : MonoBehaviour
{
	public enum AIstate
	{
		attacking,
		retreating,
		holding,
		maintain
	}

	public _Ship.eShipColor team;

	public List<GameObject> ships = new List<GameObject>();

	public TSTransform[] enemyships = new TSTransform[0];

	private UnitMovementcommandcontroller unitcontrol;

	public TSTransform Target;

	[SerializeField]
	private int money = 1000;

	public int increaserate = 20;

	public string debugmaxcount;

	public AIstate state;

	private GameObject World;

	public int NextBuy;

	private CrossLevelVariableHolder crosslevelvar;

	public int NextBuyCost;

	public FP timepassed;

	public TSRandom randominst;

	public RelayController InputRelay;

	public TSVector startpos;

	public int debugseed;

	public bool ismission;

	public MainMenuCampaignControlScript.mapcontainer mission;

	private List<int> shipstartspawn;

	private TSVector defaultspawnpos;

	private int shipsmissionspawned;

	private int spawnerhold;

	private int timelefttilorder;

	private InputRelay inputrelay;

	public int actualshiptobuy;

	private int lastbuy;

	public FP timepassedactual;

	private int[] agentsPerSide = new int[35];

	public void setmission(MainMenuCampaignControlScript.mapcontainer missioninput, TSVector defaultspawnposin)
	{
		ismission = true;
		mission = missioninput;
		shipstartspawn = mission.ships.ToList();
		if (shipstartspawn.Contains(0))
		{
			shipstartspawn.Remove(0);
		}
		defaultspawnpos = defaultspawnposin;
	}

	public void StartMain(int randominput)
	{
		int num = UnitMovementcommandcontroller.findspawnteamreverse(team);
		unitcontrol = transform.parent.GetComponent<UnitMovementcommandcontroller>();
		crosslevelvar = unitcontrol.crosslevelholder;
		increaserate = getaimoneyamount(crosslevelvar.botdifficulty, unitcontrol.moneyincreaserate).AsInt();
		if (unitcontrol.crosslevelholder.campaign && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive && unitcontrol.crosslevelholder.campaignlevel.name == "Final Assault")
		{
			increaserate = 80;
		}
		else if (unitcontrol.crosslevelholder.campaign && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
		{
			increaserate = 70;
		}
		World = GameObject.Find("World").transform.Find("Objects").gameObject;
		state = AIstate.attacking;
		debugseed = 5 + num + randominput;
		randominst = TSRandom.New(debugseed);
		startpos = new TSVector(0, 0, 0);
		spawnerhold = num;
	}

	public static FP getaimoneyamount(CrossLevelVariableHolder.BotDifficultyhol input, FP inputmon)
	{
		FP fP = 1;
		switch (input)
		{
		case CrossLevelVariableHolder.BotDifficultyhol.easy:
			fP = 0.6f;
			break;
		case CrossLevelVariableHolder.BotDifficultyhol.medium:
			fP = 0.9f;
			break;
		default:
			if (input == CrossLevelVariableHolder.BotDifficultyhol.medium)
			{
				fP = 1.4f;
			}
			break;
		}
		return fP * inputmon;
	}

	public void SpecUpdate()
	{
		timelefttilorder++;
		timepassedactual += TrueSyncManager.DeltaTime;
		if (timelefttilorder > 50 && ismission && PhotonNetwork.isMasterClient && shipstartspawn.Count > 0)
		{
			shipsmissionspawned++;
			TSVector tSVector = PositinonRelativeToHeadset.getspawnpos(team);
			if (crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
			{
				tSVector = startpos;
			}
			if (tSVector == new TSVector(0, 0, 0))
			{
				tSVector = defaultspawnpos;
			}
			if (InputRelay == null)
			{
				InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
			}
			if (InputRelay != null)
			{
				InputRelay.ordershipspawn(shipstartspawn[0], defaultspawnpos * 500 + new TSVector(0, 0, shipsmissionspawned * 1000), UnitMovementcommandcontroller.findspawnteamreverse(team), PhotonNetwork.AllocateViewID());
			}
			shipstartspawn.RemoveAt(0);
		}
	}

	public bool checkmoney(int spawner)
	{
		bool flag = false;
		GameObject gameObject = unitcontrol.getshipbynumber(actualshiptobuy);
		if ((bool)gameObject)
		{
			int pointsToDeploy = gameObject.GetComponent<_Ship>().PointsToDeploy;
			flag = ((pointsToDeploy <= money) ? true : false);
			int num = UnitMovementcommandcontroller.getmaxshipnumbers();
			int count = unitcontrol.teammembersout(team).Count;
			if (count >= num - 1)
			{
				flag = false;
			}
			if ((!crosslevelvar.campaign || (crosslevelvar.campaign && crosslevelvar.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive)) && unitcontrol.checkiffactiondead(team))
			{
				flag = false;
			}
			if (flag)
			{
				money -= pointsToDeploy;
			}
		}
		return flag;
	}

	private int getshiptobuy(int nextbuyinput)
	{
		int result = 1;
		if (NextBuy <= 5)
		{
			result = 1;
		}
		if (NextBuy > 5 && NextBuy <= 15)
		{
			result = 2;
		}
		if (NextBuy > 15 && NextBuy <= 25)
		{
			result = 3;
		}
		if (NextBuy > 25 && NextBuy <= 30)
		{
			result = 4;
		}
		if (NextBuy > 30 && NextBuy <= 40)
		{
			result = 5;
		}
		if (NextBuy > 40 && NextBuy <= 52)
		{
			result = 6;
		}
		if (NextBuy > 52 && NextBuy <= 64)
		{
			result = 7;
		}
		if (NextBuy > 64 && NextBuy <= 76)
		{
			result = 8;
		}
		if (NextBuy > 76 && NextBuy <= 82)
		{
			result = 9;
		}
		if (NextBuy > 82 && NextBuy <= 92)
		{
			result = 10;
		}
		if (NextBuy > 92 && NextBuy <= 97)
		{
			result = 11;
		}
		if (NextBuy > 97 && NextBuy <= 100)
		{
			result = 12;
		}
		if (NextBuy > 100)
		{
			result = lastbuy;
		}
		return result;
	}

	private void BuyShip()
	{
		addmoney(increaserate);
		if (actualshiptobuy == 0)
		{
			NextBuy = randominst.Next(0, 200);
			actualshiptobuy = getshiptobuy(NextBuy);
			if (unitcontrol.getshipbynumber(actualshiptobuy) != null)
			{
				NextBuyCost = unitcontrol.getshipbynumber(actualshiptobuy).GetComponent<_Ship>().PointsToDeploy;
			}
		}
		if (PhotonNetwork.isMasterClient && (bool)unitcontrol && checkmoney(money))
		{
			TSVector pos = startpos;
			if (unitcontrol.teammembersout(team).Count != 0)
			{
				pos = PositinonRelativeToHeadset.getspawnpos(team);
			}
			if (crosslevelvar.campaign && crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
			{
				pos = startpos;
			}
			if (InputRelay == null)
			{
				InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
			}
			if (InputRelay != null)
			{
				InputRelay.ordershipspawn(actualshiptobuy, pos, UnitMovementcommandcontroller.findspawnteamreverse(team), PhotonNetwork.AllocateViewID());
			}
			lastbuy = actualshiptobuy;
			actualshiptobuy = 0;
		}
	}

	public void addmoney(int amount)
	{
		if (money < 7000)
		{
			money += amount;
		}
	}

	public void takemoney(int amount)
	{
		money -= amount;
	}

	public int getmoney()
	{
		return money;
	}

	public void GiveOrderwait()
	{
		if (!(TrueSyncManager.Time > 0) || randominst == null)
		{
			return;
		}
		ships.Clear();
		ships = unitcontrol.teammembersout(team);
		List<TSTransform> list = unitcontrol.targetsout(team);
		List<GameObject> list2 = new List<GameObject>();
		List<TSTransform> list3 = new List<TSTransform>();
		foreach (GameObject ship in ships)
		{
			if (ship == null || ship.gameObject == null)
			{
				list2.Add(ship);
			}
		}
		foreach (GameObject item in list2)
		{
			ships.Remove(item);
		}
		foreach (TSTransform item2 in list)
		{
			if (item2 == null || item2.gameObject == null)
			{
				list3.Add(item2);
			}
		}
		foreach (TSTransform item3 in list3)
		{
			list.Remove(item3);
		}
		debugmaxcount = ships.Count + " out of " + UnitMovementcommandcontroller.getmaxshipnumbers();
		if (startpos == new TSVector(0, 0, 0) && ships.Count > 0)
		{
			startpos = ships[0].GetComponent<TSTransform>().position + new TSVector(1, 0, 0);
		}
		Target = getclosesttarget();
		if (((!ismission && ships.Count > 0) || (crosslevelvar.campaign && crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)) && (bool)unitcontrol)
		{
			BuyShip();
		}
		if (list.Count != 0)
		{
			enemyships = list.ToArray();
		}
		if (ships != null && ships.Count != 0 && ships.Count > 4)
		{
			state = AIstate.attacking;
		}
		if (ships != null && ships.Count != 0 && ships.Count <= 3)
		{
			state = AIstate.retreating;
		}
		if (ships != null && ships.Count != 0 && ships.Count > 2 && ships.Count < 4)
		{
			state = AIstate.maintain;
		}
		if (ships != null && ships.Count != 0 && state == AIstate.retreating && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) < 1000)
		{
			state = AIstate.holding;
		}
		if (ships != null && ships != null && ships.Count != 0 && (bool)ships[0] && Target != null && ships[0].gameObject != null && state == AIstate.holding && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, Target.GetComponent<TSTransform>().position) < 1000)
		{
			state = AIstate.attacking;
		}
		if (ships != null && ships.Count != 0 && state == AIstate.maintain && ships.Count > 0 && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) < 1000)
		{
			state = AIstate.retreating;
		}
		if (ships != null && ships.Count != 0 && state == AIstate.maintain && ships.Count > 0 && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) > 1000)
		{
			state = AIstate.attacking;
		}
		if (ismission && timepassedactual > 30)
		{
			state = AIstate.attacking;
		}
		if (ismission && timepassedactual < 30)
		{
			state = AIstate.holding;
		}
		if (ismission && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)
		{
			state = AIstate.attacking;
		}
		int num = 0;
		foreach (GameObject ship2 in ships)
		{
			if (Target != null && ship2.gameObject != null && state == AIstate.attacking)
			{
				ship2.GetComponent<_Ship>().asignMoveOrder(Target.GetComponent<TSTransform>().position, Target.gameObject, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), addtoorderlist: false);
			}
			if (ship2.gameObject != null && state == AIstate.retreating)
			{
				ship2.GetComponent<_Ship>().asignMoveOrder(TargetPosition(num, startpos, ships.Count), null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), addtoorderlist: false);
			}
			if (ship2.gameObject != null && state == AIstate.holding)
			{
				_Ship component = ship2.GetComponent<_Ship>();
				if (component.HullType == eHullType.Light)
				{
					component.asignMoveOrder(startpos, null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), addtoorderlist: false);
				}
				else
				{
					component.asignMoveOrder(ship2.GetComponent<TSTransform>().position, null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), addtoorderlist: false);
				}
			}
			num++;
		}
	}

	private TSTransform getclosesttarget()
	{
		int num = randominst.Next(0, 2);
		List<TSTransform> list = new List<TSTransform>();
		if (enemyships.Length != 0)
		{
			list = enemyships.ToList();
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
			if (list.Count != 0)
			{
				if (ships.Count > 0 && ships[0].gameObject != null)
				{
					list = list.OrderBy((TSTransform x) => TSVector.Distance(ships[0].GetComponent<TSTransform>().position, x.GetComponent<TSTransform>().position)).ToList();
				}
				return list[0];
			}
			return null;
		}
		return null;
	}

	private TSVector gettargetmove()
	{
		TSTransform tSTransform = getclosesttarget();
		if (tSTransform != null)
		{
			return tSTransform.position;
		}
		return new TSVector(0, 0, 0);
	}

	private TSVector TargetPosition(int index, TSVector sphere, int agentsnum)
	{
		if (agentsnum != 0)
		{
			int num = 150;
			agentsPerSide[index] = agentsnum / 3 + ((agentsnum % 3 > 0) ? 1 : 0);
			int num2 = agentsnum * 200;
			int num3 = index % 3;
			FP fP = (FP)(index / 3) / (FP)agentsPerSide[num3];
			fP = 1 - (fP - (int)(long)fP);
			FP fP2 = num2 / 2 * TSMath.Sqrt(3);
			if (index == 0)
			{
				return sphere;
			}
			return sphere + new TSVector(num * ((index % 2 != 0) ? 1 : (-1)) * ((index - 1) / 2 + 1), 0, num * ((index - 1) / 2 + 1));
		}
		return sphere;
	}
}
