using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class ControllerInteractable : MonoBehaviour {
    private BoxCollider bcol;
    private bool triggerdown;
    private GameObject colliderObj;
    public GameObject heldobj;
    private bool respawnnow;
    private int childcountstart;
    private onTriggerEnterEnemy triggerCheck;
    public GameObject shipclose;
    public GameObject grabbedobject;
    private Vector3 startdisparity;
    private Quaternion relativerot;
    private GameObject childobj;
    GameObject tempheld;
    private VRTK_ControllerEvents vrtkcontrollerevents;

    // Use this for initialization
    void Start()
    {
        bcol = this.GetComponent<BoxCollider>();
        vrtkcontrollerevents = GetComponent<VRTK_ControllerEvents>();
        StartCoroutine(Example());
        triggerCheck = GetComponent<onTriggerEnterEnemy>();
        vrtkcontrollerevents.TriggerClicked += new ControllerInteractionEventHandler(triggerdownmeth);
        vrtkcontrollerevents.TriggerUnclicked += new ControllerInteractionEventHandler(triggerup);
    }


    // Update is called once per frame
    public void triggerdownmeth (object sender, ControllerInteractionEventArgs e)
    {
            triggerdown = true;
            if (colliderObj)
            {
                Checkpickup(colliderObj);
                checkpickuptrigger(colliderObj);
            }
            else if (shipclose) grab(shipclose);
    }
    public void triggerup (object sender, ControllerInteractionEventArgs e)
    {
            triggerdown = false;
            if (respawnnow)
            {
               tempheld.GetComponent<WeaponBaseClass>().respawn();
               tempheld = null;
               respawnnow = false;
            }
            if (heldobj)
            {
               heldobj.GetComponent<WeaponBaseClass>().reset(0);
                heldobj = null;
            }
            if (grabbedobject)
            {
             GameObject temp = grabbedobject.transform.parent.gameObject;
            grabbedobject.transform.parent = grabbedobject.transform.parent.parent;
            temp.transform.parent = grabbedobject.transform;
            grabbedobject = null;
            }
    }
    void Update()
    {
        colliderObj = triggerCheck.focus;
        shipclose = triggerCheck.holofocus;
        if (grabbedobject && childobj)
        {
            childobj.transform.position = transform.position;
            childobj.transform.rotation = transform.rotation * relativerot;
        }
    }
    
    void grab (GameObject grabbed)
    {
        grabbedobject = grabbed;
        startdisparity = grabbed.transform.position -  transform.position ;
        GameObject parent = grabbed.transform.Find("Transformparent").gameObject;
        parent.transform.parent = grabbed.transform.parent;
        grabbed.transform.parent = parent.transform;
        Vector3 start = grabbed.transform.position;
        parent.transform.position = transform.position;
        grabbed.transform.position = start;
        relativerot = Quaternion.Inverse(transform.rotation) * grabbed.transform.rotation;
        childobj = parent;
    }

    void OnTriggerEnter (Collider col)
    {
        if (grabbedobject == null) Checkpickup(col.gameObject);
        
    }
    void OnTriggerExit (Collider col)
    {
  
    }

    bool Checkpickup (GameObject col)
    {
        bool output = false;

        WeaponAttachPoint wap = col.GetComponent<WeaponAttachPoint>(); 
        
        if (wap != null && heldobj && col.transform.childCount < 1 && (( wap.equipment == true && heldobj.GetComponent<WeaponBaseClass>().equipment == true) ||( wap.special == true && heldobj.GetComponent<WeaponBaseClass>().special == true)) )
        {
            bool check = true;
            WeaponBaseClass bas = heldobj.GetComponent<WeaponBaseClass>();
            if (heldobj.GetComponent<_Ship>())
            {
                check = GameObject.Find("Fleet Builder Parent").GetComponent<FleetBuilderController>().enough(bas.cost);
                if( check)
                {
                    col.transform.parent.GetComponent<AttachPointsController>().AttachShip(heldobj);
                   tempheld = heldobj;
                   heldobj = null;
                   respawnnow = true;
                }
               
            }
            else
            {
              heldobj.transform.parent = col.gameObject.transform;
              heldobj.transform.localPosition = new Vector3(0, 0, 0);
              heldobj.transform.rotation = new Quaternion(0, 0, 0, 0);
              tempheld = heldobj;
              heldobj = null;
              respawnnow = true;
            }
        }
        return output;
    }
    void checkpickuptrigger ( GameObject col)
    {
        if (col.gameObject.tag == "Pickup" && triggerdown && transform.childCount < (childcountstart + 1) && heldobj == null && grabbedobject == null)
        {
            if (col.transform.parent.GetComponent<WeaponAttachPoint>()) { col.GetComponent<WeaponBaseClass>().reset(0); }
            else
            {
                WeaponBaseClass bas = col.GetComponent<WeaponBaseClass>();
                bas.scaletosize();
                Debug.Log(transform.childCount < (childcountstart + 1));
                col.transform.parent = transform;
                col.transform.localPosition = new Vector3(0, 0, 0);
                bas.disabletext();
                heldobj = col.gameObject;
            }
        
        }
    }

    IEnumerator Example()
    {
        yield return new WaitForSeconds(1);
        childcountstart = transform.childCount;
    }

}
