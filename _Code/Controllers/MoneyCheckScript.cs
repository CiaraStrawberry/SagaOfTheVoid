using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyCheckScript : MonoBehaviour {
    public GameObject ship;
    private _Ship shipscript;
    private TextMesh textmesh;
	// Use this for initialization
	void Start () {
        shipscript = ship.GetComponent<_Ship>();
        textmesh = GetComponent<TextMesh>();
        textmesh.text = shipscript.PointsToDeploy.ToString();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
