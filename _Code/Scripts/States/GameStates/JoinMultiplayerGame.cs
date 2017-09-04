using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using DFNetwork.Simulation.Modules;
using simple;
using TrueSync;
using VRTK;

public class JoinMultiplayerGame : aGameState {

    // Photon Controller
    PhotonView pv;
    NetImplement_Photon netObject;
    DFNetwork.Simulation.SimulationLayer simObject;
    private IOComponent FleetBuilder;
    public bool visualUI;
    // room name
   public string roomName = "";
    public VRTK_SDKManager vrtk_sdkman;

    private GameController _gc;

    private bool Spawned = false;
    public bool alreadyspawned;
    private enum UIState {
        Connecting,
        Lobby,
        WaitingRoom,
        LoadingLevel,
        Game,
        Options
    }

    private Vector2 scrollPos = Vector2.zero;
    private UIState CurrentState = UIState.Connecting;

    /// <summary>
    /// Local cache of the module registry
    /// </summary>
    private ModuleTypeRegistry ModuleRegistry {
        get
        {
            return ModuleTypeRegistry.GetInstance;
        }
    }

    /// <summary>
    /// Get a module from the module registry
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    private T GetModule<T>(string name) where T : iModule {
        return (T)ModuleRegistry.GetModuleByName(name).Module;
    }

    void OnGUI() {

        switch (CurrentState) {

            case UIState.Connecting:
                Spawned = false;
                ShowConnecting();
                break;
            case UIState.Lobby:
                ShowLobby();
                break;
            case UIState.WaitingRoom:
              //  spawnallships();
                //ShowWaitingRoom();
                break;
            case UIState.LoadingLevel:
                //SpawnObjects();
                break;
            case UIState.Game:
                //ShowGame();
                break;
            case UIState.Options:
                //ShowOptions();
                break;

        }

    }

    public override void OnActive() {

        pv = this.GetComponent<PhotonView>();

        //Connect to the main photon server. This is the only IP and port we ever need to set(!)
        Debug.Log("Connecting to network");
        netObject.DoNetworkConnect();

        // Capture the "on joined" event
        netObject.OnNetworkConnect += NetConnected;
        netObject.OnRoomJoined += JoinedRoom;
        netObject.OnRoomLeft += LeftRoom;

        //Load name from PlayerPrefs
        
   
       
      
        // Set the room name to the player name
        roomName = PhotonNetwork.playerName + "'s Room";

        Debug.Log("Connecting to PUN");
        if (PhotonNetwork.connected == false)
        {
            PhotonNetwork.ConnectUsingSettings("0.1");
            PhotonNetwork.ConnectToBestCloudServer("0.1");
        }
      
    }


    #region Test Code

    void JoinedRoom() {

        CurrentState = UIState.WaitingRoom;
        //Camera.main.farClipPlane = 1000;

        // test packet
        //LS_NETSIM.NetworkLayer.Commands.LS_NetCommand cmd = new LS_NETSIM.NetworkLayer.Commands.LS_NetCommand("SpawnObject", 0, -1, (short)0, Vector3.zero);

        //SendPacket(cmd, PhotonTargets.All);

    }

    void LeftRoom() {

        // Change to lobby
        CurrentState = UIState.Lobby;

    }

    void NetConnected() {

        // Change to Lobby
        CurrentState = UIState.Lobby;

    }

    void OnMasterInstructLoad() {

        // Change to loading
        CurrentState = UIState.LoadingLevel;

    }

    void OnStartGame() {

        Camera.main.farClipPlane = 10000f;

        CurrentState = UIState.Game;

    }

    #endregion

