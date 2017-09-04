using UnityEngine;
using System.Collections;

public class _Frigate : _Ship {
    
    /// <summary>
    /// Default Constructor
    /// </summary>
    public _Frigate() {
        base.HullType = eHullType.Frigate;
        // Use medium shields
        base.ShieldMaterial = "Assets/_Content/Materials/ForceField_SpaceShip_Static_UV1_Big.mat";
    }

}
