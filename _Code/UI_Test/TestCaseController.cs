using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCaseController : MonoBehaviour {

    [Tooltip("Use one bool from each section at a time")]

    [Header("Zooming")]
    public bool oneControllerTriggerScale;
    public bool TwoControllerTriggerScale;
    public bool linearScaleWithTouchpad;
    public bool incrimenatalScaleWithToucpad;
    public bool controllerpositionscale;
    [Header("Movement")]
    public bool OneHandDrag;
    public bool TwoHandDrag;
    public bool dpadnavigation;
    public bool raycastjump;
    public bool accelerateviamovement;
    [Header("Select all")]
    public bool eh;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
