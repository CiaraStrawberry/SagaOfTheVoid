using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class MainMenuSelector : MonoBehaviour {
    public bool Multiplayer;
    public bool singleplayer;
    public bool settings;
    public bool returntomenu;
    public bool exit;
    public bool FindAGame;
    public bool TestArea;
    public bool ShipBuilder;
    public bool vbots;
    public bool right;
    public bool left;
    public bool fleetbuilder;
    public bool tutorial;
    public bool rightfind;
    public bool leftfind;
    public bool creatematch;
    public bool joinrandom;
    private int cooldown;
	// Use this for initialization
	void Start () {
    }
	
	// Update is called once per frame
	void Update () {
        cooldown++;
	}
    void ButtonPress ()
    {
        if (Multiplayer && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().findagameactual(); cooldown = 0; }
        else if (singleplayer && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().SinglePlayerSort(); cooldown = 0; }
        else if (settings && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().SettingsSort(); cooldown = 0; }
        else if (ShipBuilder && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().shipbuilderbuilder(); cooldown = 0; }
        else if (returntomenu && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().returntonormal(); cooldown = 0; }
        else if (fleetbuilder && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().fleetbuildersort(); cooldown = 0; }
        else if (FindAGame && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().findagame(); cooldown = 0; }
        else if (TestArea && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().CampaignTrailsort(); cooldown = 0; }
        else if (vbots && cooldown > 60) { GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().Displayvbots(); cooldown = 0; }
        else if (tutorial && cooldown > 60) { GameObject.Find("Controllers").GetComponent<GalaxyController>().Tutorial(); cooldown = 0; }
        else if (exit && cooldown > 60) Quit();
        if (rightfind) GameObject.Find("FindGameParent").GetComponent<MainMenuNetworkingController>().next();
            if (leftfind) GameObject.Find("FindGameParent").GetComponent<MainMenuNetworkingController>().last();
            if (creatematch) Debug.Log("ERROR: NEED TO SPECIFY IN WHAT WAY");
    }
    void Quit ()
    {
        if (GameObject.Find("VRTK_SDK").GetComponent<VRTK_SDK_CONTROLLER_MANAGER>().issteamversion == true)
        {
            if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
        else
        {
            Application.Quit();
        }
    }
    public void disableall ()
    {
     Multiplayer = false;
     singleplayer = false;
     settings = false;
     returntomenu =false;
     exit = false;
     FindAGame = false;
     TestArea= false;
     ShipBuilder = false;
        fleetbuilder = false;
}

}
