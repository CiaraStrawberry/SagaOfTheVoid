using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TrueSync;

public class ControllerMenuController : MonoBehaviour {
    public TextMesh Kills_Text;
    public TextMesh Ships_Left_Text;
    public TextMesh Time_Left_Text;
    public GameObject Map;
    public UnitMovementcommandcontroller unitcontrol;
    public List<bliplinecontroller> allyblips = new List<bliplinecontroller>();
    public List<bliplinecontroller> enemyblips = new List<bliplinecontroller>();
    public GameObject playerblip;
    public GameObject allyblip;
    public GameObject enemyblip;
    public GameObject parentforblips;
    public AIController AIController;
    public bool ismission;
    public GameObject Playericon;
    public GameObject Cameratracking;
    private GameObject Objectsholder;
    public TextMesh Blue;
    public TextMesh Green;
    public TextMesh Grey;
    public TextMesh Red;
    public TextMesh White;
    public TextMesh Yellow;
    int waitforit;

	// Use this for initialization
	void Start () {
        if (Map)
        {
            // create icons for the map to use as ship representations.
            for (int i = 0; i < 40; i++)
            {
                spawnblip(ref allyblips, allyblip);
                spawnblip(ref enemyblips, enemyblip);
            }
            playerblip = Instantiate(Playericon) as GameObject;
            playerblip.transform.parent = Map.transform;
            playerblip.transform.localScale = new Vector3(600, 600, 600);
            playerblip.transform.localEulerAngles = new Vector3(0, 90, 0);

            foreach (bliplinecontroller blip in allyblips)
            {
                blip.transform.parent = Map.transform;
                blip.transform.localScale = new Vector3(400, 400, 400);
                blip.transform.localEulerAngles = new Vector3(0, 90, 0);
            }
            foreach (bliplinecontroller blip in enemyblips)
            {
                blip.transform.parent = Map.transform;
                blip.transform.localScale = new Vector3(400, 400, 400);
                blip.transform.localEulerAngles = new Vector3(0, 90, 0);
            }
        }
        if (GameObject.Find("Camera (eye)") != null) Cameratracking = GameObject.Find("Camera (eye)");
        else Cameratracking = GameObject.Find("CenterEyeAnchor");
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnLevelFinishedLoading;
        OnLevelFinishedLoading(new Scene(),new LoadSceneMode());
        InvokeRepeating("Rep", 0, 1);
    }
    // Updates map, only hapens once a second for performance reasons.
    void Rep ()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1)
        {
            if (Blue)
            {
                // give the player an indicator or how many ships each color owns and the player name assosiated with those ships.
                if (Green.gameObject.GetActive() == true) Green.text = getplayername(LobbyScreenController.getteam1(), 0) + "Green: " + unitcontrol.teammembersout(_Ship.eShipColor.Green).Count + " ships";
                if (Blue.gameObject.GetActive() == true) Blue.text = getplayername(LobbyScreenController.getteam1(), 1) + "Blue: " + unitcontrol.teammembersout(_Ship.eShipColor.Blue).Count + " ships";
                if (White.gameObject.GetActive() == true) White.text = getplayername(LobbyScreenController.getteam1(), 2) + "White: " + unitcontrol.teammembersout(_Ship.eShipColor.White).Count + " ships";
                if (Yellow.gameObject.GetActive() == true) Yellow.text = getplayername(LobbyScreenController.getteam2(), 0) + "Yellow: " + unitcontrol.teammembersout(_Ship.eShipColor.Yellow).Count + " ships";
                if (Grey.gameObject.GetActive() == true) Grey.text = getplayername(LobbyScreenController.getteam2(), 1) + "Grey: " + unitcontrol.teammembersout(_Ship.eShipColor.Grey).Count + " ships";
                if (Red.gameObject.GetActive() == true) Red.text = getplayername(LobbyScreenController.getteam2(), 2) + "Red: " + unitcontrol.teammembersout(_Ship.eShipColor.Red).Count + " ships";

            }
            if (unitcontrol == null)  unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            // moves unused blips out of view.   
            foreach (bliplinecontroller blip in allyblips) blip.transform.localPosition = new Vector3(100000, 100000, 100000);
            foreach (bliplinecontroller blip in enemyblips) blip.transform.localPosition = new Vector3(100000, 100000, 100000);
            foreach (bliplinecontroller blip in allyblips) blip.targetgam = new Vector3(0, 0, 0);
            List<GameObject> temp = unitcontrol.teammembersout(unitcontrol.team);
            if (temp != null && Ships_Left_Text != null)
            {
                Kills_Text.text = " ships : " + temp.Count.ToString() + " out of " + UnitMovementcommandcontroller.getmaxshipnumbers().ToString();
                List<TSTransform> enemies = unitcontrol.targetsout(unitcontrol.team);
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemyblips[i] != null && enemyblips[i].gameObject != null && enemies[i] != null && enemies[i].gameObject != null) enemyblips[i].transform.localPosition = new Vector3(0, enemies[i].transform.localPosition.x * 0.35f, enemies[i].transform.localPosition.z * 0.35f);
                }
            }
            if (Time_Left_Text && ismission == false) Time_Left_Text.text = unitcontrol.gettimeleft();
            else if (AIController && AIController.timepassedactual < 30) Time_Left_Text.text = "Time til attack: " + (30 - TSMath.Round(AIController.timepassedactual)).ToString();
            else if (unitcontrol.crosslevelholder.campaign == true && unitcontrol.crosslevelholder.campaignlevel.objective == MainMenuCampaignControlScript.eMissionObjective.Survive) Time_Left_Text.text = "Survive for: " + unitcontrol.gettimeleft();
            else if (Time_Left_Text) Time_Left_Text.text = unitcontrol.gettimeleft();
            List<TSTransform> allies = unitcontrol.alliesout();
            for (int i = 0; i < allies.Count; i++)
            {
                if (allyblip && allyblips[i].gameObject != null)
                {
                    // asigns blip positions
                    allyblips[i].source = allies[i];
                    allyblips[i].transform.localPosition = new Vector3(0, allies[i].transform.localPosition.x * 0.35f, allies[i].transform.localPosition.z * 0.35f);
                    Vector3 tempa = allies[i].shipscript.fac.localtarget.ToVector() * 0.35f;
                    allyblips[i].targetgam = new Vector3(0, tempa.x, tempa.z);
                }
            }
            if (Objectsholder == null) Objectsholder = GameObject.Find("Objects");
            if (Cameratracking == null)
            {
                if (GameObject.Find("Camera (eye)") != null) Cameratracking = GameObject.Find("Camera (eye)");
                else Cameratracking = GameObject.Find("CenterEyeAnchor");
            }
            Vector3 playerloc = Objectsholder.transform.InverseTransformPoint(Cameratracking.transform.position);
            playerblip.transform.localPosition = new Vector3(0, playerloc.x * 0.35f, playerloc.z * 0.35f);
        }
    }

    // gets the player name of a position within a team.
    string getplayername (List<int> inputarray,int inputpos)
    {
        string output = "";
         if(inputarray[inputpos] != 300) output = PhotonPlayer.Find(inputarray[inputpos]).NickName + " - ";
        return output;
    }

    // creates the blip
    void spawnblip(ref List<bliplinecontroller> blips,GameObject thingtospawn)
    {
        GameObject gam = Instantiate(thingtospawn) as GameObject;
        blips.Add(gam.GetComponent<bliplinecontroller>());
    }

    private CrossLevelVariableHolder crossvar;

    // resets everything every level
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (SceneManager.GetActiveScene().buildIndex != 0 && SceneManager.GetActiveScene().buildIndex != 1)
        {
            Objectsholder = GameObject.Find("Objects");
            unitcontrol = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            waitforit = 0;
            
            if (PhotonNetwork.room != null)
            {
                Debug.Log(PhotonNetwork.room.MaxPlayers);
                if (Blue)
                {
                    Green.gameObject.SetActive(true);
                    Blue.gameObject.SetActive(true);
                    White.gameObject.SetActive(true);
                    Yellow.gameObject.SetActive(true);
                    Grey.gameObject.SetActive(true);
                    Red.gameObject.SetActive(true);
                  
                  
                    if (PhotonNetwork.room.MaxPlayers == 2)
                    {
                        Blue.gameObject.SetActive(false);
                        White.gameObject.SetActive(false);
                        Grey.gameObject.SetActive(false);
                        Red.gameObject.SetActive(false);
                    }
                    if (PhotonNetwork.room.MaxPlayers == 4 && (string)PhotonNetwork.room.CustomProperties["3v3"] == "No")
                    {
                        White.gameObject.SetActive(false);
                        Red.gameObject.SetActive(false);
                    }
                }
            }
        }
        

    }

    // Update is called once per frame
    void Update()
    {
        waitforit++;
        if (waitforit == 10)
        {
            if(GameObject.Find("AIController"))  AIController = GameObject.Find("AIController").GetComponent<AIController>();
          if(AIController)  ismission = AIController.ismission;
        }
    }
}
