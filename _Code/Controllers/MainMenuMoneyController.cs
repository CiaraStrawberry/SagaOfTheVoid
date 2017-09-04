using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMoneyController : MonoBehaviour {

    private TextMesh textmesh;
    FleetBuilderController fleetparent;
	// Use this for initialization
	void Start () {
        textmesh = GetComponent<TextMesh>();
        fleetparent = GameObject.Find("Fleet Builder Parent").GetComponent<FleetBuilderController>();
	}
	
	// Update is called once per frame
	void Update () {
        textmesh.text = fleetparent.currentmoney.ToString() + " left \n out of " + fleetparent.limit.ToString();
    }
}
