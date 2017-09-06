using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This class attaches to the Controller and gives its position to the other clients connected inside this game.
/// </summary>
public class ControllerPositionTracker : MonoBehaviour {

    // The Object this script is tracking.
    public GameObject Tracker;
    // right or leftcontroller.
    public bool rightcontroller;
    // The Photonview connected to this gameobject.
    PhotonView locview;
    // The prefab used for the Vive controllers.
    private GameObject ViveObj;
    // The Prefab used for the oculus controllers.
    private GameObject OculusObj;
    // The bool to determin if the source user, uses a vive or an oculus.
    public bool vivetype;
    // the check if the local controller is currently moving.
    public bool moving;

    /// <summary>
    /// the start function to initialise everything.
    /// </summary>
    void Start() {
      
        locview = GetComponent<PhotonView>();
        foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice")) if (gam.GetComponent<PhotonView>().owner.ID == locview.owner.ID) transform.parent = gam.transform;
        if (locview.isMine == true)
        {
            if (rightcontroller && GameObject.Find("RightController")) Tracker = GameObject.Find("RightController").transform.parent.gameObject;  
            else if (GameObject.Find("LeftController")) Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        }
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        if (SceneManager.GetActiveScene().name == "mptest") moving = false;
        else moving = true;
        ViveObj = transform.Find("ViveModel").gameObject;
        OculusObj = transform.Find("OculusModel").gameObject;
        InvokeRepeating("Rep", 0, 1);
        if (locview.isMine)
        {
            if (GameObject.Find("CenterEyeAnchor")) locview.RPC("setcontrollerType", PhotonTargets.All, false);
            else locview.RPC("setcontrollerType", PhotonTargets.All, true);
        }
    }
   
    /// <summary>
    /// the replicated function to tell the other players that you are using a vive.
    /// </summary>
    /// <param name="vive"> the bool , if true == vive, if false == oculus</param>
    [PunRPC]
    void setcontrollerType (bool vive)
    {
        vivetype = vive;
    }

    /// <summary>
    /// the updating function that keeps this object at the same position of the client using it, but only locally.
    /// </summary>
    void Update() {
        if (Tracker != null && moving == true)
        {
            transform.position = Tracker.transform.position;
            transform.rotation = Tracker.transform.rotation;
        }
    }

    /// <summary>
    /// the function to keep the type of controller constant across clients.
    /// </summary>
    void Rep ()
    {
        if (moving == true)
        {
            if (vivetype == true)
            {
                if (OculusObj.GetActive() == true) OculusObj.SetActive(false);
                if (ViveObj.GetActive() == false) ViveObj.SetActive(true);
            }
            else
            {
                if (OculusObj.GetActive() == false) OculusObj.SetActive(true);
                if (ViveObj.GetActive() == true) ViveObj.SetActive(false);
            }
        }
        if(Tracker == null  && locview.isMine == true)
        {
            if (rightcontroller && GameObject.Find("RightController"))  Tracker = GameObject.Find("RightController").transform.parent.gameObject;
            else if (GameObject.Find("LeftController")) Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
        }
    }

    /// <summary>
    /// The local function to update the function of this object depending on the type of scene loaded.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (this != null && this.gameObject != null)
        {
            if (scene.name == "mptest")
            {
                moving = false;
                foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
            }
            else
            {
                foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = true;
                moving = true;
            }
            if (locview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        }
    }

}
