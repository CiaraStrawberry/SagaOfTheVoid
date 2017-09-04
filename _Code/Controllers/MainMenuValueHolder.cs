using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class MainMenuValueHolder : MonoBehaviour {
    public VRTK_SDKSetup loadedsetup;
    public bool testarea;
    public bool tutorial;
    public bool currentlyloading;
    public bool campaign;
    public MainMenuCampaignControlScript.mapcontainer selectedcampaign;
    public List<GameObject> TutorialObjs = new List<GameObject>();
    // Use this for initialization
    void Start () {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void OnApplicationQuit()
    {
        if (!Application.isEditor) System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
    void OnLevelFinishedLoading(Scene scene,LoadSceneMode inputmode)
    {
       if(scene.buildIndex == 0)
        {
          testarea = false;
          tutorial = false;
          campaign = false;
            if (GameObject.Find("CrossLevelVariables") != null) {
                CrossLevelVariableHolder crossvar =   GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
                crossvar.tutorial = false;
                crossvar.campaign = false;
            }
        }
     
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
