using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;


/// <summary>
/// This script contains the data relevant to the campaign and allows the user to change it.
/// </summary>
public class MainMenuCampaignControlScript : MonoBehaviour {

    // The classes to draw the campaign level data from.
    public static mapcontainer[] maps = new mapcontainer[8];
    // the number of lists of maps the player has cycled through.
    int countthrough;
    // the textmeshes containing the names of the campaign levels.
    public TextMesh[] bts = new TextMesh[4];
    // if the player completes a level, it fills out a tick, this is the tick.
    public TextMesh[] completedicons = new TextMesh[4];
    // the images used to show what the campaign levels are like.
    public GameObject[] panels = new GameObject[8];
    // the saving component used to keep player progress.
    private IOComponent FleetBuilder;
    // the number of campaign levels the player has completed.
    public int completedthrough;

    // a list of player objects for the levels to use.
    public enum eMissionObjective {
        Destroyall,
        Survive,
        killTarget,
    }

    // allows me to disable the players abilities to call in ships at lower levels to force them to learn their strengths and weaknesses.
    public enum eshipsavailable {
        fighers,
        frigates,
        all
    }



    /// <summary>
    /// Initialises the networking to be offline.
    /// </summary>
    void Start () {
        PhotonNetwork.Disconnect();
        PhotonNetwork.offlineMode = true;
        Populatemaps();
        loadshipstotext(0); 
    }


  
    /// <summary>
    /// signals that the player has been disconnected successfully.
    /// </summary>
    public void OnDisconnectedFromPhoton ()
    {
        PhotonNetwork.offlineMode = true;
        Debug.Log("offlinemode activated");
    }

    /// <summary>
    /// fills out the map containers with the data about each level.
    /// </summary>
	void Populatemaps ()
    {
        maps[0] = new mapcontainer("Basic Training",    new int[] { 1, 1, 1,1 },                CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox3, 0, 2000 ,eMissionObjective.Destroyall, "Practice your skills \nagainst drones in \npreperation for \ndefense of the  \nempire."                                                       , eshipsavailable.fighers );    // Scount x 4
        maps[1] = new mapcontainer("Core Outskirts",   new int[] { 3, 3, 3 },                   CrossLevelVariableHolder.mapcon.map2, CrossLevelVariableHolder.skyboxcon.skybox1, 1, 2000, eMissionObjective.Destroyall, "Rebels have been \nsending sorties into \ncivilian shiping lanes. \n\nDestroy their ships \nand protect our \neconomic interests."            , eshipsavailable.fighers );    // Bomber x 3
        maps[2] = new mapcontainer("System Patrol",     new int[] { 7,7,7,13 },                 CrossLevelVariableHolder.mapcon.map3, CrossLevelVariableHolder.skyboxcon.skybox4, 2, 4000, eMissionObjective.killTarget, "The outskirts have \nalways been a pit \nof despair and piracy. \n\n A Famed Pirate boss \nhas been located \nconducting a raid. \nend him."  , eshipsavailable.fighers );    // Missile frigate x 3
        maps[3] = new mapcontainer("Pirate Base",       new int[] { 6,2,2,2,2 },                CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox5, 3, 5500, eMissionObjective.Destroyall, "Our intelligence has \nlocated one of their \ndens. \nDestroy it."                                                                            , eshipsavailable.frigates);    // Flack frigate x 1, Fighter x 4
        maps[4] = new mapcontainer("Military Excursion", new int[] { 7,7,0 },                    CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox3, 4, 4000,eMissionObjective.Survive,    "We need you to make \na full excursion into \nrebel territory \nand recorver a high \nranking prisoner."                                      , eshipsavailable.frigates);    // Cannon frigate x 4, Fighters x 1
        maps[5] = new mapcontainer("All out war",       new int[] { 3,3,3,2,2,2,11,9 },         CrossLevelVariableHolder.mapcon.map2, CrossLevelVariableHolder.skyboxcon.skybox4, 5, 8000, eMissionObjective.Destroyall, "The rebels have \nlaunched an all out \nassault on the \nCapital. \n\nDestroy one of their \nen-route fleets."                                , eshipsavailable.frigates);    // Bomber x 3, Fighter x 3, Destroyer x 1, Support x 1
        maps[6] = new mapcontainer("Final Assault",     new int[] { 11,11,0 },                  CrossLevelVariableHolder.mapcon.map3, CrossLevelVariableHolder.skyboxcon.skybox3, 6, 6000, eMissionObjective.Survive,    "The Imperial palace \nis under siege! \n\nyou need to survive \nagainst the rebel \nfleets!", eshipsavailable.all     );    // Destroyer x 2, Flack Frigate x 4, Fighters x 3
        maps[7] = new mapcontainer("Last Hope",         new int[] { 12,11,11,3,3,3,3,3,2,2,13 },CrossLevelVariableHolder.mapcon.map1, CrossLevelVariableHolder.skyboxcon.skybox4, 7,12000, eMissionObjective.killTarget, "The rebel flagship is \nexposed. \n\nstrike now and \nend the rebellion."                                                                     , eshipsavailable.all     );    // Capital x 1, Destroyer x 2, Bomber x 6, fighter x 2
        GetCompleted();
        updatedisplay();
    }

