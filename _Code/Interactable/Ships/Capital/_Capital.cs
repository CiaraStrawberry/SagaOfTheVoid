using UnityEngine;
using System.Collections;

public class _Capital : _Ship {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public _Capital() {
        base.HullType = eHullType.Capital;
        // Use medium shields
        base.ShieldMaterial = "Assets/_Content/Materials/ForceField_SpaceShip_Static_UV1_Big.mat";
    }

}
