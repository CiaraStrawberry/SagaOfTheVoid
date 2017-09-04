using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

public class LeftControl_Onehanddrag : MonoBehaviour {

    public Resizescript resizing;
    public RightHand_triggerInstantOrder righthandscript;
    private LineRenderer lineren;


    // bools
    private GameObject World;
    public bool touchpadDown;
    private bool gripdown;
    private bool triggerbuttondown;
 
    public bool movenormal;
    public float touchpadamount;
    public Material linemat;
    private GameObject floor;
    public VRTK_ControllerEvents vrtkcontrolevents;
    private onTriggerEnterEnemy ontriggerenter;
    public GameObject MenuObj;
    private CrossLevelVariableHolder crossvar;
    public TutorialTextControlScript tutcontrol;
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if(GameObject.Find("CrossLevelVariables") != null) crossvar = GameObject.Find("CrossLevelVariables").GetComponent<CrossLevelVariableHolder>();
            if (crossvar.tutorial == true)
            {
                if(GameObject.Find("TutorialPanelHolder")) tutcontrol = GameObject.Find("TutorialPanelHolder").GetComponent<TutorialTextControlScript>();
            }
            World = GameObject.Find("World");
            floor = GameObject.Find("Floor");
            if (floor) floor.SetActive(false);
            ontriggerenter = GetComponent<onTriggerEnterEnemy>();
            lineren = gameObject.GetComponent<LineRenderer>();
            if(lineren == null) lineren = gameObject.AddComponent<LineRenderer>();
            lineren.startWidth = 0.01f;
            lineren.endWidth = 0.01f;
            lineren.enabled = false;
            lineren.material = linemat;
        }
    }
    void Awake ()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        righthandscript = GameObject.Find("RightController").GetComponent<RightHand_triggerInstantOrder>();
        MenuObj.SetActive(false);
        vrtkcontrolevents = GetComponent<VRTK_ControllerEvents>();
        vrtkcontrolevents.TouchpadPressed += new ControllerInteractionEventHandler(Touchpaddown);
        vrtkcontrolevents.TouchpadReleased += new ControllerInteractionEventHandler(Touchpadup);
        vrtkcontrolevents.TriggerPressed += new ControllerInteractionEventHandler(Trigger);
        vrtkcontrolevents.TriggerReleased += new ControllerInteractionEventHandler(TriggerUp);
        vrtkcontrolevents.GripPressed += new ControllerInteractionEventHandler(gripdownin);
        vrtkcontrolevents.GripReleased += new ControllerInteractionEventHandler(gripupin);
        vrtkcontrolevents.ButtonTwoPressed += new ControllerInteractionEventHandler(Menu);
        vrtkcontrolevents.ButtonOnePressed += new ControllerInteractionEventHandler(turnbutton);

        
    }
    void turnbutton(object sender, ControllerInteractionEventArgs e)
    {
        resizing.rotateleft();
    }
    void Menu(object sender, ControllerInteractionEventArgs e)
    {
       
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
             righthandscript.setlinerenpos();
            if (crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.leftMenuDown);
            if (righthandscript.MenuObj.GetActive() == false)
            {


                if (MenuObj.GetActive() == false)
                {
                    MenuObj.SetActive(true);
                  //  righthandscript.lineren.enabled = true;
                    MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
                }
                else
                {
                    MenuObj.SetActive(false);
                    righthandscript.lineren.enabled = false;
                }
            }
            else
            {
                righthandscript.MenuObj.SetActive(false);
                MenuObj.SetActive(true);
                
              //  righthandscript.lineren.enabled = true;
                MenuObj.GetComponent<PositinonRelativeToHeadset>().TurnOn();
            }
        }
    }
        
  
    bool insidepulse;
    void Update()
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            touchpadamount = vrtkcontrolevents.GetTouchpadAxis().y;
            if ((touchpadamount < -0.1f || touchpadamount > 0.1f) && touchpadDown == true) resizing.change(transform.position, scalealterout());        
        } 
        if(triggerbuttondown)
        {
           // resizing.starttranslate(transform.position);
            resizing.translate(transform.position);
        }
    }
    void Touchpaddown(object sender, ControllerInteractionEventArgs e)
    {
        touchpadDown = true;
        if (crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTouchpadPressed);

    }
    void Touchpadup(object sender, ControllerInteractionEventArgs e)
    {
        touchpadDown = false;
       
    }

    void Trigger(object sender, ControllerInteractionEventArgs e)
    {
        if (crossvar && tutcontrol && crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTriggerDown);
        resizing.starttranslate(transform.position);
        triggerbuttondown = true;
        
    }
    void TriggerUp(object sender, ControllerInteractionEventArgs e)
    {
      //  resizing.starttranslate(transform.position);
        triggerbuttondown = false;
    }
 
    void gripdownin(object sender, ControllerInteractionEventArgs e)
    {
        gripdown = true;
        // resizing.SwitchToWasp();
        if(resizing.transform.parent.name == "VRTK_SDK" && crossvar != null)
        {
            if (crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.leftGripDown);
            if (resizing.DragTarget == null) resizing.DragTarget = ontriggerenter.closest;
           else if (resizing.DragTarget != null) resizing.DragTarget = null;
        }
        else
        {
            resizing.SwitchToWasp();
        }
     
    }
    void gripupin(object sender, ControllerInteractionEventArgs e)
    {
        gripdown = false;
    }
    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }
    public float scalealterout()
    {
        if (touchpadamount < -0.1f || touchpadamount > 0.1f ) {
            float scalealter = resizing.prevnumber;
            scalealter = 1 + (touchpadamount / 20);
            return scalealter;
        }
        else return 1;
    }


}
