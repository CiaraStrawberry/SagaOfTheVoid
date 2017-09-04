using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using simple;
using Oculus.Platform;

public class VRTK_SDK_CONTROLLER_MANAGER : MonoBehaviour {
    public bool issteamversion;
    public GameObject steamManager;
    // public GameObject friendsonly;
    // Use this for initialization
    public MainMenuValueHolder crosslevelholder;
    public VRTK_SDKManager stkman;
    public bool lefthandmode;
    private IOComponent FleetBuilder;
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        if (steamManager == null) steamManager = GameObject.Find("SteamManager");
        if (issteamversion == false)
        {
            Debug.Log("setupoculus");
        //    Core.AsyncInitialize();
            if (steamManager) steamManager.SetActive(false);
            
        }
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
        crosslevelholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        steamManager = GameObject.Find("SteamManager");
        stkman = GetComponent<VRTK_SDKManager>();
        InvokeRepeating("Rep", 0, 1);
      
        stkman.LoadedSetupChanged += new VRTK_SDKManager.LoadedSetupChangeEventHandler(Load);

       
        if ( FleetBuilder.get<bool>("LeftMode"))
        {
            GameObject lefthand = stkman.scriptAliasLeftController;
            stkman.scriptAliasLeftController = stkman.scriptAliasRightController;
            stkman.scriptAliasRightController = lefthand;
            FleetBuilder.read();
        }
    }
    void Start ()
    {
        UnityEngine.Application.runInBackground = false;
    }
    
    void Rep ()
    {
    }
    void Update()
    {
        
    }
   
	public void Load (VRTK_SDKManager sender,VRTK_SDKManager.LoadedSetupChangeEventArgs e) {
        crosslevelholder.loadedsetup = stkman.loadedSetup;
	}
}
