using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using simple;

public class VariableStorage : MonoBehaviour
{
    public GameObject enemyfleetEasiest;
    public GameObject enemyfleetEasy;
    public GameObject enemyfleetMedium;
    public GameObject enemyfleetHard;
    public GameObject enemyfleetExtreme;
    public GameObject CameraRigbig;
    public GameObject menu;
    private Vector3 tempos;
    private GameObject Ioroot;
    public string difficulty;
    public GameObject Easiestobj;
    private string fleet;

    public bool skirmish;
    private IOComponent FleetBuilder;
    // Use this for initialization
    void Awake()
    {
        Ioroot = GameObject.Find("IORoot");
        FleetBuilder = IORoot.findIO("fleet1");

       
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Man");
        foreach (GameObject obj in objs)
        {
            if (obj != this.gameObject)
            {
                Destroy(obj.gameObject);
            }
        }


    }
    void Start()
    {
        Medium();
        Fleet1();
        Fleet1();
        SetQuality qual = Ioroot.GetComponent<SetQuality>();
        qual.fleettest();
    }

    public void Easiest()
    {
   
        difficulty = "Easiest";

    }
    public void Easy()
    {
  
        difficulty = "Easy";
    }

    public void Medium()
    {
  
        difficulty = "Medium";

    }

    public void Hard()
    {
        difficulty = "Hard";
    }
    public void Extreme()
    {

        difficulty = "Extreme";
    }
  
    public void Fleet1()
    {
    
    }
    public void Fleet2()
    {
   
    }
    public void Fleet3()
    {
        SetQuality qual = Ioroot.GetComponent<SetQuality>();
        qual.fleet3fun();
    }
    public void startskirmish ()
    {
        skirmish = true;
        GameObject gamo = GameObject.Find("HologramBase");
        save(gamo);
   
       
    }
    public void start()
    {
        skirmish = false;
      
       
    }
   
    public void save(GameObject basesav)
    {
        GameObject gamo = GameObject.FindWithTag("Hol");
        SetQuality qual = Ioroot.GetComponent<SetQuality>();
        qual.save(fleet,basesav);
    }
    public void reset()
    {
        GameObject gamo = GameObject.Find("ResetBase");
        SetQuality qual = Ioroot.GetComponent<SetQuality>();
        qual.save(fleet, gamo);
    }
   public void spawn_allied_fleet ()
    {

        GameObject basehol = GameObject.FindGameObjectWithTag("Hol");
        foreach (Transform child in basehol.transform)
        {
            Destroy(child.gameObject);
        }
        int o = FleetBuilder.get<int>(fleet + "num");
        FleetBuilder.read();
        Vector3 temppos = new Vector3(0,0,0);
        for (int a = 0; a < o; a++)
        {
            string fleettemp1 = FleetBuilder.get<string>(fleet + a);
            float fleettemp1locx = FleetBuilder.get<float>(fleet + "locx" + a);
            float fleettemp1locy = FleetBuilder.get<float>(fleet + "locy" + a);
            float fleettemp1locz = FleetBuilder.get<float>(fleet + "locz" + a);
            float fleettemp1rotx = FleetBuilder.get<float>(fleet + "rotx" + a);
            float fleettemp1roty = FleetBuilder.get<float>(fleet + "roty" + a);
            float fleettemp1rotz = FleetBuilder.get<float>(fleet + "rotz" + a);
            FleetBuilder.read();
            FleetBuilder.add(fleet + a, fleettemp1);
            FleetBuilder.write();
            GameObject temp = Resources.Load(fleettemp1) as GameObject;
            GameObject gam1 = Instantiate(temp, temp.transform.position, Quaternion.Euler(0, 180, 0)) as GameObject;
            gam1.transform.parent = basehol.transform;
            gam1.transform.localPosition = new Vector3(fleettemp1locx, fleettemp1locy, fleettemp1locz);
            gam1.transform.localRotation = Quaternion.Euler(fleettemp1rotx, fleettemp1roty, fleettemp1rotz);
            Destroy(gam1);
        }
    }
    public void startcor ()
    {
        StartCoroutine("go");
    }
    public IEnumerator go ()
    {
        yield return new WaitForSeconds(5);
        Debug.Log(skirmish);
        if (skirmish == true)
        {
    Destroy(GameObject.Find("BattleCapital"));
         GameObject camerarig = Instantiate(CameraRigbig,tempos + new Vector3(0,-300, -1000), Quaternion.Euler(0,0, 0)) as GameObject;
          camerarig.transform.localScale = new Vector3(200,200,200);
          GameObject camchild = camerarig.transform.GetChild(0).gameObject;
         camchild.transform.eulerAngles = new Vector3(0,0,0);
        }


    }
    void spawn_enemy_fleet ()
    {
        if (difficulty == "Hard")
        {
            spawn(enemyfleetEasiest);
        }
        if (difficulty == "Easy")
        {
            spawn(enemyfleetEasy);
        }
        if (difficulty == "Medium")
        {
            spawn(enemyfleetMedium);
        }
        if (difficulty == "Hard")
        {
            spawn(enemyfleetHard);
        }
        if (difficulty == "Extreme")
        {
            spawn(enemyfleetExtreme);
        }

    }
    void spawn (GameObject spawner)
    { 
        GameObject spawnclone = Instantiate(spawner) as GameObject;
        GameObject origin = GameObject.Find("Origin");
        spawnclone.transform.position = origin.transform.position;
    }
    public void spawnstart ()
    {
        if (skirmish == true)
        {
            GameObject[] gams = GameObject.FindGameObjectsWithTag("Team1");

            foreach (GameObject gam in gams)
            {
                Destroy(gam);
            }
            GameObject[] gams2 = GameObject.FindGameObjectsWithTag("Team2");
            foreach (GameObject gam in gams2)
            {
                if (gam.name != "BattleCapital")
                Destroy(gam);
            }
            spawn_allied_fleet();
            spawn_enemy_fleet();
        }
    }
}
