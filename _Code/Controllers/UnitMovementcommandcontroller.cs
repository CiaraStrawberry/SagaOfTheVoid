using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.Steering;
using System.Linq;
using TrueSync;
using simple;
using UnityEngine.SceneManagement;

/// <summary>
/// This is the main Game controller, it controlls everything inside the game and allows the ships to know what to shoot at and what to not shoot at.
/// </summary>
public class UnitMovementcommandcontroller : TrueSyncBehaviour {
    
    // this is the the channel the local photon voice is on.
    public PlayerPrefabVoiceControlScript.VoiceChannel curchannel;
    // the team of the local player.
    public _Ship.eShipColor team;
    // this is all ships inside the blue team.
    [SerializeField]
    private GameObject[] Blue = new GameObject[5];
    // this is all ships inside the Green team.
    [SerializeField]
    private GameObject[] Green = new GameObject[5];
    // this is all ships inside the Grey team.
    [SerializeField]
    private GameObject[] Grey  = new GameObject[5];
    // this is all ships inside the Red team.
    [SerializeField]
    private GameObject[] Red  = new GameObject[5];
    // this is all ships inside the White team.
    [SerializeField]
    private GameObject[] White = new GameObject[5];
    // this is all ships inside the Yellow team.
    [SerializeField]
    private GameObject[] Yellow = new GameObject[5];
    // this is all the ships the blue team can target.
    [SerializeField]
    private TSTransform[] BlueTargets = new TSTransform[0];
    // this is all the ships the Green team can target.
    [SerializeField]
    private TSTransform[] GreenTargets = new TSTransform[0];
    // this is all the ships the Grey team can target.
    [SerializeField]
    private TSTransform[] GreyTargets = new TSTransform[0];
    // this is all the ships the Red team can target.
    [SerializeField]
    private TSTransform[] RedTargets = new TSTransform[0];
    // this is all the ships the White team can target.
    [SerializeField]
    private TSTransform[] WhiteTargets = new TSTransform[0];
    // this is all the ships the Yellow team can target.
    [SerializeField]
    private TSTransform[] YellowTargets = new TSTransform[0];
    // this is all the ships in the game
    public List<GameObject> all_ships = new List<GameObject>();
    // this is all the ships in the game referenced by their _Ship class.
    public List<_Ship> all_shipsScript = new List<_Ship>();
    // time passed since the start of the match.
    private FP timepassed;
    // time til the end of the match.
    public  FP Timeleft = 600;
    // the current match gamemode.
    public CrossLevelVariableHolder.gamemode Gamemode;
    // the current match crosslevelvariableholder singleton.
    public CrossLevelVariableHolder crosslevelholder;
    // the Photonview attached to this gameobject.
    public PhotonView phopview;
    // the panel that the player sees if team 1 wins.
    public GameObject Team1victory;
    // the panel that the player sees if team 2 wins.
    public GameObject Team2Victory;
    // the ships on the field that belong to team 1 and are capitals.
    private List<GameObject> Team1Capitals = new List<GameObject>();
    // the ships on the field that belong to team 2 and are capitals.
    private List<GameObject> Team2Capitals = new List<GameObject>();
    // the scenery for map1.
    public GameObject map1area;
    // the scenery for map2.
    public GameObject map2area;
    // the scenery for map 3
    public GameObject map3area;
    // the world object parented to all objects that are ships.
    [SerializeField]
    private GameObject Objectsholder;
    // the local players money.
    [SerializeField]
    private int money = 1500;
    // the gameobject that appears when things are still loading.
    public GameObject Loading;
    // the tutorial controller.
    private TutorialTextControlScript tutcontrol;
    // the local music source.
    AudioSource audiosource;
    // a list of player checks to see if each one is ready.
    List<playerisreadychecker> players = new List<playerisreadychecker>();
    // is the game running?
    public bool running;
    // a networked random number to allow deterministic randoms that change each match.
    public int randomnumber;
    // all ships addressed by their _ship components.
    public List<_Ship> allshipsscript = new List<_Ship>();
    // all ships addressed by their gameobjects.
    public List<GameObject> allshipsgams = new List<GameObject>();
    // all AI controllers.
    public List<AIController> aicontrollers = new List<AIController>();
    // all TStransforms.
    public TSTransform[] allshipststransform = new TSTransform[0];
    // the money increase rate for the local player.
    public int moneyincreaserate = 30;
    // has the simulation started?
    public bool started;
    // has someone already won?
    public bool isalreadywon;
    // the local component used for saving data to the harddrive.
    private IOComponent FleetBuilder;
    // is the victory resulting in everyone leaving the match?
    public bool endall;
    // if the game has ended, have i won?
    bool victorytest;
    // the UI element that tells the player the host has left.
    public GameObject hostlefticon;
    // is the simulation (independant of the game) running?
    private bool simrunning;

