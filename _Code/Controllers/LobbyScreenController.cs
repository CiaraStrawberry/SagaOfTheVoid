using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;
using UnityEngine.SceneManagement;
using ExitGames.Client.Photon;

/// <summary>
/// This is the class that handels everything about the game lobbies.
/// </summary>
public class LobbyScreenController : MonoBehaviour {

    // the text that shows how much money the player starts off with.
    public TextMesh StartMoneytext;
    // the text that shows the ongoing story of the campaign mission.
    public TextMesh Storytext;
    // the text that shows what ships the AI starts off with.
    public TextMesh OpposingShipstext;
    // the text that shows the current mission name.
    public TextMesh MissionName;
    // the text that shows the countdown until the game starts.
    public TextMesh countdown2;
    // the array that shows the campaign mission objective.
    public TextMesh MissionObjective;
    // the array of buttons allowing the player to select a relevant skybox.
    public MeshRenderer[] skyboxbuttonsrend = new MeshRenderer[5];
    // the array of buttons that allows the player to select what scenery to use inside the game.
    private MeshRenderer[] mapbuttonsrend = new MeshRenderer[3];
    // the array of text that shows the players names.
    private TextMesh[] displaytext = new TextMesh[6];
    // the list of assorted buttons (??)
    public GameObject[] display = new GameObject[6];
    public GameObject[] fleetshelf = new GameObject[3];
    public GameObject[] mapbuttons = new GameObject[3];
    public GameObject[] icons = new GameObject[6];
    public GameObject[] Ships;
    // the text showing the photon server status.
    public TextMesh statustext;
    // the text showing the games ping.
    public TextMesh pingtext;
    // the text containing the countdown, there are 2 since the screen has 2 modes and 2 countdowns, one for each mode.
    public TextMesh countdowntext;
    // text showing the players name.
    public TextMesh playernametext;
    // the component used to save the players current fleet.
    public IOComponent FleetBuilder;
    // the material to show a button that is not selected.
    public Material lightmat;
    // the material to show a button that has been selected.
    public Material darkmat;
    // the photonvoice recorder that is used locally to record voices.
    private PhotonVoiceRecorder phoprec;
    // the photonview that is used to record the location of the local player.
    private PhotonView phopview;
    // the textmesh used to record the state of the lobby (i.e in game, in lobby, loading , that kinda thing)
    public TextMesh lobbystateindicatortext;
    // the countdown to the start of the game.
    private int countdown = 10;
    // the bool to instruct the game to start the countdown next frame.
    private bool startrunning;
    // the material to show the scenery in the map1. Probably should have used an array.
    public Material map1mat;
    // the material to show the scenery in the map2.
    public Material map2mat;
    // the material to show the scenery in the map3.
    public Material map3mat;
    // the local singleton to hold game properties.
    private CrossLevelVariableHolder crosslevelvarholder;
    // all the voicerecorders to record everyones voice in the scene.
    private List<PhotonVoiceRecorder> phopvoicerecoreders = new List<PhotonVoiceRecorder>();
    // the enum containing the currently selected gamemode.
    public CrossLevelVariableHolder.gamemode Gamemode;
    // the textmesh to tell the player what is the currently selected gamemode.
    public TextMesh GameModeDisplay;
    // the bool to show if bots are currently enabled.
    public bool bots;
    // the script asigned to handle scene switching.
    private SceneSwitcherController sceneswitch;
    // the display to show the name of the currently joined room.
    public TextMesh RoomNameDisplay;
    // the holder of the names of the players in team1.
    public GameObject team1displaygam;
    // the holder of the names of the players in team2.
    public GameObject team2displaygam;
    // the text mesh to give the players the option to add bots to their game.
    public TextMesh Addbotstext;
    // the textmesh telling the players they can change the number of players in the room.
    public TextMesh ChangeRoomNumberText;
    // all the panels containing buttons in the game.
    public GameObject[] panels = new GameObject[8];
    // the icon that pops up upon the scene beggining to asyncronously load.
    public GameObject Loadingobj;
    // the text showing what difficult the bot was currently on.
    public TextMesh botdificultytext;
    // the textmesh that says "start match"
    public TextMesh Startmatchwriting;
    // the textmesh that says "start match" but on the secondary screen.
    public TextMesh startmatchwriting2;

