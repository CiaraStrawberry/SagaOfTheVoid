using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;
using UnityEngine.SceneManagement;

public class PositinonRelativeToHeadset : TrueSyncBehaviour 
{
    public GameObject cam1;
    public GameObject cam2;
    public RelayController InputRelay;
    public GameObject Reasourcesthing;
    public TextMesh reasourcestextmesh;
    public UnitMovementcommandcontroller unitcon;
    public CrossLevelVariableHolder crossvar;
    private BuildQueController bulcontrol;
    public GameObject Fighters;
    public GameObject Frigates;
    public GameObject Special;
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (Reasourcesthing && reasourcestextmesh == null) reasourcestextmesh = Reasourcesthing.GetComponent<TextMesh>();
        if (GameObject.Find("Controllers") != null)
        {
            if (GameObject.Find("Controllers")) unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            if (GameObject.Find("CrossLevelVariables")) crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        }
        if (this != null && this.gameObject != null && scene.name != null) this.gameObject.SetActive(false);
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        }
        if(Fighters != null)
        {
            Fighters.SetActive(true);
            Frigates.SetActive(true);
            Special.SetActive(true);
            if(crossvar.campaign == true)
            {
                if(crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.fighers)
                {
                    Fighters.SetActive(true);
                    Frigates.SetActive(false);
                    Special.SetActive(false);
                }
                else if (crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.frigates)
                {
                    Fighters.SetActive(true);
                    Frigates.SetActive(true);
                    Special.SetActive(false);
                }
                else if (crossvar.campaignlevel.shipsavaiable == MainMenuCampaignControlScript.eshipsavailable.all)
                {
                    Fighters.SetActive(true);
                    Frigates.SetActive(true);
                    Special.SetActive(true);
                }
            }
        }

    }

    void Awake ()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        bulcontrol = GetComponent<BuildQueController>();
        OnLevelFinishedLoading(new Scene(),new LoadSceneMode());
    }

    public override void OnSyncedStart()
    {
        InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
    }

    void Update ()
    {
     
        if (Reasourcesthing)
        {
         if(unitcon == null) unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            if (reasourcestextmesh == null) reasourcestextmesh = Reasourcesthing.GetComponent<TextMesh>();
         if(unitcon)   reasourcestextmesh.text = "Resources : " + unitcon.getmoney();

        }
    
   }

   public GameObject CameraObj;
    public void TurnOn()
    {
        GameObject actualcamobj = null;
       if (cam1 != null && cam1.gameObject != null && cam1.GetActive() == true) actualcamobj = cam1;
       else if (cam2 != null &&  cam2.gameObject != null && cam2.GetActive() == true) actualcamobj = cam2;
        transform.position = CameraObj.transform.position;
        //    transform.localPosition = new Vector3(transform.localPosition.x, 0, transform.localPosition.z);
        transform.LookAt(actualcamobj.transform.position);
        transform.eulerAngles = new Vector3(0, transform.eulerAngles.y , 0);
        if (unitcon == null)
        {
            unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        }
    }
    public void spawnship(int a,string name,float timetospawn)
    {
        if (a != 0)
        {
            if (unitcon == null)
            {
                unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
                crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
            }
            if (checkmoney(unitcon.getmoney(), a) == true)
            {
                bulcontrol.addbuildorder(new BuildQueController.buildorder(name,a,1,timetospawn));
            }
        }
    }
    public void spawnshiprightnow (int a)
    {
        if (InputRelay == null) InputRelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
        if (unitcon == null)
        {
            unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
            crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
        }
        if (InputRelay != null) InputRelay.ordershipspawn(a, getspawnpos(unitcon.team), crossvar.findspawnpos(PhotonNetwork.player.ID), PhotonNetwork.AllocateViewID());
    }
    public static TSVector getspawnpos(_Ship.eShipColor teamin) {
        List<GameObject> ships = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().teammembersout(teamin);
        TSVector output = new TSVector(0,0,0);
        if (ships.Count > 0)
        {
           if(ships[0].gameObject != null) output = ships[0].GetComponent<TSTransform>().position;
            if (ships.Count == 1) output = output + new TSVector(300, 300, 300);
            output = output + (new TSVector(5, 5, 5) * ships.Count);
            foreach (GameObject gam in ships) if (gam.gameObject != null && gam != ships[0]) output = gam.transform.GetComponent<TSTransform>().position + output;
            return output / ships.Count;
        }
        else return output;
    }

    public static TSQuaternion getspawnrotation(_Ship.eShipColor teamin) {
        List<GameObject> ships = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>().teammembersout(teamin);
        if (ships.Count > 0)
        {
            GameObject Temp = getrotationprime(ships);
            if (Temp != null && Temp.gameObject != null) return Temp.GetComponent<TSTransform>().rotation;
            else return new TSQuaternion(0, 0, 0, 0); 
        }
        else return new TSQuaternion(0, 0, 0, 0); 
    }

    public static GameObject getrotationprime (List<GameObject> ships)
    {
        GameObject output = null;
        foreach (GameObject gam in ships) if (gam.gameObject != null && gam.GetComponent<_Ship>().HullType != eHullType.Light) output = gam;
        if (output == null) output = ships[0];
        return output;
    }
    public bool checkmoney (int money,int spawner)
    {
        bool output = false;
        int pointstodeploy = unitcon.getshipbynumber(spawner).GetComponent<_Ship>().PointsToDeploy;
        if (pointstodeploy <= money) output = true;
        else output = false;
        int maxships = UnitMovementcommandcontroller.getmaxshipnumbers();
        int currentships = unitcon.teammembersout(unitcon.team).Count;
        if (currentships >= (maxships - 1)) output = false;
        if (unitcon.running == false) output = false;
        if (output) unitcon.takemoney(pointstodeploy);
        if (unitcon.checkiffactiondead(unitcon.team) == true) output = false;
        return output;
    }

    public void Scout () { spawnship(1,"Scout",5); }

    public void Fighter() { spawnship(2,"Fighter",7); }

    public void Bomber () { spawnship(3, "Bomber",7); }

    public void AttackCraft () { spawnship(4,"Attack Craft",8); }

    public void MissileFrigate () { spawnship(5,"Missile Frigate",6); }

    public void FlackFrigate () { spawnship(6, "Flack Frigate", 6); }

    public void CannonFrigate () { spawnship(7, "Cannon Frigate", 7); }

    public void LazerFrigate () { spawnship(8, "Lazer Frigate", 6); }

    public void SupportFrigate () { spawnship(9, "Support Frigate", 6); }

    public void RepairFrigate () { spawnship(10, "Repair Frigate", 8); }

    public void Destroyer () { spawnship(11, "Destroyer", 15); }

    public void Capital () { spawnship(12, "Capital", 20); }
   
}
