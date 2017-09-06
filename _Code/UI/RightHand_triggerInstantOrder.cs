using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.Steering;
using TrueSync;
using VRTK;
using Forge3D;
using UnityEngine.SceneManagement;

/// <summary>
/// Script controls the inputs and behaviors of the players right hand.
/// </summary>
public class RightHand_triggerInstantOrder : TrueSyncBehaviour {

    // Do you have an order to do on the next frame?
    public bool ordernow;
    // the linerenderer attached to the local Gameobject.
    public LineRenderer lineren;
    // the ships currently selected by the player.
    public List<GameObject> selectedships = new List<GameObject>();       
    // the material attached to the linerenderer.     
    public Material linemat;
    // is the touchpad down?
    public bool touchpadDown;
    // how many ships are selected?
    private int selectioncount;
    // is the trigger down?
    public bool triggerdown;
    // the resizing script.
    private Resizescript resizing;
    // the photonview attached to this gameobject.
    private PhotonView photonview;
    // the players ships.
    public _Ship.eShipColor team;
    // the world root base.
    private GameObject WorldBase;
    // the main camera in the world
    public Camera cameramain;
    // the main relay to send input to.
    public RelayController inputrelay;
    // the pulsewave prefab to spawn on an order.
    public GameObject pulsewave;
    // the vrtk source of input allowing the script to take inputs from both the oculus rift and the vive.
    public VRTK_ControllerEvents vrtkcontrolevents;
    // the effective trigger, allows you to see the closest ship with a tag.
    private onTriggerEnterEnemy ontriggerenter;
    // the touchpad degree from center.
    public float touchpadamount;
    // the menu Object this Controller can activate.
    public GameObject MenuObj;
    // the scale of this gameobject at the start of the match.
    private Vector3 startscale;
    // the position of the object at the start of the match.
    private Vector3 startpos;
    // the script corresponding to the the left hand instructions.
    public LeftControl_Onehanddrag lefthandscriptl;
    // the Camera attached to the oculus headset.
    public GameObject oculuscam;
    // the Camera attached to the vive headset.
    public GameObject Vivecam;
    // the pulsewave prefab to give on an attack order.
    public GameObject pulsewavered;
    // the Game Controller
    private UnitMovementcommandcontroller unitcon;
    // the tutorial controller.
    private TutorialTextControlScript tutcontrol;
    // the cross level variable holder.
    private CrossLevelVariableHolder crosslevelvarhol;
    // material for the linerender on an attack order.
    public Material attackordermat;
    // The pulsewave to spawn if a waypoint order has been given.
    public GameObject Pulserenderwaypoint;

    /// <summary>
    /// Initalise everything and deligates relating to the controller input.
    /// </summary>
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

    /// <summary>
    /// reset the position of the parent gameobject.
    /// </summary>
    void resetallpos ()
    {
        if (resizeobj.gameObject != null)
        {
            resizeobj.transform.localScale = startscale;
            resizeobj.transform.position = startpos;
            resizeobj.transform.GetChild(0).transform.localPosition = new Vector3(0, 0, 0);
        }
    }
    
    /// <summary>
    /// Turn the player manually with the extra button on the oculus touch.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void turnbutton(object sender, ControllerInteractionEventArgs e)   {    resizing.rotateright();  }
 

    /// <summary>
    /// Reset everything when nessesitated by the level loading.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
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

    /// <summary>
    /// the truesync deterministic start, update everything relating to that.
    /// </summary>
    public override void OnSyncedStart()
    {
        inputrelay = GameObject.Find("TrueSyncManager").GetComponent<RelayController>();
        owner = TrueSyncManager.LocalPlayer;
        ownerIndex = TrueSyncManager.LocalPlayer.Id;
    }

    /// <summary>
    /// the update to update everything relating to things like touchpad amount and the linerender positioning.
    /// </summary>
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
    
    /// <summary>
    /// Update the linerender position.
    /// </summary>
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

    /// <summary>
    /// Do event on the controllers touchpad down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Touchpaddown(object sender, ControllerInteractionEventArgs e)  {  touchpadDown = true;    }
  
      
    /// <summary>
    /// Do events on the touchpadup
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Touchpadup(object sender, ControllerInteractionEventArgs e) { touchpadDown = false;  }

    /// <summary>
    /// Do events on trigger down
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Do events on trigger up
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TriggerUp(object sender, ControllerInteractionEventArgs e) {triggerdown = false;  }
   
    /// <summary>
    /// Do events on grip down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void grip(object sender, ControllerInteractionEventArgs e)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6 && lineren.enabled == false)
        { 
           team = GameObject.Find("JoinMultiplayer").transform.parent.GetComponent<UnitMovementcommandcontroller>().team;
           selectAll();
        }
    }

    /// <summary>
    /// Create a waypoint order if the trigger is pressed whilst the touchpad is down.
    /// </summary>
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

    /// <summary>
    /// Open the controller menu upon the menu press.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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


    /// <summary>
    /// Enable the menu.
    /// </summary>
    /// <param name="ison"></param>
    void turnonmenu (bool ison)
    {
        setlinerenpos();
        MenuObj.SetActive(ison);
    }

    /// <summary>
    /// Check if a move order is valid and define its parameters.
    /// </summary>
    /// <param name="waypoints"></param>
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

    /// <summary>
    /// Actually use the parameters to create move order. 
    /// </summary>
    /// <param name="waypoints"></param>
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

    /// <summary>
    /// Get the average speed of every ship in the fleet
    /// </summary>
    /// <param name="ships">every ship in the fleet</param>
    /// <returns></returns>
    public static FP calculate_average_speed(GameObject[] ships)
    {
        FP output = 0;
        foreach (GameObject gam in ships)
        {
            if (gam != null && gam.gameObject != null) output += gam.GetComponent<_Ship>().MaxSpeed;
        }
        output = output / ships.Length;
        return output;
    }

    /// <summary>
    /// Create a little icon to show that an attack order has been given.
    /// </summary>
    /// <param name="startpos"></param>
    /// <param name="pulsewavetospawn"></param>
    /// <returns></returns>
    GameObject spawnpulsewave (Vector3 startpos, GameObject pulsewavetospawn)
    {
        GameObject gam = Instantiate(pulsewavetospawn);
        gam.transform.position = startpos; 
        gam.GetComponent<F3DPulsewave>().ScaleSize = new Vector3(0.01f,0.0005f,0.01f) * resizeobj.transform.localScale.x;
        return gam;
    }

    
    /// <summary>
    /// check if the target gameobject is on the same team as the player.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    GameObject isalliedtarget(GameObject input)
    {
        if (UnitMovementcommandcontroller.getteam(input.GetComponent<_Ship>().ShipColor, unitcon.crosslevelholder.Gamemode) == UnitMovementcommandcontroller.getteam(unitcon.team, unitcon.crosslevelholder.Gamemode))
            return pulsewave;
        else return pulsewavered;
    }

    /// <summary>
    /// Give the pulse wave the ability to track the attack order target.
    /// </summary>
    /// <param name="gam"></param>
    /// <param name="target"></param>
    void givepulsewaveorder (GameObject gam,GameObject target){ gam.GetComponent<F3DPulsewave>().trackobj = target;  }

    /// <summary>
    /// Select every ship on the map.
    /// </summary>
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

    /// <summary>
    /// Checks if a ship can be added to the selected ships list and if so adds it.
    /// </summary>
    /// <param name="col"></param>
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

    /// <summary>
    /// Calculates positions in a delta around a point.
    /// </summary>
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
