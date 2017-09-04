using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Left_Hand_OneControllerScale : MonoBehaviour {


    // Controller Stuff

    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId MenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId Abutton = Valve.VR.EVRButtonId.k_EButton_A;

    // Script References
    private Resizescript resizing;
    private RightControllerUI righthandscript;
    private LineRenderer lineren;

    private CrossLevelVariableHolder crossvar;
    // bools

    public bool touchpadDown;
    private bool gripdown;
    private bool triggerbuttondown;
    public bool teleport;
    public bool movenormal;
    public float touchpadamount;
    public Material linemat;
    private GameObject floor;
    private TutorialTextControlScript tutcontrol;
    void Start()
    {
        
        floor = GameObject.Find("Floor");
        floor.SetActive(false);
        resizing = transform.parent.parent.GetComponent<Resizescript>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
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
        output = Vector3.Lerp(righthandscript.gameObject.transform.position, transform.position, 0.5f);     
        return output;
    }

    void Update()
    {

        if (controller.GetPressDown(touchpad)) Touchpaddown();

        if (controller.GetPressUp(touchpad)) Touchpadup();

        if (controller.GetPressDown(MenuButton)) Menu();                         //inputcalls

        if (controller.GetPressDown(triggerButton)) Trigger();

        if (controller.GetPressUp(triggerButton)) Triggerup();

        if (controller.GetPressDown(gripButton)) gripdownin();

        if (controller.GetPressUp(gripButton)) gripupin();

        floor.transform.position = transform.parent.position;
        if (triggerbuttondown == true) resizing.change(getposition(), scalealterout());
        touchpadamount = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[1];


    }


    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    void Touchpaddown()
    {
        touchpadDown = true;
    }
    void Touchpadup()
    {
        touchpadDown = false;
        teleport = false;
        movenormal = false;

    }

    void Menu()
    {

    }
    void Trigger()
    {
        triggerbuttondown = true;
        resizing.disable();
    }
    void Triggerup()
    {
        triggerbuttondown = false;


        resizing.disable();
    }
    void gripdownin()
    {
        gripdown = true;
    }
    void gripupin()
    {
        gripdown = false;
    }
    public float scalealterout()
    {

        float scalealter = resizing.prevnumber; 
        float number = Vector3.Distance(righthandscript.transform.localPosition, transform.localPosition);
        float compositenumber;
        compositenumber = number - resizing.prevnumber;
        scalealter = resizing.prevnumber / number;
        resizing.prevnumber = number;
        return scalealter;
    }


}
