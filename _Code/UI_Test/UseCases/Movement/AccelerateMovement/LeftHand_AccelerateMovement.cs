using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_AccelerateMovement : MonoBehaviour {


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


    // bools

    public bool touchpadDown;
    private bool gripdown;
    private bool triggerbuttondown;
    public bool teleport;
    public bool movenormal;
    public float touchpadamount;
    public Material linemat;
    private GameObject floor;

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
        Vector3 output = transform.position;
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

        touchpadamount = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[1];

        if (touchpadDown) resizing.move(touchpadamount);
        
     
    }
    Vector3 movepoint()
    {
        Vector3 moveaboutpoint = transform.position;
        return moveaboutpoint;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    void Touchpaddown()
    {
        resizing.starttranslate(movepoint());
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
        return scalealter;
    }


}