     /// <summary>
     /// Use this for initialization
     /// TODO: make this function cleaner.
     /// </summary>
     void Start() {
        campaignseg.SetActive(true);
        multimatchseg.SetActive(true);
        foreach (GameObject tutorial in GameObject.FindGameObjectsWithTag("TutorialThings")) tutorial.SetActive(false);
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
        if (GameObject.Find("Team1wins(Clone)")) Destroy( GameObject.Find("Team1wins(Clone)"));
        if (GameObject.Find("Team2Wins(Clone)")) Destroy(GameObject.Find("Team2wins(Clone)"));
        if (PhotonNetwork.inRoom && PhotonNetwork.isMasterClient && PhotonNetwork.offlineMode == false)
        {   
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.room.IsVisible = true;
        }
        RoomNameDisplay = RoomNameDisplaygam.GetComponent<TextMesh>();
        crosslevelvarholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        crosslevelvarholder.wipebots();
        int i = 0;
        foreach (GameObject map in mapbuttons) { mapbuttonsrend[i] = map.GetComponent<MeshRenderer>(); i++; }
        phopview = this.GetComponent<PhotonView>();
        FleetBuilder = IORoot.findIO("fleet1");
        statustext = status.GetComponent<TextMesh>();
        int a = 0;
        foreach (GameObject map in display) { displaytext[a] = map.GetComponent<TextMesh>(); a++; }
        InvokeRepeating("refresh", 0, 1);
        pingtext = ping.GetComponent<TextMesh>();
        countdowntext = countdownobj.GetComponent<TextMesh>();
        countdowntext.text = countdown.ToString();
        countdown2.text = countdown.ToString();
        InvokeRepeating("secondrep", 1, 1);
        startrunning = false;
        playernametext = playernamedisply.GetComponent<TextMesh>();
        if(GameObject.Find("VRTK_SDK")) sceneswitch = GameObject.Find("VRTK_SDK").GetComponent<SceneSwitcherController>();
        FleetBuilder.read();
        PhotonVoiceSettings.Instance.DebugInfo = true;
        Save_current_Ship(DefaultBot);
        Save_current_Fleet(DefaultShipBot);
        if (PhotonNetwork.connectedAndReady) OnJoinedRoom();
        if (PhotonNetwork.inRoom == true && PhotonNetwork.isMasterClient)
        {
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = PhotonNetwork.room.CustomProperties;
            customRoomPropertiesToSet["st"] = "in Lobby";
            PhotonNetwork.room.SetCustomProperties(customRoomPropertiesToSet);
        }
        if (PhotonNetwork.offlineMode == false) ChangeRoomNumber.SetActive(false);
        else UpdateMaxPlayerDisplay();
        if(crosslevelvarholder.campaign == true)
        {
            StartMoneytext.text = crosslevelvarholder.campaignlevel.startmoney.ToString();
            OpposingShipstext.text = "";
            MissionName.text = crosslevelvarholder.campaignlevel.name;
            Storytext.text = crosslevelvarholder.campaignlevel.story;
            MissionObjective.text = getstringfromobjective( crosslevelvarholder.campaignlevel.objective);
            foreach (int o in crosslevelvarholder.campaignlevel.ships) OpposingShipstext.text = OpposingShipstext.text + "\n" + MainMenuCampaignControlScript.getshipnamebynumber(o);
            foreach (GameObject panel in panels) panel.SetActive(false);
            panels[crosslevelvarholder.campaignlevel.panelnum].SetActive(true);
        }
        if (crosslevelvarholder.tutorial == true)
        {
            startrunning = true;
            countdown = 3;
            Loadingobj.SetActive(true);
        }
    }

    /// <summary>
    /// returns the objective based on the objective enum
    /// </summary>
    /// <param name="input">the mission objective enum contained within the mission type class</param>
    /// <returns></returns>
    string getstringfromobjective (MainMenuCampaignControlScript.eMissionObjective input)
    {
        if (input == MainMenuCampaignControlScript.eMissionObjective.Destroyall) return "Destroy all";
        else if (input == MainMenuCampaignControlScript.eMissionObjective.killTarget) return "Kill Target";
        else return "Survive";
    }
   
    /// <summary>
    /// The map data is stored as an int based on the mapcon in the roomdata hash, this function converts the int back to the enum.
    /// </summary>
    /// <param name="input"> the networked int giving the map enum type</param>
    void setmapclientbasedonint (int input)
    {
        if (input == 0) directlysetmap(CrossLevelVariableHolder.mapcon.map1);
        if(input == 1) directlysetmap(CrossLevelVariableHolder.mapcon.map2);
        if (input   == 2) directlysetmap(CrossLevelVariableHolder.mapcon.map3);
    }

