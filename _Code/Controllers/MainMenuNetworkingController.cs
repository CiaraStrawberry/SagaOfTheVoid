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

/// <summary>
///  this class controlls the networking screen whilst the player is looking for a game.
/// </summary>
public class MainMenuNetworkingController : MonoBehaviour
{
    // this textmesh tells you how many players are in the game.
    public TextMesh PlayersOnlinetext;
    // the crosslevelvariableholder singleton.
    private CrossLevelVariableHolder crossvar;
    // list of texts to be used to show currently avaiable games.
    private List<TextMesh> matchtext = new List<TextMesh>();
    // the list of meshrenderes to show if the player is pointing at a game or not.
    private MeshRenderer[] backgroundrend = new MeshRenderer[5];
    // the button to allow the player to create a game.
    public GameObject creategamecontrol;
    // the button to allow the player to create a private game.
    public GameObject creategamecontrolprivate;
    // the text that shows the players name.
    private TextMesh namedisplaytext;
    // the text that shows the players ping.
    private TextMesh pingtext;
    // the text that shows how many pages of avaiabable games can be accessed.
    private TextMesh pagenumtext;
    // the text to show your connection to photon.
    private TextMesh statustext;
    // the material to show a button has been selected.
    public Material selectmat;
    // the material to show a button has not been selected.
    public Material nonselectmat;
    // the text to show steams or the oculus stores connection status.
    private TextMesh steamconnectiontext;
    // the players current ping.
    int ping;
    // the players name.
    private string playername;
    // the number of games avaiable.
    public int gamescount;
    // the vr_sdk controller.
    private VRTK_SDKManager vrtk_sdkman;
    // the vr_sdk controller attachment script that changes steam related stuff.
    public VRTK_SDK_CONTROLLER_MANAGER steammangercheck;
    // the textmesh to allow the player to toggle to only see their friends private games.
    public TextMesh ToggleStatus;
    // the toggled status of only seeing your friends private games.
    private bool friendslist = false;
    // the list of avaiable rooms.
    public List<RoomInfo> roomscurrent = new List<RoomInfo>();
    // the players player name according to the oculus store.
    public string oculusplayername;
    // A counter that tracks number of players.
    int counter;
    // has the player joined a room yet?
    bool joinedyet;
    // A chache holding the usersfriendlist.
    public List<string> friendslisttemp = new List<string>();
    // The loading icon that comes up during async loading.
    public GameObject Loading;

    /// <summary>
    /// A function to update the fact the player hasnt joined a room yet.
    /// </summary>
    void OnEnable()   {  joinedyet = false;  }
 
      
  
    /// <summary>
    /// Search initalisation stuff.
    /// </summary>
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
        InvokeRepeating("refreshrep", 0, 1);