	/// <summary>
    /// allows the player to interact with the level via the keyboard.
    /// </summary>
	void Update () {
        if (Input.GetKeyDown(KeyCode.Alpha1)) loadcampaignactual(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) loadcampaignactual(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) loadcampaignactual(2);
        if (Input.GetKeyDown(KeyCode.Alpha4)) loadcampaignactual(3);
        if (Input.GetKeyDown(KeyCode.Alpha5)) loadcampaignactual(4);
        if (Input.GetKeyDown(KeyCode.Alpha6)) loadcampaignactual(5);
        if (Input.GetKeyDown(KeyCode.Alpha7)) loadcampaignactual(6);
        if (Input.GetKeyDown(KeyCode.Alpha8)) loadcampaignactual(7);
        if (Input.GetKeyDown(KeyCode.RightArrow)) next();
        if (Input.GetKeyDown(KeyCode.LeftArrow)) last();
    }

    /// <summary>
    /// gets the current player progress through the levels.
    /// </summary>
    void GetCompleted ()
    {
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
        completedthrough = FleetBuilder.get<int>("Completednum");
        Debug.Log(completedthrough);
    }

    /// <summary>
    /// updates the display to show the data about each campaign level it contains ✓.
    /// </summary>
    void updatedisplay ()
    {
        foreach (GameObject gam in panels) if(gam && gam.gameObject != null) gam.SetActive(false);
        for (int i = 0; i < 4; i++) { 
            bts[i].text = maps[i + (countthrough * 4)].name;
            if(panels[maps[i + (countthrough * 4)].panelnum] != null) panels[maps[i + (countthrough * 4)].panelnum].SetActive(true);
            Debug.Log((i + (countthrough * 4)));
            if ((i + (countthrough * 4)) < completedthrough) completedicons[i].text = "✓";
            else if((i + (countthrough * 4)) < completedthrough + 2) completedicons[i].text = "";
            else completedicons[i].text = "x";

        }
    }

    /// <summary>
    /// moves the level display to the next page.
    /// </summary>
    public void next()
    {
        if(countthrough < 1) countthrough++;
        updatedisplay();
    }


    /// <summary>
    /// moves the level display to the last page.
    /// </summary>
    public void last ()
    {
        if (countthrough > 0) countthrough--;
        updatedisplay();
    }

    /// <summary>
    /// checks if the player is ready for that mission and if so starts it.
    /// </summary>
    /// <param name="map">which map the player is attempting to start.</param>
    void loadcampaign (int map) { if (completedicons[map].text != "x") loadcampaignactual(map); }
   
    /// <summary>
    /// Loads up the lobby level and starts boot up progress to begin the mission.
    /// </summary>
    /// <param name="map">the selected map.</param>
    void loadcampaignactual (int map)
    {
          MainMenuValueHolder valhol = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
          valhol.campaign = true;
          valhol.selectedcampaign = maps[map + (countthrough * 4)];
          GameObject.Find("Controllers").GetComponent<GalaxyController>().findagame();
    }

    // these functions all allow the player to see what is in the levels and select them.

    public void Button1() {  loadcampaign(0); }
   
    public void Button2() {  loadcampaign(1); }
     
    public void Button3() {  loadcampaign(2); }
  
    public void Button4() {  loadcampaign(3); }

    public void ButtonHover1() { loadshipstotext(0); }

    public void ButtonHover2() { loadshipstotext(1); }
    
    public void ButtonHover3() { loadshipstotext(2); }

    public void ButtonHover4() { loadshipstotext(3); }
 
    void loadshipstotext (int map)
    {
        mapcontainer mapcon = maps[map + (countthrough * 4)];
    }

    /// <summary>
    /// The function that allows the player to see what ships are inside a level via a list of ints.
    /// </summary>
    /// <param name="i">the ship designation by int</param>
    /// <returns>the ship designation by name</returns>
    public static string getshipnamebynumber(int i)
    {
        string output = null;
        switch (i)
        {
            case 0: output = "+Reinforcements"; break;
            case 1: output = "Scout"; break;
            case 2: output = "Fighter"; break;
            case 3: output = "Bomber"; break;
            case 4: output = "AttackCraft"; break;
            case 5: output = "MissileFrigate"; break;
            case 6: output = "FlackFrigate"; break;
            case 7: output = "CannonFriage"; break;
            case 8: output = "LazerFrigate"; break;
            case 9: output = "SupportFrigate"; break;
            case 10: output = "RepairFrigate"; break;
            case 11: output = "Destroyer"; break;
            case 12: output = "Capital"; break;
            case 13: output = "FlagShip"; break;
        }
        return output;
    }

    /// <summary>
    /// the class containing the data relevant to its level.
    /// </summary>
    public class mapcontainer {
        public string name;
        public int[] ships;
        public CrossLevelVariableHolder.mapcon map;
        public CrossLevelVariableHolder.skyboxcon skybox;
        public int panelnum;
        public int startmoney;
        public string story;
        public eMissionObjective objective;
        public eshipsavailable shipsavaiable;
        public mapcontainer(string namein, int[] shipsin, CrossLevelVariableHolder.mapcon mapin, CrossLevelVariableHolder.skyboxcon skyboxin,int panelnumin,int startmoneyin,eMissionObjective objectivein,string storyin,eshipsavailable shipsavaialbein)
        {
            name = namein;
            ships = shipsin;
            map = mapin;
            skybox = skyboxin;
            panelnum = panelnumin;
            startmoney = startmoneyin;
            story = storyin;
            objective = objectivein;
            shipsavaiable = shipsavaialbein;
        }
    }
   
}