    /// <summary>
    /// When changing skybox, you arent directly setting that, just the roomdata hash containing it, this function changes it for everyone.
    /// </summary>
    /// <param name="input">the networked int giving the skybox enum type</param>
    void setskyboxclientbasedonint (int input)
    {
        if (input == 0) directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox1);
        if (input == 1) directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox2);
        if (input == 2) directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox3);
        if (input == 3) directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox4);
        if (input == 4) directlysetskybox(CrossLevelVariableHolder.skyboxcon.skybox5);
    }

    /// <summary>
    /// see above comment.
    /// </summary>
    ///  /// <param name="botdifficulty">the networked int giving the difficulty enum type</param>
    void setbotdifficultybasedonint(int botdifficulty)
    {
        if (botdifficulty == 0) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.easy;
        if (botdifficulty == 1) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.medium;
        if (botdifficulty == 2) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.hard;

        if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.medium)  botdificultytext.text = "Bot Difficulty = medium";
        else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.hard)   botdificultytext.text = "Bot Difficulty = hard";
        else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.easy)  botdificultytext.text = "Bot Difficulty = easy";
    }


    /// <summary>
    /// see above comment.
    /// </summary>
    ///  /// <param name="botdifficulty">the networked int giving the game mode enum type</param>
    void setgamemodebasedonint (int input)
    {
        if (input == 0)
        {
            Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
            crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
        }
        if (input == 1)
        {
            Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
            crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
        }
        if(input == 2)
        {
            Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
            crosslevelvarholder.Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
        }
        if (GameModeDisplay) GameModeDisplay.text = Gamemode.ToString();
    }

    /// <summary>
    /// see above comment.
    /// </summary>
    ///  /// <param name="botdifficulty">the networked int giving bots enabled bool</param>
    void setbotsbasedonbool (bool input)
    {
        crosslevelvarholder.bots = input;
        bots = input;
        if (crosslevelvarholder.bots == false)
        {
            Addbotstext.text = "add bots";
        }
        else
        {
            Addbotstext.text = "Remove bots";
        }
    }

    /// <summary>
    ///  gets sorted list of all players in room.
    /// </summary>
    /// <param name="input">the list of all players in the room</param>
    /// <returns>the list of all players in the room, but sorted</returns>
    public List<int> getplayerlistinorder (List<PhotonPlayer> input)
    {
        List<int> players = new List<int>();
        foreach(PhotonPlayer player in input) players.Add(player.ID);
        players.Sort();
        return players;
    }

    /// <summary>
    /// move the player based on their position within the room.
    /// </summary>
    /// <param name="a">The target position</param>
    void setLobbyPosition (int a)
    {
        GameObject cam = GameObject.Find("VRTK_SDK");
        transform.position = MenuLocations[a].transform.position;
        transform.rotation = MenuLocations[a].transform.rotation;
        cam.transform.position = CameraLocations[a].transform.position;
        cam.transform.rotation = CameraLocations[a].transform.rotation;
        cam.transform.Find("ResizeObj").transform.localPosition = new Vector3(0.119999f,4.86f,5.71f);
    }

    /// <summary>
    ///  updates list of players on someone else joining.
    /// </summary>
    /// <param name="other">the other person joining</param>
    void OnPhotonPlayerConnected( PhotonPlayer other )
    {
        List<int> playerlist = getplayerlistinorder(new List<PhotonPlayer>(PhotonNetwork.playerList));
        setLobbyPosition(playerlist.IndexOf(PhotonNetwork.player.ID));
    }
    
    /// <summary>
    /// room initialisation stuff.
    /// </summary>
    void OnJoinedRoom()
    {
        if (PhotonNetwork.isMasterClient == false) startbutton.SetActive(false);
        else startbutton.SetActive(true);
        ExitGames.Client.Photon.Hashtable hash =  PhotonNetwork.player.CustomProperties;
        SwitchBotDificulty();
        List<int> playerlist = getplayerlistinorder(new List<PhotonPlayer>( PhotonNetwork.playerList));
        setLobbyPosition(playerlist.IndexOf(PhotonNetwork.player.ID));
        if (PhotonNetwork.isMasterClient == true && getteam1().Count == 0) crosslevelvarholder.AsignTeam( PhotonNetwork.player.ID, GetStartTeam());
        Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
        MainMenuValueHolder valhol = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        if (crosslevelvarholder.campaign == false)
        {
            crosslevelvarholder.campaign = valhol.campaign;
            crosslevelvarholder.campaignlevel = valhol.selectedcampaign;
            if (valhol.selectedcampaign != null) crosslevelvarholder.map = valhol.selectedcampaign.map;
            if (valhol.selectedcampaign != null) crosslevelvarholder.skybox = valhol.selectedcampaign.skybox;

        }
        crosslevelvarholder.TestArea = valhol.testarea;
        crosslevelvarholder.tutorial = valhol.tutorial;
        
        if(crosslevelvarholder.campaign == true)
        {
            crosslevelvarholder.skybox = crosslevelvarholder.campaignlevel.skybox;
            crosslevelvarholder.map = crosslevelvarholder.campaignlevel.map;
        }
      
        if ( crosslevelvarholder.campaign == true)
        {
            pseudodisable(multimatchseg);
        }
        else pseudodisable(campaignseg);
        if (PhotonNetwork.isMasterClient && crosslevelvarholder.campaign == false && crosslevelvarholder.tutorial == false)
        {
            SwitchMode();
            float random = Random.value;
            if (random < 0.33f) map1();
            if (random > 0.33f && random < 0.66f) map2();
            if (random > 0.66f) map3();

            float random2 = Random.value;
            if (random2 < 0.2f) skybox1();
            if (random2 > 0.2f && random2 < 0.4f) skybox2();
            if (random2 > 0.4f && random2 < 0.6f) skybox3();
            if (random2 > 0.6f && random2 < 0.8f) skybox4();
            if (random2 > 0.8f) skybox5();
        }
        else if (crosslevelvarholder.campaign == true)
        {
            switch (crosslevelvarholder.map)
            {
                case CrossLevelVariableHolder.mapcon.map1: map1client(); break;
                case CrossLevelVariableHolder.mapcon.map2: map2client(); break;
                case CrossLevelVariableHolder.mapcon.map3: map3client(); break;
            }
            switch (crosslevelvarholder.skybox)
            {
                case CrossLevelVariableHolder.skyboxcon.skybox1: skybox1client(); break;
                case CrossLevelVariableHolder.skyboxcon.skybox2: skybox2client(); break;
                case CrossLevelVariableHolder.skyboxcon.skybox3: skybox3client(); break;
                case CrossLevelVariableHolder.skyboxcon.skybox4: skybox4client(); break;
                case CrossLevelVariableHolder.skyboxcon.skybox5: skybox5client(); break;
            }

        }
        else if (crosslevelvarholder.tutorial == true)
        {
            map1client();
            skybox3client();
        }
        Loadingobj.SetActive(false);
        refresh();
        if(PhotonNetwork.room != null)  if( PhotonNetwork.room.MaxPlayers == 4) ChangeRoomNumber.SetActive(true);
        UpdateMaxPlayerDisplay();
    }

    /// <summary>
    /// disables everything around an object, doesnt actually disable scripts and functions, just the meshes and colliders.
    /// </summary>
    /// <param name="disableobj"> the parent to the screen you want to disable</param>
    public void pseudodisable(GameObject disableobj)
    {
        foreach (MeshRenderer mesh in disableobj.GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        foreach (TextMesh text in disableobj.GetComponentsInChildren<TextMesh>()) text.characterSize = 0;
        foreach (BoxCollider box in disableobj.GetComponentsInChildren<BoxCollider>()) box.enabled = false;
    }

    

    /// <summary>
    /// gets the room you should be starting in.
    /// </summary>
    /// <returns>the team the player should want to join upon starting the match</returns>
    int GetStartTeam()
    {
       int output = 1;
       if (PhotonNetwork.inRoom) {

            if (getteam1().Count > getteam2().Count) output = 2;
            else if (getteam2().Count > getteam1().Count) output = 1;
            else if(getteam1().Count == getteam2().Count)
            {
                int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
                if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
                int maxplayersperteam = maxplayerstemp / 2;
                if (getteam1().Count < maxplayersperteam) output = 1;
                else output = 2;
            }
        
      }
      return output;
    }

    /// <summary>
    ///  allows you to bring the number of players upto 6 from 4 for bots in multiplayer or switch it to however you like in skirmish.
    /// </summary>
    public void ChangePlayerNumber ()
    {
        if (PhotonNetwork.offlineMode == true)
        {
            if (PhotonNetwork.room.MaxPlayers == 2)
            {
                PhotonNetwork.room.MaxPlayers = 4;
                ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
                hash["3v3"] = "No";
                PhotonNetwork.room.SetCustomProperties(hash);

            }
            else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
            {
                PhotonNetwork.room.MaxPlayers = 4;
                ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
                hash["3v3"] = "Yes";
                PhotonNetwork.room.SetCustomProperties(hash);

            }
            else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
            {
                PhotonNetwork.room.MaxPlayers = 2;
                ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
                hash["3v3"] = "No";
                PhotonNetwork.room.SetCustomProperties(hash);
            }
        }
        else
        {
            if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
            {
                PhotonNetwork.room.MaxPlayers = 4;
                ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
                hash["3v3"] = "Yes";
                PhotonNetwork.room.SetCustomProperties(hash);

            }
            else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
            {
                PhotonNetwork.room.MaxPlayers = 4;
                ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
                hash["3v3"] = "No";
                PhotonNetwork.room.SetCustomProperties(hash);
            }
        }
        UpdateMaxPlayerDisplay();
        refresh();
    }

    /// <summary>
    ///  updates the max player display based upon the room settings.
    /// </summary>
    void UpdateMaxPlayerDisplay ()
    {
        if (PhotonNetwork.room != null)
        {
            if (PhotonNetwork.room.MaxPlayers == 2) ChangeRoomNumberText.text = "one v one";  
            else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No") ChangeRoomNumberText.text = "two v two";
            else if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")   ChangeRoomNumberText.text = "three v three"; 
        }
    }

    /// <summary>
    /// sets the game mode via the hash.
    /// </summary>
    public void SwitchMode()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch) Gamemode = CrossLevelVariableHolder.gamemode.FreeForAll;
            else if (Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll) Gamemode = CrossLevelVariableHolder.gamemode.CapitalShip;
            else if (Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip) Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
            else Gamemode = CrossLevelVariableHolder.gamemode.TeamDeathMatch;
            if(Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll)
            {
                team1displaygam.SetActive(false);
                team2displaygam.SetActive(false);
            }
            else
            {
                team1displaygam.SetActive(true);
                team2displaygam.SetActive(true);
            }
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
            hash["GameMode"] = (int)Gamemode;
            PhotonNetwork.room.SetCustomProperties(hash);
            refresh();
        }
    }

    /// <summary>
    /// gets the team the player is not on and checks that team isnt full.
    /// </summary>
    /// <returns>returns the team the player will be joining.</returns>
    public int gettargetteam ()
    {
        int output = 1;
        int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
        if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
        int maxplayersperteam = maxplayerstemp / 2;
        if (getteam1().Contains(PhotonNetwork.player.ID) && getteam2().Count < maxplayersperteam) output = 2;
        if (getteam2().Contains(PhotonNetwork.player.ID) && getteam1().Count < maxplayersperteam) output = 1;
       
        return output;
    }

    /// <summary>
    /// Switch the local players team.
    /// </summary>
    public void SwitchTeam ()
    {
        Debug.Log(PhotonNetwork.player.ID + " " + gettargetteam());
        crosslevelvarholder.AsignTeam(PhotonNetwork.player.ID, gettargetteam());
        refresh();
    }

    /// <summary>
    /// the updating function that updates voice icons and keyboard user input.
    /// </summary>
    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) StartMatch();
        if (Input.GetKeyDown(KeyCode.Alpha1)) map1();
        if (Input.GetKeyDown(KeyCode.Alpha2)) skybox4();
        if (Input.GetKeyDown(KeyCode.Alpha3)) skybox5();
        if (Input.GetKeyDown(KeyCode.B)) SwitchBotDificulty();
        if (Input.GetKeyDown(KeyCode.S)) SwitchTeam();
        if (Input.GetKeyDown(KeyCode.I)) SwitchMode();
        if (Input.GetKeyDown(KeyCode.P)) map1();
        foreach (GameObject gam in icons) gam.SetActive(false);
        List<int> playerlist = getplayerlistinorder(new List<PhotonPlayer>(PhotonNetwork.playerList));

        if (PhotonNetwork.offlineMode == false)
        {
            test = PhotonVoiceNetwork.CurrentRoomName;

            foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice"))
            {
                if (gam.GetComponent<PhotonVoiceRecorder>().IsTransmitting || gam.GetComponent<PhotonVoiceSpeaker>().IsPlaying)
                {
                    int a = gam.GetComponent<PhotonView>().owner.ID;
                    int position = 1;
                    if (getteam1().Contains(a) == true)
                    {
                        int temp = getteam1().IndexOf(a);
                        if (temp == 0) position = 0;
                        if (temp == 1) position = 1;
                        if (temp == 2) position = 2;
                    }
                    if (getteam2().Contains(a) == true)
                    {
                        int temp = getteam2().IndexOf(a);
                        if (temp == 0) position = 3;
                        if (temp == 1) position = 4;
                        if (temp == 2) position = 5;
                    }
                    icons[position].SetActive(true);
                }
            }
        }
        wait++;
        if (wait == 200 && PhotonNetwork.inRoom == false) PhotonNetwork.LoadLevel(0);
    }

    /// <summary>
    /// A Repeating function that counts down until the match starts.
    /// </summary>
    void secondrep()
    {
         if (startrunning)
        {
            countdown--;
            if (countdown > -1)
            {
                countdowntext.text = countdown.ToString();
                countdown2.text = countdown.ToString();
            }
            if (countdown == 0 && PhotonNetwork.isMasterClient) startmatchactual();
        }
          
    }

    /// <summary>
    /// calculates hash key and returns data from room hash about teams.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static List<int> getteaminput(string input)
    {
        if (PhotonNetwork.room != null)
        {
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
            List<int> output = new List<int>();
            if ((int)hash[input +"a"] != 400) output.Add((int)hash[input + "a"]);
            if ((int)hash[input +"b"] != 400) output.Add((int)hash[input + "b"]);
            if ((int)hash[input +"c"] != 400) output.Add((int)hash[input + "c"]);
            return output;
        }
        else return null;
    }

    // gets the current team1 from the room hash.
    public static List<int> getteam1 ()    { return getteaminput("team1");   }



    // gets the current team2 from the room hash.
    public static List<int> getteam2 () { return getteaminput("team2"); }

    // sets hash team data based on provided keys.
    public static void setteaminput(List<int> teamsset,string targetteam)
    {
        if (PhotonNetwork.room != null)
        {
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
            if (teamsset.Count > 0) hash[targetteam +"a"] = teamsset[0]; else hash[targetteam + "a"] = 400;
            if (teamsset.Count > 1) hash[targetteam + "b"] = teamsset[1]; else hash[targetteam + "b"] = 400;
            if (teamsset.Count > 2) hash[targetteam + "c"] = teamsset[2]; else hash[targetteam + "c"] = 400;
            PhotonNetwork.room.SetCustomProperties(hash);
        }
    }

    /// <summary>
    /// sets team 1 in the room hash.
    /// </summary>
    /// <param name="teamsset">the new version of team 1</param>
    public static void setteam1 (List<int> teamsset)  { setteaminput(teamsset,"team1"); }

    /// <summary>
    /// sets team 2 in the room hash.
    /// </summary>
    /// <param name="teamsset">the new version of team 2</param>
    public static void setteam2 (List<int> teamsset) { setteaminput(teamsset, "team2"); }

    /// <summary>
    /// gets the key for the team number
    /// </summary>
    /// <param name="input">the team to key the hash key from</param>
    /// <returns>the hash key to access the teams</returns>
    public static string getstartkeystring (int input)
    {
        if (input == 1) return "team1";
        else return "team2";
                
    }

    /// <summary>
    /// checks the room hash and updates the lobby display.
    /// </summary>
    void refresh()
    {
        if (PhotonNetwork.room != null)
         {
            if (PhotonNetwork.isMasterClient == false) startbutton.SetActive(false);
            else startbutton.SetActive(true);
            team1 = getteam1();
            team2 = getteam2();
            foreach (int counter in getteam1()) if (PhotonPlayer.Find(counter) == null && counter != 300) crosslevelvarholder.removetoteam(counter,"team1");
            foreach (int counter in getteam2()) if (PhotonPlayer.Find(counter) == null && counter != 300) crosslevelvarholder.removetoteam(counter, "team2");
            if (getteam1().Contains(PhotonNetwork.player.ID) == false && getteam2().Contains(PhotonNetwork.player.ID) == false) crosslevelvarholder.addtoteam(PhotonNetwork.player.ID, getstartkeystring(GetStartTeam()));
            debugid = (PhotonNetwork.player.ID);
            team1Debug = getteam1();
            team2Debug = getteam2();
            playernametext.text = "player name: " + PhotonNetwork.playerName;
            pingtext.text = "ping: " + PhotonNetwork.networkingPeer.RoundTripTime.ToString() + "ms";
            statustext.text = "status: " + PhotonNetwork.connectionState.ToString();
            RoomNameDisplay.text = "Room name: " + PhotonNetwork.room.Name;
            foreach (GameObject gam in icons) if (gam.GetActive() == true) gam.SetActive(false);

            if (crosslevelvarholder.campaign == false)
            {
                setgamemodebasedonint((int)PhotonNetwork.room.CustomProperties["GameMode"]);
                setbotsbasedonbool((bool)PhotonNetwork.room.CustomProperties["Bots"]);
                setbotdifficultybasedonint((int)PhotonNetwork.room.CustomProperties["Botdif"]);
                setskyboxclientbasedonint((int)PhotonNetwork.room.CustomProperties["sky"]);
                setmapclientbasedonint((int)PhotonNetwork.room.CustomProperties["Scenery"]);
            }
        
            if (PhotonNetwork.inRoom)
            {
                foreach (TextMesh tex in displaytext)
                {
                    tex.text = "";
                }
                if (PhotonNetwork.room.MaxPlayers == 2)
                {
                     displaytext[0].text = getplayername(0,1);
                     displaytext[1].text = getplayername(0,2);
                }
                if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
                {
                     displaytext[0].text = getplayername(0, 1);
                     displaytext[1].text = getplayername(0, 2);
                     displaytext[2].text = getplayername(1, 1);
                     displaytext[3].text = getplayername(1, 2);
                }
                if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes")
                {

                    displaytext[0].text = getplayername(0, 1); 
                    displaytext[1].text = getplayername(0, 2);
                    displaytext[2].text = getplayername(1, 1);
                    displaytext[3].text = getplayername(1, 2);
                    displaytext[4].text = getplayername(2, 1);
                    displaytext[5].text = getplayername(2, 2);
                }
                foreach (PhotonPlayer pha in PhotonNetwork.playerList) Debug.Log(pha.ID);
                lobbystateindicatortext.text = "lobby state : " + (string)PhotonNetwork.room.CustomProperties["st"];
            }

 
            List<string> ships = loadships(fleet);
            string outstring = "";
            foreach (string ship in ships) if (ship != "nothing") outstring = outstring + "\n" + ship;
        }
    }

    /// <summary>
    /// gets the player name at position inside which team.
    /// </summary>
    /// <param name="index">the index of the player inside the team</param>
    /// <param name="team">the team to get the index from</param>
    /// <returns></returns>
    public string getplayername (int index,int team)
    {
        string output = "";
        if (crosslevelvarholder.bots == true) output = "bot";
        if (index != 300 )
        { 
          if (team == 1 && getteam1().Count > (index) && PhotonPlayer.Find(getteam1()[index]) != null) output = PhotonPlayer.Find(getteam1()[index]).NickName;
          if (team == 2 && getteam2().Count > (index ) && PhotonPlayer.Find(getteam2()[index]) != null) output = PhotonPlayer.Find(getteam2()[index]).NickName;
        }
        return output;
    }
  

  
    /// <summary>
    ///  starts the match countdown.
    /// </summary>
    public void StartMatch()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (startrunning == false)
            {
                phopview.RPC("StartMatchclient", PhotonTargets.AllViaServer);
                Startmatchwriting.text =  "     Reset";
                startmatchwriting2.text = "     Reset";
            }
            else if (countdown > 2)
            {
                phopview.RPC("stopstartmatchclient", PhotonTargets.All);
                Startmatchwriting.text = "Start Match";
                startmatchwriting2.text = "Start Match";
            }

        }

    }

    /// <summary>
    /// networked function that resets the countdown to the start of the match.
    /// </summary>
    [PunRPC]
    public void stopstartmatchclient ()
    {
        startrunning = false;
        countdown = 10;
        countdowntext.text = countdown.ToString();
        countdown2.text = countdown.ToString();
    }

    /// <summary>
    /// function to start the match countdown on the client.
    /// </summary>
    [PunRPC]
    public void StartMatchclient() { startrunning = true; }

    /// <summary>
    /// The function that switches the bot difficulty and updates it in the hash.
    /// </summary>
    public void SwitchBotDificulty()
    {
        if (PhotonNetwork.isMasterClient)
        {
            if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.easy) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.medium;
            else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.medium) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.hard;
            else if (crosslevelvarholder.botdifficulty == CrossLevelVariableHolder.BotDifficultyhol.hard) crosslevelvarholder.botdifficulty = CrossLevelVariableHolder.BotDifficultyhol.easy;
            ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
            hash["Botdif"] = (int)crosslevelvarholder.botdifficulty;
            PhotonNetwork.room.SetCustomProperties(hash);
            refresh();
        }
    }
     
    /// <summary>
    ///  calls the removing bots function.
    /// </summary>
    public void RemoveBots ()
    { if (PhotonNetwork.isMasterClient)   RemoveBotsClient(); }
       
    /// <summary>
    /// sets the removed bots status in the room hash.
    /// </summary>
    [PunRPC]
    public void RemoveBotsClient ()
    {
        if (crosslevelvarholder.bots == true)   crosslevelvarholder.bots = false;
        else    crosslevelvarholder.bots = true;
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
        hash["Bots"] = crosslevelvarholder.bots;
        PhotonNetwork.room.SetCustomProperties(hash);
        refresh();
    }

    /// <summary>
    /// leaves the room.
    /// </summary>
    public void Quit()
    {
        PhotonNetwork.LeaveRoom();
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// starts the match immidiately for all clients.
    /// </summary>
    void startmatchactual () {

        if(PhotonNetwork.isMasterClient)
        {    
                ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = PhotonNetwork.room.CustomProperties;
                customRoomPropertiesToSet["st"] = "in Game";
                PhotonNetwork.room.SetCustomProperties(customRoomPropertiesToSet);
            
            phopview.RPC("startmatchactualclient", PhotonTargets.AllViaServer);
        }
   
    }

    /// <summary>
    /// the function that starts the match immidiately clientside.
    /// </summary>
    [PunRPC]
    public void startmatchactualclient ()
    {
        if (PhotonNetwork.offlineMode == false)
        {
            PhotonNetwork.room.IsOpen = true;
            PhotonNetwork.room.IsVisible = true;
        }
        Loadingobj.SetActive(true); 
        PhotonNetwork.LoadLevel(3);
    }

    /// <summary>
    /// DEPRECIATED function that showed all ships in your fleets.
    /// </summary>
    /// <param name="fleet"></param>
    /// <returns></returns>
    List<string> loadships (int fleet)
    {
        string loadObj = "AttachPointsList" + fleet.ToString() + "num";
        string loadObj2 = "AttachPointsList" + fleet.ToString();
        List<string> shipnames = new List<string>();
        int o = FleetBuilder.get<int>(loadObj);
        if (o == 0)
            if (Ships.Length <  fleet - 1) Save_current_Ship(defaultparent.transform.Find(Ships[fleet - 1].name).gameObject);
        else
            for (int a = 0; a < o; a++) shipnames.Add(FleetBuilder.get<string>(loadObj2 + a));
       // FleetBuilder.read();
        return shipnames;
    }
    /// <summary>
    /// Strangely designed functions that used to send a message to all clients to change the map and skybox.
    /// This was removed when i switched to using a networked hash, but the structure remains
    /// TODO: fix it.
    /// </summary>
    public void map1() { if (PhotonNetwork.isMasterClient) map1client(); }
    public void map2() { if (PhotonNetwork.isMasterClient) map2client(); }
    public void map3()  {  if (PhotonNetwork.isMasterClient) map3client(); }

    public void skybox1() { if (PhotonNetwork.isMasterClient) skybox1client(); }
    public void skybox2() { if (PhotonNetwork.isMasterClient) skybox2client(); }
    public void skybox3() { if (PhotonNetwork.isMasterClient) skybox3client(); }
    public void skybox4() { if (PhotonNetwork.isMasterClient) skybox4client(); }
    public void skybox5() { if (PhotonNetwork.isMasterClient) skybox5client(); }

    [PunRPC]
    private void map1client () { changemapselected( CrossLevelVariableHolder.mapcon.map1); }
   
    [PunRPC]
    private void map2client()  { changemapselected( CrossLevelVariableHolder.mapcon.map2);  }
  
    [PunRPC]
    private void map3client()  { changemapselected( CrossLevelVariableHolder.mapcon.map3); }
  
    [PunRPC] 
    private void skybox1client() {  changeskyboxrendselected( CrossLevelVariableHolder.skyboxcon.skybox1);  }
  
    [PunRPC]
    private void skybox2client() {  changeskyboxrendselected( CrossLevelVariableHolder.skyboxcon.skybox2); }
    
    [PunRPC]
    private void skybox3client() {  changeskyboxrendselected( CrossLevelVariableHolder.skyboxcon.skybox3); }
  
    [PunRPC]
    private void skybox4client() { changeskyboxrendselected( CrossLevelVariableHolder.skyboxcon.skybox4); }
 
    [PunRPC]
    private void skybox5client() { changeskyboxrendselected( CrossLevelVariableHolder.skyboxcon.skybox5);    }

    /// <summary>
    /// directly set the map locally.
    /// </summary>
    /// <param name="maptoset">the map to use.</param>
    void directlysetmap (CrossLevelVariableHolder.mapcon maptoset)
    {
        for (int a = 0; a < 3; a++)
        {
            if (a == (int) maptoset) mapbuttonsrend[a].material = lightmat;
            else mapbuttonsrend[a].material = darkmat;
        }
        crosslevelvarholder.map = maptoset;
    }


    /// <summary>
    /// The function that changes the value in the hash to the appropriate value.
    /// </summary>
    /// <param name="maptoset">the map to change the hash value to</param>
    void changemapselected (CrossLevelVariableHolder.mapcon maptoset)
    {
        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
        hash["Scenery"] = (int)maptoset;
        PhotonNetwork.room.SetCustomProperties(hash);
        refresh();
    }

    /// <summary>
    /// directly set the skybox locally.
    /// </summary>
    /// <param name="skyboxtoset">the skybox to use.</param>
    void directlysetskybox(CrossLevelVariableHolder.skyboxcon skyboxtoset)
    {
        for (int a = 0; a < 5; a++)
        {
            if (a == (int)skyboxtoset) skyboxbuttonsrend[a].material = lightmat;
            else skyboxbuttonsrend[a].material = darkmat;
        }
        crosslevelvarholder.skybox = skyboxtoset;
    }

    /// <summary>
    /// The function that changes the value in the hash to the appropriate value.
    /// </summary>
    /// <param name="skyboxtoset">the skybox to change the hash value to</param>
    void changeskyboxrendselected (CrossLevelVariableHolder.skyboxcon skyboxtoset)
    {

        ExitGames.Client.Photon.Hashtable hash = PhotonNetwork.room.CustomProperties;
        hash["sky"] = (int)skyboxtoset;
        PhotonNetwork.room.SetCustomProperties(hash);
        refresh();
    }

    // DEPRECIATED
    void Save_current_Ship(GameObject saveobject)
    {
        int i = 0;
        List<GameObject> Children = new List<GameObject>();
        foreach (Transform child in saveobject.transform)
        {
            if (child.childCount > 0)
                Children.Add(child.GetChild(0).gameObject);
            else Children.Add(null);
        }

        foreach (GameObject stri in Children)
        {
            if (stri != null)
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, Children[i].name);
                Vector3 loc = Children[i].transform.localPosition;
                Vector3 rot = Children[i].transform.localEulerAngles;
                FleetBuilder.add(saveobject.name + "parent" + i, stri.transform.parent.name);
                FleetBuilder.write();
                FleetBuilder.add(saveobject.name + "num", i + 1);
                FleetBuilder.add(saveobject.name + "scalex" + i, stri.transform.localScale.x);
                FleetBuilder.add(saveobject.name + "scaley" + i, stri.transform.localScale.y);
                FleetBuilder.add(saveobject.name + "scalez" + i, stri.transform.localScale.z);
            }
            else
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, "nothing");
            }
            FleetBuilder.write();
            i++;
        }
    }

    // DEPRECIATED
    void Save_current_Fleet(GameObject input)
    {
        foreach (Transform saveobject in input.transform)
        {
            List<GameObject> Children = new List<GameObject>();
            int i = 0;
            Children.Clear();
            if (saveobject.transform.Find("Weapons").childCount > 0 && saveobject.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
                Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0).gameObject);
            else Children.Add(null);
            if (saveobject.transform.Find("Weapons").childCount > 1 && saveobject.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
                Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0).gameObject);
            else Children.Add(null);
            if (saveobject.transform.Find("Weapons").childCount > 2 && saveobject.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
                Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0).gameObject);
            else Children.Add(null);
            if (saveobject.transform.Find("Weapons").childCount > 3 && saveobject.transform.Find("Weapons").Find("Attachpoint 4").childCount > 0)
                Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 4").GetChild(0).gameObject);
            else Children.Add(null);
            if (saveobject.transform.Find("Weapons").childCount > 4 && saveobject.transform.Find("Weapons").Find("Attachpoint 5").childCount > 0)
                Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 5").GetChild(0).gameObject);
            else Children.Add(null);

            foreach (GameObject stri in Children)
            {
                if (stri != null)
                {

                    string fleetstring = saveobject.name + "bot" + i;
                    FleetBuilder.add(fleetstring, Children[i].name);
                    Vector3 loc = Children[i].transform.localPosition;
                    Vector3 rot = Children[i].transform.localEulerAngles;
                    FleetBuilder.add(saveobject.name + "bot" + "parent" + i, stri.transform.parent.name);
                  
                    FleetBuilder.add(saveobject.name + "bot" + "num", i + 1);
                    FleetBuilder.add(saveobject.name + "bot" + "scalex" + i, stri.transform.localScale.x);
                    FleetBuilder.add(saveobject.name + "bot" + "scaley" + i, stri.transform.localScale.y);
                    FleetBuilder.add(saveobject.name + "bot" + "scalez" + i, stri.transform.localScale.z);
                    FleetBuilder.add(saveobject.name + "bot" + "posx" + i, stri.transform.parent.localPosition.x);
                    FleetBuilder.add(saveobject.name + "bot" + "posy" + i, stri.transform.parent.localPosition.y);
                    FleetBuilder.add(saveobject.name + "bot" + "posz" + i, stri.transform.parent.localPosition.z);
                    //Destroy(stri);
                }
                else
                {
                    string fleetstring = saveobject.name + i;
                    FleetBuilder.add(fleetstring, "nothing");
           
                }
                FleetBuilder.write();

                i++;
            }
        }
    }
}