    void ShowConnecting() {


    }
    public int getrooms ()
    {
        return PhotonNetwork.GetRoomList().Length;
    }
    public void Createroom ()
    {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true }, null);
    }
    void ShowLobby() {

        #region Waiting room GUI

        if (PhotonNetwork.room != null)
            return; //Only when we're not in a Room
        if (visualUI == false)
            return;

        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));

        GUILayout.Label("Main Menu");

        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        if (GUI.changed)//Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);


        //Join room by title
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("GO")) {
            PhotonNetwork.JoinRoom(roomName);
        }
        GUILayout.EndHorizontal();

        //Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("GO")) {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true }, null);
        }
        GUILayout.EndHorizontal();

        //Join random room
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
        if (/*PhotonNetwork.GetRoomList().Length == 0*/ false) {
            GUILayout.Label("..no games available...");
        } else {
            if (GUILayout.Button("GO")) {
                PhotonNetwork.JoinRandomRoom();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        GUILayout.Label("ROOM LISTING:");

        GUILayout.Space(30);
        GUILayout.Label("ROOM LISTING:");
        if (PhotonNetwork.GetRoomList().Length == 0) {
            GUILayout.Label("..no games available..");
        } else {
            //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (RoomInfo game in PhotonNetwork.GetRoomList()) {
                GUILayout.BeginHorizontal();
                GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                if (GUILayout.Button("JOIN")) {
                    PhotonNetwork.JoinRoom(game.name);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();


        #endregion

    }

    void ShowWaitingRoom() {

        #region Waiting room GUI

        GUILayout.BeginArea(new Rect((Screen.width - 400) / 2, (Screen.height - 300) / 2, 400, 300));
        {

            GUILayout.Label("Waiting Room");

            GUILayout.BeginVertical();
            {

                #region Players

                GUILayout.Label("Players:");

                // This is not working because the event fires before the room is actually left
                // work on the timing here
                if (true) {
                    // List of players
                    foreach (PhotonPlayer plyr in PhotonNetwork.playerList) {

                        GUILayout.BeginHorizontal(GUILayout.Width(200));
                        {

                            // Show the ready button
                            GUILayout.Toggle(GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer((short)plyr.ID), "Ready");

                            if (plyr.isMasterClient) {

                                // Show master client properties
                                GUILayout.Label("*Master* " + plyr.name);

                            } else {

                                // Show other properties
                                GUILayout.Label(plyr.name);

                            }

                        }
                        GUILayout.EndHorizontal();

                    }
                }

                #endregion

                #region Map Details

                // Master chooses map, show dropdown
                GUILayout.Label("The map will be \"Map1\"");

                #endregion

                #region Action Buttons

                GUILayout.BeginHorizontal();
                {

                    if (GUILayout.Button("Quit")) {

                        PhotonNetwork.LeaveRoom();

                    }

                    if (PhotonNetwork.isMasterClient) {

                        // Only show start if all players are ready
                        if (GetModule<MOD_ReadyState>("Ready State").AllPlayersReady) {

                            // Start Loading
                            CurrentState = UIState.LoadingLevel;

                            // Send the packet to load a level
                            //SceneManager.SendLevelToLoad("Map1");

                        } else {

                            GUILayout.Box("Start Game");

                        }

                    } else {

                        GUILayout.Box("Only master can start");

                    }

                    // Ready state button, only user can control theirs
                    if (GUILayout.Button(((GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer(netObject.GetCurrentPlayerID) == false) ? "I am ready" : "I am not ready"))) {
                        GetModule<MOD_ReadyState>("Ready State").SendReady(!GetModule<MOD_ReadyState>("Ready State").GetReadyStateForPlayer(netObject.GetCurrentPlayerID));
                    } 

                    //GUILayout.Button("");

                }
                GUILayout.EndHorizontal();

                #endregion

            }
            GUILayout.EndVertical();

        }
        GUILayout.EndArea();

        return;

        #region Other stuff

        /*
        //Player name
        GUILayout.BeginHorizontal();
        GUILayout.Label("Player name:", GUILayout.Width(150));
        PhotonNetwork.playerName = GUILayout.TextField(PhotonNetwork.playerName);
        if (GUI.changed)//Save name
            PlayerPrefs.SetString("playerName", PhotonNetwork.playerName);
        GUILayout.EndHorizontal();

        GUILayout.Space(15);


        //Join room by title
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN ROOM:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("GO"))
        {
            PhotonNetwork.JoinRoom(roomName);
        }
        GUILayout.EndHorizontal();

        //Create a room (fails if exist!)
        GUILayout.BeginHorizontal();
        GUILayout.Label("CREATE ROOM:", GUILayout.Width(150));
        roomName = GUILayout.TextField(roomName);
        if (GUILayout.Button("GO"))
        {
            PhotonNetwork.CreateRoom(roomName, true, true, 4);
        }
        GUILayout.EndHorizontal();

        //Join random room
        GUILayout.BeginHorizontal();
        GUILayout.Label("JOIN RANDOM ROOM:", GUILayout.Width(150));
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("..no games available...");
        }
        else
        {
            if (GUILayout.Button("GO"))
            {
                PhotonNetwork.JoinRandomRoom();
            }
        }
        GUILayout.EndHorizontal();

        GUILayout.Space(30);
        GUILayout.Label("ROOM LISTING:");
        if (PhotonNetwork.GetRoomList().Length == 0)
        {
            GUILayout.Label("..no games available..");
        }
        else
        {
            //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (RoomInfo game in PhotonNetwork.GetRoomList())
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(game.name + " " + game.playerCount + "/" + game.maxPlayers);
                if (GUILayout.Button("JOIN"))
                {
                    PhotonNetwork.JoinRoom(game.name);
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
        }

        GUILayout.EndArea();
		
		*/

        #endregion

        #endregion

    }

    public void spawnallships()
    {
      if(PhotonNetwork.connected == true)
      {
         if(PhotonNetwork.isMasterClient)
         {
            int i = 0;
            foreach (int a in crosslevelholder.team1wbots())
            {
                i++;
                if(a == 300)  SpawnObjects(true, i,4);
                
            }
            i = 3;
            foreach (int a in crosslevelholder.team2wbots())
            {
                i++;
                if (a == 300) SpawnObjects(true, i,4);
            }
        }
        else
        {
            int i = 0;
            foreach (int a in crosslevelholder.team1wbots())
            {
                i++;
                if (a == 300) spawnbotcontroller(getshipcolbot(i));

            }
                i = 3;
            foreach (int a in crosslevelholder.team2wbots())
            {
                i++;
                if (a == 300) spawnbotcontroller(getshipcolbot(i));
            }
        }
      }
      if(PhotonNetwork.connected) SpawnObjects(false, crosslevelholder.findspawnpos( PhotonNetwork.player.ID),4);
    }
    public _Ship.eShipColor getshipcolbot (int a)
    {
        _Ship.eShipColor ShipColor = _Ship.eShipColor.Green;
        switch (a)
        {
            case 1:
                ShipColor = _Ship.eShipColor.Green;
                break;
            case 2:
                ShipColor = _Ship.eShipColor.Blue;
                break;
            case 3:
                ShipColor = _Ship.eShipColor.White;
                break;
            case 4:
                ShipColor = _Ship.eShipColor.Yellow;
                break;
            case 5:
                ShipColor = _Ship.eShipColor.Grey;
                break;
            case 6:
                ShipColor = _Ship.eShipColor.Red;
                break;
        }
        return ShipColor;
    }

    // Spawn objects
   private void SpawnObjects(bool bot, int a, int spawnfleet) {
        Debug.Log(a);
      
        if ((bot && crosslevelholder.bots == true && crosslevelholder.TestArea == false && crosslevelholder.campaign == false) || (bot == false && alreadyspawned == false))
        {
            if (bot == false) alreadyspawned = true;
            Spawned = true;
            Debug.Log("Joined room \"" + PhotonNetwork.room.Name + "\"");
            if (PhotonNetwork.isMasterClient && PhotonNetwork.offlineMode == false) PhotonNetwork.room.IsOpen = true;
            CrossLevelVariableHolder crosslevelvarholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
            _gc.convertspawnpoints(crosslevelvarholder.Gamemode);
            Vector3 spawnLocation = spawnpoint(_gc,a);
            Quaternion lookDirection = Quaternion.identity;
            _Ship.eShipColor ShipColor = getspawncolor(a);
            string jsonFile = "TestFleet.json";
            string supercap = "Super-Capital";
         
            savefleetform2(loadshipsform1(spawnfleet));
            
            Vector3 relPos = Vector3.zero - spawnLocation;
            lookDirection = Quaternion.LookRotation(relPos);
            fleetFileSerializer handl = fleetFileSerializer.Load(jsonFile);
            Debug.Log("Loaded \"" + jsonFile + "\" with the fleet named \"" + handl.FleetName + "\"");
            Debug.Log(handl.Ships.Count);
            if (bot && crosslevelholder.tutorial == false) spawnbotcontroller(ShipColor);
            float i = 0;
            foreach (fleetFileSerializer.Ship shp in handl.Ships)
            {
                if (shp.Type != "nothing")
                {
                    Debug.Log(shp.Type);
                    int viewID = PhotonNetwork.AllocateViewID();
                    int playerID = PhotonNetwork.player.ID;
                    GetModule<MOD_SpawnObject>("Spawn Object").SendSpawnObject(shp.Type, (spawnLocation + new Vector3(0, 0, i)), lookDirection, viewID);
                    if(crosslevelholder.tutorial == false || bot == true) pv.RPC("fixturrets", PhotonTargets.All, viewID, shp.Weapon1, shp.Weapon2, shp.Weapon3, shp.Weapon4, shp.Weapon5, shp.WeaponPos1, shp.WeaponPos2, shp.WeaponPos3, shp.WeaponPos4, shp.WeaponPos5, shp.WeaponScale1, shp.WeaponScale2, shp.WeaponScale3, shp.WeaponScale4, shp.WeaponScale5, a, bot, (spawnLocation * 1000) + new Vector3(0, 0, i * 1000));
                    else pv.RPC("fixturrets", PhotonTargets.All, viewID, shp.Weapon1, shp.Weapon2, shp.Weapon3, shp.Weapon4, shp.Weapon5, shp.WeaponPos1, shp.WeaponPos2, shp.WeaponPos3, shp.WeaponPos4, shp.WeaponPos5, shp.WeaponScale1, shp.WeaponScale2, shp.WeaponScale3, shp.WeaponScale4, shp.WeaponScale5, a, bot,new Vector3(0, 0, i * 100) + new Vector3(1,1,1) );
                    i++;
                }
            }
            int viewID2 = PhotonNetwork.AllocateViewID();
            StartCoroutine("Waitforread");
            if (crosslevelholder.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
            {
                GetModule<MOD_SpawnObject>("Spawn Object").SendSpawnObject(supercap, ((spawnLocation * 1000) + new Vector3(0, 0, i * 1000)), lookDirection, viewID2);
                pv.RPC("fixShip", PhotonTargets.All, viewID2 ,a, (spawnLocation * 1000) + new Vector3(0, 0, i * 1000));
            }
            
        }
        if(bot && crosslevelholder.campaign == true)
        {
            _Ship.eShipColor ShipColor = getspawncolor(a);
            Debug.Log(ShipColor);
            AIController gam = spawnbotcontroller(ShipColor);
            _gc.convertspawnpoints(CrossLevelVariableHolder.gamemode.TeamDeathMatch);
            gam.setmission(crosslevelholder.campaignlevel, (spawnpoint(_gc,a)).ToTSVector());
           
        }
    }
    IEnumerator Waitforread ()
    {
        yield return new WaitForSeconds(3);
        PhotonView phop = GetComponent<PhotonView>();
        Debug.Log("callall");
        phop.RPC("ready", PhotonTargets.All,PhotonNetwork.player.ID);
    }
    [PunRPC]
    public void ready (int player)
    {
       
        transform.parent.GetComponent<UnitMovementcommandcontroller>().playerready(player);
    }
    _Ship.eShipColor getspawncolor (int a)
    {
        _Ship.eShipColor ShipColor = _Ship.eShipColor.Yellow;
        switch (a)
        {
            case 1: ShipColor = _Ship.eShipColor.Green;   break;
            case 2: ShipColor = _Ship.eShipColor.Blue;    break;
            case 3: ShipColor = _Ship.eShipColor.White;   break;  
            case 4:  ShipColor = _Ship.eShipColor.Yellow; break;    
            case 5: ShipColor = _Ship.eShipColor.Grey;    break;   
            case 6: ShipColor = _Ship.eShipColor.Red;     break; 
        }
        return ShipColor;
    }

    Vector3 spawnpoint (GameController _gc, int a)
    {
        Vector3 spawnLocation = Vector3.zero;
        switch (a)
        {
            case 1: spawnLocation = spawnLocation = _gc.SpawnPoint1; break;
            case 2: spawnLocation = spawnLocation = _gc.SpawnPoint2; break;
            case 3: spawnLocation = spawnLocation = _gc.SpawnPoint3; break;
            case 4: spawnLocation = spawnLocation = _gc.SpawnPoint4; break;
            case 5: spawnLocation = spawnLocation = _gc.SpawnPoint5; break;
            case 6: spawnLocation = spawnLocation = _gc.SpawnPoint6; break;
        }
        return spawnLocation;
    }

    public AIController spawnbotcontroller ( _Ship.eShipColor colour)
    {
        GameObject obj = new GameObject("AIController");
        obj.transform.parent = GameObject.Find("Controllers").transform;
        obj.transform.localPosition = new Vector3(0, 0, 0);
        AIController controller = obj.AddComponent<AIController>();
        controller.team = colour;
        return controller;
    }


    [PunRPC]
    public void fixturrets (int viewID, string Weapon1, string Weapon2,string Weapon3,string Weapon4,string Weapon5,  Vector3 WeaponPos1, Vector3 WeaponPos2, Vector3 WeaponPos3, Vector3 WeaponPos4, Vector3 WeaponPos5, Vector3 WeaponScale1, Vector3 WeaponScale2, Vector3 WeaponScale3, Vector3 WeaponScale4, Vector3 WeaponScale5, int botnum,bool bot,Vector3 input)
    {
        PhotonView.Find(viewID).gameObject.GetComponent<_Ship>().SpawnWeapons(Weapon1, Weapon2, Weapon3,Weapon4,Weapon5, WeaponPos1, WeaponPos2, WeaponPos3, WeaponPos4, WeaponPos5, WeaponScale1, WeaponScale2, WeaponScale3, WeaponScale4, WeaponScale5,botnum,bot,input);
    }
    [PunRPC]
    public void fixShip (int viewID,int spawnpos,Vector3 pos)
    {
        PhotonView.Find(viewID).GetComponent<_Ship>().spawnnum = spawnpos;
        PhotonView.Find(viewID).GetComponent<TSTransform>().position = pos.ToTSVector();
    }
    List<ShipCon> loadshipsform1(int fleet)
    {
        string loadObj = "AttachPointsList" + fleet.ToString() + "num";
        string loadObj2 = "AttachPointsList" + fleet.ToString();
        List<ShipCon> shipnames = new List<ShipCon>();
        int o = FleetBuilder.get<int>(loadObj);
        bool botin = false;
        if (fleet > 3) botin = true;
        for (int a = 0; a < o; a++) {
            List<GunCon> Weapons = LoadWeapons(FleetBuilder.get<string>(loadObj2 + a),botin);
            float scalex = FleetBuilder.get<float>(loadObj2 + "scalex" + a);
            float scaley = FleetBuilder.get<float>(loadObj2 + "scaley" + a);
            float scalez = FleetBuilder.get<float>(loadObj2 + "scalez" + a);
            List<string> WeaponNames = new List<string>();
            List<Vector3> WeaponPos = new List<Vector3>();
            List<Vector3> WeaponScale = new List<Vector3>();
            foreach (GunCon gun in Weapons)
            {
                WeaponNames.Add(gun.name);
                WeaponPos.Add(gun.pos);
                WeaponScale.Add(gun.scale);
            }
            shipnames.Add( new ShipCon( FleetBuilder.get<string>(loadObj2 + a) , new Vector3(scalex,scaley,scalez), WeaponNames,WeaponPos, WeaponScale));
        }

        FleetBuilder.read();
        return shipnames;
    }
    public void savefleetform2(List<ShipCon> ships)
    {
        string jsonFile = "TestFleet.json";
        fleetFileSerializer hnd = new fleetFileSerializer();
        hnd.FleetName = "Another Test Fleet"; 

        foreach (ShipCon ship in ships) {
            string[] shipweapon = new string[5] {null,null,null,null,null };
            for (int a = 0; a < 5; a++) if (ship.Weapons.Count > a) shipweapon[a] = ship.Weapons[a];
            Vector3[] shipweaponpos = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            for (int i = 0; i < 5; i++)  if (ship.Positions.Count > i) shipweaponpos[i] = ship.Positions[i];
            Vector3[] shipweaponscale = new Vector3[5] { new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 0) };
            for (int f = 0; f < 5; f++) if (ship.Scales.Count > f) shipweaponscale[f] = ship.Scales[f];
            hnd.Ships.Add(new fleetFileSerializer.Ship()
            {
                Type = ship.ship,
                Position = ship.loc,
                Weapon1 = shipweapon[0],
                Weapon2 = shipweapon[1],
                Weapon3 = shipweapon[2],
                Weapon4 = shipweapon[3],
                Weapon5 = shipweapon[4],
                WeaponPos1 = shipweaponpos[0],
                WeaponPos2 = shipweaponpos[1],
                WeaponPos3 = shipweaponpos[2],
                WeaponPos4 = shipweaponpos[3],
                WeaponPos5 = shipweaponpos[4],
                WeaponScale1 = shipweaponscale[0],
                WeaponScale2 = shipweaponscale[1],
                WeaponScale3 = shipweaponscale[2],
                WeaponScale4 = shipweaponscale[3],
                WeaponScale5 = shipweaponscale[4]
            });
      
        }
        
        fleetFileSerializer.Save(hnd, jsonFile);
    }

    #region Default Ships
    public void jsonsavedefaultshiplist (string jsonFile)
    {

        //// Create a temp file
        fleetFileSerializer hnd = new fleetFileSerializer();
        hnd.FleetName = "Another Test Fleet";

        /*
        // Add Super Capital (SuperCapitalOffense)
        hnd.Ships.Add(new fleetFileSerializer.Ship() {
            Type = "SuperCapitalOffense",
            Position = new Vector3(0, 0, 0)
        }); */

        // Add Capital (CapitalDefense)
        /*
        hnd.Ships.Add(new fleetFileSerializer.Ship() {
            Type = "CapitalDefense",
            Position = new Vector3(0, -1, 0)
        });

        // Add Destroyers (DestroyerOffense, DestroyerSupport)
        hnd.Ships.Add(new fleetFileSerializer.Ship() {
            Type = "DestroyerOffense",
            Position = new Vector3(-1, 0, 0)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship() {
            Type = "DestroyerSupport",
            Position = new Vector3(1f, 0, 0)
        });
        */
        //pathfinding errors, fix later
        // Add Cruisers (CruiserDefense, CruiserOffense)
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CruiserDefense",
            Position = new Vector3(-0.5f, 1, 0)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CruiserOffense",
            Position = new Vector3(0, 1, 0)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CruiserDefense",
            Position = new Vector3(0.5f, 1, 0)
        });

        // Add Frigates
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "FrigateOffense",
            Position = new Vector3(-1, 0, 2.5f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "FrigateOffense",
            Position = new Vector3(-0.5f, 0, 2.5f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "FrigateOffense",
            Position = new Vector3(0, 0, 2.5f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "FrigateOffense",
            Position = new Vector3(0.5f, 0, 2.5f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "FrigateOffense",
            Position = new Vector3(1, 0, 2.5f)
        });

        // Add Corvettes
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CorvetteOffense",
            Position = new Vector3(-1, 0, 3)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CorvetteSupport",
            Position = new Vector3(-0.5f, 0, 3)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CorvetteOffense",
            Position = new Vector3(0, 0, 3)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CorvetteSupport",
            Position = new Vector3(0.5f, 0, 3)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "CorvetteOffense",
            Position = new Vector3(1, 0, 3)
        });

        // Add fighters
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "LightCraftOffense",
            Position = new Vector3(-1, 0, 3.25f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "LightCraftDefense",
            Position = new Vector3(-0.5f, 0, 3.25f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "LightCraftSupport",
            Position = new Vector3(0, 0, 3.25f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "LightCraftTank",
            Position = new Vector3(0.5f, 0, 3.25f)
        });
        hnd.Ships.Add(new fleetFileSerializer.Ship()
        {
            Type = "LightCraftOffense",
            Position = new Vector3(1, 0, 3.25f)
        });

        fleetFileSerializer.Save(hnd, jsonFile);

    }
    #endregion

    public override void OnDeactivate() {
        /*Debug.Log("Disconnecting from PUN");
        PhotonNetwork.Disconnect(); */
    }

    public override void OnTransitionFrom() {
        
    }

    public override void OnTransitionTo() {

        /*PhotonNetwork.playerName = "plyr" + new System.Random().Next() % 99;

        Debug.Log("Player " + PhotonNetwork.playerName); */

    }

    public void OnJoinedLobby() {
        /*Debug.Log("Join Random Room");
        PhotonNetwork.JoinRandomRoom(); */
    }

    public void OnConnectedToMaster() {
        // when AutoJoinLobby is off, this method gets called when PUN finished the connection (instead of OnJoinedLobby())
        /*Debug.Log("Join Random Room");
        PhotonNetwork.JoinRandomRoom(); */
        PhotonNetwork.JoinLobby();
    }

    public void OnPhotonRandomJoinFailed() {
        /*Debug.Log("Could not join room, creating room");
        PhotonNetwork.CreateRoom("Tmp Room", new RoomOptions() { MaxPlayers = 2 }, null); */
    }

    void Awake() {
       netObject = GameObject.FindObjectOfType(typeof(NetImplement_Photon)) as NetImplement_Photon;
    }

    // Use this for initialization
    CrossLevelVariableHolder crosslevelholder;
    void Start()
    {
        if (PhotonNetwork.connectedAndReady) crosslevelholder = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        OnActive();
        // Get the game controller
        _gc = GameObject.FindObjectOfType<GameController>();
        simObject = this.GetComponent<DFNetwork.Simulation.SimulationLayer>();
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
   
    }
    // Update is called once per frame
    int i;
    void Update () {
        i++;
        if(i == 50)
        {
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
            {
                spawnallships();
            }
        }
	}

    List<GunCon> LoadWeapons(string loadObj, bool bot)
    {
        List<GunCon> output = new List<GunCon>();
        string defaultaddition = "";
        if (bot) defaultaddition = "bot";
        loadObj = loadObj + defaultaddition;
        int o = FleetBuilder.get<int>(loadObj + "num");
        for (int a = 0; a < o; a++)
        {
            string objparent = FleetBuilder.get<string>(loadObj + a);
            float posx = FleetBuilder.get<float>(loadObj + "posx" + a);
            float posy = FleetBuilder.get<float>(loadObj + "posy" + a);
            float posz = FleetBuilder.get<float>(loadObj + "posz" + a);
            float scalex = FleetBuilder.get<float>(loadObj + "scalex" + a);
            float scaley = FleetBuilder.get<float>(loadObj + "scaley" + a);
            float scalez = FleetBuilder.get<float>(loadObj + "scalez" + a);
            if (objparent != "nothing")output.Add(new GunCon(objparent,new Vector3(posx,posy,posz), new Vector3(scalex,scaley,scalez)));
            
        }
        FleetBuilder.read();
        return output;
    }
    public class ShipCon
    {
        public string ship;
        public Vector3 loc;
        public List<String> Weapons;
        public List<Vector3> Positions;
        public List<Vector3> Scales;
        public ShipCon(string nam, Vector3 startlo, List<String> Weaponsinput, List<Vector3> Positionsin, List<Vector3> Scalesin)
        {
            ship = nam;
            loc = startlo;
            Weapons = Weaponsinput;
            Positions = Positionsin;
            Scales = Scalesin;
        }
    }
    public class GunCon
    {
        public string name;
        public Vector3 pos;
        public Vector3 scale;
        public GunCon(string nam, Vector3 startlo, Vector3 scalelo )
        {
            name = nam;
            pos = startlo;
            scale = scalelo;
        }

     }
    
}
