using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Photon;

/// <summary>
/// This class controls the input for truesync and allows everyone to buy ships and give move orders.
/// </summary>
public class InputRelay : TrueSyncBehaviour
{
    // a stack of ship buy orders to give, only does one every input as to prevent the byte access point from overloading.
    private List<shipbuycontainer> shipstobuy = new List<shipbuycontainer>();
    // a stack of ship move orders to give, only does one every input as to prevent the byte access point from overloading.
    private List<ordermovecontainer> orderstodo = new List<ordermovecontainer>();
    // all AI controllers in the game.
    private List<AIController> aicontrollers = new List<AIController>();
    // Relay controller connected to the game controller to relay commands to this script.
    public RelayController manager;
    // the root parent to all ingame objects.
    private GameObject Objects;
    // the time since game started.
    public FP timepassed;
    // start simulation this input frame.
    public bool runsimulationnow;
    // is the simulation runnning?
    public bool running = false;
    // the PhotonView Connected to the game controller.
    PhotonView photonview;
    // the PhotonView Connected to this GameObject.
    PhotonView photonviewthis;
    // Time Passed since the game started.
    public FP timepassedmain;
    // is the truesync simluation running.
    private bool simrunning;

    /// <summary>
    /// Initialise everything.
    /// </summary>
    void Start()
    {
        Objects = GameObject.Find("Objects");
        if (GameObject.Find("JoinMultiplayer")) photonview = GameObject.Find("JoinMultiplayer").GetComponent<PhotonView>();
        photonviewthis = GetComponent<PhotonView>();
        photonviewthis.viewID = PhotonNetwork.AllocateViewID();
    }
   
    /// <summary>
    /// Update all the time functions.
    /// </summary>
    void Update()
    {
        timepassedmain = TrueSyncManager.Time;
        timepassed += Time.deltaTime;
        if (running) ActualGameTimePassed += TrueSyncManager.DeltaTime;
    }

    /// <summary>
    /// Add an order to the order stack.
    /// </summary>
    /// <param name="selectedshipsin">ships to move</param>
    /// <param name="positionin">position for ships to move to.</param>
    /// <param name="gaminput">target for ships to attack.</param>
    /// <param name="speedin">speed for ships to move at.</param>
    /// <param name="waypoints">is waypoint mode enable?</param>
    public void ordermove(List<GameObject> selectedshipsin, TSVector positionin, GameObject gaminput, FP speedin,bool waypoints)
    {
        orderstodo.Add(new ordermovecontainer(selectedshipsin,positionin,gaminput, speedin,waypoints));
    }

    /// <summary>
    /// Add a ship buy request to the ship buy request stack.
    /// </summary>
    /// <param name="spawnshipin">Ship to Spawn</param>
    /// <param name="spawnposin">Ship spawn location</param>
    /// <param name="spawnteamin">Ship team</param>
    /// <param name="ViewIDin">Ship ViewID</param>
    public void ordershipspawn(int spawnshipin, TSVector spawnposin, int spawnteamin, int ViewIDin)
    {
        shipstobuy.Add(new shipbuycontainer(spawnshipin, spawnposin, spawnteamin, ViewIDin));
        Debug.Log(spawnshipin);
    }

    /// <summary>
    /// Do a deterministic Start to the Game.
    /// </summary>
    void managerstart()
    {
        TSRandom.instance = TSRandom.New(5);
        unitcon = GameObject.Find("Controllers");
        unitcomscript = unitcon.GetComponent<UnitMovementcommandcontroller>();
        unitcomscript.StartMain();
        foreach (Transform gam in unitcon.transform) if (gam.GetComponent<AIController>() != null) aicontrollers.Add(gam.GetComponent<AIController>());
        foreach (Transform t in Objects.transform)
            if (t.name != "Working" && t.name != "Engines") t.GetComponent<_Ship>().StartMain();
    }

