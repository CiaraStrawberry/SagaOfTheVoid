using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This script allows UI elements to track the headset to ensure the player gets the info.
/// </summary>
public class TrackHeadset : MonoBehaviour {
    // the local photonview attached to the player.
    PhotonView personalphotonview;
    // the CameraObj of the player.
    public GameObject Cameraobj;
    // is the camera moving right now?
    bool moving;

	/// <summary>
    /// initialise everything.
    /// </summary>
	void Start () {
        personalphotonview = GetComponent<PhotonView>();
        if (personalphotonview.isMine == true)
        {
            if (GameObject.Find("CenterEyeAnchor")) Cameraobj = GameObject.Find("CenterEyeAnchor");
            else Cameraobj = GameObject.Find("Camera (eye)");

            PhotonNetwork.Instantiate("LeftController", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
            PhotonNetwork.Instantiate("RightController", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        }
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
   
        moving = true; 
        if(personalphotonview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
    }

    /// <summary>
    /// Move everything based on the camera position.
    /// </summary>
    void Update()
    {
        if (personalphotonview.isMine == true && Cameraobj != null)
        {
            transform.position = Cameraobj.transform.position;
            transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
            transform.rotation = Cameraobj.transform.rotation;
        }
    }

    /// <summary>
    /// Disable the UI if the scene changes.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="mode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (this != null && this.gameObject != null)
        {
            moving = true;
            if (personalphotonview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;

        }
    
    }
    
}
