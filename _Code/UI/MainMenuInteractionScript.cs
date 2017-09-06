using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class allows the controller to interact with the main menu.
/// </summary>
public class MainMenuInteractionScript : MonoBehaviour {

    // the material to asign to the linerenderer.
    public Material linemat;
    // the linerenderer to point to things with.
    private LineRenderer lineren;
    // The controller to use for input.
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }
    // the tracked object to recieve input from.
    private SteamVR_TrackedObject trackedObj;
    // the trigger event system to recieve a trigger event from.
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    // the gameobject inside a collisino box.
    private GameObject inside;
    // a layermask to use when looking for things.
    public LayerMask lay;
    // start the game.
    public bool runnow;

    /// <summary>
    /// This start function sets up input and the linerenderer.
    /// </summary>
    void Start() {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        lineren = gameObject.AddComponent<LineRenderer>();
        lineren.startWidth = 0.01f;
        lineren.endWidth = 0.01f;
        lineren.material = linemat;
    }

    /// <summary>
    /// The Update button allows for the setting of the linerderer in the correct position and input to be recieved.
    /// </summary>
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
            else    lineren.SetPosition(1, transform.position);
            if (controller.GetPressDown(triggerButton) && (Physics.Raycast(transform.position, transform.forward, out hit, lay)) && inside == null)  hit.transform.gameObject.SendMessage("Load", SendMessageOptions.DontRequireReceiver);
            if (Physics.Raycast(transform.position, transform.forward, out hit)) {
                if (GameObject.Find("GalaxyParent"))
                {
                    if (GameObject.Find("GalaxyParent").transform.localScale.x > 5.9f)
                        hit.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
                }
                else  hit.transform.gameObject.SendMessage("up", SendMessageOptions.DontRequireReceiver);
            }
        }
        else lineren.SetPosition(1, transform.position);
    }

    /// <summary>
    /// This function asigns inside to "col" when it enteres the trigger.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter ( Collider col)
    {
        if (col.tag == "Pickup" || col.tag == "Ship") inside = col.gameObject;
    }


    /// <summary>
    /// if the inside variable leaves it is nulled.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerExit( Collider col)
    {
        if (col.gameObject == inside) inside = null;    
    }
}
