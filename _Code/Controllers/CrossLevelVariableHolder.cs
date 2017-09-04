using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CrossLevelVariableHolder : MonoBehaviour {
    public int fleet = 1;
    public gamemode Gamemode;
    public AudioClip musicmain;
    public AudioClip music1;
    public AudioClip music2;
    public AudioClip music3;
    //private List<int> team1 = new List<int>();
   // private List<int> team2 = new List<int>();
    public VRTK_SDKSetup SDKLOADEDCURRENTLY;
    public mapcon map;
    public skyboxcon skybox;
    public bool tutorial;
    public bool TestArea;
    public bool loading;
    public BotDifficultyhol botdifficulty;
    public Material[] skyboxes = new Material[3];
    public bool campaign;
    public bool campaignnextlevelset;
    public MainMenuCampaignControlScript.mapcontainer campaignlevel;
    public enum BotDifficultyhol {
        easy,
        medium,
        hard
    }
    public Material getskybox (skyboxcon input)
    {
        Material output = skyboxes[0];
        switch (input) {
            case skyboxcon.skybox1: output = skyboxes[0]; break;
            case skyboxcon.skybox2: output = skyboxes[1]; break;
            case skyboxcon.skybox3: output = skyboxes[2]; break;
            case skyboxcon.skybox4: output = skyboxes[3]; break;
            case skyboxcon.skybox5: output = skyboxes[4]; break;
        }
        return output;
    }
    public enum gamemode {
        TeamDeathMatch,
        FreeForAll,
        CapitalShip
    }
    public enum mapcon {
        map1,
        map2,
        map3
    }
    public enum skyboxcon {
        skybox1,
        skybox2,
        skybox3,
        skybox4,
        skybox5
    }


    public bool bots;
    // Use this for initialization
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
    }
   void Start() {
      DontDestroyOnLoad(this.gameObject);
      bots = true;
      Gamemode = gamemode.TeamDeathMatch;
    }

    public AudioClip getmusic()
    {
        AudioClip output = music1;
        float a = Random.value;
        if (a < 0.33f) output = music1;
        if (a > 0.33f && a < 0.66f) output = music2;
        if (a > 0.66f) output = music3;
        return output;
    }
    public void AsignTeam(int ownerID, int team)
    {
        List<int> team1 = LobbyScreenController.getteam1();
        List<int> team2 = LobbyScreenController.getteam2();
        int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
        if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
        int maxplayersperteam = maxplayerstemp / 2;
        if (team == 1 && team1.Count <= maxplayersperteam )
        {
            
            if (team2.Contains(ownerID)) team2.Remove(ownerID);
            if(team1.Contains(ownerID) == false) team1.Add(ownerID);
        }
        else if(team2.Count <= maxplayersperteam)
        {
            if (team1.Contains(ownerID)) team1.Remove(ownerID);
            if (team2.Contains(ownerID) == false) team2.Add(ownerID);
        }
        LobbyScreenController.setteam1( team1);
        LobbyScreenController.setteam2(team2);
    }
    
    public void addtoteam(int thingtoadd,string teamkey)
    {
        List<int> templist = LobbyScreenController.getteaminput(teamkey);
      //  Debug.Log(templist.Count);
        int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
        if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
        int maxplayersperteam = maxplayerstemp / 2;
        if (templist.Count <= maxplayersperteam)
        {
            if (templist.Contains(thingtoadd) == false) templist.Add(thingtoadd);
        }
        else Debug.Log("ERROR: TEAM FULL");

        LobbyScreenController.setteaminput(templist, teamkey);
    }
    public void removetoteam(int thingtoadd, string teamkey)
    {
        List<int> templist = LobbyScreenController.getteaminput(teamkey);
        if (templist.Contains(thingtoadd) == true) templist.Remove(thingtoadd);
        LobbyScreenController.setteaminput(templist,teamkey);
        Debug.Log("removed: " + thingtoadd + " from " + teamkey);
    }
    public int findteam( int check)
    {
        if (PhotonNetwork.inRoom)
        {
            int output = 2;
            if (LobbyScreenController.getteam1().Contains(check)) output = 1;
            else if (LobbyScreenController.getteam2().Contains(check)) output = 2;
            return output;
        }
        else return 1;
    }
    public List<int> team1wbots ()
    {
        List<int> output = LobbyScreenController.getteam1();
        int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
        if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
        while (output.Count < (maxplayerstemp / 2))
        {
            if (bots) output.Add(300);
        }
        LobbyScreenController.setteam1(output);
        return output;
    }
    public List<int> team2wbots()
    {
        List<int> output = LobbyScreenController.getteam2();
        int maxplayerstemp = PhotonNetwork.room.MaxPlayers;
        if (maxplayerstemp == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "Yes") maxplayerstemp = 6;
        while (output.Count < (maxplayerstemp / 2))
        {
            if(bots) output.Add(300);
        }
        LobbyScreenController.setteam2(output);
        return output;
    }
    public int findspawnpos (int input)
    {
        int output = 0;
        Debug.Log(LobbyScreenController.getteam1());
        if (LobbyScreenController.getteam1().Contains(input)) output = LobbyScreenController.getteam1().IndexOf(input) + 1;
        if (LobbyScreenController.getteam2().Contains(input)) output = 4 + LobbyScreenController.getteam2().IndexOf(input);
        return output;
    }
    public  _Ship.eShipColor findspawnteam (int input)
    {
        int inside = findspawnpos(input);
        _Ship.eShipColor output = _Ship.eShipColor.Green;
        if (inside == 1) output = _Ship.eShipColor.Green;
        if (inside == 2) output = _Ship.eShipColor.Blue;
        if (inside == 3) output = _Ship.eShipColor.White;
        if (inside == 4) output = _Ship.eShipColor.Yellow;
        if (inside == 5) output = _Ship.eShipColor.Grey;
        if (inside == 6) output = _Ship.eShipColor.Red;
        return output;
    }
    public void wipebots()
    {
        LobbyScreenController.setteam1(new List<int>());
            LobbyScreenController.setteam2(new List<int>());
    }
}
