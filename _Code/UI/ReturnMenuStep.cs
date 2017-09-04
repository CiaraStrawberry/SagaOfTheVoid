using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnMenuStep : MonoBehaviour {

    private GalaxyController galControl;

	void Start () {
        galControl = GameObject.Find("Controllers").GetComponent<GalaxyController>();
    }

	void Update () {
		
	}

    void Load() {
        galControl.returntonormal();
        Debug.Log("returntoMenu");
    }

}
