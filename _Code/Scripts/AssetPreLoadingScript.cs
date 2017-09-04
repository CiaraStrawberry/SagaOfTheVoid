using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetPreLoadingScript : MonoBehaviour {
    private int updates;
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(updates < 30)
        {
            updates++;
            if (updates == 2) Destroy(this.gameObject);
        }
	}
}
