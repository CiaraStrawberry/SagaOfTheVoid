using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class UnitActualSelection : MonoBehaviour
{
    /*
    public LayerMask layermas1;
    public Material formmat;
    private Material orimat;
    private GameObject childmodel;
    private MeshRenderer childmodelrend;
    private Component selection_box;
    private Component Cone_collider;
    private GameObject capsule;
    private Resizescript resizing;
    private Childcollision capsulecol;
    private GameObject Sphere1;
    private Childcollision sphere1col;
    public GameObject cone;
    private Childcollision conecol;
    public bool select;
    public GameObject smallcone;
    private Childcollision smallconecol;
    private Childcollision smallcone_collider;
    private GameObject overlappingactors1;
    public List<GameObject> selectedactors = new List<GameObject>();
    private AllyMovement allymovement;
    private AllyMovementConnection allymovement2;
    private Vector3 position1;
    public GameObject panelinterface;
    public GameObject panelinterface2;
    public GameObject controller2;
    private bool UI;
    private bool touchpaddown;
    private Color origincolor;
    public GameObject objecttoselec;
    private Selector sel2;
    public bool mainmen;


    private GameObject hitobject;
    private MeshRenderer meshrender1;
    public bool triggerhelddown;
    private Childcollision col;
    private GameObject gridbase;
    private GameObject[] gridbasepoinnts;
    private GameObject gridbase2;
    private GameObject[] gridbasepoinnts2;
    private GameObject camerarig;
    private int i;
    public bool sel;
    public Vector3 origin;
    public LineRenderer line2;
    private GameObject sphere;
    private SphereCollider sph;
    private bool startmove;
    private Vector3 vec;
    public bool mainmenu;
    private onTriggerEnterEnemy trig;
    private MeshRenderer caprend;
    private MeshRenderer conesmallrend;
    private MeshRenderer spherebigrend;
    private MeshRenderer spheresmallrend;
    private MeshRenderer conebigrend;
    public GameObject grabbed;

    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId touchpad1 = Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad;
    private Valve.VR.EVRButtonId touchpad2 = Valve.VR.EVRButtonId.k_EButton_DPad_Right;
    private Valve.VR.EVRButtonId MenuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;
    private Valve.VR.EVRButtonId Abutton = Valve.VR.EVRButtonId.k_EButton_A;


    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Vector3 location;
    private bool move;
    private Vector3 position2;
    public GameObject position5;

    private bool triangle;
    private Vector3 centerpos;

    // Use this for initialization
    void Awake()
    {
  trig = this.GetComponent<onTriggerEnterEnemy>();
        if (objecttoselec)
        {
            sel2 = objecttoselec.GetComponent<Selector>();
        }
       // childmodel = transform.FindChild("vr_controller_vive_1_5").gameObject.transform.GetChild(0).gameObject;
        //controller mesh
       // childmodelrend = childmodel.GetComponent<MeshRenderer>();
     //   orimat = childmodelrend.material;

        trackedObj = GetComponent<SteamVR_TrackedObject>();
        if (mainmenu == false)
        {
          
            
            capsule = GameObject.FindWithTag("Cap");
            caprend = capsule.GetComponent<MeshRenderer>();
            origincolor = caprend.material.color;
            capsulecol = capsule.GetComponent<Childcollision>();
            smallcone = transform.FindChild("ConeSmall").gameObject;
            conesmallrend = smallcone.GetComponent<MeshRenderer>();
            smallconecol = smallcone.GetComponent<Childcollision>();
            Sphere1 = transform.FindChild("SPH").gameObject;
            // accounts for the sphere that is childed
            spherebigrend = Sphere1.GetComponent<MeshRenderer>();

            sphere1col = Sphere1.GetComponent<Childcollision>();
            //selection shape stuff
            selection_box = capsule.GetComponent<CapsuleCollider>();
            capsulecol.Disable();
            sphere1col.Disable();
            cone = GameObject.FindWithTag("Cone");
            conecol = cone.GetComponent<Childcollision>();
            conebigrend = conecol.GetComponent<MeshRenderer>();
            Cone_collider = cone.GetComponent<CapsuleCollider>();
            conecol.Disable();
            smallconecol.Disable();
            UI = false;
            if (panelinterface)
            {
 meshrender1 = panelinterface.GetComponent<MeshRenderer>();
            }
           if (panelinterface)
            {
  panelinterface.SetActive(false);
            }
          
        }
        InvokeRepeating("fastrepeat", 0, 0.003f);
         GameObject parent1 = this.transform.parent.parent.gameObject;
        resizing = parent1.GetComponent<Resizescript>();

        if (Application.loadedLevelName != "level1select" && Application.loadedLevelName != "Victory" && Application.loadedLevelName != "Defeat")
        {
            if (transform.parent != null && transform.parent.parent != null && transform.parent.parent.parent != null)
            {
                camerarig = this.transform.parent.parent.parent.gameObject;
            }
            if (camerarig)
            {
                gridbase2 = camerarig.transform.FindChild("HologramBase").gameObject;

                gridbasepoinnts2 = new GameObject[11];
                gridbasepoinnts2[0] = gridbase2.transform.FindChild("1").gameObject;
                gridbasepoinnts2[1] = gridbase2.transform.FindChild("2").gameObject;
                gridbasepoinnts2[2] = gridbase2.transform.FindChild("3").gameObject;
                gridbasepoinnts2[3] = gridbase2.transform.FindChild("4").gameObject;
                gridbasepoinnts2[4] = gridbase2.transform.FindChild("5").gameObject;
                gridbasepoinnts2[5] = gridbase2.transform.FindChild("6").gameObject;
                gridbasepoinnts2[6] = gridbase2.transform.FindChild("7").gameObject;
                gridbasepoinnts2[7] = gridbase2.transform.FindChild("8").gameObject;
                gridbasepoinnts2[8] = gridbase2.transform.FindChild("9").gameObject;
                gridbasepoinnts2[9] = gridbase2.transform.FindChild("10").gameObject;
                gridbasepoinnts2[10] = gridbase2.transform.FindChild("11").gameObject;

            }


            gridbase = this.transform.FindChild("Points").gameObject;
            gridbasepoinnts = new GameObject[11];
            gridbasepoinnts[0] = gridbase.transform.FindChild("1").gameObject;
            gridbasepoinnts[1] = gridbase.transform.FindChild("2").gameObject;
            gridbasepoinnts[2] = gridbase.transform.FindChild("3").gameObject;
            gridbasepoinnts[3] = gridbase.transform.FindChild("4").gameObject;
            gridbasepoinnts[4] = gridbase.transform.FindChild("5").gameObject;
            gridbasepoinnts[5] = gridbase.transform.FindChild("6").gameObject;
            gridbasepoinnts[6] = gridbase.transform.FindChild("7").gameObject;
            gridbasepoinnts[7] = gridbase.transform.FindChild("8").gameObject;
            gridbasepoinnts[8] = gridbase.transform.FindChild("9").gameObject;
            gridbasepoinnts[9] = gridbase.transform.FindChild("10").gameObject;
            gridbasepoinnts[10] = gridbase.transform.FindChild("11").gameObject;
            sphere = gridbase.transform.FindChild("Sphere (1)").gameObject;
            sph = sphere.GetComponent<SphereCollider>();
            spheresmallrend = sph.GetComponent<MeshRenderer>();
            

        }
        switchmode();

        line2 = this.gameObject.AddComponent<LineRenderer>();
        line2.SetVertexCount(2);
        line2.SetWidth(0.01f, 0.01f);
        //   line2.material = new Material(Shader.Find("Specular"));
        line2.material.color = Color.red;

        line2.enabled = false;
        if (Application.loadedLevelName != "level1select" && Application.loadedLevelName != "Victory" && Application.loadedLevelName != "Defeat")
        {

        }
        else
        {
            MenuButton1();
        }

    }
    public GameObject curhit;
    // Update is called once per frame
    void Update()
    {
        if (controller.GetPressDown(Abutton))
        {
            resizing.rotateright();
        }
        if (triggerhelddown == false)
        {
            nullall();
        }
        foreach (GameObject actor in selectedactors)
        {
            if (actor.gameObject == null)
            {
                selectedactors.Remove(actor);
            }
        }

        if (trig)
        {
            if (trig.focus == null)
            {
                if (triggerhelddown == true)
                {
                    Vector3 mid;
                    mid = new Vector3(0, 0, 0);
                    foreach (GameObject ac in selectedactors)
                    {
                        mid += ac.transform.position;

                    }
                    mid = mid / selectedactors.Count;

                    centerpos = mid;

                    int i = 0;
                    foreach (GameObject actor in selectedactors)
                    {
                        AllyMovement al;
                        al = actor.GetComponent<AllyMovement>();
                        if (al != null)
                        {
                            if (triangle == true)
                            {
                                al.SelectionTarget = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                            }
                            else
                            {
                                Vector3 distancefromcenter = actor.transform.position - centerpos;
                                al.SelectionTarget = sphere.transform.position + distancefromcenter;

                            }

                        }
                        else
                        {
                            AllyMovementConnection al2;
                            al2 = actor.GetComponent<AllyMovementConnection>();
                            if (al2)
                            {

                                if (triangle == true)
                                {
                                    al2.selectiontarget = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                                }
                                else
                                {
                                    Vector3 distancefromcenter = actor.transform.position - centerpos;
                                    al2.selectiontarget = sphere.transform.position + distancefromcenter;
                                }
                            }

                        }

                        i++;
                    }



                }

                }
                else
                {
                    if (triggerhelddown == true)
                    {
                        
                        foreach (GameObject actor in selectedactors)
                        {
                            AllyMovement al;
                            al = actor.GetComponent<AllyMovement>();
                            if (al != null)
                            {
                               
                                     al.SelectionTarget = trig.focus.transform.position;                           
                               
                            }
                            else
                            {
                                AllyMovementConnection al2;
                                al2 = actor.GetComponent<AllyMovementConnection>();
                                if (al2)
                                {
                                    if (trig.focus != null)
                                    {
                                        al2.selectiontarget = trig.focus.transform.position;
                                    }

                        

                                }

                            }

                            i++;
                        }
                    }




                }
            
        }
        if (line2)
        {
if (line2.enabled == true)
        {
            line2.SetWidth(transform.parent.parent.lossyScale.x / 60, transform.parent.parent.lossyScale.x / 60);
            RaycastHit hit;
            if (Physics.Raycast(transform.position, this.transform.forward, out hit, 10000))
            {

                curhit = hit.transform.gameObject;
                TextHighlight tx;
                tx = hit.transform.gameObject.GetComponent<TextHighlight>();

                if (tx != null)
                {
                    tx.uni = this;
                    tx.highlighter();
                    curhit = hit.transform.gameObject;

                }

            }
            else
            {
                curhit = null;

            }


        }
        }
        
        if (sphere)
        {
            vec = sphere.transform.position;
        }

        if (startmove == true)
        {

        }

        if (position5 != null)
        {
            position1 = trackedObj.transform.position;
            position2 = position5.transform.localPosition;
        }


        if (controller.GetPressUp(triggerButton))
        {
            if (childmodelrend)
            {
   childmodelrend.material = orimat;
            }
         
            triggerhelddown = false;
            i = 0;

            if (line2.enabled == false)

            {
                listsort();
                foreach (GameObject actor in selectedactors)
                {
                    if (actor.gameObject != null)
                    {

                        if (trig.focus != null)
                        {

                            if ((actor.GetComponent("AllyMovementConnection") as AllyMovementConnection != null))
                            {
                                allymovement2 = actor.GetComponent<AllyMovementConnection>();

                                allymovement2.position1 = trig.focus.transform.position;
                                //  allymovement2.position1 = vec;

                                allymovement2.start_move();
                                startmove = true;
                            }
                            else
                            {
                                if ((actor.GetComponent("AllyMovement") as AllyMovement != null))
                                {

                                    allymovement = actor.GetComponent<AllyMovement>();
                                    Debug.Log("crap");
                                    allymovement.target = trig.focus;
                                    

                                    
                                    //      allymovement.position1 = trig.focus.transform.position;
                                    allymovement.start_move(selectedactors.ToArray());
                                    allymovement.moving = true;
                                    startmove = true;

                                    Debug.Log(trig.focus);
                                }
                            }
                        }
                        else
                        {
                            if ((actor.GetComponent("AllyMovementConnection") as AllyMovementConnection != null))
                            {
                                allymovement2 = actor.GetComponent<AllyMovementConnection>();
                                Vector3 target2;
                                if (triangle == true)
                                {
                                    target2 = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                                }
                                else
                                {
                                    Vector3 distancefromcenter = actor.transform.position - centerpos;
                                    target2 = sphere.transform.position + distancefromcenter;
                                   
                                }

                                allymovement2.position1 = target2;
                                allymovement2.start_move();
                                allymovement2.moving = true;
                                startmove = true;
                            }
                            else
                            {
                                if ((actor.GetComponent("AllyMovement") as AllyMovement != null))
                                {

                                    allymovement = actor.GetComponent<AllyMovement>();
                                    allymovement.target = null;

                                    Vector3 target2;
                                    if (triangle == true)
                                    {
                                        target2 = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                                    }
                                    else
                                    {
                                        Vector3 distancefromcenter = actor.transform.position - centerpos;
                                        target2 = sphere.transform.position + distancefromcenter;
                                        Debug.Log("centerpos = " + centerpos);
                                       // Debug.Log("move");
                                    }


                                    allymovement.position1 = target2;
                                    allymovement.start_move(selectedactors.ToArray());
                                    allymovement.moving = true;
                                    startmove = true;


                                }

                            }
                        }


                    }

                    i++;
                }



            }
            triangle = false;
        }

        if (controller.GetPressDown(MenuButton))
        {

            MenuButton1();
            removeall();

        }

   
        if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[0] < 0
&& controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))

        {
            touchpaddown = true;
            smallconecol.Enable();
            smallconecol.expand();

        }

        if (controller.GetAxis(Valve.VR.EVRButtonId.k_EButton_Axis0)[0] > 0
&& controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))

        {
            touchpaddown = true;
            sphere1col.Enable();
            sphere1col.expand();

        }

        if (controller.GetPressUp(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            touchpaddown = false;
            if (conecol)
            {

            }
            else
            {
                cone = GameObject.FindWithTag("Cone");
                conecol = cone.GetComponent<Childcollision>();
                smallcone = transform.FindChild("ConeSmall").gameObject;
                smallconecol = smallcone.GetComponent<Childcollision>();
                Sphere1 = transform.FindChild("SPH").gameObject;
                sphere1col = Sphere1.GetComponent<Childcollision>();
                conecol.Disable();
                smallconecol.Disable();
                sphere1col.Disable();
            }
            if (conecol.gameObject != null && smallconecol.gameObject != null && sphere1col.gameObject != null)
            {
             conecol.Disable();
            smallconecol.Disable();
            sphere1col.Disable();
            }
            else
            {
                cone = GameObject.FindWithTag("Cone");
                conecol = cone.GetComponent<Childcollision>();
                smallcone = transform.FindChild("ConeSmall").gameObject;
                smallconecol = smallcone.GetComponent<Childcollision>();
                Sphere1 = transform.FindChild("SPH").gameObject;
                sphere1col = Sphere1.GetComponent<Childcollision>();
                conecol.Disable();
                smallconecol.Disable();
                sphere1col.Disable();
            }
           

            if (select == false)
            {
                switchmode();
            }
        }
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_SteamVR_Touchpad))
        {
            if (triggerhelddown == true)
            {
                triangle = true;
            }



        }
        if (controller.GetPressDown(Valve.VR.EVRButtonId.k_EButton_Grip))
        {
            if (grabbed)
            {
               
                Destroy(grabbed);
               
                grabbed = null;
            }
            if (touchpaddown == true)
            {
                switchmode();
            }
            else
            {
                if (line2.enabled == true)
                {

                }
                else
                {
                    removeall();
                }

            }


        }



        if (controller.GetPressDown(triggerButton))
        {

            triggerhelddown = true;

            i = 0;
            if (Application.loadedLevelName == "level1select")
            {

                spawngrabsmall();
            }
            if (line2.enabled == true)
            {
              
                RaycastHit hit;
                if (Physics.Raycast(transform.position, this.transform.forward, out hit, 10000))
                {


                    if ((hit.transform.GetComponent("UI_DetectionScript") as UI_DetectionScript != null))
                    {
                        UI_DetectionScript UI;
                        UI = hit.transform.GetComponent<UI_DetectionScript>();
                        UI.Clicked();



                    }
                }
                if (grabbed)
                {
                    spawngrabsmall();
                   

                }
            }
            else
            {
                listsort();
                foreach (GameObject actor in selectedactors)
                {
                    if (actor.gameObject != null)
                    {

                        if (trig.focus != null)
                        {

                            if ((actor.GetComponent("AllyMovementConnection") as AllyMovementConnection != null))
                            {
                                allymovement2 = actor.GetComponent<AllyMovementConnection>();

                                allymovement2.position1 = trig.focus.transform.position;
                                //  allymovement2.position1 = vec;

                                allymovement2.start_move();
                                startmove = true;
                            }
                            else
                            {
                                if ((actor.GetComponent("AllyMovement") as AllyMovement != null))
                                {

                                    allymovement = actor.GetComponent<AllyMovement>();

                                    allymovement.target = trig.focus;


                                    //      allymovement.position1 = trig.focus.transform.position;
                                    allymovement.start_move(selectedactors.ToArray());
                                    allymovement.moving = true;
                                    startmove = true;


                                }
                            }
                        }
                        else
                        {
                            if ((actor.GetComponent("AllyMovementConnection") as AllyMovementConnection != null))
                            {
                                allymovement2 = actor.GetComponent<AllyMovementConnection>();

                                Vector3 target2 = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                                allymovement2.position1 = target2;
                                allymovement2.start_move();

                                startmove = true;
                            }
                            else
                            {
                                if ((actor.GetComponent("AllyMovement") as AllyMovement != null))
                                {

                                    allymovement = actor.GetComponent<AllyMovement>();
                                    allymovement.target = null;

                                    Vector3 target2 = TargetPosition(i, sphere.transform.position, selectedactors[0], selectedactors.Count);
                                    allymovement.position1 = target2;
                                    allymovement.start_move(selectedactors.ToArray());
                                    allymovement.moving = true;
                                    startmove = true;
                                }



                            }
                        }


                    }

                    i++;
                }


            }
        
            if (Application.loadedLevelName != "Victory" && Application.loadedLevelName != "Defeat")
            {
                if (line2.enabled == true || Application.loadedLevelName == "level1select")
                {
                    Grab();

                }
                else
                {

                }
            }

        }



    }





    public void MenuButton1()
    {
        if (Application.loadedLevelName == "level1select")
        {

        }
     

        if (UI == false)
        {

            line2.enabled = true;
            if (panelinterface)
            {
                panelinterface.SetActive(true);
            }

            UI = true;
        }
        else
        {
            line2.enabled = false;

            if (panelinterface)
            {
                panelinterface.SetActive(false);
            }

            UI = false;
        }





    }
    void fastrepeat()
    {


    }
    private void LateUpdate()
    {

        if (line2)
        {
            if (line2.enabled == true)
            {

                RaycastHit hit;
                
                if (Physics.Raycast(transform.position, this.transform.forward,out hit,layermas1))
                {
                    line2.SetPosition(0, this.transform.position);
                    line2.SetPosition(1, hit.point);


                }
                else
                {
                    line2.SetPosition(0, this.transform.position);
                    line2.SetPosition(1, transform.position + transform.forward * 1000);
                }

            }

        }

    }
    public void removeall()
    {
        foreach (GameObject actor in selectedactors)
        {
            deselect(actor);
        }
        selectedactors.Clear();
    }
    public void deselect(GameObject actor)
    {

        if (actor.gameObject != null)
        {
            if ((actor.GetComponent("AllyMovementConnection") as AllyMovementConnection != null))
            {
                allymovement2 = actor.GetComponent<AllyMovementConnection>();
                allymovement2.not_selected();

            }


            if ((actor.gameObject != null))
            {
                if ((actor.GetComponent("AllyMovement") as AllyMovement != null))
                {
                    allymovement = actor.GetComponent<AllyMovement>();
                    allymovement.NotSelected();

                }

            }

        }

    }

    public void switchmode()
    {
        if (select == true)
        {
            conesmallrend.material.color = Color.red;
            if (spheresmallrend)
            {
            spheresmallrend.material.color = Color.red;
            }
            else
            {
                sphere = gridbase.transform.FindChild("Sphere (1)").gameObject;
                spheresmallrend = sphere.GetComponent<MeshRenderer>();
                spheresmallrend.material.color = Color.red;
            }
            if (caprend)
            {
caprend.material.color = Color.red;
            }
            else
            {
                capsule = GameObject.FindWithTag("Cap");
                caprend = capsule.GetComponent<MeshRenderer>();
                caprend.material.color = Color.red;
            }
            if (conebigrend)
            {
  conebigrend.material.color = Color.red;
            }
            else
            {
                cone = GameObject.FindWithTag("Cone");
                conebigrend = cone.GetComponent<MeshRenderer>();
                conebigrend.material.color = Color.red;
            }
            if (conesmallrend)
            {
                conesmallrend.material.color = Color.red;
            }
            else
            {
                smallcone = transform.FindChild("ConeSmall").gameObject;
                conesmallrend = smallcone.GetComponent<MeshRenderer>();
                conesmallrend.material.color = Color.red;
            }
            if (spherebigrend)
            {
                spherebigrend.material.color = Color.red;
            }
            else 
            {
                Sphere1 = transform.FindChild("SPH").gameObject;
                spherebigrend = Sphere1.GetComponent<MeshRenderer>();
                spherebigrend.material.color = Color.red;
            }
            select = false;
           
        }
        else
        {
            if (origincolor != null)
            {
                if (conesmallrend)
                {
                    if (caprend)
                    {
                        if (spheresmallrend)
                        {
 conesmallrend.material.color = origincolor;
                        spheresmallrend.material.color = origincolor;
                        caprend.material.color = origincolor;
                        conebigrend.material.color = origincolor;
                        spherebigrend.material.color = origincolor;
                        select = true;

                        }
                       

                    }
                }
                else
                {
                    sphere = gridbase.transform.FindChild("Sphere (1)").gameObject;
                    spheresmallrend = sphere.GetComponent<MeshRenderer>();
                    spheresmallrend.material.color = origincolor;

                    capsule = GameObject.FindWithTag("Cap");
                    caprend = capsule.GetComponent<MeshRenderer>();
                    caprend.material.color = origincolor;

                    cone = GameObject.FindWithTag("Cone");
                    conebigrend = cone.GetComponent<MeshRenderer>();
                    conebigrend.material.color = origincolor;

                    smallcone = transform.FindChild("ConeSmall").gameObject;
                    conesmallrend = smallcone.GetComponent<MeshRenderer>();
                   conesmallrend.material.color = origincolor;

                    Sphere1 = transform.FindChild("SPH").gameObject;
                    spherebigrend = Sphere1.GetComponent<MeshRenderer>();
                    spherebigrend.material.color = origincolor;


                }

            }

        }
    }

    public bool mainmenupotential;
    public GameObject center;
    void Grab()
    {
        Debug.Log("grab");

        if (mainmenupotential == true)
        {
            if (Vector3.Distance(center.transform.position, transform.position) != 2)
            {
                if (grabbed == null)
                {
                 
               
                    if (trig.holofocus != null)
                    {

                        spawnobjectholder hol;
                        hol = trig.holofocus.GetComponent<spawnobjectholder>();
                        bool bol = hol.Spawnable();
                    
                        if (hol.placed == false)
                        {
                              if (bol == true)
                               {
                                     GameObject gam;
                                     grabbed = trig.holofocus;
                                     gam = sel2.Respawn(trig.holofocus, trig.holofocus.transform);
                                    
                                     trig.holofocus.transform.parent = transform;
                                     trig.holofocus.transform.localPosition = new Vector3(0, 0, 0.1f);
                                     trig.holofocus.transform.rotation = transform.rotation;

                               }
                        }
                        else
                        {

                           
                            grabbed = trig.holofocus;
                            trig.holofocus.transform.parent = transform;
                            trig.holofocus.transform.localPosition = new Vector3(0, 0, 0.1f);
                            trig.holofocus.transform.rotation = transform.rotation;
                            
                        



                        }
                        hol.placed = false;

                    }
                }
            }
        }
        else
        {

            if (grabbed == null)
            {
                if (trig.holofocus != null)
                {
                  
                    spawnobjectholder hol = trig.holofocus.GetComponent<spawnobjectholder>();
                    if (hol.placed == false)
                    {

                        if (hol.Spawnable() == true)
                        {

                            grabbed = trig.holofocus;
                            trig.holofocus.transform.parent = transform;
                            trig.holofocus.transform.localPosition = new Vector3(0, 0, 0.1f);
                            trig.holofocus.transform.rotation = transform.rotation;
                      
                        }
                    }
                    else
                    {

                        grabbed = trig.holofocus;
                        trig.holofocus.transform.parent = transform;
                        trig.holofocus.transform.localPosition = new Vector3(0, 0, 0.1f);
                        trig.holofocus.transform.rotation = transform.rotation;
                    }
                    hol.placed = false;
                }

            }

        }
        
    }

    private int[] agentsPerSide = new int[20];
    private Vector3 TargetPosition(int index, Vector3 target, GameObject leader, int agentsnum)
    {
        if (agentsnum != 0)
        {
            var separation = 400;
            agentsPerSide[index] = agentsnum / 3 + (agentsnum % 3 > i ? 1 : 0);
            var length = agentsnum * 200;
            var side = index % 3;
            var lengthMultiplier = (index / 3) / (float)agentsPerSide[side];
            lengthMultiplier = 1 - (lengthMultiplier - (int)lengthMultiplier);
            var height = length / 2 * Mathf.Sqrt(3); // Equilaterial triangle height
            var leaderTransform = leader == null ? transform : leader.transform;
            if (index == 0)
            {
                return sphere.transform.position;

            }
            else
            {
                return sphere.transform.position + new Vector3(separation * (index % 2 == 0 ? -1 : 1) * (((index - 1) / 2) + 1), 0, separation * (((index - 1) / 2) + 1));
            }
        }
        else
        {
            return sphere.transform.position;
        }
    }
    public List<GameObject> listsort()
    {
        List<GameObject> temp;
        temp = selectedactors;
        var continueSort = true;
        if (temp.Count > 1)
        {
            while (continueSort)
            {
                for (var i = 0; i < temp.Count; i++)
                {
                    for (var j = i + 1; j < temp.Count; j++)
                    {
                        if (temp[i].ToString().GetHashCode() > temp[j].ToString().GetHashCode())
                        {
                            continueSort = true;
                            var leftMost = temp[i];
                            temp[i] = temp[j];
                            temp[j] = leftMost;
                        }
                        else continueSort = false;
                    }
                }
            }
        }
        return temp;
    }


    void nullall()
    {
        GameObject remover = null;
        foreach (GameObject actor in selectedactors)
        {
            if (actor.gameObject == null)
            {
                remover = actor;
            }
        }
        if (remover != null)
        {
           selectedactors.Remove(remover);
           remover = null;
        }
     
        foreach (GameObject actor in selectedactors)
        {
           if (actor.gameObject != null )
            {
                
   AllyMovement al = actor.GetComponent<AllyMovement>();
            if (al != null)
            {
                al.SelectionTarget = new Vector3(0, 0, 0);
            }
            else
            {
                AllyMovementConnection al2;
                al2 = actor.GetComponent<AllyMovementConnection>();
                if (al2)
                {


                    al2.selectiontarget = new Vector3(0, 0, 0);

                }

            }
            }
           else
            {

            }
           
            
       


        }
    }
    void spawngrabsmall ()
    {
        if (grabbed)
        {
            if (mainmen == false)
        {

            spawnobjectholder spo;
            spo = grabbed.GetComponent<spawnobjectholder>();
            spo.Spawn(this.gameObject);
            Destroy(grabbed);
            grabbed = null;
            //    Debug.Log("spawn");
        }
        else
        {
            

            GameObject basehol = GameObject.Find("HologramBase");
            GameObject money = GameObject.FindWithTag("Mon");
            GameObject count = GameObject.FindWithTag("Count");
            MainMenuMoneyControl mon = money.GetComponent<MainMenuMoneyControl>();
            shipnumCounter num = count.GetComponent<shipnumCounter>();
            spawnobjectholder objhol = grabbed.GetComponent<spawnobjectholder>();

            if (mon.totalmoney > objhol.req - 1 && num.i < num.limit && objhol.inside == true)
            {
                string oriname = grabbed.name;
                GameObject spawnclone = Instantiate(grabbed) as GameObject;
                spawnobjectholder spawnhol = spawnclone.GetComponent<spawnobjectholder>();
                spawnhol.placed = true;
                spawnclone.transform.position = grabbed.transform.position;
                spawnclone.transform.parent = basehol.transform;
                spawnclone.transform.rotation = grabbed.transform.rotation;
                spawnclone.transform.localScale = grabbed.transform.lossyScale;
                Destroy(grabbed);
                grabbed = null;
                    spawnclone.name = oriname;


            }




        }

        }
      
    }
    */
}

