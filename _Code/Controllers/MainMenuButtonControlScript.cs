using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using Oculus.Platform;
using Oculus;
using Oculus.Platform.Models;
using Oculus.Avatar;
using Steamworks;

public class MainMenuButtonControlScript : MonoBehaviour {
    private GameObject Top;
    private TextMesh TopText;
    private MainMenuSelector TopScript;
    private string topStartText;
    private GameObject Left;
    private TextMesh LeftText;
    private MainMenuSelector LeftScript;
    private string leftStartText;
    private GameObject Right;
    private TextMesh RightText;
    private MainMenuSelector RightScript;
    private string rightStartText;
    public GameObject rightsidebutton;
    private TextMesh rightsidetext;
    private MainMenuSelector Rightsidemainmenuselector;
    private string rightsidebuttonstarttext;
    public GameObject leftsidebutton;
    private TextMesh leftsidetext;
    private MainMenuSelector leftsidemenuselector;
    private string leftsidebuttonstarttext;
    public GameObject vbotsstartbutton;
    // Use this for initialization
    void Start()
    {
        Loading.SetActive(false);
        if (GameObject.Find("Team1wins(Clone)")) Destroy(GameObject.Find("Team1wins(Clone)"));
        if (GameObject.Find("Team2Wins(Clone)")) Destroy(GameObject.Find("Team2wins(Clone)"));
        Top = transform.Find("Main Button").Find("Top").gameObject;
        TopText = Top.transform.Find("New Text").GetComponent<TextMesh>();
        topStartText = TopText.text;
        TopScript = Top.transform.Find("Circle").GetComponent<MainMenuSelector>();
        Left = transform.Find("Main Button").Find("Left").gameObject;
        LeftText = Left.transform.Find("New Text").GetComponent<TextMesh>();
        leftStartText = LeftText.text;
        LeftScript = Left.transform.Find("Circle").GetComponent<MainMenuSelector>();
        Right = transform.Find("Main Button").Find("Right").gameObject;
        RightText = Right.transform.Find("New Text").GetComponent<TextMesh>();
        rightStartText = RightText.text;
        RightScript = Right.transform.Find("Circle").GetComponent<MainMenuSelector>();
        leftsidetext = leftsidebutton.transform.Find("New Text").GetComponent<TextMesh>();
        leftsidemenuselector = leftsidebutton.transform.Find("Circle").GetComponent<MainMenuSelector>();
        leftsidebuttonstarttext = leftsidetext.text;
        rightsidetext = rightsidebutton.transform.Find("New Text").GetComponent<TextMesh>();
        Rightsidemainmenuselector = rightsidebutton.transform.Find("Circle").GetComponent<MainMenuSelector>();
        rightsidebuttonstarttext = rightsidetext.text;
        vbotsstartbutton.SetActive(false);
        
        if (GameObject.Find("VRTK_SDK"))
        {
            steammangercheck = GameObject.Find("VRTK_SDK").GetComponent<VRTK_SDK_CONTROLLER_MANAGER>();
            if (steammangercheck.issteamversion == false)
            {
                Core.Initialize();
                Entitlements.IsUserEntitledToApplication().OnComplete(callbackMethod1);
                Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);
            }
        }
       
    }
    public GameObject Loading;
    private bool called;
    public VRTK_SDK_CONTROLLER_MANAGER steammangercheck;// Use this for initialization
    void Update ()
    {
        //if (Input.GetKeyDown(KeyCode.L)) callbackMethod1(new Message(new System.IntPtr()));
        if (Input.GetKeyDown(KeyCode.V)) twovtwo();
       if(PhotonNetwork.connectedAndReady && called == false)
        {
            loadup();
            called = true;
        }
    }
	
	// Update is called once per frame
	public void Multiplayer ()
    {
        TopText.text = "Find a game";
        TopScript.disableall();
        TopScript.FindAGame = true;
        LeftText.text = "ShipBuilder";
        LeftScript.disableall();
        LeftScript.ShipBuilder = true;
        RightText.text = "FleetBuilder";
        RightScript.disableall();
        RightScript.settings = false;
        RightScript.fleetbuilder = true;
        RightScript.returntomenu = false;
        rightsidetext.text = "Return to menu";
        Rightsidemainmenuselector.disableall();
        Rightsidemainmenuselector.returntomenu = true;
    }
    public void returntomenu ()
    {
        if (TopText == null)
        {
            Start();
        }
        TopText.text = topStartText;
        TopScript.disableall();
        TopScript.Multiplayer = true;
        LeftText.text = leftStartText;
        LeftScript.disableall();
        LeftScript.singleplayer = true;
        RightText.text = rightStartText;
        RightScript.disableall();
        RightScript.settings = true;
        rightsidetext.text = "Exit";
        Rightsidemainmenuselector.disableall();
        Rightsidemainmenuselector.exit = true;
       
    }
    
    public void fleetbulder ()
    {

    }

    public void SinglePlayer ()
    {
        TopText.text = "Campaign";
        TopScript.disableall();
        TopScript.TestArea = true; 
        LeftText.text = "Skirmish";
        LeftScript.disableall();
        LeftScript.vbots = true;
        RightText.text = "Tutorial";
        RightScript.disableall();
        RightScript.settings = false;
        RightScript.tutorial = true;
        RightScript.returntomenu = false;
        rightsidetext.text = "Return to menu";
        Rightsidemainmenuselector.disableall();
        Rightsidemainmenuselector.returntomenu = true;
    }
    public void Displayvbots ()
    {
        wait = "";
        vbotsstartbutton.SetActive(true);
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
    }

    public void OnDisconnectedFromPhoton()
    {
        PhotonNetwork.offlineMode = true;
        Debug.Log("offlinemode activated");
        if(wait == "1v1")   onevone();
        if (wait == "2v2") twovtwo();
        if (wait == "3v3") threevthree();
    }
    string wait;

    public void onevone ()
    {
        wait = "1v1";
        if (PhotonNetwork.offlineMode == true) createLobby(2,"No");
    }
   
    public void twovtwo ()
    {
        wait = "2v2";
        if (PhotonNetwork.offlineMode == true) createLobby(4,"No");
    }   

    public void threevthree ()
    {
        wait = "3v3";
        if (PhotonNetwork.offlineMode == true) createLobby(4,"Yes");
    }

    void createLobby (byte maxplayersin, string teststring)
    {
        Loading.SetActive(true);
        ExitGames.Client.Photon.Hashtable customRoomPropertiesToSet = new ExitGames.Client.Photon.Hashtable() { { "Fr", "true" }, { "Un", PhotonNetwork.playerName }, { "st", "in Lobby" }, { "3v3", teststring }, { "team1a", 400 }, { "team1b", 400 }, { "team1c", 400 }, { "team2a", 400 }, { "team2b", 400 }, { "team2c", 400 }, { "GameMode", 0 }, { "Bots", true }, { "Botdif", 0 }, { "sky", 0 }, { "Scenery", 0 } };
        string[] customlobby = new string[4] { "Fr", "Un", "st", "3v3" };
        //   PhotonNetwork.FindFriends(getfriendslist());
        PhotonNetwork.CreateRoom("OfflineMode", new RoomOptions() { MaxPlayers = maxplayersin, IsOpen = false, IsVisible = false, PublishUserId = true, CustomRoomProperties = customRoomPropertiesToSet, CustomRoomPropertiesForLobby = customlobby }, null);
        PhotonNetwork.automaticallySyncScene = false;
        PhotonNetwork.LoadLevel(1);
        Debug.Log("1v1");
    }
    public void loadup()
    {
        if(steammangercheck != null)
        {
            if (steammangercheck.issteamversion == false)
            {
                Entitlements.IsUserEntitledToApplication().OnComplete(callbackMethod1);
                Users.GetLoggedInUser().OnComplete(GetLoggedInUserCallback);

            }
            else PhotonNetwork.playerName = SteamFriends.GetPersonaName();
        }
        else
        {

        }
      
    }
    public void callbackMethod1(Message msg)
    {
        Debug.Log(msg.GetError());
        if (!msg.IsError) Debug.Log("Permission Check Passed");
        else PermissionDenied();

    }
    void PermissionDenied()
    {
        Debug.Log("PermissionDenied");
        GameObject.Find("Controllers").GetComponent<GalaxyController>().deniedsort();
        StartCoroutine(waitquit());
    }
    IEnumerator waitquit()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("Quit");
        UnityEngine.Application.Quit();

    }
    private void GetLoggedInUserCallback(Message msg)
    {
        if (!msg.IsError) PhotonNetwork.playerName = msg.GetUser().OculusID;
        Debug.Log(msg.GetUser().OculusID);
    }
}