    /// <summary>
    /// Run 90 times a second to accept input from the player and reduce the build and move order stack.
    /// </summary>
    public override void OnSyncedInput()
    {

        if (running) foreach (_Ship ship in unitcomscript.allshipsscript) if (ship && ship.gameObject != null) ship.Update();
        if (shipstobuy.Count == 0 && orderstodo.Count == 0) TrueSyncInput.SetInt(0, 10);
        if (manager == null || (manager.relay == null && unitcomscript != null))
        {
            if (unitcomscript)
            {
                manager = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
                unitcomscript.output = true;
                manager.relay = this;
                Debug.Log(manager.relay);
            }

        }
        if (unitcomscript) unitcomscript.output = true;
        else unitcomscript = unitcomscript = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
        if (shipstobuy.Count > 0)
        {
            TrueSyncInput.SetInt(0, 1);
            TrueSyncInput.SetInt(1, shipstobuy[0].spawnship);
            TrueSyncInput.SetTSVector(2, shipstobuy[0].spawnpos);
            TrueSyncInput.SetInt(3, shipstobuy[0].spawnteam);
            TrueSyncInput.SetInt(4, shipstobuy[0].viewID);
            Debug.Log(shipstobuy[0].spawnship + " " + shipstobuy[0].spawnpos + " " + shipstobuy[0].viewID);
            shipstobuy.RemoveAt(0);
        }
        else TrueSyncInput.SetInt(1, 0);

        if (orderstodo.Count > 0)
        {
            int i = 2;
            TrueSyncInput.SetInt((byte)0, 0);
            //  i++;
            foreach (GameObject gam in orderstodo[0].selectedships)
            {
                if (orderstodo[0].targethold == null && gam.gameObject != null)
                {
                    PhotonView targetview = gam.GetComponent<PhotonView>();
                    TrueSyncInput.SetInt((byte)i, targetview.viewID);
                    i++;
                    TrueSyncInput.SetTSVector((byte)i, TargetPosition((i / 2), GameObject.Find("World").transform.Find("Objects").InverseTransformPoint(orderstodo[0].targetpos.ToVector()).ToTSVector(), orderstodo[0].selectedships.Count)); 
                    i++;
                    TrueSyncInput.SetInt((byte)i, 3000);
                    i++;
                    TrueSyncInput.SetFP((byte)i, orderstodo[0].averagespeed);
                    i++;
                    TrueSyncInput.SetBool((byte)i, orderstodo[0].waypoints);
                    i++;
                }
                else if (gam.gameObject != null)
                {
                    PhotonView targetview = gam.GetComponent<PhotonView>();
                    TrueSyncInput.SetInt((byte)i, targetview.viewID);
                    i++;
                    TrueSyncInput.SetTSVector((byte)i, TargetPosition((i / 2), GameObject.Find("World").transform.Find("Objects").InverseTransformPoint(orderstodo[0].targetpos.ToVector()).ToTSVector(), orderstodo[0].selectedships.Count));
                    i++;
                    TrueSyncInput.SetInt((byte)i, orderstodo[0].targethold.GetComponent<PhotonView>().viewID);
                    i++;
                    TrueSyncInput.SetFP((byte)i, orderstodo[0].averagespeed);
                    i++;
                    TrueSyncInput.SetBool((byte)i, orderstodo[0].waypoints);
                    i++;
                }
            }
            TrueSyncInput.SetInt(1, i);
            orderstodo.RemoveAt(0);
        }

        if (runsimulationnow)
        {
            TrueSyncInput.SetInt(100, 1);
            runsimulationnow = false;
        }

    }

