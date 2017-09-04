using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class LeftControl : MonoBehaviour {

    // Controller Stuff
    public SceneField scene;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId MenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId Abutton = Valve.VR.EVRButtonId.k_EButton_A;

    // Script References
    private Resizescript resizing;
    private TestCaseController testcontrol;
    private RightControllerUI righthandscript;
    private LineRenderer lineren;
    

    // bools

    public bool touchpadDown;
    private bool gripdown;
    private bool triggerbuttondown;
    public bool teleport;
    public bool movenormal;
    public float touchpadamount;
    public Material linemat;
    private GameObject floor;

    void Start() {
        floor = GameObject.Find("Floor");
        floor.SetActive(false);
        resizing = transform.parent.parent.GetComponent<Resizescript>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        testcontrol = transform.parent.parent.GetComponent<TestCaseController>();
        lineren = gameObject.AddComponent<LineRenderer>();
        lineren.startWidth = 0.01f;
        lineren.endWidth = 0.01f;
        lineren.enabled = false;
        lineren.material = linemat;
        righthandscript = transform.parent.Find("Controller (right)").GetComponent<RightControllerUI>();
       
    }

    public Vector3 getposition()
    {
        Vector3 output = new Vector3(0, 0, 0);
        if (testcontrol.oneControllerTriggerScale || testcontrol.TwoControllerTriggerScale)
        {
            output = Vector3.Lerp(righthandscript.gameObject.transform.position, transform.position, 0.5f);
        }
        if (testcontrol.linearScaleWithTouchpad || testcontrol.controllerpositionscale)
        {
            output = this.transform.position;
        }
        return output;
    }

    void Update() {

        if (controller.GetPressDown(touchpad)) Touchpaddown();

        if (controller.GetPressUp(touchpad)) Touchpadup();

        if (controller.GetPressDown(MenuButton)) Menu();                         //inputcalls

        if (controller.GetPressDown(triggerButton)) Trigger();

        if (controller.GetPressUp(triggerButton)) Triggerup();

        if (controller.GetPressDown(gripButton)) gripdownin();

        if (controller.GetPressUp(gripButton)) gripupin();

        floor.transform.position = transform.parent.position;
        if (triggerbuttondown == true && testcontrol.oneControllerTriggerScale == true) resizing.change(getposition(),scalealterout());

        if (testcontrol.TwoControllerTriggerScale == true && righthandscript.triggerdown == true)resizing.change(getposition(), scalealterout());

        if (testcontrol.controllerpositionscale == true && triggerbuttondown == true) resizing.change(getposition(), scalealterout());

        if (testcontrol.linearScaleWithTouchpad) resizing.change(getposition(), scalealterout());

        touchpadamount = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[1];

        if (touchpadDown && (testcontrol.TwoHandDrag) && righthandscript.touchpadDown) resizing.translate(movepoint());

        if (touchpadDown && (testcontrol.OneHandDrag)) resizing.translate(movepoint());

        if ((testcontrol.raycastjump || teleport) && touchpadDown)
        {
            RaycastHit hit;
            lineren.SetPosition(0,transform.position);
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10000))
            {
                lineren.SetPosition(1, hit.point);
               
            }
            else lineren.SetPosition(1, transform.forward * 500);
            lineren.SetWidth(resizing.gameObject.transform.lossyScale.x / 50, resizing.transform.lossyScale.x / 50);
        }
        if (touchpadDown && (testcontrol.accelerateviamovement || movenormal))
        {
            resizing.move(touchpadamount);
        }
        if (touchpadDown)
        {
            if (testcontrol.TwoHandDrag && righthandscript.touchpaddownonce) resizing.starttranslate(movepoint());
        }
    }
    Vector3 movepoint ()
    {
        Vector3 moveaboutpoint = new Vector3(0,0,0);
        if (testcontrol.OneHandDrag) moveaboutpoint = transform.position;
        if (testcontrol.TwoHandDrag) moveaboutpoint = LerpByDistance(transform.position, righthandscript.transform.position, 0.5f);
        return moveaboutpoint;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    void Touchpaddown()
    {
     
        if (testcontrol.dpadnavigation)
        {
            if (touchpadamount < 0) movenormal = true;
            else teleport = true;

        }
        if (testcontrol.incrimenatalScaleWithToucpad)
        {
            if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[1] > 0) resizing.scaleup();

            else resizing.scaledown();
        }
        if (testcontrol.OneHandDrag || testcontrol.TwoHandDrag || testcontrol.accelerateviamovement || movenormal) resizing.starttranslate(movepoint());
        touchpadDown = true;
        
        if (testcontrol.raycastjump || teleport) 
        {
             floor.SetActive(true);
            lineren.enabled = true;
        }
    }
    void Touchpadup ()
    {
        touchpadDown = false;
       
 
   if (testcontrol.raycastjump || teleport)
        {
            lineren.enabled = false;
            RaycastHit hit;
            if (Physics.Raycast(transform.position, transform.forward,out hit, 10000))
            {
                Vector3 loc = resizing.gameObject.transform.position;
                loc.x = hit.point.x;
                loc.z = hit.point.y;
                transform.parent.position = loc;
                 Debug.Log(hit.transform.gameObject);
            }
            Debug.DrawLine(transform.position, transform.forward * 10000);
            floor.SetActive(false);
        }
 teleport = false;
        movenormal = false;
    
    }

    void Menu()
    {

    }
    void Trigger()
    {
        triggerbuttondown = true;
        if (testcontrol.oneControllerTriggerScale == true)resizing.disable();
        if (testcontrol.TwoControllerTriggerScale == true && righthandscript.triggerdown == true) resizing.disable();
        if (testcontrol.controllerpositionscale == true) resizing.lastposscale = righthandscript.gameObject.transform.localPosition;
      
    }
    void Triggerup()
    {
        triggerbuttondown = false;
     
     
       resizing.disable();
    }
    void gripdownin()
    {     
        gripdown = true;
        SceneManager.LoadSceneAsync(scene);
    }
    void gripupin()
    {
        gripdown = false;
    }
    public float scalealterout()
    {

        float scalealter = resizing.prevnumber;

        if (testcontrol.oneControllerTriggerScale || testcontrol.TwoControllerTriggerScale)
        {
            float number = Vector3.Distance(righthandscript.transform.localPosition, transform.localPosition);
            float compositenumber;
            compositenumber = number - resizing.prevnumber;
            scalealter = resizing.prevnumber / number;
            resizing.prevnumber = number;
        }

        if (testcontrol.linearScaleWithTouchpad)
        {
            scalealter = 1 + (touchpadamount / 20);
        }

        if (testcontrol.controllerpositionscale)
        {
            Vector3 positionmain = righthandscript.transform.localPosition;
            float output = -((resizing.lastposscale.y - positionmain.y) / 10);
            scalealter = 1 + output;

        }
        return scalealter;
    }


}
