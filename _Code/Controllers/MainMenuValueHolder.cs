using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

/// <summary>
/// This class essentialy serves as a variable holder singleton that only transfers between scenes 0 and 1.
/// </summary>
public class MainMenuValueHolder : MonoBehaviour {
    public VRTK_SDKSetup loadedsetup;
    public bool testarea;
    public bool tutorial;
    public bool currentlyloading;
    public bool campaign;
    public MainMenuCampaignControlScript.mapcontainer selectedcampaign;
    public List<GameObject> TutorialObjs = new List<GameObject>();
   
    /// <summary>
    /// initialise.
    /// </summary>
    void Start () {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    /// <summary>
    /// Prevents the game crashing on close, bit ad-hoc admittedly but its better then the error message that comes up.
    /// </summary>
    void OnApplicationQuit()
    {
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
    }


    void OnLevelFinishedLoading(Scene scene,LoadSceneMode inputmode)
    {
        if (scene.buildIndex == 0)
        {
            testarea = false;
            tutorial = false;
            campaign = false;
            if (GameObject.Find("CrossLevelVariables") != null)
            {
                CrossLevelVariableHolder crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
                crossvar.tutorial = false;
                crossvar.campaign = false;
            }
        }
     
    }
	
}
