using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipBuilderRight : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Load () {
        GameObject.Find("ShipParent").SendMessage("Addone");
	}
}