        foreach (GameObject match in MultiplayerMatchDisplay) matchtext.Add(match.GetComponent<TextMesh>());
        Debug.Log("running?");
        pingtext = pingdisplay.GetComponent<TextMesh>();
        pagenumtext = pagenumdisplay.GetComponent<TextMesh>();
        for (int i = 0; i < 5; i++) backgroundrend[i] = background[i].GetComponent<MeshRenderer>();
        creategamecontrol.SetActive(false);
        creategamecontrolprivate.SetActive(false);
        steamconnectiontext = steamconnectionthing.GetComponent<TextMesh>();
        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true))  Loading.SetActive(true);
    }


   /// <summary>
   /// the callback from oculus to get the players name.
   /// </summary>
   /// <param name="msg"></param>
    private void GetLoggedInUserCallback(Message msg)
    {
        if (!msg.IsError) PhotonNetwork.playerName = msg.GetUser().OculusID;
        else Debug.Log(msg.GetError().Message);
    }


    /// <summary>
    /// The callback from oculus to get the players friends.
    /// </summary>
    /// <param name="msg"></param>
    private void GetLoggedInUserCallbackfriends(Message msg)
    {
        friendslisttemp.Clear();
        if (!msg.IsError) {
            foreach (var friend in msg.GetUserList())
            {
                Debug.Log(friend.OculusID);
                friendslisttemp.Add(friend.OculusID);
            }
        }
    }

    /// <summary>
    /// A function that repeats every second to check games status.
    /// </summary>
    void refreshrep()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            PhotonVoiceNetwork.Connect();
            if (steammangercheck.issteamversion == true &&  SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagAll) > 0) getfriendslist();
            if (PhotonNetwork.playerName != null) namedisplaytext.text = "Player Name: " + PhotonNetwork.playerName;
            if (steammangercheck.issteamversion == false) steamconnectiontext.text = "Oculus Connected";
            else steamconnectiontext.text = "SteamConnection: " + SteamAPI.IsSteamRunning().ToString();
            refresh();
        }
        else if (PhotonNetwork.connectionStateDetailed != ClientState.JoinedLobby)
        {
            counter++;
            if (counter == 3)
            {
                PhotonNetwork.ConnectUsingSettings("1.0");
                counter = 0;
            }
        }
        statustext.text = "status: " + PhotonNetwork.connectionStateDetailed;
        MainMenuValueHolder manvalueholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        MainMenuValueHolder manvalueholder2 = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true))
                Loading.SetActive(true);
        if ((manvalueholder.testarea == true || manvalueholder.tutorial == true || manvalueholder.campaign == true) && joinedyet == false)
        {
            joinedyet = true;
            onevone();
            Debug.Log("1v1");
        }
    }

    /// <summary>
    /// The update function that handels player input.
    /// </summary>
    void Update()
    {
        if (timer < 125) timer++;
        if (timer == 121) refresh();
        if (Input.GetKeyDown(KeyCode.Space)) twovtwo();
        if (Input.GetKeyDown(KeyCode.Alpha1)) JoinRoom();
        if (Input.GetKeyDown(KeyCode.K)) onevone();
        if (Input.GetKeyDown(KeyCode.Alpha3)) threevthree();
        if (Input.GetKeyDown(KeyCode.Alpha9)) createprivateroom();
        if (Input.GetKeyDown(KeyCode.Alpha0)) twovtwoprivate();
        if (joinedyet == false && PhotonNetwork.insideLobby == true) OnJoinedLobby();
       if(PhotonNetwork.playerName != null) roomName = PhotonNetwork.playerName + "'s Room";
    }


    /// <summary>
    /// Debug info to show the player has joined a room successfully.
    /// </summary>
    public void OnJoinedLobby() 
    {
        joinedyet = true;
        Debug.Log("part of: " + PhotonNetwork.networkingPeer.CloudRegion);
        Debug.Log("players connected in master: " + PhotonNetwork.countOfPlayersOnMaster);
    }

    /// <summary>
    /// the refresh function that changes things every second based on updated data.
    /// </summary>
    void refresh()
    {
        ToggleStatus.text = "Private Lobby Toggle: " + friendslist;
        if (steammangercheck.issteamversion == false)
        {
            Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            Users.GetLoggedInUserFriends().OnComplete((Message<UserList> msg) =>
            {
                if (msg.IsError == false)
                {
                    foreach (var friend in msg.Data)
                    {
                        Debug.Log("friend: " + friend);
                        friendslisttemp.Add(friend.OculusID);
                    }
                }
                else   Debug.Log(msg.GetError().Message);
            });
        }
        else    PhotonNetwork.playerName = PhotonNetwork.playerName = SteamFriends.GetPersonaName();

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
                    Debug.Log(PhotonNetwork.GetRoomList().Length + " games available..");
                    List<RoomInfo> gameslist = new List<RoomInfo>();
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
                    }
                    roomscurrent = gameslist;
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

            pingtext.text = "ping: " + PhotonNetwork.networkingPeer.RoundTripTime.ToString() + "ms";
            PlayersOnlinetext.text = "Current Players: " + PhotonNetwork.countOfPlayers.ToString();
            float page = Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 6);
            if (page == 0) page = 1;
            int temproomlist = roomlistcount + 1;
            pagenumtext.text = "page: " + temproomlist + " out of " + page.ToString();
        }
    }
   
   
    /// <summary>
    /// Start the wait until the game starts properly (to allow correct initialisation).
    /// </summary>
    /// <returns></returns>
    IEnumerator startwait()
    {
        yield return new WaitForSeconds(1);
        refresh();
    }

    /// <summary>
    /// gives the player the option to setup a 1v1, 2v2 or 3v3 match.
    /// </summary>
    public void creatematchdisplay()
    {
        creategamecontrol.SetActive(true);
        foreach (GameObject gam in MultiplayerMatchDisplay) gam.SetActive(false); 
    }

    /// <summary>
    /// gives the player the option to setup a 1v1, 2v2 or 3v3 private match.
    /// </summary>
    public void createprivateroom ()   { creategamecontrolprivate.SetActive(true);  }
 
       
  
    /// <summary>
    /// Setup a 1v1 private match that no-one can see, for debugging.
    /// </summary>
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

    /// <summary>
    /// Setup a 1v1 regular match.
    /// </summary>
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

    /// <summary>
    /// Setup a 2v2 regular match.
    /// </summary>
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

    /// <summary>
    /// Setup a 3v3 regular match.
    /// </summary>
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

    /// <summary>
    /// Setup a 1v1 private match.
    /// </summary>
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

    /// <summary>
    /// Setup a 2v2 private match.
    /// </summary>
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

    /// <summary>
    /// Setup a 3v3 private match.
    /// </summary>
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

    /// <summary>
    /// Toggle the private room list.
    /// </summary>
    public void joinroomprivate()
    {
        if (friendslist == false) friendslist = true;
        else friendslist = false;
    }

    /// <summary>
    /// Allows the player to join a selected room.
    /// </summary>
    public void JoinRoom()
    {

        PhotonNetwork.JoinRoom(roomscurrent[gamescount].Name);
        Loading.SetActive(true);
        PhotonNetwork.LoadLevel(1);
    }

    // UI Buttons
    public void button1 () { PhotonNetwork.JoinRoom(roomscurrent[0 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button2() { PhotonNetwork.JoinRoom(roomscurrent[1 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button3() { PhotonNetwork.JoinRoom(roomscurrent[2 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button4() { PhotonNetwork.JoinRoom(roomscurrent[3 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    public void button5() { PhotonNetwork.JoinRoom(roomscurrent[4 + (roomlistcount * 5)].Name); PhotonNetwork.LoadLevel(1); Loading.SetActive(true); }

    /// <summary>
    /// Gets the steam friendslist
    /// </summary>
    /// <returns>a list of friends list names</returns>
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

    /// <summary>
    /// Move one to the right on the game list.
    /// </summary>
    public void next()
    {
        if (roomlistcount < Mathf.Ceil(PhotonNetwork.GetRoomList().Length / 5)) roomlistcount++;
        refresh();
    }

    /// <summary>
    /// Move one to the left on the game list.
    /// </summary>
    public void last()
    {
        if (roomlistcount > 0) roomlistcount--;
        refresh();
        Debug.Log(roomlistcount);
    }

    /// <summary>
    /// Move one game up selected on the gameslist.
    /// </summary>
    public void up ()
    {
        if (gamescount > 0)gamescount--;
        updatecolour();
    }

    /// <summary>
    /// Move one game down selected on the gameslist.
    /// </summary>
    public void down ( )
    {
        if (gamescount < 4) gamescount++;
        updatecolour();
    }

    /// <summary>
    /// Updates the games selection color
    /// </summary>
    void updatecolour ()
    {
        foreach (MeshRenderer mesh in backgroundrend) mesh.material = nonselectmat;
        backgroundrend[gamescount].material = selectmat;
    }

    /// <summary>
    /// Returns the player to the start of the game.
    /// </summary>
    void returntonormal ()
    {
        if (Loading != null) Loading.SetActive(true);
        SceneManager.LoadSceneAsync(0);
    }
}