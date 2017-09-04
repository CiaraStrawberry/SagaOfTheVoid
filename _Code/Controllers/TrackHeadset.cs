using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrackHeadset : MonoBehaviour {
    PhotonView personalphotonview;
    public GameObject Cameraobj;
    public GameObject LeftControler;
    public GameObject RightControler;
    public GameObject LeftControllerLocal;
    public GameObject RightControllerLocal;
	// Use this for initialization
	void Start () {
        personalphotonview = GetComponent<PhotonView>();
        if(personalphotonview.isMine == true)
        {
            if (GameObject.Find("CenterEyeAnchor"))
            {
                Cameraobj = GameObject.Find("CenterEyeAnchor");

            }
            else Cameraobj = GameObject.Find("Camera (eye)");

            PhotonNetwork.Instantiate("LeftController", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
            PhotonNetwork.Instantiate("RightController", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        }
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
   
        moving = true;
        
        if(personalphotonview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
        if (personalphotonview.isMine == true && Cameraobj != null)
        {
          transform.position =  Cameraobj.transform.position;
            transform.localScale = new Vector3( 0.1f,0.1f,0.1f);
            transform.rotation = Cameraobj.transform.rotation;
        }
      
      
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        if (this != null && this.gameObject != null)
        {
            moving = true;
            if (personalphotonview.isMine == true) foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>()) mesh.enabled = false;

        }
    
    }
    bool moving;
}
