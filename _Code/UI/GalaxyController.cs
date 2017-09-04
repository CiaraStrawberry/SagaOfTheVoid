using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using simple;

public class GalaxyController : MonoBehaviour {
    private GameObject Galaxy;
    public GameObject Singleplayer;
    private GameObject SinglePlayerCollider;
    public GameObject Multiplayer;
    public GameObject MultiplayerCollider;
    public GameObject Settings;
    private GameObject SettingsCollider;
    public GameObject Ship1;
    public GameObject Ship2;
    public GameObject Ship3;
    public GameObject SinglePlayerTestLevel;
    public GameObject ShipBuilder;
    public GameObject ShipBuilderMain;
    public GameObject fleetBuilder;
    public GameObject ShipParent;
    public GameObject Menu;
    public GameObject settingsparent;
    public GameObject fleetbuildermenu;
    public GameObject findgamemenu;
    private IOComponent FleetBuilder;
    public GameObject deniedobj;
    public bool startup;
    public int wait;
    private Vector3 startpos;
    private Vector3 galstartpos2;
    public GameObject CampaignController;
    // Use this for initialization
    public GameObject CreateGameMenu;
    public void deniedsort ()
    {
       deniedobj.SetActive(true);
    }
    void Awake()
    {
        foreach (GameObject tutorial in GameObject.FindGameObjectsWithTag("TutorialThings")) tutorial.SetActive(false);
        Galaxy = GameObject.Find("GalaxyParent");
        startpos = Galaxy.transform.position;
        galstartpos2 = Galaxy.transform.Find("Galaxy").position;
        findgamemenu.SetActive(false);
        returntonormal();
    }
   public void fleetbuildersort ()
   {
        
        Vector3 startspot = Galaxy.transform.Find("Galaxy").position;
        Galaxy.transform.position = Singleplayer.transform.position;
        Galaxy.transform.Find("Galaxy").position = startspot;
        mainmenuDisable();
        
        SinglePlayerTestLevel.SetActive(false);
        fleetBuilder.SetActive(false);
        ShipBuilderMain.SetActive(false);
        Menu.SetActive(false);
        
        settingsparent.SetActive(false);
        
        fleetbuildermenu.SetActive(true);
        
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("fleetbuilder");
     //   GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().fleetbulder();
     
    }
    void Start()
    {
        if (GameObject.Find("Team1wins(Clone)")) Destroy(GameObject.Find("Team1wins(Clone)"));
        if (GameObject.Find("Team2Wins(Clone)")) Destroy(GameObject.Find("Team2wins(Clone)"));

        MultiplayerCollider = Multiplayer.transform.Find("Multiplayer").gameObject;
        Ship1.SetActive(false);
        Ship2.SetActive(false);
        Ship3.SetActive(false);

        ShipBuilder.SetActive(false);
        SinglePlayerTestLevel.SetActive(false);
        ShipBuilder.SetActive(true);
        Galaxy.SetActive(true);
        Multiplayer.SetActive(true);
        Singleplayer.SetActive(true);
        ShipBuilderMain.SetActive(false);
        fleetBuilder.SetActive(false);
        ShipParent.SetActive(false);
        fleetbuildermenu.SetActive(false);
        mainmenuDisable();
        returntonormal();
        if (startup) findagame();
        Debug.Log("start");
        FleetBuilder = IORoot.findIO("fleet1");
        if (FleetBuilder)
        {
            FleetBuilder.read();
            string settings = FleetBuilder.get<string>("Settings");
            FleetBuilder.read();

            if (settings == "High")
            {
                QualitySettings.SetQualityLevel(5, true);
            }
            else if (settings == "Medium")
            {
                QualitySettings.SetQualityLevel(3, true);
            }
            else if (settings == "Low")
            {
                QualitySettings.SetQualityLevel(1, true);
            }
            else if (settings == null)
            {
                FleetBuilder.add("Settings", "Medium");
                FleetBuilder.write();
                QualitySettings.SetQualityLevel(3, true);
                settings = "Medium";
            }
            Debug.Log(settings);
            returntonormal();

        }
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.S)) shipbuilderbuilder();
        if (Input.GetKeyDown(KeyCode.M)) findagame();
        if (Input.GetKeyDown(KeyCode.Z)) fleetbuildersort();
        if (Input.GetKeyDown(KeyCode.P)) Tutorial();
        if (Input.GetKeyDown(KeyCode.C)) CampaignTrailsort();
        wait++;
        if (wait == 1) ShipParent.SetActive(true);
        if (wait == 2) ShipParent.GetComponent<MainMenuShipController>().checkresetall();
        if (wait == 3) ShipParent.SetActive(false);
      //  if (wait == 5) returntonormal();
    }
    void mainmenuDisable ()
    {
       Singleplayer.SetActive(false);
        Multiplayer.SetActive(false);
        Settings.SetActive(false);
        MultiplayerCollider.SetActive(false);
        ShipBuilder.SetActive(false);
        Menu.SetActive(false);
        fleetbuildermenu.SetActive(false);
        findgamemenu.SetActive(false);
        creditsobj.SetActive(false);
        CampaignController.SetActive(false);
    }
    public GameObject creditsobj;
    public void credits ()
    {
        Vector3 startspot = Galaxy.transform.Find("Galaxy").position;
        Galaxy.transform.position = ShipBuilder.transform.position;
        Galaxy.transform.Find("Galaxy").position = startspot;
        mainmenuDisable();
        ShipBuilderMain.SetActive(false);
        fleetBuilder.SetActive(false);
        settingsparent.SetActive(false);
        findgamemenu.SetActive(false);
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("settiings");
        creditsobj.SetActive(true);
    }
    public void returntonormal ()
    {
      //  GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>().tutorial = false;
        SinglePlayerTestLevel.SetActive(false);
        Ship1.SetActive(false);
        Ship2.SetActive(false);
        Ship3.SetActive(false);
        Galaxy.SetActive(true);
        ShipParent.SetActive(false);
        findgamemenu.SetActive(false);
        Galaxy.transform.position = startpos;
        Galaxy.transform.Find("Galaxy").transform.position = galstartpos2;
        Menu.SetActive(true);
        fleetBuilder.SetActive(false);
        fleetbuildermenu.SetActive(false);
        settingsparent.SetActive(false);
        CampaignController.SetActive(false);
        creditsobj.SetActive(false);
        CreateGameMenu.SetActive(false);
        if(PhotonNetwork.inRoom) PhotonNetwork.LeaveRoom();
        GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().returntomenu();
        iTween.ScaleTo(Galaxy, new Vector3(1, 1, 1), 8);
    }
 
    public void CampaignTrailsort ()
    {
        mainmenuDisable();
        CampaignController.SetActive(true);
    }

    public void settingsscaleup ()
    {
        Vector3 startspot = Galaxy.transform.Find("Galaxy").position;
        Galaxy.transform.position = Settings.transform.position;
        Galaxy.transform.Find("Galaxy").position = startspot;
        mainmenuDisable();
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("settiings");
    }
 
    public void scaledown() {
        Singleplayer.SetActive(true);
        Multiplayer.SetActive(true);
        Settings.SetActive(true);
        MultiplayerCollider.SetActive(true);
        ShipBuilder.SetActive(true);
        SinglePlayerTestLevel.SetActive(false);
        Ship1.SetActive(false);
        Ship2.SetActive(false);
        Ship3.SetActive(false);
        Galaxy.SetActive(true);
        iTween.ScaleTo(Galaxy, new Vector3(1, 1, 1), 8);
    }

    public void shipbuilderbuilder ()
    {
       Vector3 startspot = Galaxy.transform.Find("Galaxy").position;
        Galaxy.transform.position = ShipBuilder.transform.position;
        Galaxy.transform.Find("Galaxy").position = startspot;
        mainmenuDisable();
        Galaxy.SetActive(false);
        ShipParent.SetActive(true);
        Menu.SetActive(false);
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("settiings");
    }
   public void Multiplayersort ()
    {
        findagame();
    }
    public void SettingsSort ()
    {
        Ship1.SetActive(false);
        Ship2.SetActive(false);
        Ship3.SetActive(false);
        fleetBuilder.SetActive(false);
        ShipBuilderMain.SetActive(false);

        ShipBuilder.SetActive(false);
        ShipParent.SetActive(false);
        mainmenuDisable();
        settingsparent.SetActive(true);
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("settiings");
    }
    public void SinglePlayerSort ()
    {
        Ship1.SetActive(false);
        Ship2.SetActive(false);
        Ship3.SetActive(false);
        fleetBuilder.SetActive(false);
        ShipBuilderMain.SetActive(false);
        ShipBuilder.SetActive(false);
        ShipParent.SetActive(false);
        GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().SinglePlayer();
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("singleplayer");
    }
    public void findagame ()
    {
        Vector3 startspot = Galaxy.transform.Find("Galaxy").position;
        Galaxy.transform.position = ShipBuilder.transform.position;
        Galaxy.transform.Find("Galaxy").position = startspot;
        mainmenuDisable();
        ShipBuilderMain.SetActive(false);
        fleetBuilder.SetActive(false);
        settingsparent.SetActive(false);
        findgamemenu.SetActive(true);
        iTween.ScaleTo(Galaxy, new Vector3(6, 6, 6), 8);
        Debug.Log("settiings");
    }
    public void findagameactual ()
    {
        PhotonNetwork.Disconnect();
        GameObject.Find("MainMenuControlParent").GetComponent<MainMenuButtonControlScript>().Loading.SetActive(true);
        UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(6);
    }
    void Save_current_Ship(GameObject saveobject)
    {
        int i = 0;
        List<GameObject> Children = new List<GameObject>();
        Children.Clear();
        if (saveobject.transform.Find("Weapons").childCount > 0 && saveobject.transform.Find("Weapons").Find("Attachpoint 1").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 1").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 1 && saveobject.transform.Find("Weapons").Find("Attachpoint 2").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 2").GetChild(0).gameObject);
        else Children.Add(null);
        if (saveobject.transform.Find("Weapons").childCount > 2 && saveobject.transform.Find("Weapons").Find("Attachpoint 3").childCount > 0)
            Children.Add(saveobject.transform.Find("Weapons").Find("Attachpoint 3").GetChild(0).gameObject);
        else Children.Add(null);

        foreach (GameObject stri in Children)
        {
            if (stri != null)
            {

                string fleetstring = saveobject.name + i;
                Debug.Log(fleetstring);
                FleetBuilder.add(fleetstring, Children[i].name);
                Vector3 loc = Children[i].transform.localPosition;
                Vector3 rot = Children[i].transform.localEulerAngles;
                FleetBuilder.add(saveobject.name + "parent" + i, stri.transform.parent.name);
                FleetBuilder.write();
                FleetBuilder.add(saveobject.name + "num", i + 1);
                FleetBuilder.add(saveobject.name + "scalex" + i, stri.transform.localScale.x);
                FleetBuilder.add(saveobject.name + "scaley" + i, stri.transform.localScale.y);
                FleetBuilder.add(saveobject.name + "scalez" + i, stri.transform.localScale.z);
                FleetBuilder.add(saveobject.name + "posx" + i, stri.transform.parent.localPosition.x);
                FleetBuilder.add(saveobject.name + "posy" + i, stri.transform.parent.localPosition.y);
                FleetBuilder.add(saveobject.name + "posz" + i, stri.transform.parent.localPosition.z);
                Destroy(stri);
            }
            else
            {
                string fleetstring = saveobject.name + i;
                FleetBuilder.add(fleetstring, "nothing");
                Debug.Log("nothing");
            }
            FleetBuilder.write();

            i++;
        }
    }
    public void TestArea ()
    {
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().testarea = true;
        PhotonNetwork.offlineMode = true;
        findagame();
    }
    public void Tutorial ()
    {
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().tutorial = true;
        if (PhotonNetwork.connected == false)
        {
            PhotonNetwork.offlineMode = true;
            findagame();
        }
        else PhotonNetwork.Disconnect();
                 
    }
    void OnDisconnectedFromPhoton ()
    {
        if (GameObject.Find("MainMenuHolder"))
        {


            if (GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().tutorial == true)
            {
                PhotonNetwork.offlineMode = true;
                findagame();
            }
        }
    }
}
