using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using simple;
using Oculus.Platform;

/// <summary>
/// This class handles things relating to the sdk like steammanager and callbacks + enforces a singleton.
/// </summary>
public class VRTK_SDK_CONTROLLER_MANAGER : MonoBehaviour {
    // this checks if the copy was bought through the steamstore or oculus store.
    public bool issteamversion;
    // this is the object you need to disable if it was bought on the oculus store.
    public GameObject steamManager;
    // this is the crosslevelvariable holder used in the first scene.
    public MainMenuValueHolder crosslevelholder;
    // this is the attached VRTK SDK manager, that acutally changes sdk, this script just does the store stuff.
    public VRTK_SDKManager stkman;
    // this allows the left hand mode to enable without bringing up the settings page.
    public bool lefthandmode;
    // this allows you to get data from the hard drive.
    private IOComponent FleetBuilder;

    /// <summary>
    /// Fill out all the information stored and disable unnessercary code.
    /// </summary>
    void Awake()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        if (steamManager == null) steamManager = GameObject.Find("SteamManager");
        if (issteamversion == false)
        {
            Debug.Log("setupoculus");
            if (steamManager) steamManager.SetActive(false);
        }
        FleetBuilder = IORoot.findIO("fleet1");
        FleetBuilder.read();
        crosslevelholder = GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>();
        steamManager = GameObject.Find("SteamManager");
        stkman = GetComponent<VRTK_SDKManager>();
        stkman.LoadedSetupChanged += new VRTK_SDKManager.LoadedSetupChangeEventHandler(Load);
        if ( FleetBuilder.get<bool>("LeftMode"))
        {
            GameObject lefthand = stkman.scriptAliasLeftController;
            stkman.scriptAliasLeftController = stkman.scriptAliasRightController;
            stkman.scriptAliasRightController = lefthand;
            FleetBuilder.read();
        }
    }

    /// <summary>
    /// forces unity to not run in the background due to oculus store requirements.
    /// </summary>
    void Start ()
    {
        UnityEngine.Application.runInBackground = false;
    }
}
