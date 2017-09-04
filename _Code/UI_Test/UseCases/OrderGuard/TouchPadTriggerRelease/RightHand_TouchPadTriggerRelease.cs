using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Apex.Steering;


public class RightHand_TouchPadTriggerRelease : MonoBehaviour {
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId MenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId Abutton = Valve.VR.EVRButtonId.k_EButton_A;
    private LineRenderer lineren;
    [HideInInspector]
    public List<GameObject> selectedships = new List<GameObject>();                                                  // Initialization
    public GameObject MenuObj;
    public Material linemat;
    private GameObject ConeBig;
    private GameObject ConeSmall;
    public bool touchpadDown;
    private int selectioncount;
    public bool triggerdown;
    public bool touchpaddownonce;
    private bool inside;
    private GameObject moveobj;
    private TestCaseController testcontrol;
    private Resizescript resizing;
    public float test;
    private Vector3 startloc;
    private Quaternion startorien;
    public bool menuOn;
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();                                                          // establish tracking
        lineren = gameObject.AddComponent<LineRenderer>();
        resizing = transform.parent.parent.GetComponent<Resizescript>();
        testcontrol = transform.parent.parent.GetComponent<TestCaseController>();
        lineren.startWidth = 0.01f;
        lineren.endWidth = 0.01f;
        lineren.enabled = false;
        lineren.material = linemat;
        MenuObj = transform.Find("MoveMenuMenu").gameObject;
        moveobj = MenuObj.transform.Find("ControllerMoveMenu").Find("Effects").Find("Menu").gameObject;
        startloc = MenuObj.transform.localPosition;
        startorien = MenuObj.transform.localRotation;
        MenuObj.SetActive(false);
        ConeBig = transform.Find("ConeBig").gameObject;
        ConeBig.SetActive(false);
        ConeSmall = transform.Find("ConeSmall").gameObject;
        ConeSmall.SetActive(false);
        selectAll();
        
    }

    // Update is called once per frame
    void Update()
    {

        /*
           if (lineren.enabled == true)
           {
               lineren.SetPosition(0, transform.position + (transform.forward * 0.1f));
               lineren.SetPosition(1, transform.position);      // linemanipulation
           }
        */
        if (Vector3.Distance(transform.position, moveobj.transform.position) < 0.05f) inside = true;
        else inside = false;
        test = Vector3.Distance(transform.position, moveobj.transform.position);

        if (controller.GetPressDown(touchpad)) Touchpaddown();

        if (controller.GetPressUp(touchpad)) Touchpadup();

        if (controller.GetPressDown(MenuButton)) Menu();                         //inputcalls

        if (controller.GetPressDown(triggerButton)) Trigger();

        if (controller.GetPressUp(triggerButton)) TriggerUp();

        if (controller.GetPressDown(gripButton)) grip();
        if (controller.GetPressUp(gripButton)) gripup();

        if (touchpadDown == true && controller.GetPressDown(triggerButton)) triggerwhiletouchpad();
        touchpaddownonce = controller.GetPressDown(touchpad);
    }
    void Touchpaddown()
    {

        touchpadDown = true;


    }
    void Touchpadup()
    {

        touchpadDown = false;

    }

    void triggerwhiletouchpad()
    {

    }



    void Trigger()
    {
        triggerdown = true;

  if (touchpadDown)
        {
            Menu();
        }
        
        
    }
    void TriggerUp()
    {
        triggerdown = false;
         if (menuOn)
        {
            menuDisable();
        }
      
        
    }

    void grip()
    {
      
    }
    void gripup ()
    {
     
    }

    void Menu()
    {
        Debug.Log("ON");
        MenuObj.SetActive(true);
        MenuObj.transform.localPosition = startloc;
        MenuObj.transform.localRotation = startorien;
        MenuObj.transform.SetParent(null);
        menuOn = true;

    }
    public void menuDisable()
    {
        MenuObj.SetActive(false);
        MenuObj.transform.SetParent(this.transform);
        if (inside)
        {
            Startmove(MenuObj.transform.position);
        }
        menuOn = false;
    }
    void Startmove(Vector3 movepoint)
    {
        foreach (GameObject gam in selectedships)
        {
            IMovable mov = gam.GetComponent<IMovable>();
            mov.MoveTo(movepoint, true);
            Debug.Log("move");
        }
    }

    void selectAll()
    {
        GameObject root = GameObject.Find("World").transform.Find("Objects").Find("Working").gameObject;
        foreach (Transform i in root.transform)
        {
            if (i.name == "Ship 1")
            {
                addtolist(i.gameObject);
            }

        }
    }


    void addtolist(GameObject col)
    {
        if (col.transform.parent != null && selectedships.Contains(col) == false && col.transform.parent.name == "Working")
        {
            selectedships.Add(col);
            Debug.Log(col);
            col.GetComponent<Selection_Controller>().enable();

        }
    }
}
