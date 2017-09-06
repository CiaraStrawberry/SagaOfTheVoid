using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

/// <summary>
/// This class changes outputs based on the bool of its function, this bool can change, could have designed this alot better tbh.
/// </summary>
public class MainMenuSelector : MonoBehaviour {

    // these are pretty self explanatory tbh.
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

	void Update () {
        cooldown++;
        // cooldown ensures you must wait more then a second between button presses.
	}

    /// <summary>
    /// Called upon this function having its button pressed.
    /// </summary>
    void ButtonPress()
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

    /// <summary>
    /// Quits the application.
    /// </summary>
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

    /// <summary>
    /// Resets the button status to the initial one.
    /// </summary>
    public void disableall()
    {
        Multiplayer = false;
        singleplayer = false;
        settings = false;
        returntomenu = false;
        exit = false;
        FindAGame = false;
        TestArea = false;
        ShipBuilder = false;
        fleetbuilder = false;
    }

}