    /// <summary>
    /// Initialise everything, set all to defaults.
    /// </summary>
    void Awake () {
        crosslevelholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        if(crosslevelholder.tutorial == true) tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
        team = crosslevelholder.findspawnteam(PhotonNetwork.player.ID);      
        Gamemode = crosslevelholder.Gamemode;
        if (SceneManager.GetActiveScene().name != "mptest")  curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        else
        {
            if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip) curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.TeamOnly;
            else curchannel = PlayerPrefabVoiceControlScript.VoiceChannel.All;
        }
        map1area.SetActive(false);
        map2area.SetActive(false);
        map3area.SetActive(false);
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map1) map1area.SetActive(true);
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map2) map2area.SetActive(true);
        if (crosslevelholder.map == CrossLevelVariableHolder.mapcon.map3) map3area.SetActive(true);
    }
  
    /// <summary>
    /// More initialisation, set everything up based on player preferance.
    /// </summary>
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

    /// <summary>
    /// This function is called across the network to allow the AI to use an actually random number, but one which is the same across all clients.
    /// </summary>
    /// <param name="randomnum"> the random number to use</param>
    [PunRPC] 
    void setrandomnumberforthismatch (int randomnum)  {   randomnumber = randomnum;    }
  
    /// <summary>
    /// This function is called once the simulation starts running, it doesnt use the synced start call because my simulation ignores it and starts the simulation itself.
    /// this was written like this due to sync issues.
    /// </summary>
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

    /// <summary>
    /// this is a one second wait function that is designed to allow call all other 1 second repeating functions in a deterministic and ordered way.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// returns local player money.
    /// </summary>
    /// <returns></returns>
    public int getmoney () {   return money;  }
   
    /// <summary>
    /// allows you to give the player more money and checks it is not going over the money cap.
    /// </summary>
    /// <param name="amount"></param>
    public void addmoney (int amount)    { if((money < 7000)) money += amount;    }

    /// <summary>
    /// takes money away from the player, the check if the player has enough money needs to be before this is called.
    /// </summary>
    /// <param name="amount"></param>
    public void takemoney (int amount) { money -= amount; }
   
       
    /// <summary>
    /// this function checks if the gamecontroller is aware of all current ships or any current ships have been destroyed.
    /// </summary>
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

    /// <summary>
    /// returns alliesoutcustom but with the local players team
    /// </summary>
    /// <returns></returns>
    public List<TSTransform> alliesout ()   {  return  alliesoutcustom(team); }
 
    /// <summary>
    /// don't know why that is there, cant risk breaking it.
    /// </summary>
    public override void OnSyncedStart()   {   InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();   }
 
    /// <summary>
    /// effectively allows you to add to an array of TStransform[] in the same manner as a list, this was needed as lists were randomly removing themselfs from memory.
    /// </summary>
    /// <param name="inputarray">the array to add things to</param>
    /// <param name="addition">thing thing to add to arrays</param>
    void addtoarrayTSTransfrom(ref TSTransform[] inputarray, TSTransform addition)
    {
        List<TSTransform> output = inputarray.ToList();
        output.Add(addition);
        inputarray = output.ToArray();
    }

    /// <summary>
    /// effectively allows you to add to an array of TStransform[] in the same manner as a list, this was needed as lists as lists were randomly removing themselfs from memory.
    /// </summary>
    /// <param name="inputarray">the array to remove things to</param>
    /// <param name="addition">thing thing to remove to arrays</param>
    void RemovefromTStransform(ref TSTransform[] inputarray, TSTransform addition)
    {
        List<TSTransform> output = inputarray.ToList();
        output.Remove(addition);
        inputarray = output.ToArray();
    }

    /// <summary>
    /// TODO: investigate weather this function breaks the "CapitalShip" Gamemode.
    /// returns the allied ships based on the currentg gamemode.
    /// </summary>
    /// <param name="control">team of which ships are allied to</param>
    /// <returns>allied ships</returns>
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

    /// <summary>
    /// returns teammember ships based on shipcolor enum.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
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

    /// <summary>
    /// checks if any ships need to be added to the custom lists
    /// TODO: see if this does the same thing as check all ships
    /// </summary>
    public void resetships ()
    {
        foreach (Transform t in GameObject.Find("Objects").transform)
        {
            if (checkelidgable(t))
            {
                switch (t.GetComponent<_Ship>().ShipColor)
                {
                    case _Ship.eShipColor.Blue: if (Blue.Contains(t.gameObject) == false) addtoarray(ref Blue, t.gameObject); break;
                    case _Ship.eShipColor.Green: if (Green.Contains(t.gameObject) == false) addtoarray(ref Green, t.gameObject); break;
                    case _Ship.eShipColor.Grey: if (Grey.Contains(t.gameObject) == false) addtoarray(ref Grey, t.gameObject); break;
                    case _Ship.eShipColor.Red: if (Red.Contains(t.gameObject) == false) addtoarray(ref Red, t.gameObject); break;
                    case _Ship.eShipColor.White: if (White.Contains(t.gameObject) == false) addtoarray(ref White, t.gameObject); break;
                    case _Ship.eShipColor.Yellow: if (Yellow.Contains(t.gameObject) == false) addtoarray(ref Yellow, t.gameObject); break;
                }

            }
        }
    }

    /// <summary>
    /// allows you to add to a Gameobject[] array in the same way as you would a list.
    /// </summary>
    /// <param name="inputarray">array to add to</param>
    /// <param name="addition">thing to add to array</param>
    void addtoarray (ref GameObject[] inputarray, GameObject addition)
    {
        List<GameObject> output = inputarray.ToList();
        output.Add(addition);
        inputarray = output.ToArray();
    }

    /// <summary>
    /// allows you to remove from a Gameobject[] array in the same way as you would a list.
    /// </summary>
    /// <param name="inputarray">array to remove toparam>
    /// <param name="addition">thing to remove to array</param>
    void Removefromarray(ref GameObject[] inputarray, GameObject addition) { inputarray = inputarray.Where(val => val != addition).ToArray();   }
   
    /// <summary>
    /// gets the TStransform components from every gameobject in multiple lists, then returns one big array containing them all.
    /// </summary>
    /// <param name="input">the list of gameobject[] arrays</param>
    /// <returns></returns>
    TSTransform[] combinearrays(List<GameObject>[] input)
    {
       List<TSTransform> temptargets = new List<TSTransform>();
       foreach(List<GameObject> listint in input) foreach (GameObject gam in listint) if(gam && gam.gameObject != null) temptargets.Add(gam.GetComponent<TSTransform>());
       return temptargets.ToArray();
    }

    /// <summary>
    /// checks if all players have entered game and are ready via their classes.
    /// </summary>
    /// <param name="playerview"></param>
    public void playerready ( int playerview) {    foreach (playerisreadychecker player in players) if (player.playerid == playerview) player.isready = true;   }
   
    
    /// <summary>
    /// derministic repeating function inside the gamecontroller
    /// </summary>
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

    /// <summary>
    /// checks if any gameobjects are destroyed or null
    /// </summary>
    /// <param name="input"></param>
    void removenullentities (ref GameObject[] input)
    {
        GameObject temprem = null;
        foreach (GameObject g in input) if (g == null || g.gameObject == null) { temprem = g; }
        Removefromarray(ref input, temprem);
    }

    /// <summary>
    /// concats lists, here because an array call looks way cleaner then concating several lists.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    GameObject[] concatarraylist(List<GameObject>[] input)
    {
        List<GameObject> output = new List<GameObject>();
        foreach (List<GameObject> listint in input) foreach (GameObject gam in listint) output.Add(gam);
        return output.ToArray();
    }

    /// <summary>
    ///  the client function recieved to inform you that the game is over, there to ensure that even if determinism breaks, the scenes every one is in do not.
    /// </summary>
    /// <param name="teamin">the team that won</param>
    /// <param name="customstring">custom message to give to the player about winning</param>
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

    /// <summary>
    /// uses the position of teams inside the crosslevelvariableholder to get the starting ship color.
    /// </summary>
    /// <param name="input">spawnposition</param>
    /// <returns></returns>
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

    /// <summary>
    /// returns the position of the player inside the teams from shipcolor.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Updates the progression tracker to update when you win a match.
    /// </summary>
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

    /// <summary>
    /// Sent from the master to say that team 1 (green, blue and white) have won.
    /// </summary>
    /// <param name="endallin">quit for everyone?</param>
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

    /// <summary>
    ///TODO: combine these functions and only seperate the stuff that is actually different.
    /// Sent from the master to say that team 2 (yellow,grey,red) have won.
    /// </summary>
    /// <param name="endallin">quit for everyone?</param>
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

    /// <summary>
    /// A RPC to inform everyone the host left, might actually not be nessercary since photon transfers master clients fine.
    /// </summary>
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
 
    /// <summary>
    ///returns the capitalship of the specified team.
    ///returns null if gamemode isnt capitalship.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

    /// <summary>
    /// checks if specified team still has ships left.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

    /// <summary>
    /// the end of the 5 second wait after the game ends to allow the victory screen to popup, this function changes levels.
    /// </summary>
    /// <returns></returns>
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

    /// <summary>
    /// gets the unit cap based on the number of teams.
    /// </summary>
    /// <returns></returns>
    public static int getmaxshipnumbers ()
    {
        int output = 0;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 2) output = 23;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No") output = 14;
        if (PhotonNetwork.room != null && PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") output = 9;
        return output;
    }

    /// <summary>
    /// gets the team of the specified shipcolor.
    /// </summary>
    /// <param name="teamin"></param>
    /// <param name="inputgamemode"></param>
    /// <returns></returns>
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

    /// <summary>
    /// checks if any ships are in visible view distance.
    /// </summary>
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

    /// <summary>
    /// gets the amount of time left this match.
    /// </summary>
    /// <returns></returns>
    public string gettimeleft()
    {
        float minutes = Mathf.Floor(Timeleft.AsFloat() / 60);
        float seconds = Mathf.RoundToInt(Timeleft.AsFloat() % 60);
        string output = "";
        if (seconds > 9) output = minutes.ToString() + ":" + seconds.ToString();
        else output = minutes.ToString() + ":" + "0" + seconds.ToString();
        return output;
    }

    /// <summary>
    /// returns any of the ""Targets arrays based on ship color.
    /// Probably should have setup a shipColor Class that stores all this stuff.
    /// </summary>
    /// <param name="control"></param>
    /// <returns></returns>
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

    /// <summary>
    /// function that checks if the object under "objects" parent isnt the engine and working holders
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
	bool checkelidgable (Transform t)
    {
        if (t.name != "Engines" && t.name != "Working")   return true;      
        else return false;
    }

    /// <summary>
    /// runs the simulation, bit pointless since everyone does that once they load in anyway.
    /// </summary>
    [PunRPC]
    public void runsim() { TrueSyncManager.RunSimulation(); }

    /// <summary>
    /// Run the Update checking if everything is working and everyone is ready.
    /// </summary>
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

    /// <summary>
    /// spawns a networked ship deterministically using a gameobject to spawn, its position, team and viewid
    /// </summary>
    /// <param name="spawnobj">the object to spawn</param>
    /// <param name="startpos">the object to spawns position</param>
    /// <param name="spawnori">the object to spawns team</param>
    /// <param name="viewidin">the object to spawns viewID</param>
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

    /// <summary>
    /// takes the input of a ship to spawn using basic datatypes, then converts them to data for SpawnShipInput() to use.
    /// </summary>
    /// <param name="spawner">the ship to spawns number</param>
    /// <param name="startpos">the ships to spawns position</param>
    /// <param name="spawnori">the ship to spawns team</param>
    /// <param name="viewidin">the ship to spawns viewID</param>
    public void spawnship (int spawner,TSVector startpos, int spawnori, int viewidin){ if (running) SpawnShipInput(getshipbynumber(spawner), startpos, spawnori, viewidin); }

    /// <summary>
    /// gets the Reference prefab gameobject
    /// </summary>
    /// <param name="i">the ships number</param>
    /// <returns>the prefab to spawn</returns>
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

    // this variable list is all the different prefab ships you can spawn.
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

    /// <summary>
    /// GameController acts as relay to the input relay to send a spawn ship input.
    /// </summary>
    /// <param name="a">ship num</param>
    /// <param name="b">ship pos</param>
    /// <param name="c">ship team</param>
    /// <param name="d">ship viewID</param>
    public void ordershipspawn(int a,TSVector b,int c, int d)
    {
        if (InputRelay == null) InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
        if (InputRelay != null) InputRelay.ordershipspawn(a,b,c,d);
    }
    
    /// <summary>
    /// Sets the transmit mode for a client, arguably this could have been equally achieved with the photon channels feature but thats what i get for not reading into the documentation enough.
    /// </summary>
    /// <param name="channeltarget"></param>
    /// <param name="playertarget"></param>
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

    /// <summary>
    ///  Provides a relay for setting up the transmit mode.
    /// </summary>
    /// <param name="input">voice channel target</param>
    /// <param name="playertarget">player to change voice channel for</param>
    public void switchvoice(PlayerPrefabVoiceControlScript.VoiceChannel input,int playertarget)
    {
        curchannel = input;
        phopview.RPC("SetTransmittmode", PhotonTargets.All, (int)input, playertarget);
    }

    /// <summary>
    /// class containing stuff relating to wether the player is ready.
    /// </summary>
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

}
