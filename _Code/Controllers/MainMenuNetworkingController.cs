using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using VRTK;
using Oculus.Platform;
using Oculus;
using Oculus.Platform.Models;
using Oculus.Avatar;
using Steamworks;
using ExitGames.Client;


public class MainMenuNetworkingController : MonoBehaviour
{
    public TextMesh PlayersOnlinetext;
    private CrossLevelVariableHolder crossvar;
    private List<TextMesh> matchtext = new List<TextMesh>();
    public GameObject[] MultiplayerMatchDisplay = new GameObject[5];
    public GameObject[] background = new GameObject [5];
    private MeshRenderer[] backgroundrend = new MeshRenderer[5];
    public GameObject creategamecontrol;
    public GameObject creategamecontrolprivate;
    public GameObject pingdisplay;
    public GameObject namedisplay;
    public GameObject pagenumdisplay;
    public GameObject status;
    public GameObject istrans;
    private TextMesh namedisplaytext;
    private TextMesh pingtext;
    private TextMesh pagenumtext;
    private TextMesh statustext;
    public Material selectmat;
    public Material nonselectmat;
    public GameObject steamconnectionthing;
    private TextMesh steamconnectiontext;
 //   private PhotonVoiceRecorder phop;
    int ping;
    private int timer = -1;
    private int countthrough = 1;
    private string playername;
    private string roomName;
    private bool count;
    public int gamescount;
    public int roomlistcount;
   // public GameObject vrtk_SdkManagerGam;
    private VRTK_SDKManager vrtk_sdkman;
   // public SceneSwitcherController sceneswitch;
    public VRTK_SDK_CONTROLLER_MANAGER steammangercheck;// Use this for initialization
    public TextMesh ToggleStatus;
    private bool friendslist = false;
    void OnEnable()
    {
        joinedyet = false;
    }
    public List<RoomInfo> roomscurrent = new List<RoomInfo>();
    void Start() { 
        MainMenuValueHolder manvalueholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        Loading.SetActive(false);
        
      if (manvalueholder.testarea == false && manvalueholder.tutorial == false && manvalueholder.campaign == false)
        {
            PhotonNetwork.offlineMode = false;
            PhotonNetwork.ConnectUsingSettings("1.0");
        }
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading = false;
        steammangercheck = GameObject.Find("VRTK_SDK").GetComponent<VRTK_SDK_CONTROLLER_MANAGER>();
       // phop = istrans.GetComponent<PhotonVoiceRecorder>();
        statustext = status.GetComponent<TextMesh>();
        ping = PhotonNetwork.networkingPeer.RoundTripTime;
        namedisplaytext = namedisplay.GetComponent<TextMesh>();
        InvokeRepeating("refreshrep",0,1);
        
        foreach (GameObject match in MultiplayerMatchDisplay) matchtext.Add(match.GetComponent<TextMesh>());
        Debug.Log("running?");
        pingtext = pingdisplay.GetComponent<TextMesh>();
        pagenumtext = pagenumdisplay.GetComponent<TextMesh>();
        for (int i = 0; i < 5; i++) backgroundrend[i] = background[i].GetComponent<MeshRenderer>();
        creategamecontrol.SetActive(false);
        creategamecontrolprivate.SetActive(false);
        steamconnectiontext = steamconnectionthing.GetComponent<TextMesh>();

        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true))
        {
            Loading.SetActive(true);
        }

    }
    public string oculusplayername;
    private void GetLoggedInUserCallback(Message msg)
    {
        if (!msg.IsError) PhotonNetwork.playerName = msg.GetUser().OculusID;
        else Debug.Log(msg.GetError().Message);
    }
    private void GetLoggedInUserCallbackfriends(Message msg)
    {
        friendslisttemp.Clear();
        if (!msg.IsError) {

           // UserList input =  msg.GetUserList();
            foreach (var friend in msg.GetUserList())
            {
                Debug.Log(friend.OculusID);
                friendslisttemp.Add(friend.OculusID);
            }
           // Debug.Log(msg.Data);
        }
      //  Debug.Log(msg);
    }
    void refreshrep()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            PhotonVoiceNetwork.Connect();
                if (steammangercheck.issteamversion == true)
                {
                    if (SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll) > 0)getfriendslist();
                }
                if (PhotonNetwork.playerName != null) namedisplaytext.text = "Player Name: " + PhotonNetwork.playerName;
                

                //  if (vrtk_sdkman.loadedSetup != null && vrtk_sdkman.loadedSetup.controllerSDKInfo.type.Name == "SDK_OculusController") steamconnectiontext.text = "Oculus Connected";
                if (steammangercheck.issteamversion == false) steamconnectiontext.text = "Oculus Connected";
                else steamconnectiontext.text = "SteamConnection: " + SteamAPI.IsSteamRunning().ToString();
                refresh();

        
            
        }
        else
        {
            if (PhotonNetwork.connectionStateDetailed != ClientState.JoinedLobby) ;
            {
                counter++;
                if (counter == 3)
                {
                    //      PhotonNetwork.connet();
                    PhotonNetwork.ConnectUsingSettings("1.0");

                    counter = 0;
                }
            }


        }
        statustext.text = "status: " + PhotonNetwork.connectionStateDetailed;
        MainMenuValueHolder manvalueholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        //  if (manvalueholder.testarea == false && manvalueholder.tutorial == false && manvalueholder.campaign == false)
        //  {
        //    PhotonNetwork.offlineMode = false;
        //     PhotonNetwork.ConnectUsingSettings("1.0");
        //  }
        // if (PhotonNetwork.connectionStateDetailed == ClientState.ConnectedToMaster && PhotonNetwork.offlineMode == false) PhotonNetwork.JoinLobby();


        MainMenuValueHolder manvalueholder2 = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true)){ 
                Loading.SetActive(true);
            }
        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true) && joinedyet == false)
        {
            
             joinedyet = true;
            onevone();
             Debug.Log("1v1");
        }
    }


    int counter;
    
    // Update is called once per frame
    void Update()
    {
        if (timer < 125) timer++;
        if (timer == 121)  Setup1();
        if (Input.GetKeyDown(KeyCode.Space)) twovtwo();
        if (Input.GetKeyDown(KeyCode.Alpha1)) JoinRoom();
        if (Input.GetKeyDown(KeyCode.K)) onevone();
        if (Input.GetKeyDown(KeyCode.Alpha3)) threevthree();
        if (Input.GetKeyDown(KeyCode.Alpha9)) createprivateroom();
        if (Input.GetKeyDown(KeyCode.Alpha0)) twovtwoprivate();
        if (joinedyet == false && PhotonNetwork.insideLobby == true) OnJoinedLobby();
       if(PhotonNetwork.playerName != null) roomName = PhotonNetwork.playerName + "'s Room";
    }
    bool joinedyet;
    public void OnJoinedLobby() 
    {
        joinedyet = true;
        Debug.Log("part of: " + PhotonNetwork.networkingPeer.CloudRegion);
        Debug.Log("players connected in master: " + PhotonNetwork.countOfPlayersOnMaster);
       
    
    }
 
    void Setup1()
    {
        refresh();   
    }
    void refresh()
    {
        ToggleStatus.text = "Private Lobby Toggle: " + friendslist;
        if (steammangercheck.issteamversion == false)
        {
            Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            //  Users.GetLoggedInUserFriends().OnComplete(GetLoggedInUserCallbackfriends);
            Users.GetLoggedInUserFriends().OnComplete((Message<UserList> msg) =>
            {
            //    Debug.Log(msg.Data[0].OculusID);
              if(msg.IsError == false) { 
                foreach (var friend in msg.Data)
                {
                    Debug.Log("friend: " + friend);
                    friendslisttemp.Add(friend.OculusID);
                }
              }
              else
              {
                 Debug.Log(msg.GetError().Message);
              }
            });
        }
        else
        {
            PhotonNetwork.playerName = PhotonNetwork.playerName = SteamFriends.GetPersonaName();
        }

        if (PhotonNetwork.connectedAndReady == true)
        {
            if (friendslist == false)
            {
                int roomliststart = ((roomlistcount) * 5);
                if (PhotonNetwork.GetRoomList().Length == 0)
                {
                    Debug.Log("..no games available..");
                    for (int p = 0; p < 5; p++) matchtext[p].text = "Slot empty";
                }
                else
                {
                    Debug.Log(PhotonNetwork.GetRoomList().Length +" games available..");
                    List <RoomInfo> gameslist = new List<RoomInfo>();
                    foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                    {
                        Debug.Log((string)game.CustomProperties["Fr"]);
                        if ((string)game.CustomProperties["Fr"] == "false")
                            gameslist.Add(game);
                    }
                    roomscurrent = gameslist;
                    //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
                    int i = 0;
                    int b = 0;
                    for (int a = 0; a < 5; a++) matchtext[a].text = "Slot empty";
                    foreach (RoomInfo game in gameslist)
                    {
                        if (i < 5 && b > (roomliststart - 1) && b < roomliststart + 5)
                        {
                            matchtext[i].text = game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers + " state: " + (string)game.CustomProperties["st"] + " 3v3: " + (string)game.CustomProperties["3v3"];
                            i++;
                        }
                        b++;
                    }

                }


            }
            else
            {
                int roomliststart = ((roomlistcount) * 5);
                if (PhotonNetwork.GetRoomList().Length == 0)
                {
                    Debug.Log("..no games available..");
                    for (int p = 0; p < 5; p++) matchtext[p].text = "Slot empty";
                }
                else
                {
                    List<RoomInfo> gameslist = new List<RoomInfo>();
                    foreach (RoomInfo game in PhotonNetwork.GetRoomList())
                    {
                        Debug.Log((string)game.CustomProperties["Un"]);
                        if ((string)game.CustomProperties["isFriendsOnly"] == "true" && friendslisttemp.Contains((string)game.CustomProperties["Un"]))
                            gameslist.Add(game);
                       // gameslist.Add(game);
                    }
                    roomscurrent = gameslist;
                    //Room listing: simply call GetRoomList: no need to fetch/poll whatever!
                    int i = 0;
                    int b = 0;
                    for (int a = 0; a < 5; a++) matchtext[a].text = "Slot empty";
                    foreach (RoomInfo game in gameslist)
                    {
                        if (i < 5 && b > (roomliststart - 1) && b < roomliststart + 5)
                        {
                            matchtext[i].text = game.Name + " " + game.PlayerCount + "/" + game.MaxPlayers + " state: " + (string)game.CustomProperties["st"] + " 3v3: " + (string)game.CustomProperties["3v3"];
                            i++;
                        }
                        b++;
                    }

                }


                //   Debug.Log(PlatformInitialize.MyOculusID);
              
            }
  
                pingtext.text = "ping: " + PhotonNetwork.networkingPeer.RoundTripTime.ToString() + "ms";
                PlayersOnlinetext.text = "Current Players: " + PhotonNetwork.countOfPlayers.ToString();
                float page = Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 6);
                if (page == 0) page = 1;
                int temproomlist = roomlistcount + 1;
                pagenumtext.text = "page: " + temproomlist + " out of " + page.ToString();
              //  updatecolour();
        }
    }
   
    public List<string> friendslisttemp = new List<string>();
    IEnumerator startwait()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("eh");
        Setup1();
    }
    public void creatematchdisplay()
    {
        creategamecontrol.SetActive(true);
        foreach (GameObject gam in MultiplayerMatchDisplay) gam.SetActive(false); 
    }
    public void createprivateroom ()
    {




        creategamecontrolprivate.SetActive(true);
    }
    public void onevoneninvisible ()
    {
        Loading.SetActive(true);
        //    PhotonNetwork.FindFriends(getfriendslist());
        ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "false" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "No" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
        string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = false, IsVisible = false, PublishUserId = true,CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
        PhotonNetwork.automaticallySyncScene = false;
        //  sceneswitch.LoadLevel();
        PhotonNetwork.LoadLevel(1);
        Debug.Log(PhotonNetwork.room.CustomProperties);
        Debug.Log("1v1");
    }
    public void onevone ()
    {
        if (PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null  && PhotonNetwork.playerName != "" && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false )
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "false" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "No" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
            //    PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet,CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log(PhotonNetwork.room.CustomProperties);
            Debug.Log("1v1");
        }
    }
    public void twovtwo ()
    {
        if ((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != "" || PhotonNetwork.offlineMode == true) && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false )
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "false" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "No" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
            //  PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("2v2");
        }
        else
        {
            Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
        }
    } 
    public void threevthree ()
    {
        if ((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != "" || PhotonNetwork.offlineMode == true) && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false)
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "false" }, { "Un", PhotonNetwork.playerName}, { "st", "in Lobby" }, {"3v3","Yes" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
            //PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("3v3");
        }
        else
        {
            Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
        }
    }
    public void onevoneprivate()
    {
        if ((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != "" || PhotonNetwork.offlineMode == true) && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false)
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "true"}, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "No" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
            //   PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 2, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("1v1");
        }
        else
        {
            Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
        }
    }
    public void twovtwoprivate()
    {
        if ((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != "" || PhotonNetwork.offlineMode == true) && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false)
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "true" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "No" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, {"Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
            //    PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("2v2");
        }
        else
        {
            Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
        }
    }
    public void threevthreeprivate()
    {
        if ((PhotonNetwork.connectedAndReady && PhotonNetwork.playerName != null && PhotonNetwork.playerName != ""  || PhotonNetwork.offlineMode == true) && GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().currentlyloading == false)
        {
            Loading.SetActive(true);
            ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "true" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", "Yes" }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, {"GameMode",0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
            string[] customlobby = new string[4] { "Fr", "Un", "st","3v3" };
            //  PhotonNetwork.FindFriends(getfriendslist());
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = 4, IsOpen = true, IsVisible = true, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
            PhotonNetwork.automaticallySyncScene = false;
            PhotonNetwork.LoadLevel(1);
            Debug.Log("3v3");
        }
        else
        {
            Debug.Log("ERROR: PLAYER NOT CONNECTED AND READY");
        }
    }
 
    public void joinroomprivate ()
    {
        
            if (friendslist == false) friendslist = true;
            else friendslist = false;
        
        
    }
    public void JoinRoom ()
    {
        
        PhotonNetwork.JoinRoom(roomscurrent[gamescount].Name);
   //     sceneswitch.LoadLevel();
            Loading.SetActive(true);
        PhotonNetwork.LoadLevel(1);
    }
    public void button1 () { PhotonNetwork.JoinRoom(roomscurrent[0 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button2() { PhotonNetwork.JoinRoom(roomscurrent[1 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button3() { PhotonNetwork.JoinRoom(roomscurrent[2 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button4() { PhotonNetwork.JoinRoom(roomscurrent[3 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button5() { PhotonNetwork.JoinRoom(roomscurrent[4 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public string[] getfriendslist ()
    {
        friendslisttemp.Clear();
        int friendscount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll);
        string[] output = new string[friendscount];
        for (int i = 0;i < friendscount;i++)
        {
            output[i] = SteamFriends.GetFriendPersonaName( SteamFriends.GetFriendByIndex(i,EFriendFlags.k_EFriendFlagAll));
            friendslisttemp.Add(output[i]);
        }
        
        return output;
    }
    public void next (){

        if (roomlistcount < Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 5)) roomlistcount++;
        refresh();
    }
    public void last ()
    {
        
        if (roomlistcount > 0) roomlistcount--;
        refresh();
        Debug.Log(roomlistcount);
        
    }
    public void up ()
    {
        if (gamescount > 0)gamescount--;
        updatecolour();
    }
    public void down ( )
    {
        if (gamescount < 4) gamescount++;
        updatecolour();
    }
    void updatecolour ()
    {
        foreach (MeshRenderer mesh in backgroundrend)
        {
            mesh.material = nonselectmat;
        }
        backgroundrend[gamescount].material = selectmat;
    }
    public GameObject Loading;
    void returntonormal ()
    {
        if (Loading != null) Loading.SetActive(true);
        SceneManager.LoadSceneAsync(0);
    }
}