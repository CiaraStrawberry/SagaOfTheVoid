using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuInteractionScript : MonoBehaviour {
    public Material linemat;
    private LineRenderer lineren;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    private SteamVR_TrackedObject trackedObj;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private GameObject inside;
    public LayerMask lay;
    public bool runnow;
    // Use this for initialization
    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        lineren = gameObject.AddComponent<LineRenderer>();
        lineren.startWidth = 0.01f;
        lineren.endWidth = 0.01f;
        lineren.material = linemat;
    }

    // Update is called once per frame
    void Update() {
        lineren.SetPosition(0, transform.position);
        if (runnow == false)
        {
            RaycastHit hit;
            if (inside == null)
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, lay)) lineren.SetPosition(1, hit.point);
                else lineren.SetPosition(1, transform.position + (transform.forward * 100));
            }
            else
            {
                lineren.SetPosition(1, transform.position);
            }
            if (controller.GetPressDown(triggerButton) && (Physics.Raycast(transform.position, transform.forward, out hit, lay)) && inside == null)
            {
                hit.transform.gameObject.SendMessage("Load", SendMessageOptions.DontRequireReceiver);
            }
            if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                if (GameObject.Find("GalaxyParent"))
                {
                    if (GameObject.Find("GalaxyParent").transform.localScale.x > 5.9f)
                        hit.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
                }
                else
                {
                    hit.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
                }

            }

        }
        else
        {
            lineren.SetPosition(1, transform.position);
        }
     

    }
    void OnTriggerEnter ( Collider col)
    {
        if (col.tag == "Pickup" || col.tag == "Ship")
        {
            inside = col.gameObject;
        }
    }
    void OnTriggerExit( Collider col)
    {

        if (col.gameObject == inside)
        {
            inside = null;
        }
    }
}
