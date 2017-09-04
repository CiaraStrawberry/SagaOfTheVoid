using UnityEngine;
using System.Collections;

public class _Destroyer : _Ship {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public _Destroyer() {
        base.HullType = eHullType.Destroyer;
        // Use medium shields
        base.ShieldMaterial = "Assets/_Content/Materials/ForceField_SpaceShip_Static_UV1_Big.mat";
    }

}
