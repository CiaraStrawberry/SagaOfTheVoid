using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TrueSync;
using DFNetwork.Simulation.Modules;

public class AIController : MonoBehaviour {
    public _Ship.eShipColor team;
    public List<GameObject> ships = new List<GameObject>();
    public TSTransform[] enemyships = new TSTransform[0];
    private UnitMovementcommandcontroller unitcontrol;
    public TSTransform Target;
    [SerializeField]
    private int money = 1000;
    public int increaserate = 20;
    public string debugmaxcount;
    public enum AIstate
    {
        attacking,
        retreating,
        holding,
        maintain
    }
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
    // Use this for initialization
    public bool ismission;
    public MainMenuCampaignControlScript.mapcontainer mission;
    private List<int> shipstartspawn;
    private TSVector defaultspawnpos;
    InputRelay inputrelay;
    public int actualshiptobuy;
    private int lastbuy;
    public FP timepassedactual;
    private int spawnerhold;
    private int timelefttilorder;
    int shipsmissionspawned = 0;

    /// <summary>
    // tells the AI wether to use any mission paramiters at the start of a match.
    /// <summary>
    public void setmission (MainMenuCampaignControlScript.mapcontainer missioninput,TSVector defaultspawnposin)
    {
        ismission = true;
        mission = missioninput;
        shipstartspawn = mission.ships.ToList();
        if (shipstartspawn.Contains(0)) shipstartspawn.Remove(0);
        defaultspawnpos = defaultspawnposin;
    }

    /// <summary>
    // initializes deterministic start.
    /// <summary>
    public void StartMain( int randominput) {
        int a = UnitMovementcommandcontroller.findspawnteamreverse(team);
        unitcontrol = transform.parent.GetComponent<UnitMovementcommandcontroller>();
        crosslevelvar = unitcontrol.crosslevelholder;
        increaserate = getaimoneyamount(crosslevelvar.botdifficulty, unitcontrol.moneyincreaserate).AsInt();
        if (unitcontrol.crosslevelholder.campaign == true && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive && unitcontrol.crosslevelholder.campaignlevel.name == "Final Assault") increaserate = 80;
        else if (unitcontrol.crosslevelholder.campaign == true && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) increaserate = 70;
        World = GameObject.Find("World").transform.Find("Objects").gameObject;
        state = AIstate.attacking;
        debugseed = 5 + a + randominput;
        randominst = TSRandom.New(debugseed);
        startpos = new TSVector(0, 0, 0);
        spawnerhold = a;
    }

    /// <summary>
    // get the increase of money for the AI based on difficulty.
    /// <summary>
    public static FP getaimoneyamount (CrossLevelVariableHolder.BotDifficultyhol input, FP inputmon)
    {
        FP modifier = 1;
        if (input == CrossLevelVariableHolder.BotDifficultyhol.easy) modifier = 0.6f;
        else  if (input == CrossLevelVariableHolder.BotDifficultyhol.medium) modifier = 0.9f;
        else if (input == CrossLevelVariableHolder.BotDifficultyhol.medium) modifier = 1.4f;
        FP output = modifier * inputmon;
        return output;
    }

    /// <summary>
    // Deterministic Update
    /// <summary>
    public void SpecUpdate() {
        
        timelefttilorder++;
        timepassedactual += TrueSyncManager.DeltaTime;
        if (timelefttilorder > 50 &&ismission && PhotonNetwork.isMasterClient && shipstartspawn.Count > 0 )
        {
            shipsmissionspawned++;
             TSVector spawnpos = PositinonRelativeToHeadset.getspawnpos(team);
            if (crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) spawnpos = startpos;
             if (spawnpos == new TSVector(0, 0, 0)) spawnpos = defaultspawnpos;
             if (InputRelay == null) InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
             if (InputRelay != null) InputRelay.ordershipspawn(shipstartspawn[0], (defaultspawnpos * 500) + new TSVector(0, 0, shipsmissionspawned * 1000), UnitMovementcommandcontroller.findspawnteamreverse(team), PhotonNetwork.AllocateViewID());
             shipstartspawn.RemoveAt(0);
        }
    }




