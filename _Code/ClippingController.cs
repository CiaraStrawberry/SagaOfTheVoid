using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClippingController : MonoBehaviour {
    [SerializeField]
    private Transform parent;
    private Camera camera1;
	// Use this for initialization
	void Start () {
        parent = GameObject.Find("WorldScaleBase").transform;
        camera1 = GetComponent<Camera>();
            camera1.nearClipPlane = 0.0000000005f;
        camera1.farClipPlane = 10;
	}
	
	// Update is called once per frame
	void Update () {
     
       
    }
}
