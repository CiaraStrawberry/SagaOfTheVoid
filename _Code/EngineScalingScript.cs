using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineScalingScript : MonoBehaviour {
    public GameObject World;
    private Quaternion startquat;
    private Vector3 startscale;
	// Use this for initialization
	void Start () {
        World = GameObject.Find("WorldScaleBase");
        startquat = transform.localRotation;
        startscale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if(World.transform.localScale.x > 145 && World.transform.localScale.x < 900)
        {
       //     transform.localRotation = startquat * new Quaternion(0,180,0,0);
      //      transform.localScale =  Vector3.Scale(( new Vector3( 1 /( World.transform.localScale.x / 145), 1 / (World.transform.localScale.y / 145),1 / (World.transform.localScale.z / 145))), startscale);
        }
        else
        {
          //  transform.localRotation = startquat;
         //   transform.localScale = startscale;
            if(World.transform.localScale.x < 145 && World.transform.localScale.x > 30)
            {
           //     transform.localScale = Vector3.Scale((new Vector3(1 / (World.transform.localScale.x / 145), 1 / (World.transform.localScale.y / 145), 1 / (World.transform.localScale.z / 145))), startscale / 3);
            }
            else if(World.transform.localScale.x > 900)
            {
       //         transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y,  ((World.transform.localScale.z / 900) * startscale.z * 5));
            }
        }

	}
}
