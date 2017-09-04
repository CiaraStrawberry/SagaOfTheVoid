using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.Steering;
using System.Linq;
using TrueSync;
using simple;
using UnityEngine.SceneManagement;


public class UnitMovementcommandcontroller : TrueSyncBehaviour {
    // TODO: reorganise variables
    public PlayerPrefabVoiceControlScript.VoiceChannel curchannel;
    public _Ship.eShipColor team;
    [SerializeField]
    private GameObject[] Blue = new GameObject[5];
    [SerializeField]
    private GameObject[] Green = new GameObject[5];
    [SerializeField]
    private GameObject[] Grey  = new GameObject[5];
    [SerializeField]
    private GameObject[] Red  = new GameObject[5];
    [SerializeField]
    private GameObject[] White = new GameObject[5];
    [SerializeField]
    private GameObject[] Yellow = new GameObject[5];
    [SerializeField]
    private TSTransform[] BlueTargets = new TSTransform[0];
    [SerializeField]
    private TSTransform[] GreenTargets = new TSTransform[0];
    [SerializeField]
    private TSTransform[] GreyTargets = new TSTransform[0];
    [SerializeField]
    private TSTransform[] RedTargets = new TSTransform[0];
    [SerializeField]
    private TSTransform[] WhiteTargets = new TSTransform[0];
    [SerializeField]
    private TSTransform[] YellowTargets = new TSTransform[0];
    public bool output;
    public List<GameObject> all_ships = new List<GameObject>();
    public List<_Ship> all_shipsScript = new List<_Ship>();
    private FP timepassed;
    public  FP Timeleft = 600;
    public CrossLevelVariableHolder.gamemode Gamemode;
    public CrossLevelVariableHolder crosslevelholder;
    public PhotonView phopview;
    public GameObject Team1victory;
    public GameObject Team2Victory;
    private List<GameObject> Team1Capitals = new List<GameObject>();
    private List<GameObject> Team2Capitals = new List<GameObject>();
    public GameObject map1area;
    public GameObject map2area;
    public GameObject map3area;
    [SerializeField]
    private GameObject Objectsholder;
    [SerializeField]
    private int money = 1500;
    public GameObject Loading;
    private TutorialTextControlScript tutcontrol;
    public GameObject[] tutorialIcons;
    AudioSource audiosource;
    List<playerisreadychecker> players = new List<playerisreadychecker>();
    public bool running;
    public int randomnumber;
    public List<_Ship> allshipsscript = new List<_Ship>();
    public List<GameObject> allshipsgams = new List<GameObject>();
    public List<AIController> aicontrollers = new List<AIController>();
    public TSTransform[] allshipststransform = new TSTransform[0];
    public int moneyincreaserate = 30;
    public bool started;
    public bool isalreadywon;
    private IOComponent FleetBuilder;
    public bool endall;
    bool victorytest;
    public GameObject hostlefticon;
    private bool simrunning;
    public GameObject debug;