    /// <summary>
    /// The function to recieve and act upon input given in the onsyncedInput function.
    /// </summary>
    public override void OnSyncedUpdate()
    {
        if (running && unitcomscript.output == true)
        {
            foreach (AIController gam in aicontrollers) gam.SpecUpdate();
        }
        if(unitcomscript) unitcomscript.Timeleft -= TrueSyncManager.DeltaTime;
        if (TrueSyncInput.GetInt(100) == 1)
        {
            if (running == false)
            {
                Debug.Log("start");
               // running = true;
                managerstart();
            }
        }
        int photon = 3000;
        TSVector pos = new TSVector(0, 0, 0);
        int target = 3000;
        FP averagespeed = 1;
        int check = TrueSyncInput.GetInt(0);
        bool waypoints = false;
        if (check == 0)
        {
            int limit = TrueSyncInput.GetInt(1);
            if (limit > 0)
            {
                for (int a = 2; a < limit;)
                {
                    photon = TrueSyncInput.GetInt((byte)a);
                    a++;
                    Debug.Log(photon + " out of " + a); 
                    pos = TrueSyncInput.GetTSVector((byte)a);
                    a++;
                    target = TrueSyncInput.GetInt((byte)a);
                    a++;
                    averagespeed = TrueSyncInput.GetFP((byte)a);
                    a++;
                    waypoints = TrueSyncInput.GetBool((byte)a);
                    a++;
                    if (photon != 3000)
                    {
                        GameObject gam = null;
                        if (PhotonView.Find(photon)) gam = PhotonView.Find(photon).gameObject;
                        GameObject targetgam = null;
                        if (target != 3000 && PhotonView.Find(target) != null) targetgam = PhotonView.Find(target).gameObject;
                        if (gam) gam.GetComponent<_Ship>().asignMoveOrder(pos, targetgam, averagespeed, waypoints);
                    }

                }

            }
        }
        else if (check == 1)
        {
            int b = TrueSyncInput.GetInt(1);
            if (b != 0) spawnshipfun(b, TrueSyncInput.GetTSVector(2), TrueSyncInput.GetInt(3), TrueSyncInput.GetInt(4));
        }
        
    }

    /// <summary>
    /// Take an input and use it to spawn a ship
    /// </summary>
    public void spawnshipfun(int a, TSVector pos, int oriplayer, int viewidin)
    {
        GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().spawnship(a, pos, oriplayer, viewidin);
    }

    /// <summary>
    /// Take an input of a position and create a delta formation around it to avoid ship clumping.
    /// </summary>
    private int[] agentsPerSide = new int[20];
    private TSVector TargetPosition(int index, TSVector sphere, int agentsnum)
    {
        if (agentsnum != 0 && agentsPerSide.Length != 0 && index < agentsPerSide.Length)
        {
            var separation = 100;
            agentsPerSide[index] = agentsnum / 3 + (agentsnum % 3 > 0 ? 1 : 0);
            var length = agentsnum * 200;
            var side = index % 3;
            var lengthMultiplier = (index / 3) / (float)agentsPerSide[side];
            lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);
            var height = length / 2 * Mathf.Sqrt(3); // Equilaterial triangle height
            if (index == 0) return sphere;
            else return sphere + new TSVector(separation * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, separation * (((index - 1) / 2) + 1));
        }
        else return sphere;
    }

    /// <summary>
    /// The class used to store ship buy orders.
    /// </summary>
    private class shipbuycontainer  {

        public int spawnship;
        public TSVector spawnpos;
        public int spawnteam;
        public int viewID;
        public shipbuycontainer(int spawnshipin, TSVector spawnposin, int spawnteamin,int viewIDin)
        {
            spawnship = spawnshipin;
            spawnpos = spawnposin;
            spawnteam = spawnteamin;
            viewID = viewIDin;
        }
    }

    /// <summary>
    /// the class used to contain ship move orders.
    /// </summary>
    private class ordermovecontainer {
        public List<GameObject> selectedships;
        public TSVector targetpos;
        public GameObject targethold;
        public FP averagespeed;
        public bool waypoints;
        public ordermovecontainer(List<GameObject> selectedshipsin,TSVector targetposin,GameObject targetholdin, FP averagespeedin,bool waypointsin)
        {
            selectedships = selectedshipsin;
            targethold = targetholdin;
            targetpos = targetposin;
            averagespeed = averagespeedin;
            waypoints = waypointsin;
        }

    }
}
