using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeftHand_DPAD : MonoBehaviour {

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

    void Start() {
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


    void Update() {

        if (controller.GetPressDown(touchpad)) Touchpaddown();

        if (controller.GetPressUp(touchpad)) Touchpadup();

        if (controller.GetPressDown(MenuButton)) Menu();                         //inputcalls

        if (controller.GetPressDown(triggerButton)) Trigger();

        if (controller.GetPressUp(triggerButton)) Triggerup();

        if (controller.GetPressDown(gripButton)) gripdownin();

        if (controller.GetPressUp(gripButton)) gripupin();

        floor.transform.position = transform.parent.position;

        touchpadamount = controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[1];

        if ((teleport) && touchpadDown)
        {
            RaycastHit hit;
            lineren.SetPosition(0,transform.position);
            if (Physics.Raycast(transform.position, transform.forward, out hit, 10000))
            {
                lineren.SetPosition(1, hit.point);
               
            }
            else lineren.SetPosition(1, transform.forward * 50);
            lineren.SetWidth(resizing.gameObject.transform.lossyScale.x / 50, resizing.transform.lossyScale.x / 50);
        }
        if (touchpadDown && (movenormal)) resizing.move(touchpadamount);
    }
    Vector3 movepoint ()
    {
        Vector3 moveaboutpoint = new Vector3(0,0,0);
        if (movenormal) moveaboutpoint = transform.position;
        return moveaboutpoint;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;
        return P;
    }

    void Touchpaddown()
    {
     
    
        if (touchpadamount < 0) movenormal = true;
        else teleport = true;

        if (movenormal) resizing.starttranslate(movepoint());
        touchpadDown = true;
        
        if (teleport) 
        {
            floor.SetActive(true);
            lineren.enabled = true;
        }
    }
    void Touchpadup ()
    {
        touchpadDown = false;
       
 
   if ( teleport)
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
            Debug.Log("move");
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
 

}