    void Awake () {
        crosslevelholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        if(crosslevelholder.tutorial == true) tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
        team = crosslevelholder.findspawnteam(PhotonNetwork.player.ID);      
        Gamemode = crosslevelholder.Gamemode;
        //Setup Voice Channel
        if (SceneManager.GetActiveScene().name != "mptest")  curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        else
        {
            if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip) curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
            else curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        }
        map1area.SetActive(false);
        map2area.SetActive(false);
        map3area.SetActive(false);
        // Sets all scenery to not render, then enables the one selected in the lobby
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map1) map1area.SetActive(true);
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map2) map2area.SetActive(true);
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map3) map3area.SetActive(true);

       
    }
  
    // Start Function, Just Sets EveryThning up.
    void Start ()
    {
        //initialize
        tutorialIcons = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.ToArray();
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
        Timeleft = 1200;
        if (crosslevelholder.campaign == true && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) Timeleft = 300;
        phopview = GetComponent<PhotonView>();
        Objectsholder = GameObject.Find("Objects");
        RenderSettings.skybox = crosslevelholder.getskybox(crosslevelholder.skybox);
        audiosource =  GetComponent<AudioSource>();
        audiosource.clip = crosslevelholder.getmusic();
        audiosource.loop = true;
        audiosource.volume = 0.25f;
        audiosource.Play();
        if(PhotonNetwork.isMasterClient)   phopview.RPC("setrandomnumberforthismatch", PhotonTargets.All, Random.Range(0, 100));
        if (crosslevelholder.campaign == false) money = 1500;
        else money = crosslevelholder.campaignlevel.startmoney;
        if(PhotonNetwork.isMasterClient)
        {
            foreach(PhotonPlayer player in PhotonNetwork.otherPlayers)  players.Add(new playerisreadychecker(player.NickName, false,player.ID));
            players.Add(new playerisreadychecker(PhotonNetwork.player.NickName, true, PhotonNetwork.player.ID));
        }
        foreach (GameObject tutorial in tutorialIcons) tutorial.SetActive(false);
    }

    // This function is called across the network to allow the AI to use an actually random number, but one which is the same across all clients.
    [PunRPC] 
    void setrandomnumberforthismatch (int randomnum)  {   randomnumber = randomnum;    }
  
    // This function is called once the simulation starts running, it doesnt use the synced start call because my simulation ignores it and starts the simulation itself, why i wrote it like this is beyond me.
    public void StartMain ()
    {
        InputRelay.relay.running = true;
        Debug.Log("Invoke");
        foreach (Transform gam in this.transform) if (gam.GetComponent<AIController>() != null) aicontrollers.Add(gam.GetComponent<AIController>());
        foreach (Transform gam in transform)  if (gam.GetComponent<AIController>() != null) gam.GetComponent<AIController>().StartMain(randomnumber);
        running = true;
        TrueSyncManager.SyncedStartCoroutine(waaait());
        OneSecond();
    }

    // this is a one second wait function that is designed to allow call all other 1 second repeating functions in a deterministic and ordered way.
    IEnumerator waaait()
    {
        while (true)
        {
             yield return 1;
             if (running)
             {
                if(Loading.gameObject != null)  Destroy(Loading);
            //    PhotonVoiceNetwork.Disconnect();
                if (aicontrollers.Count == 0 || (aicontrollers.Count > 0 && aicontrollers[0].ismission == false) || (crosslevelholder.campaign == true && crosslevelholder.campaignlevel.objective ==    MainMenuCampaignControlScript.eMissionObjective.Survive) )  addmoney(moneyincreaserate);
                if (teammembersout(team).Count == 0 && timepassed > 25) money = 0;
                OneSecond();
                resetships();
                all_ships = concatarraylist(new List<GameObject>[] { Green.ToList(), Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList(), Yellow.ToList() }).ToList<GameObject>();
                foreach (AIController gam in aicontrollers) gam.GiveOrderwait();
                checkallships();
                foreach (_Ship ship in allshipsscript) ship.callallreps();
             }       
        }

    }
    // returns local player money.
    public int getmoney () {   return money;  }
   

    // allows you to give the player more money and checks it is not going over the money cap.
    public void addmoney (int amount)    { if((money < 7000)) money += amount;    }

    // takes money away from the player, the check if the player has enough money needs to be before this is called.
    public void takemoney (int amount) { money -= amount; }
   
       
    // this function checks if the gamecontroller is aware of all current ships or any current ships have been destroyed.
    void checkallships ()
    {
        foreach (Transform t in Objectsholder.transform)
        if (t.name != "Working" && t.name != "Engines" && allshipsgams.Contains(t.gameObject) == false)
        {
                allshipsscript.Add(t.GetComponent<_Ship>());
                allshipsgams.Add(t.gameObject);
                addtoarrayTSTransfrom(ref allshipststransform, t.GetComponent<TSTransform>());
        }
        List<GameObject> removable = new List<GameObject>();
        List<_Ship> removableships = new List<_Ship>();
        List<TSTransform> removabletstransform = new List<TSTransform>();

        foreach (GameObject gam in allshipsgams) if(gam  == null|| gam.gameObject == null) removable.Add(gam);
        foreach (_Ship ship in allshipsscript) if (ship == null || ship.gameObject == null) removableships.Add(ship);
        foreach (TSTransform ship in allshipststransform) if (ship == null || ship.gameObject == null) removabletstransform.Add(ship);

        foreach (GameObject gam in removable) allshipsgams.Remove(gam);
        foreach (_Ship ship in removableships) allshipsscript.Remove(ship);
        foreach (TSTransform ship in removabletstransform) RemovefromTStransform(ref allshipststransform, ship);

    }

    // returns alliesoutcustom but with the local players team
    public List<TSTransform> alliesout ()   {  return  alliesoutcustom(team); }
 
    // don't know why that is there, cant risk breaking it.
    public override void OnSyncedStart()   {   InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();   }
 
    // effectively allows you to add to an array of TStransform[] in the same manner as a list, this was needed as lists were randomly removing themselfs from memory.
    void addtoarrayTSTransfrom(ref TSTransform[] inputarray, TSTransform addition)
    {
        List<TSTransform> output = inputarray.ToList();
        output.Add(addition);
        inputarray = output.ToArray();
    }

    // effectively allows you to add to an array of TStransform[] in the same manner as a list, this was needed as lists as lists were randomly removing themselfs from memory.
    void RemovefromTStransform(ref TSTransform[] inputarray, TSTransform addition)
    {
        List<TSTransform> output = inputarray.ToList();
        output.Remove(addition);
        inputarray = output.ToArray();
    }

    // TODO: investigate weather this function breaks the "CapitalShip" Gamemode.
    // returns the allied ships based on the currentg gamemode.
    public List<TSTransform> alliesoutcustom(_Ship.eShipColor control)
    {
        List<TSTransform> output = new List<TSTransform>();
        if(Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
        {
           switch (control)
           {
              case _Ship.eShipColor.Green: output = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() }).ToList<TSTransform>(); break;
              case _Ship.eShipColor.Blue:  output = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() }).ToList<TSTransform>(); break;
              case _Ship.eShipColor.White:output = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() }).ToList<TSTransform>(); break;
              case _Ship.eShipColor.Yellow: output = combinearrays(new List<GameObject>[] { Yellow.ToList(), Grey.ToList(), Red.ToList() }).ToList<TSTransform>(); break;
              case _Ship.eShipColor.Grey:  output = combinearrays(new List<GameObject>[] { Yellow.ToList(), Grey.ToList(), Red.ToList() }).ToList<TSTransform>(); break;
              case _Ship.eShipColor.Red: output = combinearrays(new List<GameObject>[] { Yellow.ToList(), Grey.ToList(), Red.ToList() }).ToList<TSTransform>(); break;
            }
        }
        else
        {
            switch (control)
            {
                case _Ship.eShipColor.Blue:  output = combinearrays(new List<GameObject>[] { Blue.ToList() }).ToList<TSTransform>(); break;
                case _Ship.eShipColor.Green:  output = combinearrays(new List<GameObject>[] { Green.ToList() }).ToList<TSTransform>(); break;                               
                case _Ship.eShipColor.Grey:  output = combinearrays(new List<GameObject>[] { Grey.ToList() }).ToList<TSTransform>(); break;                 
                case _Ship.eShipColor.Red: output =   combinearrays(new List<GameObject>[] { Red.ToList() }).ToList<TSTransform>(); break;
                case _Ship.eShipColor.White:  output = combinearrays(new List<GameObject>[] { White.ToList() }).ToList<TSTransform>(); break;
                case _Ship.eShipColor.Yellow:  output = combinearrays(new List<GameObject>[] { Yellow.ToList() }).ToList<TSTransform>(); break;  
            }
        }

        return output;
    }

    // returns teammembers based on shipcolor enum
    public List<GameObject> teammembersout (_Ship.eShipColor control)
    {
        List<GameObject> output = new List<GameObject>();
        switch (control)
        {
            case _Ship.eShipColor.Blue:   output = Blue.ToList(); break;             
            case _Ship.eShipColor.Green:  output = Green.ToList(); break;                    
            case _Ship.eShipColor.Grey:   output = Grey.ToList(); break;             
            case _Ship.eShipColor.Red:    output = Red.ToList(); break;             
            case _Ship.eShipColor.White:  output = White.ToList(); break;
            case _Ship.eShipColor.Yellow: output = Yellow.ToList(); break;
        }
        return output;
    }

    // checks if any ships need to be added to the custom lists
    // TODO: see if this does the same thing as check all ships
    public void resetships ()
    {
            foreach (Transform t in GameObject.Find("Objects").transform)
            {
                if (checkelidgable(t))
                {
                    switch (t.GetComponent<_Ship>().ShipColor)
                    {
                        case _Ship.eShipColor.Blue:    if(Blue.Contains(t.gameObject) == false)  addtoarray(ref Blue,t.gameObject); break;
                        case _Ship.eShipColor.Green:  if (Green.Contains(t.gameObject) == false) addtoarray(ref Green, t.gameObject); break;
                        case _Ship.eShipColor.Grey:  if (Grey.Contains(t.gameObject) == false) addtoarray(ref Grey, t.gameObject); break;
                        case _Ship.eShipColor.Red:  if (Red.Contains(t.gameObject) == false) addtoarray(ref Red, t.gameObject); break;
                        case _Ship.eShipColor.White:  if (White.Contains(t.gameObject) == false) addtoarray(ref White, t.gameObject); break;
                        case _Ship.eShipColor.Yellow:   if (Yellow.Contains(t.gameObject) == false) addtoarray(ref Yellow, t.gameObject); break;     
                    }
               
              }
        }
    }

    // allows you to add to a Gameobject[] array in the same way as you would a list.
    void addtoarray (ref GameObject[] inputarray, GameObject addition)
    {
        List<GameObject> output = inputarray.ToList();
        output.Add(addition);
        inputarray = output.ToArray();
    }

    // allows you to remove from a Gameobject[] array in the same way as you would a list.
    void Removefromarray(ref GameObject[] inputarray, GameObject addition) { inputarray = inputarray.Where(val => val != addition).ToArray();   }
   
    // gets the TStransform components from every gameobject in multiple lists, then returns one big array containing them all.
    TSTransform[] combinearrays(List<GameObject>[] input)
    {
       List<TSTransform> temptargets = new List<TSTransform>();
       foreach(List<GameObject> listint in input) foreach (GameObject gam in listint) if(gam && gam.gameObject != null) temptargets.Add(gam.GetComponent<TSTransform>());
       return temptargets.ToArray();
    }

    // checks if all players have entered game and are ready.
    public void playerready ( int playerview) {    foreach (playerisreadychecker player in players) if (player.playerid == playerview) player.isready = true;   }
   
    
 
    // derministic repeating function inside the gamecontroller
    public void OneSecond ()
    {
        // checks if anything is destroyed.
        removenullentities(ref Blue);
        removenullentities(ref Green);
        removenullentities(ref Yellow);
        removenullentities(ref Grey);
        removenullentities(ref Red);
        removenullentities(ref White);

        //checks fog of war
        fogcheck();

        //creates list of capitals.
        if(crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || (crosslevelholder.campaign == true && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.killTarget))
        {
            Team1Capitals.Clear();
            Team2Capitals.Clear();
            foreach (Transform t in Objectsholder.transform)
            {
                if (t.name == "Super-Capital(Clone)")
                {
                    _Ship tempship = t.GetComponent<_Ship>();
                    if (tempship.ShipColor == _Ship.eShipColor.Green || tempship.ShipColor == _Ship.eShipColor.Blue || tempship.ShipColor == _Ship.eShipColor.White) Team1Capitals.Add(t.gameObject);
                    if (tempship.ShipColor == _Ship.eShipColor.Yellow || tempship.ShipColor == _Ship.eShipColor.Grey || tempship.ShipColor == _Ship.eShipColor.Red) Team2Capitals.Add(t.gameObject);
                }
            }
        }
      
        // creates custom list of targets for all players.
        if (Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
        {
            GreenTargets = combinearrays(new List<GameObject>[] { Grey.ToList(), Red.ToList(), Yellow.ToList() });
            YellowTargets = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() });
            if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
            {
                BlueTargets = combinearrays(new List<GameObject>[] { Grey.ToList(), Red.ToList(), Yellow.ToList() });
                GreyTargets = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() });
            }
            if(PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
            {
                WhiteTargets = combinearrays(new List<GameObject>[] { Grey.ToList(), Red.ToList(), Yellow.ToList() });
                RedTargets = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), White.ToList() });
            }
        }
        else
        {
            YellowTargets = combinearrays(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList() });
            GreenTargets = combinearrays(new List<GameObject>[] { Blue.ToList(),  Grey.ToList(), Red.ToList(), White.ToList(), Yellow.ToList() });
            if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
            {
                BlueTargets = combinearrays(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), Red.ToList(), White.ToList(), Yellow.ToList() });
                GreyTargets = combinearrays(new List<GameObject>[] { Green.ToList(), Blue.ToList(), Red.ToList(), White.ToList(), Yellow.ToList() });
            }
            if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
            {
                RedTargets = combinearrays(new List<GameObject>[] { Green.ToList(),Grey.ToList(), Blue.ToList(), White.ToList(), Yellow.ToList() });
                WhiteTargets = combinearrays(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), Red.ToList(), Blue.ToList(), Yellow.ToList() });
            }

        }

        // checks if anyone has won the game.
        // TODO: make section below look cleaner.
        if(PhotonNetwork.isMasterClient)
        {
            if (timepassed > 30 && crosslevelholder.tutorial == false && crosslevelholder.TestArea == false &&( crosslevelholder.campaign == false || timepassed > 25))
            {
                if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
                {
                    if (PhotonNetwork.offlineMode == false)
                    {
                        if (alliesout().Count == 0)
                        {
                            if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 1) phopview.RPC("team2won", PhotonTargets.All, false);
                            else phopview.RPC("team1won", PhotonTargets.All, false);
                        }
                        if (targetsout(team).Count == 0)
                        {
                            if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 2) phopview.RPC("team2won", PhotonTargets.All, false);
                            else phopview.RPC("team1won", PhotonTargets.All, false);
                        }
                    }
                    else
                    {
                        if (teammembersout(team).Count == 0)
                        {
                            if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 1) phopview.RPC("team2won", PhotonTargets.All, false);
                            else phopview.RPC("team1won", PhotonTargets.All, false);
                        }

                        if (targetsout(team).Count == 0)
                        {
                            if (crosslevelholder.campaign == false || (crosslevelholder.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive))
                            {
                                if (crosslevelholder.findteam(PhotonNetwork.player.ID) == 2) phopview.RPC("team2won", PhotonTargets.All, false);
                                else phopview.RPC("team1won", PhotonTargets.All, false);
                            }
                        } 
                    }
                }

                if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
                {
                    if (PhotonNetwork.offlineMode == false)
                    {
                        if (concatarraylist(new List<GameObject>[] {  Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green", false);
                        if (concatarraylist(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Yellow", false);
                        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2)
                        {
                            if (concatarraylist(new List<GameObject>[] { Green.ToList(), Grey.ToList(), Red.ToList(), White.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Blue", false);
                            if (concatarraylist(new List<GameObject>[] { Green.ToList(),Red.ToList(), White.ToList(), Blue.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Grey", false);
                        }
                        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers > 2 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") 
                        {
                            if (concatarraylist(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), Red.ToList(), Blue.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "White", false);
                            if (concatarraylist(new List<GameObject>[] { Green.ToList(),  Grey.ToList(), White.ToList(), Blue.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Red", false);
                        }
                    }
                    else
                    {
                        if (team == _Ship.eShipColor.Green)
                        {
                            if (concatarraylist(new List<GameObject>[] { Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList(), Yellow.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green", false);
                            if (teammembersout(team).Count == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "AI wins!",true);
                        }
                        if (team == _Ship.eShipColor.Yellow)
                        {
                            if (concatarraylist(new List<GameObject>[] { Grey.ToList(), Red.ToList(), White.ToList(), Blue.ToList(), Green.ToList() }).Length == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Yellow", false);
                            if (teammembersout(team).Count == 0) phopview.RPC("teamfreeforallwon", PhotonTargets.All, "AI wins", true);
                        }
                    }
                
                }
                if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip || (crosslevelholder.campaign == true && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.killTarget))
                {
                    if(crosslevelholder.campaign == false)   if (Team1Capitals.Count == 0 && isalreadywon == false)   phopview.RPC("team2won", PhotonTargets.All, false);
                    if (Team2Capitals.Count == 0 && isalreadywon == false) phopview.RPC("team1won", PhotonTargets.All, false);
                }
                if (Timeleft < 0 && (crosslevelholder.campaign == false || crosslevelholder.campaignlevel.objective != MainMenuCampaignControlScript.eMissionObjective.Survive)) teamfreeforallwon("time ran out",true);
                if (Timeleft < 0 && (crosslevelholder.campaign == true && crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive)) team1won(false);
            }
        }  
    }


    // checks if any gameobjects are destroyed or null
    void removenullentities (ref GameObject[] input)
    {
        GameObject temprem = null;
        foreach (GameObject g in input) if (g == null || g.gameObject == null) { temprem = g; }
        Removefromarray(ref input, temprem);
    }

    // concats lists, here because an array call looks way cleaner then concating several lists.
    GameObject[] concatarraylist(List<GameObject>[] input)
    {
        List<GameObject> output = new List<GameObject>();
        foreach (List<GameObject> listint in input) foreach (GameObject gam in listint) output.Add(gam);
        return output.ToArray();
    }

    // the client function recieved to inform you that the game is over, there to ensure that even if determinism breaks, the scenes every one is in do not.
    [PunRPC]
    void teamfreeforallwon(string teamin,bool customstring)
    {
        isalreadywon = true;
        GameObject[] playervoices = GameObject.FindGameObjectsWithTag("Voice");
        foreach (GameObject voice in playervoices) voice.GetComponent<AudioSource>().enabled = true;
        StartCoroutine("wait");
        GameObject Victory = Instantiate(Team1victory);
        if (GameObject.Find("DragOBJ"))
        {
            Victory.transform.parent = GameObject.Find("DragOBJ").transform;
            Victory.transform.localPosition = new Vector3(0, 1, 4.25f);
            Victory.transform.localScale = new Vector3(15, 2, 2);
            if (customstring == false) Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Team : " + teamin + " Wins!";
            else Victory.transform.Find("New Text").GetComponent<TextMesh>().text = teamin;
        }
    }

    //uses the position of teams inside the crosslevelvariableholder to get the starting ship color.
    public static _Ship.eShipColor findspawnteam(int input)
    {
        _Ship.eShipColor output = _Ship.eShipColor.Green;
        if (input == 1) output = _Ship.eShipColor.Green;
        if (input == 2) output = _Ship.eShipColor.Blue;
        if (input == 3) output = _Ship.eShipColor.White;
        if (input == 4) output = _Ship.eShipColor.Yellow;
        if (input == 5) output = _Ship.eShipColor.Grey;
        if (input == 6) output = _Ship.eShipColor.Red;
        return output;
    }

    // returns the position of the player inside the teams from shipcolor.
    public static int findspawnteamreverse(_Ship.eShipColor input)
    {
        int output = 1;
        if (input == _Ship.eShipColor.Green) output = 1;
        if (input == _Ship.eShipColor.Blue) output = 2;
        if (input == _Ship.eShipColor.White) output = 3;
        if (input == _Ship.eShipColor.Yellow) output = 4;
        if (input == _Ship.eShipColor.Grey) output = 5;
        if (input == _Ship.eShipColor.Red) output = 6;
        return output;
    }

    // Updates the progression tracker to update when you win a match.
    void VictoryCampaign ()
    {
        if (crosslevelholder.campaignlevel != null)
        {
            FleetBuilder = IORoot.findIO("fleet1");
            FleetBuilder.read();
            int completedthrough = FleetBuilder.get<int>("Completednum");
            FleetBuilder.read();
            if (completedthrough < crosslevelholder.campaignlevel.panelnum + 1)
            {
                FleetBuilder.add("Completednum", crosslevelholder.campaignlevel.panelnum + 1);
                FleetBuilder.write();
            }
        }
    }

    // Sent from the master to say that team 1 (green, blue and white) have won.
    [PunRPC]
    public void team1won (bool endallin)
    {
        victorytest = true;
        if (crosslevelholder.campaign == true) VictoryCampaign();
         isalreadywon = true;
         Debug.Log("Team1won");
         GameObject[] playervoices = GameObject.FindGameObjectsWithTag("Voice");
         foreach (GameObject voice in playervoices) voice.GetComponent<AudioSource>().enabled = true;
         StartCoroutine("wait");
         GameObject Victory = Instantiate(Team1victory);
         Victory.transform.parent = GameObject.Find("DragOBJ").transform;
         Victory.transform.localPosition = new Vector3(0, 1, 4.25f);
         Victory.transform.localScale = new Vector3(15, 2, 2);
        int teammaybe = getteam(team,crosslevelholder.Gamemode);
        if (teammaybe == 1) Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Victory!";
        else Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Defeat!";
        if (endallin == true) endall = true;
    }

    //TODO: combine these functions and only seperate the stuff that is actually different.
    // Sent from the master to say that team 2 (yellow,grey,red) have won.
    [PunRPC]
    public void team2won (bool endallin)
    {
        victorytest = false;
        isalreadywon = true;
        Debug.Log("Team2won");
        GameObject[] playervoices = GameObject.FindGameObjectsWithTag("Voice");
        foreach (GameObject voice in playervoices) voice.GetComponent<AudioSource>().enabled = true;
        StartCoroutine("wait");
        GameObject Victory = Instantiate(Team2Victory);
        if (GameObject.Find("DragOBJ"))
        {
            Victory.transform.parent = GameObject.Find("DragOBJ").transform;
            Victory.transform.localPosition = new Vector3(0, 1, 4.25f);
            Victory.transform.localScale = new Vector3(15, 2, 2);
            int teammaybe = getteam(team, crosslevelholder.Gamemode);
            if (teammaybe == 2) Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Victory!";
            else Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Defeat!";
            if (endallin == true) endall = true;
        }
    }

    // A RPC to inform everyone the host left, might actually not be nessercary since photon transfers master clients fine.
    [PunRPC]
    public void HostLeft ()
    {
        victorytest = false;
        Debug.Log("hostleft");
        GameObject[] playervoices = GameObject.FindGameObjectsWithTag("Voice");
        foreach (GameObject voice in playervoices) voice.GetComponent<AudioSource>().enabled = true;
        StartCoroutine("wait");
        //add victory sound
        if (GameObject.Find("DragOBJ"))
        {
            GameObject Victory = Instantiate(hostlefticon);
            if (PhotonNetwork.offlineMode == true) Victory.transform.Find("New Text").GetComponent<TextMesh>().text = "Left Game";
            Victory.transform.parent = GameObject.Find("DragOBJ").transform;
            Victory.transform.localPosition = new Vector3(0, 1, 4.25f);
            Victory.transform.localScale = new Vector3(15, 2, 2);
        }
        endall = true;
    }
 
    //returns the capitalship of the specified team.
    //returns null if gamemode isnt capitalship.
    GameObject getcapitalofteam (_Ship.eShipColor  input)
    {
        GameObject output = null;
        foreach (Transform t in Objectsholder.transform)
        {
            if (t.name == "Super-Capital(Clone)")
            {
                _Ship tempship = t.GetComponent<_Ship>();
                if (tempship.ShipColor == input) output = tempship.gameObject;
            }
        }
        return output;
    }

    // checks if specified team still has ships left.
    public bool checkiffactiondead (_Ship.eShipColor input)
    {
        bool output = false;
        if(crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
        {
            if (teammembersout(input).Count == 0) output = true; 
        }
        else if(crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
        {
            if (getcapitalofteam(input) == null) output = true;
        }
        return output;
    }

    // the end of the 5 second wait after the game ends to allow the victory screen to popup, this function changes levels
    public IEnumerator wait ()
    {

        if (GameObject.Find("ResizeObj")) GameObject.Find("ResizeObj").GetComponent<Resizescript>().forcereturntonormal();
        yield return new WaitForSeconds(5);
        bool test = PhotonNetwork.offlineMode;
        if (GameObject.Find("ResizeObj")) GameObject.Find("ResizeObj").GetComponent<Resizescript>().forcereturntonormal();
        transform.localScale = new Vector3(1, 1, 1);
        if (crosslevelholder.campaign == true && crosslevelholder.campaignnextlevelset == false)
        {
            crosslevelholder.campaignnextlevelset = true;
            if (crosslevelholder.campaignlevel.panelnum < (MainMenuCampaignControlScript.maps.Length - 1))  if (victorytest == true)  crosslevelholder.campaignlevel = MainMenuCampaignControlScript.maps[crosslevelholder.campaignlevel.panelnum + 1];
            else
            {
                if (victorytest == true)
                {
                    Debug.Log("Campain == false");
                    GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().campaign = false;
                    crosslevelholder.campaign = false;
                }
              
            }
        }
        bool temp = crosslevelholder.campaign;
        if (endall ||( test == true && temp == false))
        {   
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.LoadLevel(0);
        }
        else PhotonNetwork.LoadLevel(1);
    }

    // gets the unit cap based on the number of teams.
    public static int getmaxshipnumbers ()
    {
        int output = 0;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 2) output = 23;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No") output = 14;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") output = 9;
        return output;
    }

    // gets the team of the specified shipcolor.
    public static int getteam (_Ship.eShipColor teamin, CrossLevelVariableHolder.gamemode inputgamemode)
    {
        int output = 0;
        if (inputgamemode == CrossLevelVariableHolder.gamemode.CapitalShip || inputgamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch)
        {
            if (teamin == _Ship.eShipColor.Blue || teamin == _Ship.eShipColor.Green || teamin == _Ship.eShipColor.White) output = 1;
            else output = 2;
        }
        else
        {
            if (teamin == _Ship.eShipColor.Green) output = 1;
            if (teamin == _Ship.eShipColor.Blue) output = 2;
            if (teamin == _Ship.eShipColor.White) output = 3;
            if (teamin == _Ship.eShipColor.Yellow) output = 4;
            if (teamin == _Ship.eShipColor.Grey) output = 5;
            if (teamin == _Ship.eShipColor.Red) output = 6;
        }
        return output;
    }

    // checks if any ships are in visible view distance.
    public void fogcheck()
    {
        List<TSTransform> allies = alliesout();
        foreach (TSTransform gam in targetsout(team))
        {
            bool output = true;
            foreach(TSTransform ally in allies) if(ally && gam && gam.gameObject != null && ally.gameObject != null && Vector3.Distance(gam.transform.localPosition, ally.transform.localPosition) < 14000) output = false;
            if(gam && gam.gameObject != null)   gam.GetComponent<_Ship>().assigncloackfog(output);
        }
    }

    // gets the amount of time left this match.
    public string gettimeleft()
    {
        float minutes = Mathf.Floor(Timeleft.AsFloat() / 60);
        float seconds = Mathf.RoundToInt(Timeleft.AsFloat() % 60);
        string output = "";
        if (seconds > 9) output = minutes.ToString() + ":" + seconds.ToString();
        else output = minutes.ToString() + ":" + "0" + seconds.ToString();
        return output;
    }

    // returns any of the ""Targets arrays based on ship color.
    // Probably should have setup a shipColor Class that stores all this stuff.
    public  List<TSTransform> targetsout (_Ship.eShipColor control)
    {
        List<TSTransform> output = new List<TSTransform>();
        switch (control)
        {
            case _Ship.eShipColor.Green: output = GreenTargets.ToList<TSTransform>();  break;
            case _Ship.eShipColor.Blue:  output = BlueTargets.ToList<TSTransform>();   break;  
            case _Ship.eShipColor.White: output = WhiteTargets.ToList<TSTransform>();  break;  
            case _Ship.eShipColor.Yellow: output = YellowTargets.ToList<TSTransform>();   break;           
            case _Ship.eShipColor.Grey: output = GreyTargets.ToList<TSTransform>();    break;         
            case _Ship.eShipColor.Red: output = RedTargets.ToList<TSTransform>(); break;
        }
        return output;
    }

    // function that checks if the object under "objects" parent isnt the engine and working holders
	bool checkelidgable (Transform t)
    {
        if (t.name != "Engines" && t.name != "Working")   return true;      
        else return false;
    }

    // runs the simulation, bit pointless since everyone does that once they load in anyway.
    [PunRPC]
    public void runsim() { TrueSyncManager.RunSimulation(); }

    // Just the Update(),
    void Update () {
        timepassed += Time.deltaTime;
       
       if(Application.isEditor) if(Input.GetKeyDown(KeyCode.S))   phopview.RPC("teamfreeforallwon", PhotonTargets.All, "Green",false);
        if (timepassed > 3 && simrunning == false && PhotonNetwork.isMasterClient)
        {
            phopview = GetComponent<PhotonView>();
            phopview.RPC("runsim", PhotonTargets.AllViaServer);
            simrunning = true;
        }

        // this section checks if everyone has signaled "ready" before starting the simulation.
        if (simrunning == true && started == false && PhotonNetwork.isMasterClient == true)
        {
            bool check = true;
            foreach (playerisreadychecker player in players) if (player.isready == false) check = false;
            if (check == true)
            {
                GameObject.Find("TrueSyncManager").GetComponent<RelayController>().relay.GetComponent<InputRelay>().runsimulationnow = true;
                Debug.Log("runsim");
                started = true;
            }
        }
    }

    // spawns a networked ship deterministically using a gameobject to spawn, its position, team and viewid
    private void SpawnShipInput (GameObject spawnobj,TSVector startpos, int spawnori,int viewidin)
    {
        if (crosslevelholder.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightMenuShipBuy);
        GameObject spawned = TrueSyncManager.SyncedInstantiate(spawnobj);
        spawned.transform.parent = GameObject.Find("Objects").transform;
        spawned.transform.GetComponent<TSTransform>().rotation = PositinonRelativeToHeadset.getspawnrotation(_Ship.getteambynumber(spawnori));
        spawned.transform.localScale = new Vector3(8, 8, 8);
        spawned.GetComponent<TSTransform>().position = startpos;
        debug = spawned;
        spawned.GetComponent<_Ship>().spawnnum = spawnori;
        if(viewidin != 0) spawned.GetComponent<PhotonView>().viewID = viewidin;
        spawned.GetComponent<_Ship>().StartMain();
    }

    // takes the input of a ship to spawn using basic datatypes, then converts them to data for SpawnShipInput() to use.
    public void spawnship (int spawner,TSVector startpos, int spawnori, int viewidin){ if (running) SpawnShipInput(getshipbynumber(spawner), startpos, spawnori, viewidin); }

    // gets the Reference prefab gameobject
    public GameObject getshipbynumber(int i)
    {
        GameObject output = null;
        switch (i)
        {
            case 1: output = Scout; break;
            case 2: output = Fighter; break;
            case 3: output = Bomber; break;
            case 4: output = AttackCraft; break;
            case 5: output = MissileFrigate; break;
            case 6: output = FlackFrigate; break;
            case 7: output = CannonFriage; break;
            case 8: output = LazerFrigate; break;
            case 9: output = SupportFrigate; break;
            case 10: output = RepairFrigate; break;
            case 11: output = Destroyer; break;
            case 12: output = Capital; break;
            case 13: output = FlagShip; break;
        }
        return output;
    }

    public GameObject Scout;
    public GameObject Fighter;
    public GameObject Bomber;
    public GameObject AttackCraft;
    public GameObject MissileFrigate;
    public GameObject FlackFrigate;
    public GameObject CannonFriage;
    public GameObject LazerFrigate;
    public GameObject SupportFrigate;
    public GameObject RepairFrigate;
    public GameObject Destroyer;
    public GameObject Capital;
    public GameObject FlagShip;
    public RelayController InputRelay;

    // GameController acts as relay to the input relay to send a spawn ship input.
    public void ordershipspawn(int a,TSVector b,int c, int d)
    {
        if (InputRelay == null) InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
        if (InputRelay != null) InputRelay.ordershipspawn(a,b,c,d);
    }
    
    //class containing stuff relating to wether the player is ready.
    public class playerisreadychecker  {
        public string name;
        public bool isready;
        public int playerid;
        public playerisreadychecker (string namein, bool isreadyin, int playeridin)
        {
            name = namein;
            isready = isreadyin;
            playerid = playeridin;
        }
    }

    // Sets the transmit mode for a client, arguably this could have been equally achieved with the photon channels feature but thats what i get for not reading into the documentation enough.
    [PunRPC]
    public void SetTransmittmode(int channeltarget, int playertarget)
    {
        PlayerPrefabVoiceControlScript target = null;
        foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice"))
        {
            PlayerPrefabVoiceControlScript playpra = gam.GetComponent<PlayerPrefabVoiceControlScript>();
            if(playpra != null && playpra.locview.owner.ID == playertarget)target = playpra;
        }
        if (target != null)
        {
            if (channeltarget == 0) target.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.Muteall;
            if (channeltarget == 1) target.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
            if (channeltarget == 2) target.curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        }
    }

    // Provides a relay for setting up the transmit mode.
    public void switchvoice(PlayerPrefabVoiceControlScript.VoiceChannel input,int playertarget)
    {
        curchannel = input;
        phopview.RPC("SetTransmittmode", PhotonTargets.All, (int)input, playertarget);
    }
}
