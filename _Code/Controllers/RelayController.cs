using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TrueSync;


/// <summary>
/// Since the InputRelay script is on multiple objects, but only one can accept inputs, this function keeps its location in cache and uses it to send function calls to the InputRelay.
/// </summary>
public class RelayController : MonoBehaviour {

    // the relay to send data to.
    public InputRelay relay;
	
    /// <summary>
    /// Send a move order.
    /// </summary>
    /// <param name="selectedshipsin">all the ships to send forwards</param>
    /// <param name="positionin">the move location to send the ships to</param>
    /// <param name="inputgam">the target gameobject to attack (can be null)</param>
    /// <param name="averagespeedin">the average speed for the ships to move at.</param>
    /// <param name="waypoints">is it in waypoint mode?</param>
    public void ordermove(List<GameObject> selectedshipsin, TSVector positionin,GameObject inputgam, FP averagespeedin,bool waypoints)
    {
        relay.ordermove(selectedshipsin, positionin,inputgam,averagespeedin,waypoints);
    }

    /// <summary>
    /// Send a ship buy order
    /// </summary>
    /// <param name="ship">which ship to buy.</param>
    /// <param name="pos">where to spawn it</param>
    /// <param name="spawnteam">what team to spawn it onto?</param>
    /// <param name="ViewIDin">the ships foreign viewID</param>
    public void ordershipspawn (int ship,TSVector pos,int spawnteam, int ViewIDin)
    {
        if(relay) relay.ordershipspawn(ship,pos,spawnteam,ViewIDin);
    }
}
