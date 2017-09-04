using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPositionController : MonoBehaviour {
    private GameObject player;
    public bool tracking;
	// Use this for initialization
	void Start () {
        if (tracking == false) player = GameObject.Find("ResizeObj");
        else if (GameObject.Find("CenterEyeAnchor") != null) player = GameObject.Find("CenterEyeAnchor");
        else player = GameObject.Find("Camera (eye)");

    }
	
	// Update is called once per frame
	void Update () {
        if (player)
        {
            transform.position = player.transform.position;
            if (tracking == true) transform.rotation = player.transform.rotation;
        }

    }
}
