using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityStandardAssets.ImageEffects;
public class Resizescript : MonoBehaviour {

    // This script is the remanents of of 6 months of attempts to create a working interaction with the world, its a bit crap :(
    public Vector3 scale1;
    private SteamVR_TrackedObject trackedObj;
    private int Tempsize;
    private Transform parent1;
    private Vector3 originscale = new Vector3(5, 5, 5);
    public Vector3 size2;
    private Vector3 startposition;
    private Quaternion startrotation;
    public GameObject lefthand;
    public GameObject righthand;
    public float prevnumber;
    public GameObject cam;
    public GameObject centerBridge;
    private GameObject parent;
   // public GameObject LeftGameobject;
    public Vector3 lastposscale;
    public GameObject World;
    public GameObject WorldBase;
    public GameObject DragTarget;
    private Vector3 lastpostarget= new Vector3(0,0,0);
    public GameObject WaspPos;
    public GameObject startparent;
    public GameObject Joystick;
    private UnitMovementcommandcontroller unitcom;
    private _Ship.eShipColor team;
    public GameObject CapitalScaleBase;
    private CrossLevelVariableHolder crosslevelhol;
    int amountin;
    GameObject objects;
    public GameObject camtestobj;
    public VignetteAndChromaticAberration fovlimiter;
    void Awake ()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        
    }
    GameObject getCapitalShip ()
    {
        GameObject Capitalout = null;
        objects = GameObject.Find("Objects");
        foreach(Transform t in objects.transform)
        {
            if (t.name == "Super-Capital(Clone)" && t.GetComponent<_Ship>().ShipColor == team)
            {
                Capitalout = t.gameObject.transform.Find("bridge1piece").Find("CamCenter").gameObject; 
            }
                      
        }
        return Capitalout;
    }

    void Start ()
    {
    //    if (GameObject.Find("Camera (eye)")) fovlimiter = GameObject.Find("Camera (eye)").GetComponent<VignetteAndChromaticAberration>();
      //  if (GameObject.Find("Camera (eye)")) fovlimiter = GameObject.Find("Camera (eye)").GetComponent<VignetteAndChromaticAberration>();
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        countup = 0;
  //      if(scene.name != "mptest" && scene.name != "MainMenuIntroLobby") transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            World = GameObject.Find("WorldScaleBase");
            WorldBase = GameObject.Find("World");
            startparent = transform.parent.gameObject;
            
                unitcom = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
                team = unitcom.team;
            
            CapitalScaleBase = getCapitalShip();
            crosslevelhol = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
            crosslevelhol.campaignnextlevelset = false;
            WaspPos = GameObject.Find("Camera Position Socket");
            Joystick = GameObject.Find("steering");
            WaspPos.transform.parent.gameObject.SetActive(false);
           // GameObject targetgam = unitcom.teammembersout(unitcom.team)[0];
        }
        countup = 0;
        waitforshiptobeready = false;
    }
    bool waitforshiptobeready;
	void Update () {
        countup++;
      
        //   if (SceneManager.GetActiveScene().buildIndex == 0 || SceneManager.GetActiveScene().buildIndex == 1) transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if (waitforshiptobeready == false && unitcom.teammembersout(unitcom.team).Count > 0)
            {
                waitforshiptobeready = true;
                World.transform.localScale = new Vector3(200, 200, 200);
                transform.position = unitcom.teammembersout(unitcom.team)[0].transform.position;
            }
            if (amountin < 305) amountin++;
            if(amountin == 5)
            {
                if(WaspPos) WaspPos.transform.parent.gameObject.SetActive(false);
            }
            if(amountin == 300)
            {
                CapitalScaleBase = getCapitalShip();
            }
            lastpos = transform.position - cam.transform.position;
            if (centerBridge && this.transform.parent != null && Vector3.Distance(transform.position, centerBridge.transform.position)> 5)
            {
               transform.position = centerBridge.transform.position;   
            }
            if ((DragTarget)) transform.position = transform.position -  DragTarget.GetComponent<CustomPathfinding>().thisposdif;
            //else lastpostarget = new Vector3(0, 0, 0);
            if(WaspPos && WaspPos.transform.parent.gameObject.GetActive() == true)
            {
                WaspPos.transform.parent.localPosition = WaspPos.transform.parent.localPosition + WaspPos.transform.parent.forward * speed;
            }
        }
        zoomedin = false;
        if ((WaspPos && WaspPos.GetActive() == true) || (CapitalScaleBase && transform.parent == CapitalScaleBase.transform))
        {
        }
        else if (transform.localScale.x < 0.05f)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
    }

    public void scaleup()
    {
        cam.transform.localPosition = new Vector3(0,0,0);
        transform.localScale += new Vector3(20,20,20);
       
       // transform.localPosition -= new Vector3(0, 300, 0);        
       // this requires fix
    }
    public void scaledown ()
    {
        cam.transform.localPosition = new Vector3(0, 0, 0);
        transform.localScale -= new Vector3(20, 20, 20);
      //  transform.localPosition += new Vector3(0, 300, 0);
    }
    
    public void positiondown()
    {
        transform.localPosition -= new Vector3(0, 300, 0);
    }
    public void positionup()
    {
        transform.localPosition += new Vector3(0, 300, 0);
    }
    private Quaternion curangle;
    private Vector3 quat;
    private Vector3 last;
    private Quaternion start;
    public float speed;
    // dear god this code is horrible
    public float minworldscale = 0.1f;
    public int maxworldscale = 4000;
    public float minplayerscale = 0.12f;
    private bool zoomedin;
    private Vector3 LastDistancefrom;
    private bool tempscaleset;
    public int countup;
    public void change ( Vector3 midpoint, float scalealter)
    {
        if((WaspPos && WaspPos.GetActive() == true) || (CapitalScaleBase  && transform.parent == CapitalScaleBase.transform))
        {
       //     speed = scalealter * 3;
            scalealter = 1;
     
        }
        else if (transform.localScale.x < 0.05f)
       {
            transform.localScale = new Vector3(1, 1, 1);
        }
       
        if (scalealter != 1)
        {
            if(zoomedin == false)
            zoomedin = true;
        }
        // Debug.Log(scalealter);
      
        if (noclue == false &&( scalealter < 1 || ( DragTarget && World.transform.localScale.x < 40) || DragTarget == null || (DragTarget && DragTarget.GetComponent<_Ship>().HullType == eHullType.Light)))
        {
         
            if ((World.transform.localScale.x < maxworldscale || scalealter < 1) && (World.transform.localScale.x > minworldscale || scalealter > 1) && World.transform.localScale.x < maxworldscale)
                World.transform.localScale = new Vector3(World.transform.localScale.x * scalealter, World.transform.localScale.y * scalealter, World.transform.localScale.z * scalealter);

            if (World.transform.localScale.x > maxworldscale && (transform.localScale.x > minplayerscale || scalealter < 1) && (transform.localScale.x < 1 || scalealter > 1))
            {
                transform.localScale = new Vector3(transform.localScale.x * (1 / scalealter), transform.localScale.y * (1 / scalealter), transform.localScale.z * (1 / scalealter));
                if (transform.localScale.x > 1 && scalealter < 1) World.transform.localScale = new Vector3(maxworldscale - 10, maxworldscale - 10, maxworldscale - 10);
            }
            if ((World.transform.localScale.x < maxworldscale))
            {

                Vector3 originpoint = WorldBase.transform.position;
                World.transform.position = midpoint;
                WorldBase.transform.position = originpoint;

            }
            else
            {
                Vector3 originpoint = cam.transform.position;
                transform.position = midpoint;
                cam.transform.position = originpoint;
            }
        }
        else disable();
        if (righthand.transform.localPosition != new Vector3(0, 0, 0) && lefthand.transform.localPosition != new Vector3(0, 0, 0))
        {
            quat = (righthand.transform.localPosition - lefthand.transform.localPosition).normalized;
            if (quat != new Vector3(0, 0, 0)) start = Quaternion.LookRotation(quat);
            noclue = false;
            last = quat;
        }
        if (tempscaleset == true)
        {
            transform.localScale = new Vector3(1, 1, 1);
            tempscaleset = false;
        }

    }
    public void move (float moveamount)
    {
        Vector3 dif = righthand.transform.localPosition - lastposscale;
     
        transform.position += dif * transform.lossyScale.x / 10;
        
    }


    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }


    public void  translate (Vector3 moveaboutpoint)
    {
        if (countup > 50)
        {
            Vector3 originpoint = cam.transform.position;

            startlocal = transform.position;
            if (noclue == false)
            {
                transform.position = moveaboutpoint;
                cam.transform.position = originpoint;
                transform.position = startlocal;
            }
            noclue = false;
        }
        else transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        
        
    }
    void LateUpdate ()
    {
        if (DragTarget != null) DragTarget.GetComponent<_Ship>().enablehealthbar();
    }
    private Vector3 startlocal;
    private Vector3 startworld;
    public void starttranslate(Vector3 moveaboutpoint)
    {

        DragTarget = null;
        Vector3 originpoint = cam.transform.position;
        transform.position = moveaboutpoint;
        cam.transform.position = originpoint;
        startlocal = transform.position;
        noclue = true;
        lastposscale = righthand.transform.localPosition;
    }
    

    private Vector3 lastpos;
    private bool noclue;
    public void disable()
    {
      //  noclue = true;
    }
    public void rotateright()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,transform.eulerAngles.y + 40,transform.eulerAngles.z);
    }
    public void rotateleft()
    {
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y - 40, transform.eulerAngles.z);
    }
    private Vector3 tempscale;
    public void SwitchToWasp ()
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if (crosslevelhol && (crosslevelhol.Gamemode == CrossLevelVariableHolder.gamemode.TeamDeathMatch || crosslevelhol.Gamemode == CrossLevelVariableHolder.gamemode.FreeForAll))
            {
                if (WaspPos && WaspPos.GetActive() == false)
                {
                    World.transform.localScale = new Vector3(2000, 2000, 2000);
                    WaspPos.transform.parent.transform.position = transform.position;
                    WaspPos.transform.parent.gameObject.SetActive(true);
                    WaspPos.transform.parent.rotation = transform.rotation;
                    transform.parent = WaspPos.transform;

                    cam.transform.localPosition = new Vector3(0, 0, 0);
                    iTween.ScaleTo(gameObject, new Vector3(4f, 4f, 4f), 2);
                    transform.localPosition = new Vector3(0, 0, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                    tempscale = World.transform.localScale;

                    if (transform.GetChild(0).Find("OculusSDK")) transform.GetChild(0).Find("OculusSDK").localPosition = new Vector3(0, 0, 0);
                    if (transform.GetChild(0).Find("ViveSDK")) transform.GetChild(0).Find("ViveSDK").localPosition = new Vector3(0, 0, 0);

                }
                else if (WaspPos)
                {
                    WaspPos.transform.parent.gameObject.SetActive(false);
                    transform.parent = startparent.transform;
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                    transform.localScale = new Vector3(1, 1, 1);
                    World.transform.localScale = tempscale;
                    //  iTween.ScaleTo(World,tempscale,2);
                    WaspPos.transform.parent.rotation = new Quaternion(0, 0, 0, 0);
                }
            }
            else if (crosslevelhol.Gamemode == CrossLevelVariableHolder.gamemode.CapitalShip)
            {
                if (DragTarget != null) DragTarget = null;
                if (CapitalScaleBase == null) CapitalScaleBase = getCapitalShip();
                if (CapitalScaleBase != null && transform.parent != CapitalScaleBase.transform)
                {
                    World.transform.localScale = new Vector3(6000, 6000, 6000);
                    CapitalScaleBase.transform.parent.gameObject.SetActive(true);
                    transform.parent = CapitalScaleBase.transform;
                    transform.localEulerAngles = new Vector3(90, 0, 0);
                    cam.transform.localPosition = new Vector3(0, 0, 0);
                    //iTween.ScaleTo(gameObject, new Vector3(0.04f, 0.04f, 0.04f), 2);
                    transform.localPosition = new Vector3(0, 0, 0);
                    transform.localScale = new Vector3(0.04f, 0.04f, 0.04f);
                    tempscale = World.transform.localScale;

                    if (transform.GetChild(0).Find("OculusSDK")) transform.GetChild(0).Find("OculusSDK").localPosition = new Vector3(0, 0, 0);
                    if (transform.GetChild(0).Find("ViveSDK")) transform.GetChild(0).Find("ViveSDK").localPosition = new Vector3(0, 0, 0);
                }
                else if (CapitalScaleBase != null)
                {
                    if (DragTarget != null) DragTarget = null;
                    transform.parent = startparent.transform;
                    transform.rotation = new Quaternion(0, 0, 0, 0);
                    World.transform.localScale = new Vector3(4, 4, 4);
                    transform.position = CapitalScaleBase.transform.parent.position;
                    transform.localScale = new Vector3(1, 1, 1);
                    tempscaleset = true;

                }

            }
        }
    }
    public void forcereturntonormal ()
    {
        if (transform.parent.name != "VRTK_SDK") SwitchToWasp();
    }
    public Quaternion startquatcontr;
    public string debug;
    public void startclick (Quaternion input)
    {
        startquatcontr = input;
    }
    public void StickInput (Quaternion input)
    {
        if(WaspPos && WaspPos.transform.parent.gameObject.GetActive() == true)
        {
           Quaternion turnamount =  Quaternion.Inverse(startquatcontr) * input;
           turnamount = Quaternion.Slerp(WaspPos.transform.parent.rotation, WaspPos.transform.parent.rotation * turnamount, 0.015f);
            //debug = turnamount.ToString();
           WaspPos.transform.parent.rotation = turnamount;
            // WaspPos.transform.parent.rotation = WaspPos.transform.parent.rotation * turnamount;
            Joystick.transform.localRotation = Quaternion.Inverse(startquatcontr) * input;
        }
    }
}
