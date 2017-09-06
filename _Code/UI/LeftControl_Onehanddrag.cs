using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;
using UnityEngine.SceneManagement;

/// <summary>
/// This Class handels input by the left hand.
/// </summary>
public class LeftControl_Onehanddrag : MonoBehaviour {

    // The "Resizing" script referance, this handes things like scaling.
    public Resizescript resizing;
    // the reference to the script on the right hand.
    public RightHand_triggerInstantOrder righthandscript;
    // the linerenderer attached to this gameobject.
    private LineRenderer lineren;
    // the world root object.
    private GameObject World;
    // is the touchpad held down?
    public bool touchpadDown;
    // is the grip held down?
    private bool gripdown;
    // is the trigger held down?
    private bool triggerbuttondown;
    // the amount the touchpad is held down.
    public float touchpadamount;
    // the material of the linerenderer attached to this gameobject.
    public Material linemat;
    // the vrtk controller events script to get inputs from both oculus and vive.
    public VRTK_ControllerEvents vrtkcontrolevents;
    // the script to get the closest Gameobject to the player.
    private onTriggerEnterEnemy ontriggerenter;
    // the left hand Menu you can bring up.
    public GameObject MenuObj;
    // the singleton variable holder.
    private CrossLevelVariableHolder crossvar;
    // the tutorial control script.
    public TutorialTextControlScript tutcontrol;


    /// <summary>
    /// Reset everything when the level finishes loading.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
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

    /// <summary>
    /// initiate all the controller event deligates.
    /// </summary>
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

    /// <summary>
    /// the button used for turning is pressed, just roate your body left.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void turnbutton(object sender, ControllerInteractionEventArgs e)
    {
        resizing.rotateleft();
    }


    /// <summary>
    /// When the menu button on the controller is pressed, just open the menu.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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
        
  
    /// <summary>
    /// update touchpad amount and resizing.translate.
    /// </summary>
    void Update()
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            touchpadamount = vrtkcontrolevents.GetTouchpadAxis().y;
            if ((touchpadamount < -0.1f || touchpadamount > 0.1f) && touchpadDown == true) resizing.change(transform.position, scalealterout());        
        } 
        if(triggerbuttondown)  resizing.translate(transform.position);
    }

    /// <summary>
    /// Do events on touchpad down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Touchpaddown(object sender, ControllerInteractionEventArgs e)
    {
        touchpadDown = true;
        if (crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTouchpadPressed);

    }

    /// <summary>
    /// Do events on touchpad up
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Touchpadup(object sender, ControllerInteractionEventArgs e)
    {
        touchpadDown = false;
       
    }

    /// <summary>
    /// Do events on the trigger down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void Trigger(object sender, ControllerInteractionEventArgs e)
    {
        if (crossvar && tutcontrol && crossvar.tutorial == true) tutcontrol.MoveToNext(TutorialTextControlScript.tutorialstateinput.LeftTriggerDown);
        resizing.starttranslate(transform.position);
        triggerbuttondown = true;
        
    }

    /// <summary>
    /// Do events on the trigger up.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void TriggerUp(object sender, ControllerInteractionEventArgs e)
    {
      //  resizing.starttranslate(transform.position);
        triggerbuttondown = false;
    }
 
    /// <summary>
    /// Do events on the grip button down.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// Do events on the grip button up.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void gripupin(object sender, ControllerInteractionEventArgs e)
    {
        gripdown = false;
    }
    /// <summary>
    /// just a lerp function across vectors.
    /// </summary>
    /// <param name="A"></param>
    /// <param name="B"></param>
    /// <param name="x"></param>
    /// <returns></returns>
    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    /// <summary>
    /// Get the difference in scale determined by the touchpad amount.
    /// </summary>
    /// <returns></returns>
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
