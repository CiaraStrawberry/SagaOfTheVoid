using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Photon;

public class InputRelay : TrueSyncBehaviour
{
    private List<shipbuycontainer> shipstobuy = new List<shipbuycontainer>();
    private List<ordermovecontainer> orderstodo = new List<ordermovecontainer>();
    private List<AIController> aicontrollers = new List<AIController>();
    // Use this for initialization
    void Start()
    {
        Objects = GameObject.Find("Objects");
       if(GameObject.Find("JoinMultiplayer")) photonview = GameObject.Find("JoinMultiplayer").GetComponent<PhotonView>();
        photonviewthis = GetComponent<PhotonView>();
        photonviewthis.viewID = PhotonNetwork.AllocateViewID();
       // TrueSyncManager.RunSimulation();
    }
    void OnJoinedRoom()
    {
       // TrueSyncManager.RunSimulation();
    }
   // public bool output;
    public RelayController manager;
    private GameObject Objects;
    public FP timepassed;
    public bool runsimulationnow;
    public bool running = false;
    PhotonView photonview;
    PhotonView photonviewthis;
    public FP timepassedmain;
    private bool simrunning;
    public FP ActualGameTimePassed;
    [PunRPC]
    public void runsim ()
    {
       // TrueSyncManager.RunSimulation();
    }
    void Update()
    {




        timepassedmain = TrueSyncManager.Time;
        timepassed += Time.deltaTime;
        if (running) ActualGameTimePassed += TrueSyncManager.DeltaTime;
        
     
      //  if (timepassed > 15 && running == false && PhotonNetwork.isMasterClient) runsimulationnow = true;
        if (exacttime != 0 && ActualGameTimePassed > exacttime)
        {
            if (temptarget)
            {
                _Ship tempship = temptarget.GetComponent<_Ship>();
                //  Debug.Log(Vector3.Distance(tempship.GetComponent<TSTransform>().position.ToVector(), pos));
                if (Quaternion.Angle(tempship.transform.rotation, targetrot) > 15) tempship.transform.rotation = targetrot;
                if (Vector3.Distance(tempship.GetComponent<TSTransform>().position.ToVector(), pos) > 200) tempship.GetComponent<TSTransform>().position = pos.ToTSVector();
                tempship.Armor = Armour;
                tempship.Shield = Shields;
                Debug.Log("check");
            }
            exacttime = 0;
        }
      
    }
    public void ordermove(List<GameObject> selectedshipsin, TSVector positionin, GameObject gaminput, FP speedin,bool waypoints)
    {
        orderstodo.Add(new ordermovecontainer(selectedshipsin,positionin,gaminput, speedin,waypoints));
    }
    public void ordershipspawn(int spawnshipin, TSVector spawnposin, int spawnteamin, int ViewIDin)
    {
        shipstobuy.Add(new shipbuycontainer(spawnshipin, spawnposin, spawnteamin, ViewIDin));
        Debug.Log(spawnshipin);
    }
    public void CheckValues()
    {
        foreach (Transform temp in Objects.transform)
        {

            if (temp.name != "Working" && temp.name != "Engines")
            {
                _Ship tempship = temp.GetComponent<_Ship>();
                photonviewthis.RpcSecure("CheckValuesClient", PhotonTargets.AllViaServer, true, temp.GetComponent<PhotonView>().viewID, ActualGameTimePassed.AsFloat() + 0.5f, temp.transform.GetComponent<TSTransform>().position.ToVector(), temp.transform.rotation, tempship.Armor, tempship.Shield);
            }

        }
    }
    public float addstuff(float a, float b)
    {
        return a + b;
    }
    private GameObject temptarget;
    private float exacttime;
    private Vector3 pos;
    private Quaternion targetrot;
    private int Armour;
    private int Shields;
    private GameObject unitcon;
    private UnitMovementcommandcontroller unitcomscript;
    void managerstart()
    {
        //    TrueSyncManager.Time = 0;
        TSRandom.instance = TSRandom.New(5);
        unitcon = GameObject.Find("Controllers");
        unitcomscript = unitcon.GetComponent<UnitMovementcommandcontroller>();
        unitcomscript.StartMain();
        foreach (Transform gam in unitcon.transform) if (gam.GetComponent<AIController>() != null) aicontrollers.Add(gam.GetComponent<AIController>());
    
        foreach (Transform t in Objects.transform)
        {
            if (t.name != "Working" && t.name != "Engines")
            {
                t.GetComponent<_Ship>().StartMain();
            }
        }
    }
    public override void OnSyncedInput()
    {

        if (running) foreach (_Ship ship in unitcomscript.allshipsscript) if (ship && ship.gameObject != null) ship.Update();
        if (shipstobuy.Count == 0 && orderstodo.Count == 0) TrueSyncInput.SetInt(0, 10);
        if (manager == null || (manager.relay == null && unitcomscript != null))
        {
            if(unitcomscript)
            {
                manager = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
                unitcomscript.output = true;
               manager.relay = this;
               Debug.Log(manager.relay);
            }
         
        }
       if(unitcomscript) unitcomscript.output = true;
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
        else TrueSyncInput.SetInt(1,0);

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
                  //  Debug.Log(orderstodo[0].targetpos + " " +  TargetPosition((i / 2), GameObject.Find("World").transform.Find("Objects").InverseTransformPoint(orderstodo[0].targetpos.ToVector()).ToTSVector(), orderstodo[0].selectedships.Count));
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

    // Update is called once per frame
    public override void OnSyncedUpdate()
    {
        if (running && unitcomscript.output == true)
        {
           // foreach (_Ship ship in unitcomscript.allshipsscript) if(ship && ship.gameObject != null) ship.SpecUpdate();
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
    public List<_Ship> allships = new List<_Ship>();
    //void checkallships ()
   // {
   //     foreach (_Ship ship in allships) if (ship.gameObject != null) ship.SpecUpdate(); 
   // }
    public void spawnshipfun(int a, TSVector pos, int oriplayer, int viewidin)
    {
        GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().spawnship(a, pos, oriplayer, viewidin);
    }
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
