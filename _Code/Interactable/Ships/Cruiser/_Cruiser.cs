using UnityEngine;
using System.Collections;

public class _Cruiser : _Ship {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public _Cruiser() {
        base.HullType = eHullType.Cruiser;
        // Use medium shields
        base.ShieldMaterial = "Assets/_Content/Materials/ForceField_SpaceShip_Static_UV1_Big.mat";
    }

}