    /// <summary>
    // checks if the bot has enough money and is allowed to spawn ship.
    /// <summary>
    public bool checkmoney(int spawner)
    {
        bool output = false;
        GameObject temp = unitcontrol.getshipbynumber(actualshiptobuy);
        if (temp)
        {
            int pointstodeploy = temp.GetComponent<_Ship>().PointsToDeploy;
            if (pointstodeploy <= money) output = true;
            else output = false;
            int maxships = UnitMovementcommandcontroller.getmaxshipnumbers();
            int currentships = unitcontrol.teammembersout(team).Count;
            if (currentships >= (maxships - 1)) output = false;
            if ((crosslevelvar.campaign == false || (crosslevelvar.campaign == true && crosslevelvar.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive)) && unitcontrol.checkiffactiondead(team) == true) output = false;
            if (output) money -= pointstodeploy;
        }
        return output;
    }



    /// <summary>
    // chooses ship to buy based on deterministic random number
    /// <summary>
    int getshiptobuy (int nextbuyinput)
    {
        int output = 1;
        if (NextBuy <= 5) output = 1;
        if (NextBuy > 5 && NextBuy <= 15) output = 2;
        if (NextBuy > 15 && NextBuy <= 25) output = 3;
        if (NextBuy > 25 && NextBuy <= 30) output = 4;
        if (NextBuy > 30 && NextBuy <= 40) output = 5;
        if (NextBuy > 40 && NextBuy <= 52) output = 6;
        if (NextBuy > 52 && NextBuy <= 64) output = 7;
        if (NextBuy > 64 && NextBuy <= 76) output = 8;
        if (NextBuy > 76 && NextBuy <= 82) output = 9;
        if (NextBuy > 82 && NextBuy <= 92) output = 10;
        if (NextBuy > 92 && NextBuy <= 97) output = 11;
        if (NextBuy > 97 && NextBuy <= 100) output = 12;
        if (NextBuy > 100) output = lastbuy;
        return output;
    }

    /// <summary>
    // checks if it can buy ship and if so, buys ship.
    /// <summary>
    void BuyShip ()
    {
        addmoney(increaserate);

        if (actualshiptobuy == 0)
        {
            NextBuy = randominst.Next(0, 200);
            actualshiptobuy = getshiptobuy(NextBuy);
            if (unitcontrol.getshipbynumber(actualshiptobuy) != null) NextBuyCost = unitcontrol.getshipbynumber(actualshiptobuy).GetComponent<_Ship>().PointsToDeploy;
        }

        if (PhotonNetwork.isMasterClient && unitcontrol && checkmoney(money))
        {
            TSVector spawnpos = startpos;
          if(unitcontrol.teammembersout(team).Count != 0)  spawnpos = PositinonRelativeToHeadset.getspawnpos(team);
            if (crosslevelvar.campaign == true && crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) spawnpos = startpos;
            if (InputRelay == null) InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
            if (InputRelay != null) InputRelay.ordershipspawn(actualshiptobuy, spawnpos , UnitMovementcommandcontroller.findspawnteamreverse(team), PhotonNetwork.AllocateViewID());
            lastbuy = actualshiptobuy;
            actualshiptobuy = 0;
        }

    }

    /// <summary>
    // gets, adds and takes away money from AIs bank.
    /// <summary>
    public void addmoney (int amount){ if(money< 7000) money += amount;  }
    
    public void takemoney (int amount){ money -= amount;}
    /// <summary>
    /// get current classes money
    /// <summary>
    public int getmoney () { return money; }

