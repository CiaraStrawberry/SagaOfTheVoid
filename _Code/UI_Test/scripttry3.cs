using UnityEngine;
using System.Collections;

public class scripttry3 : MonoBehaviour
{
    /*
    public bool mainmenu;
    private bool ChangedState;
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad1 = Valve.VR.EVRButtonId.k_EButton_DPad_Up;
    private Valve.VR.EVRButtonId Touchpad2 = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId MenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId Abutton = Valve.VR.EVRButtonId.k_EButton_A;

    public GameObject grabbed;
    private GameObject parent;
    private bool UI;
    private GameObject child;
    private GameObject childchild;
    public GameObject rightcontrol;
    private LineRenderer rend;
    public Material linematerial;
    private bool gripbuttondown;
    
    public Material Scalemat;
    public Material positionmat;
    private MeshRenderer mesh;
    public bool Scale;
    private bool position;
    private Vector3 originscale;
    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    public bool triggerdown;
  

    public GameObject Resizeobject;

    private Resizescript resizing;
    private Resizescript Resizeparent;
    public GameObject parent1;
    private bool scaledup;
    private GameObject parentmain;
    private bool touchpaddown;

    private GameObject pickup;
    public GameObject lefthand;
    private UnitActualSelection uni;
    public GameObject turntable;
    private Selector sel;
    private onTriggerEnterEnemy trig;
    private VariableStorage va;
    // Use this for initialization
    void Start()
    {
        GameObject linemanager = GameObject.FindWithTag("Man");
        va = linemanager.GetComponent<VariableStorage>();
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        trig = GetComponent<onTriggerEnterEnemy>();
        uni = lefthand.GetComponent<UnitActualSelection>();
        if (turntable)
        {
   sel = turntable.GetComponent<Selector>();
        }
     
        rend =  this.gameObject.AddComponent<LineRenderer>();
        rend.SetWidth(1, 1);
        parent1 = this.transform.parent.parent.gameObject;
        resizing = parent1.GetComponent<Resizescript>();
        rend.SetPosition(0, new Vector3(0, 0, 0));
        rend.SetPosition(1, new Vector3(0, 0, 0));
        rend.material = linematerial;
        Scale = true;
      
        
        originscale = new Vector3(5F, 5, 5);
        menu();

      
        
       if (this.transform.parent != null)
            {
               
              if (this.transform.parent.parent != null)
                 {
                    if (this.transform.parent.parent.parent != null)
                        {
                            parentmain = this.transform.parent.parent.parent.gameObject;
                       }
                   
                 }
                
        }

           child = transform.GetChild(0).gameObject;
        
       
       
    }

    // Update is called once per frame
    void Update()
    {

        if (controller.GetPressDown(Abutton))
        {
            resizing.rotateleft();
        }
        rend.SetWidth(transform.parent.parent.lossyScale.x / 8, transform.parent.parent.lossyScale.x / 8);
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
           
                 Debug.Log("STARTtranslate");
                
           if (mainmenu == false)
            {
              
                if (va.skirmish == false)
                {
                    resizing.starttranslate();
                }
               
                touchpaddown = true;
                
            }
      
               
            
            
        }
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) && controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[0] < 0)
        {
            if(uni.line2.enabled == true || Application.loadedLevelName == "level1select")
            {
                if ( sel.Numberthrough != 3)
                {
                    Debug.Log(sel.Numberthrough);
             sel.Numberthrough = sel.Numberthrough + 1;

                }
               

            }
           


        }
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad) && controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[0] > 0)
        {
            Debug.Log("pressdown");
            if (uni.line2.enabled == true || Application.loadedLevelName == "level1select")
            {

                {
                    if (sel)
                    {
  sel.Numberthrough = sel.Numberthrough - 1;
                    }
                  

                }
            }



        }
        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
                touchpaddown = false;
        }


        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }
        if (controller.GetPressDown(MenuButton))
        {
            menu();

        }



            if (ChangedState == true)
        { 
           resizing.scale1 = new Vector3(0,0,0);
            ChangedState = false;

        }
        if (controller.GetPressDown(triggerButton))
        {
            if (gripbuttondown != true)
            {
              triggerdown = true;
            if (trig != null)
            {
                if (trig.holofocus != null)
                {
                    grabbed = trig.holofocus;
                    trig.holofocus.transform.parent = this.transform;
                }

            }
            resizing.disable();
            }
       
        }
        else {
        if (controller.GetPressDown(gripButton))
        {
         if (Application.loadedLevelName != "level1select") 
          {
            if (parent1.transform.parent != null)
              {
                        if (va.skirmish == false)
                        {
                            parent1.transform.parent = null;
                            scaledup = true;
                            Debug.Log("scaleup");
                            resizing.scaleup();
                        }
               }
           else
                {
                     //   Debug.Log();
                     if (va.skirmish == false)
                        {
                           resizing.returnToship();
                        }
                  
                  
                  else {
                     if (triggerdown != true)
                        {
                                
                                    resizing.starttranslate();

                                
                    
                             
                        }
                    }
                     
                    
                    }

        //    resizing.starttranslate();
            gripbuttondown = true;
                }
                

        }
        }
        if (gripbuttondown)
        {
            if (va.skirmish == true)
            {
                if (triggerdown == false)
                {
                    if (transform.parent.parent.parent == null)
                    {
                        resizing.translate();
                    }
                
                }
                
            }
        }
        if (controller.GetPressUp(triggerButton))
        {
            triggerdown = false;
            resizing.disable();
            if (grabbed != null) {
                draggable drag = trig.holofocus.GetComponent<draggable>(); ;
                drag.reattach();
                grabbed = null;
            }

            
        }
        else
        {
          if (controller.GetPressUp(gripButton))
           {
                if (Application.loadedLevelName != "level1select")
                {
                    gripbuttondown = false;
                }
          }
        }

   
       if (touchpaddown)
        {
            if(triggerdown == false)
            {
                if (parent1.transform.parent == null && va.skirmish == false)
                {
                    resizing.translate();
                }
             
            }
            
        }
     if (triggerdown == true)
        {
            if (Application.loadedLevelName != "level1select")
            {
            if (rightcontrol) {
  rend.SetPosition(0, transform.position);
            rend.SetPosition(1, rightcontrol.transform.position);
            resizing.change();
            }
            }

          
        }
     else
        {
            rend.SetPosition(0, transform.position);
            rend.SetPosition(1, transform.position);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        pickup = collider.gameObject;

    }

    private void OnTriggerExit(Collider collider)
    {
        pickup = null;
    }
    void menu()
    {

        if (Scale == true)
        {

        
        //    position = true;
         //   Scale = false;
        }
       else
        {
         if (position == true)
        {
            Debug.Log("scale");
          
          Scale = true;
            position = false;
        }
        
        }
       
     
    }
    */
}