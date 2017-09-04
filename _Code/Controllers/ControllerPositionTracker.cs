using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class ControllerPositionTracker : MonoBehaviour {
    public GameObject Tracker;
    public bool rightcontroller;
    PhotonView locview;
    private GameObject ViveObj;
    private GameObject OculusObj;
    // Use this for initialization
    void Start() {
      
        locview = GetComponent<PhotonView>();
        foreach (GameObject gam in GameObject.FindGameObjectsWithTag("Voice")) if (gam.GetComponent<PhotonView>().owner.ID == locview.owner.ID) transform.parent = gam.transform;
        if (locview.isMine == true)
        {
            if (rightcontroller && GameObject.Find("RightController"))
            {
                Tracker = GameObject.Find("RightController").transform.parent.gameObject;
                
            }
            else if (GameObject.Find("LeftController"))
            {
                Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
            }
            foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
        }
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
        if (SceneManager.GetActiveScene().name == "mptest")
        {
            moving = false;
        }
        else moving = true;
       
        ViveObj = transform.Find("ViveModel").gameObject;
        OculusObj = transform.Find("OculusModel").gameObject;
        InvokeRepeating("Rep", 0, 1);
        if (locview.isMine)
        {


            if (GameObject.Find("CenterEyeAnchor"))
            {
                locview.RPC("setcontrollerType", PhotonTargets.All, false);

            }
            else locview.RPC("setcontrollerType", PhotonTargets.All, true);
        }
     //  locview.RPC("setcontrollerType")
    }
    public bool vivetype;
    [PunRPC]
    void setcontrollerType (bool vive)
    {
        vivetype = vive;
    }

    // Update is called once per frame
    void Update() {
        if (Tracker != null && moving == true)
        {
            transform.position = Tracker.transform.position;
            transform.rotation = Tracker.transform.rotation;
        }
    }
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
            if (rightcontroller && GameObject.Find("RightController"))
            {
                Tracker = GameObject.Find("RightController").transform.parent.gameObject;
            }
            else if (GameObject.Find("LeftController"))
            {
                Tracker = GameObject.Find("LeftController").transform.parent.gameObject;
            }
        }
     
    }
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
    public bool moving;
}
