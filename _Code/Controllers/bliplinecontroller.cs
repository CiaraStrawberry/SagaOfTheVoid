using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class bliplinecontroller : MonoBehaviour {
    public LineRenderer Lineren;
    public Vector3 targetgam;
    public TSTransform source;
	// Use this for initialization
	void Start () {
        Lineren = GetComponent<LineRenderer>();
	}
	
	void Update () {
        if (targetgam != new Vector3(0, 0, 0))
        {
            Lineren.SetPosition(0, transform.position);
            Lineren.SetPosition(1, transform.parent.TransformPoint( targetgam ));
        }
        else
        {
            Lineren.SetPosition(0, transform.position);
            Lineren.SetPosition(1, transform.position);
        }
	}
}
