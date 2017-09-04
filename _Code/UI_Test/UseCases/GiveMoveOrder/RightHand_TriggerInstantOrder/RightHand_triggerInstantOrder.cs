using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.Steering;
using TrueSync;
using VRTK;
using Forge3D;
using UnityEngine.SceneManagement;

public class RightHand_triggerInstantOrder : TrueSyncBehaviour {


    public bool ordernow;
    public LineRenderer lineren;
 //   [HideInInspector]
    public List<GameObject> selectedships = new List<GameObject>();                                                  // Initialization
    public Material linemat;
    private GameObject ConeBig;
    private GameObject ConeSmall;
    public bool touchpadDown;
    private int selectioncount;
    public bool triggerdown;
    public GameObject resizeobj;
   // public bool touchpaddownonce;

    private TestCaseController testcontrol;
    private Resizescript resizing;
    public  GameObject photonviewobj;
    private PhotonView photonview;
    public _Ship.eShipColor team;
    private GameObject WorldBase;
    public Camera cameramain;
    public RelayController inputrelay;
    public GameObject pulsewave;
    public GameObject tutorial;
    public VRTK_ControllerEvents vrtkcontrolevents;
    private onTriggerEnterEnemy ontriggerenter;
    public float touchpadamount;
    public GameObject MenuObj;
    private Vector3 startscale;
    private Vector3 startpos;
    public LeftControl_Onehanddrag lefthandscriptl;
    public GameObject oculuscam;
    public GameObject Vivecam;
    public GameObject pulsewavered;
    private UnitMovementcommandcontroller unitcon;
    MainMenuControllerController maincon;
    private TutorialTextControlScript tutcontrol;
    CrossLevelVariableHolder crosslevelvarhol;
    void Awake()
    {
        maincon = GetComponent<MainMenuControllerController>();
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        lefthandscriptl = GameObject.Find("LeftController").GetComponent<LeftControl_Onehanddrag>();
        WorldBase = GameObject.Find("World").transform.Find("Objects").gameObject;
        startscale = resizeobj.transform.localScale;
        startpos = resizeobj.transform.position;
        resizing = resizeobj.GetComponent<Resizescript>();
        ConeBig = transform.Find("ConeBig").gameObject;
        ConeBig.SetActive(false);
        ConeSmall = transform.Find("ConeSmall").gameObject;
        ConeSmall.SetActive(false);
        MenuObj.SetActive(false);
        vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
        vrtkcontrolevents.TouchpadPressed += new ControllerInteractionEventHandler(Touchpaddown);
        vrtkcontrolevents.TouchpadReleased += new ControllerInteractionEventHandler(Touchpadup);
        vrtkcontrolevents.TriggerPressed += new ControllerInteractionEventHandler(Trigger);
        vrtkcontrolevents.TriggerReleased += new ControllerInteractionEventHandler(TriggerUp);
        vrtkcontrolevents.ButtonTwoPressed += new ControllerInteractionEventHandler(Menu);
        vrtkcontrolevents.GripPressed += new ControllerInteractionEventHandler(grip);
        vrtkcontrolevents.ButtonOnePressed += new ControllerInteractionEventHandler(turnbutton);
        if (GetComponent<LineRenderer>() == null) lineren = gameObject.AddComponent<LineRenderer>();
        else lineren = GetComponent<LineRenderer>();
        lineren.material = linemat;
        lineren.SetPosition(0, new Vector3(0, 0, 0));
        lineren.SetPosition(1, new Vector3(0, 0, 0));
       
    }
    void resetallpos ()
    {
        if (resizeobj.gameObject != null)
        {
            resizeobj.transform.localScale = startscale;
            resizeobj.transform.position = startpos;
            resizeobj.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    void turnbutton(object sender, ControllerInteractionEventArgs e)   {    resizing.rotateright();  }
 
    void OnLevelFinishedLoading(Scene scene,LoadSceneMode scenemode)
    {
        if ( this != null && this.gameObject != null)
        {

            selectedships.Clear();
            if (oculuscam != null && oculuscam.gameObject != null && oculuscam.GetActive() == true) cameramain = oculuscam.GetComponent<Camera>();
            if (Vivecam != null && Vivecam.gameObject != null && Vivecam.GetActive() == true) cameramain = Vivecam.GetComponent<Camera>();
            if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1) resetallpos();
            else GameObject.Find("VRTK_SDK").transform.localPosition = new Vector3(0, 0, 0);

            if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
            {
                GameObject varhold = GameObject.Find("CrossLevelVariables");
                if(varhold) crosslevelvarhol = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
                if (crosslevelvarhol.tutorial == true)
                {
                    if (GameObject.Find("TutorialPanelHolder")) tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
                }

                unitcon = GameObject.Find("Controllers").GetComponent<UnitMovementcommandcontroller>();
                photonview = GameObject.Find("JoinMultiplayer").GetComponent<PhotonView>();// establish tracking
                selectAll();
                lineren.enabled = false;
                vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
                team = GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().team;
                ontriggerenter = GetComponent<onTriggerEnterEnemy>();
                tutorial = GameObject.Find("TutorialPanelHolder");
            
                if (varhold && varhold.GetComponent<CrossLevelVariableHolder>().tutorial == false)
                {
                    tutorial.SetActive(false);
                    tutorial = null;
                }
                else tutorial.SetActive(true);

            }
        }
    }
   
     public override void OnSyncedStart()
    {
        inputrelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
        owner = TrueSyncManager.LocalPlayer;
        ownerIndex = TrueSyncManager.LocalPlayer.Id;

    }
    int revertcountdown = 0;
    // Update is called once per frame
    public Material startmat;
    void Update()
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            revertcountdown--;
            if(revertcountdown == 0)
            {
                lineren.material = startmat;
            }
            if (vrtkcontrolevents == null) vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
            touchpadamount = vrtkcontrolevents.GetTouchpadAxis().y;
            if(resizing.transform.localScale.x != 1)
            {
                lineren.startWidth = 0.002f * resizing.transform.lossyScale.x;
                lineren.endWidth = 0.002f * resizing.transform.lossyScale.x;
            }
            resizing.speed = (lefthandscriptl.touchpadamount * 5) + 0.5f;
            if (triggerdown)
            {
                resizing.StickInput(transform.parent.localRotation);
            }
            if ((touchpadamount < -0.05f || touchpadamount > 0.05f) && touchpadDown == false)
            {
                if (resizing.WaspPos != null && resizing.WaspPos.GetActive() == false) lineren.enabled = true;
                
            }
            else if (maincon == null || maincon.hitobj == null) lineren.enabled = false;
            if (touchpadamount > -0.05f && touchpadamount < 0.05f && ConeSmall.GetActive() == true) ConeSmall.SetActive(false);
            if (lineren.enabled == true)  setlinerenpos();
              

            if (ConeSmall.GetActive() == true) lineren.enabled = false;
        }
    }
    public void setlinerenpos()
    {
        lineren.SetPosition(0, transform.position);
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
        {
            if (crosslevelvarhol.tutorial == true && hit.transform.gameObject.GetComponent<_Ship>() != null && hit.transform.gameObject.transform.parent != null && selectedships.Contains(hit.transform.gameObject) == false && hit.transform.gameObject.GetComponent<_Ship>().ShipColor == team) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTouchpadtouch);
            addtolist(hit.transform.gameObject);
            lineren.SetPosition(1, hit.point);
        }
        else lineren.SetPosition(1, transform.position + transform.forward * 1000);
    }
    //  on touchpad press down.
    void Touchpaddown(object sender, ControllerInteractionEventArgs e)  {  touchpadDown = true;    }
  
      
    // ontouchpadrelease.
    void Touchpadup(object sender, ControllerInteractionEventArgs e) { touchpadDown = false;  }
   
   // void gripwhiletouchpad() {   ConeSmall.SetActive(true);    }
   
     

    // on trigger pull
    void Trigger(object sender, ControllerInteractionEventArgs e)
    {
        resizing.startclick(transform.parent.localRotation);
        if (MenuObj.GetActive() == true  || lefthandscriptl.MenuObj.GetActive() == true)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10000  ) && hit.collider.gameObject.name == "LeaveMatch")
            {
                if (PhotonNetwork.isMasterClient == true) GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().phopview.RPC("HostLeft", PhotonTargets.All);
                else
                {
                    PhotonNetwork.LeaveRoom();
                    PhotonNetwork.LoadLevel(0);
                }
            }
        }

        if (resizing.WaspPos == null || resizing.WaspPos.GetActive() == false)
        {
            if (touchpadDown == false && selectedships.Count > 0 && lineren.enabled == false) Startmove(false);
            else if (touchpadDown) Startmove(true);
            if (lineren.enabled == true && touchpadDown == false && selectedships.Count > 0) triggerwhiletouchpad();
        }
        if (touchpadDown == false) triggerdown = true;
        
    }

    void TriggerUp(object sender, ControllerInteractionEventArgs e) {triggerdown = false;  }
   
    void grip(object sender, ControllerInteractionEventArgs e)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6 && lineren.enabled == false)
        { 
           team = GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().team;
           selectAll();
        }
    }
    void triggerwhiletouchpad ()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
        {
            if (hit.transform.gameObject.GetComponent<_Ship>() != null && hit.transform.gameObject.transform.parent != null && hit.transform.gameObject.GetComponent<_Ship>().ShipColor != team)
            {
                if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTriggerGiveLongAttackOrder);
                inputrelay.ordermove(selectedships, hit.transform.position.ToTSVector(), hit.transform.gameObject, calculate_average_speed(selectedships.ToArray()),false);
                givepulsewaveorder(spawnpulsewave(hit.collider.transform.position, pulsewavered), hit.transform.gameObject);
                lineren.material = attackordermat;
                revertcountdown = 30;
            }
        }
    }
    public Material attackordermat;
    void Menu(object sender, ControllerInteractionEventArgs e)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightMenu);
            if (lefthandscriptl.MenuObj.GetActive() == false)
            {
                Debug.Log(MenuObj.GetActive());
                if (MenuObj.GetActive() == false)
                {
                    turnonmenu(true);
                    MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
                }
                else turnonmenu(false);
            }
            else
            {
                lefthandscriptl.MenuObj.SetActive(false);
                if (MenuObj.GetActive() == false)
                {
                    turnonmenu(true);
                    MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
                }
            }
           
        }
        else
        {      
           resizeobj.transform.localPosition = new Vector3(0.119999f, 4.86f, 5.71f);
            resizing.countup = 0;
            resizeobj.transform.Find("DragOBJ").transform.localPosition = new Vector3(0, 0, 0);
        }
    } 

    void turnonmenu (bool ison)
    {
        setlinerenpos();
        MenuObj.SetActive(ison);
    }

    void Startmove(bool waypoints)
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, 1000))
        {
            ButtonPressedMessageSender hitscripttemp = hit.transform.gameObject.GetComponent<ButtonPressedMessageSender>();
            if (hitscripttemp == null)  startmoveactual(waypoints);
        }
        else  startmoveactual(waypoints);
    }

    public void startmoveactual (bool waypoints)
    {

        if (waypoints == true && crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTriggerGiveWayPoint);


        if (ontriggerenter.closest == null)
        {
            if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightTrigger);

            inputrelay.ordermove(selectedships, transform.parent.Find("MovePos").position.ToTSVector(), null, calculate_average_speed(selectedships.ToArray()), waypoints);
            if (waypoints == false) spawnpulsewave(transform.parent.Find("MovePos").position, pulsewave);
            else spawnpulsewave(transform.parent.Find("MovePos").position, Pulserenderwaypoint);
        }
        else
        {
            if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightAttackOrder);
            inputrelay.ordermove(selectedships, ontriggerenter.closest.transform.position.ToTSVector(), ontriggerenter.closest, calculate_average_speed(selectedships.ToArray()), waypoints);
            if (waypoints == false) givepulsewaveorder(spawnpulsewave(ontriggerenter.closest.transform.position, isalliedtarget(ontriggerenter.closest)), ontriggerenter.closest);
            else spawnpulsewave(transform.parent.Find("MovePos").position, Pulserenderwaypoint);

        }
    }
   public static FP calculate_average_speed (GameObject[] ships)
    {
        FP output = 0;
        foreach(GameObject gam in ships)
        {
         if(gam != null && gam.gameObject != null)   output += gam.GetComponent<_Ship>().MaxSpeed;
        }
        output = output / ships.Length;
        return output;
    }

    GameObject spawnpulsewave (Vector3 startpos, GameObject pulsewavetospawn)
    {
        GameObject gam = Instantiate(pulsewavetospawn);
        gam.transform.position = startpos; 
        gam.GetComponent<F3DPulsewave>().ScaleSize = new Vector3(0.01f,0.0005f,0.01f) * resizeobj.transform.localScale.x;
        return gam;
    }

    public GameObject Pulserenderwaypoint;

    GameObject isalliedtarget(GameObject input)
    {
        if (UnitMovementcommandcontroller.getteam(input.GetComponent<_Ship>().ShipColor, unitcon.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(unitcon.team, unitcon.crosslevelholder.Gamemode))
            return pulsewave;
        else return pulsewavered;
    }

    void givepulsewaveorder (GameObject gam,GameObject target){ gam.GetComponent<F3DPulsewave>().trackobj = target;  }

    void selectAll()
    {
        if(WorldBase == null || WorldBase.gameObject == null) WorldBase = GameObject.Find("World").transform.Find("Objects").gameObject;
        if (selectedships.Count > 0)
        {
            foreach (GameObject a in selectedships) if (a.gameObject != null) a.GetComponent<_Ship>().Disable();
            selectedships.Clear();
            if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightGripdeselectall);
        }
        else
        {
            if (crosslevelvarhol.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.RightGripSelectall);
            foreach (Transform i in WorldBase.transform) if (i.gameObject.name != "Engines" && i.gameObject.name != "Working" && i.gameObject.GetComponent<_Ship>() != null && i.gameObject.GetComponent<_Ship>().ShipColor == team) addtolist(i.gameObject);
        }
    }

    // adds ship to selectedships.
    void addtolist(GameObject col)
    {
        col.GetComponent<_Ship>().enablehealthbar();
        if (col.GetComponent<_Ship>() != null && col.transform.parent != null && selectedships.Contains(col) == false)
        {
            if (col.GetComponent<_Ship>().ShipColor == team)
            {
                selectedships.Add(col);
                col.GetComponent<_Ship>().enable();
                col.GetComponent<_Ship>().disablehealthbar();
                
            }
            else
            {
                col.GetComponent<_Ship>().enablehealthbar();
            }
        } 
    }

    // gets the position required for a ship to move to.
    private int[] agentsPerSide = new int[20];
    private TSVector TargetPosition(int index, TSVector sphere, int agentsnum)
    {
        if (agentsnum != 0 && agentsPerSide.Length != 0 && index < agentsPerSide.Length)
        {
            var separation = 100;
            agentsPerSide[index] = agentsnum / 3 + (agentsnum % 3 > 0 ? 1 : 0);
            var length = agentsnum * 200;
            var side = index % 3;
            var lengthMultiplier = (index / 3) / (float)agentsPerSide[side];
            lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);
            var height = length / 2 * Mathf.Sqrt(3); // Equilaterial triangle height
            if (index == 0) return sphere;
            else return sphere + new TSVector(separation * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, separation * (((index - 1) / 2) + 1));
        }
        else return sphere;
    }
}
