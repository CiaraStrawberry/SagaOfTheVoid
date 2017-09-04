using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;

public class RelayController : MonoBehaviour {
    public InputRelay relay;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void ordermove(List<GameObject> selectedshipsin, TSVector positionin,GameObject inputgam, FP averagespeedin,bool waypoints)
    {
        relay.ordermove(selectedshipsin, positionin,inputgam,averagespeedin,waypoints);
    }
    public void ordershipspawn (int ship,TSVector pos,int spawnteam, int ViewIDin)
    {
        if(relay) relay.ordershipspawn(ship,pos,spawnteam,ViewIDin);
    }
}
