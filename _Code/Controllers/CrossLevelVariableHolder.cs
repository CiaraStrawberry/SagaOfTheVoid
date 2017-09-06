using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

/// <summary>
/// This class acts as a singleton to hold the variables needed to be stored across levels relevant to the player.
/// </summary>
public class CrossLevelVariableHolder : MonoBehaviour {

    // the instance of the gamemode enum kept there to track the current gamemode if in skirmish or multiplayer.
    public gamemode Gamemode;
    // the currently asigned music track.
    public AudioClip musicmain;
    // the first music track to potentially use should have just used an array tbh.
    public AudioClip music1;
    // the second music track to potentially use.
    public AudioClip music2;
    // the third music track to potentially use.
    public AudioClip music3;
    // the currently asigned map enum.
    public mapcon map;
    // the currently asign skybox enum.
    public skyboxcon skybox;
    // the check if the current level is the tutorial.
    public bool tutorial;
    // the check if the level is currently loading.
    public bool loading;
    // the currently asigned bot difficulty enum
    public BotDifficultyhol botdifficulty;
    // the skyboxes to choose from.
    public Material[] skyboxes = new Material[3];
    // the bool to remeber if this is current inside a campaign mission
    public bool campaign;
    // If the last mission was passed, if so set the level after it, or if it was failed, if so set the same level.
    public bool campaignnextlevelset;
    // if we are doing a campaign mission, this describes which one, DO NOT CALL IF CAMPAIGN IS FALSE.
    public MainMenuCampaignControlScript.mapcontainer campaignlevel;
    // the bool if there are bots on this multiplayer match.
    public bool bots;

    // the enum containing the bot difficulty.
    public enum BotDifficultyhol {
        easy,
        medium,
        hard
    }

    // the gamemode holder enum
    public enum gamemode {
        TeamDeathMatch,
        FreeForAll,
        CapitalShip
    }

    // the current scenery container.
    public enum mapcon {
        map1,
        map2,
        map3
    }

    // the current skybox contaier
    public enum skyboxcon {
        skybox1,
        skybox2,
        skybox3,
        skybox4,
        skybox5
    }

    /// <summary>
    /// The skybox material currently in use.
    /// </summary>
    /// <param name="input"> the skybox enum relevant</param>
    /// <returns>returns a material to set the skybox as.</returns>
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

   /// <summary>
   /// enforce singleton pattern.
   /// </summary>
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
    }

    /// <summary>
    /// Initialise the script.
    /// </summary>
    void Start() {
      DontDestroyOnLoad(this.gameObject);
      bots = true;
      Gamemode = gamemode.TeamDeathMatch;
    }

    /// <summary>
    /// Returns a random music track to use.
    /// </summary>
    /// <returns> Returns the music track in question</returns>
    public AudioClip getmusic()
    {
        AudioClip output = music1;
        float a = Random.value;
        if (a < 0.33f) output = music1;
        if (a > 0.33f && a < 0.66f) output = music2;
        if (a > 0.66f) output = music3;
        return output;
    }

    /// <summary>
    /// Asign the current player to a team at the start of a match, this uses a rooms custom properties to ensure continuaty across the entire program.
    /// </summary>
    /// <param name="ownerID">The PhotonID to add to the customproperties</param>
    /// <param name="team">the team to add the PhotonID to.</param>
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
    
    /// <summary>
    /// Asign the current player to a team after the match starts.
    /// </summary>
    /// <param name="thingtoadd">The players PhotonID</param>
    /// <param name="teamkey">The team to add the PhotonID too.</param>
    public void addtoteam(int thingtoadd,string teamkey)
    {
        List<int> templist = LobbyScreenController.getteaminput(teamkey);
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

    /// <summary>
    /// Removes the team in the event of a player leaving.
    /// </summary>
    /// <param name="thingtoadd">The left players PhotonID.</param>
    /// <param name="teamkey">The left players Team</param>
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

    /// <summary>
    /// Adds bots to the teams in the custom properties.
    /// </summary>
    /// <returns>returns a full list of players and bots</returns>
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

    /// <summary>
    /// Adds bots to the teams in the custom properties.
    /// </summary>
    /// <returns>returns a full list of players and bots</returns>
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

    /// <summary>
    /// find the spawn position (with an int) from the starting position of the player.
    /// </summary>
    /// <param name="input"></param>
    /// <returns>returns the spawn position to spawn into</returns>
    public int findspawnpos (int input)
    {
        int output = 0;
        Debug.Log(LobbyScreenController.getteam1());
        if (LobbyScreenController.getteam1().Contains(input)) output = LobbyScreenController.getteam1().IndexOf(input) + 1;
        if (LobbyScreenController.getteam2().Contains(input)) output = 4 + LobbyScreenController.getteam2().IndexOf(input);
        return output;
    }

    /// <summary>
    /// Finds the spawn team from its spawn position.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
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

    /// <summary>
    /// Remove all bots from the team lists and make those entities null.
    /// </summary>
    public void wipebots()
    {
         LobbyScreenController.setteam1(new List<int>());
         LobbyScreenController.setteam2(new List<int>());
    }
}
