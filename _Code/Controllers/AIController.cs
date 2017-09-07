using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TrueSync;
using DFNetwork.Simulation.Modules;

/// <summary>
/// This class is the default class to control a team of ships in the game, 1 per team.
/// </summary>
public class AIController : MonoBehaviour {
    // eShipColor enum dictating the colour and team of the ship
    public _Ship.eShipColor team;
    // all ships connected to this controller.
    public List<GameObject> ships = new List<GameObject>();
    // all List of all ships the AI controllers team can target.
    public TSTransform[] enemyships = new TSTransform[0];
    // Cached Game Controller
    private UnitMovementcommandcontroller unitcontrol;
    // Current Target being tracked.
    public TSTransform Target;
    // Current money held.
    [SerializeField]
    private int money = 1000;
    // The amount the money amount increases every second.
    public int increaserate = 20;
    // Current AIState state.
    public AIstate state;
    // Current World Root GameObject.
    private GameObject World;
    // The number between 0 and 200 to determin which ship is spawned.
    public int NextBuy;
    // The CrossLevelVariableHolder Singleton in use currently, holds things like map type and gamemode.
    private CrossLevelVariableHolder crosslevelvar;
    // The Cost of the next Ship Planned to be bought.
    public int NextBuyCost;
    // FP is a deterministic float, this tracks the amount of time since the start of the game, probs should have kept this in a static variable.
    public FP timepassed;
    // The Random number class kept to produce random deterministic floats for all players.
    public TSRandom randominst;
    // The Relay Controller, this is the thing you Send Ship buy requests and move orders to, to get them sent to all players.
    public RelayController InputRelay;
    // The Starting position for the Team, used as a spot to fall back to.
    public TSVector startpos;
    // The networked integer used as a seed to ensure the Random number generators for all clients change every match, but stay the same for all players.
    public int debugseed;
    // The bool to dermin if the match is a campaign mission, and if so can referance the stored mission in crosslevelvariableholder.
    public bool ismission;
    // The class containing relevant data for the current mission
    public MainMenuCampaignControlScript.mapcontainer mission;
    // The List of ships that the AI starts off with.
    private List<int> shipstartspawn;
    // The Spawning Position of the AI.
    private TSVector defaultspawnpos;
    // The integer corseponding to the next Ship to be bought, this should probably have been an enum
    public int actualshiptobuy;
    // The last ship bought.
    private int lastbuy;
    // THe time passed since the "start of the match", i use a different simulation frame to truesync, due to an inability to start the match in the same way across clients.
    public FP timepassedactual;
    // the alloted spawn team for this AI.
    private int spawnerhold;
    // time left til next order to be sent out.
    private int timelefttilorder;
    // ships spawned so far.
    int shipsmissionspawned = 0;

    // The enum to hold the current state of the AI.
    public enum AIstate
    {
        attacking,
        retreating,
        holding,
        maintain
    }

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
    /// checks if the bot has enough money and is allowed to spawn ship.
    /// </summary>
    /// <param name="spawner"> the unit number corresponding to the desired ship to spawn</param>
    /// <returns>do you have enough money?</returns>
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
    public void GiveOrderwait(int waittime)
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
    /// if not attacking ship, get positions around target movement position
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
