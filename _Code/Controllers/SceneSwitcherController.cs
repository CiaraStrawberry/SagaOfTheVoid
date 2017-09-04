using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitcherController : MonoBehaviour {
    public GameObject CameraMain1;
    public GameObject CameraMain2;
    public GameObject CameraLoading;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void LoadLevel ()
    {
     //   CameraMain1.SetActive(false);
     //   CameraMain2.SetActive(false);
     //   CameraLoading.SetActive(true);
    }
    public void StartLevel()
    {
    //    CameraMain1.SetActive(true);
    //    CameraMain2.SetActive(true);
    //    CameraLoading.SetActive(false);
    }
}