    /// <summary>
    // called every second to update the AIs orders to adapt to the situation.
    /// <summary>
    public void GiveOrderwait()
    {
        if (TrueSyncManager.Time > 0 && randominst != null)
        {
            ships.Clear();
            ships = unitcontrol.teammembersout(team);
            List<TSTransform> temp = unitcontrol.targetsout(team);
            List<GameObject> removableships = new List<GameObject>();
            List<TSTransform> enemyremovable = new List<TSTransform>();
            foreach (GameObject ship in ships) if (ship == null || ship.gameObject == null) removableships.Add(ship);
            foreach (GameObject remov in removableships) ships.Remove(remov);
            foreach (TSTransform ship in temp) if (ship == null || ship.gameObject == null) enemyremovable.Add(ship);
            foreach (TSTransform remov in enemyremovable) temp.Remove(remov);
            debugmaxcount = ships.Count.ToString() + " out of " + UnitMovementcommandcontroller.getmaxshipnumbers().ToString();
            if (startpos == new TSVector(0, 0, 0) && ships.Count > 0) startpos = ships[0].GetComponent<TSTransform>().position + new TSVector(1, 0, 0);
            Target = getclosesttarget();

            if (((ismission == false  && ships.Count > 0)|| (crosslevelvar.campaign == true && crosslevelvar.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)) && unitcontrol) BuyShip();

            if (temp.Count != 0) enemyships = temp.ToArray();
            if (ships != null && ships.Count != 0 && ships.Count > 4) state = AIstate.attacking;
            if (ships != null && ships.Count != 0 && ships.Count <= 3) state = AIstate.retreating;
            if (ships != null && ships.Count != 0 && ships.Count > 2 && ships.Count < 4) state = AIstate.maintain;
            if (ships != null && ships.Count != 0 && state == AIstate.retreating && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) < 1000) state = AIstate.holding;
            if (ships != null && ships != null && ships.Count != 0 && ships[0] && Target != null && ships[0].gameObject != null && state == AIstate.holding && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, Target.GetComponent<TSTransform>().position) < 1000) state = AIstate.attacking;
            if (ships != null && ships.Count != 0 && state == AIstate.maintain && ships.Count > 0 && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) < 1000) state = AIstate.retreating;
            if (ships != null && ships.Count != 0 && state == AIstate.maintain && ships.Count > 0 && TSVector.Distance(ships[0].GetComponent<TSTransform>().position, startpos) > 1000) state = AIstate.attacking;
            if (ismission && timepassedactual > 30) state = AIstate.attacking;
            if (ismission && timepassedactual < 30) state = AIstate.holding;
            if(ismission && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) state = AIstate.attacking;
            int i = 0;

            foreach (GameObject ship in ships)
            {
                if (Target != null && ship.gameObject != null && state == AIstate.attacking) { ship.GetComponent<_Ship>().asignMoveOrder(Target.GetComponent<TSTransform>().position, Target.gameObject, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), false); }
                if (ship.gameObject != null && state == AIstate.retreating) { ship.GetComponent<_Ship>().asignMoveOrder(TargetPosition(i, startpos, ships.Count), null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), false); }
                if (ship.gameObject != null && state == AIstate.holding)
                {
                    _Ship shipscript = ship.GetComponent<_Ship>();
                    if (shipscript.HullType == eHullType.Light) shipscript.asignMoveOrder(startpos, null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), false);
                    else shipscript.asignMoveOrder(ship.GetComponent<TSTransform>().position, null, RightHand_triggerInstantOrder.calculate_average_speed(ships.ToArray()), false);
                }
                i++;
            }
        }


    }

    /// <summary>
    // get closest targfet to main ship to give attack order at.
    /// <summary>
    TSTransform getclosesttarget()
    {
        int randomtemp = randominst.Next(0, 2);
        List<TSTransform> output = new List<TSTransform>();
        if (enemyships.Length != 0) output = enemyships.ToList();
        if (output.Count != 0)
        {
            List<TSTransform> removable = new List<TSTransform>();
            foreach (TSTransform gam in output)  if (gam == null || gam.gameObject == null) removable.Add(gam);
            foreach (TSTransform gam in removable) output.Remove(gam);
            if (output.Count != 0)
            {
                if (ships.Count > 0 && ships[0].gameObject != null) output = output.OrderBy(x => TSVector.Distance(ships[0].GetComponent<TSTransform>().position, x.GetComponent<TSTransform>().position)).ToList();
                return output[0];
            }
            else return null;
        }
        else return null; 
    }

    /// <summary>
    // get the target ship to attack.
    /// <summary>
    TSVector gettargetmove()
    {
        TSTransform gam = getclosesttarget();
        if (gam != null) return gam.position;
        else return new TSVector(0,0,0);
    }

    /// <summary>
    // if not attacking ship, get positions around target movement position
    /// <summary>
    private int[] agentsPerSide = new int[35];
    private TSVector TargetPosition(int index, TSVector sphere, int agentsnum)
    {
        if (agentsnum != 0)
        {
            int separation = 150;
             agentsPerSide[index] = agentsnum / 3 + (agentsnum % 3 > 0 ? 1 : 0);
            int length = agentsnum * 200;
            int side = index % 3;
            FP lengthMultiplier = (index / 3) / (FP)agentsPerSide[side];
            lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);
            FP height = length / 2 * TSMath.Sqrt(3); // Equilaterial triangle height
            if (index == 0) return sphere;
            else return sphere + new TSVector(separation * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, separation * (((index - 1) / 2) + 1));
        }
        else return  sphere;
    }
}
