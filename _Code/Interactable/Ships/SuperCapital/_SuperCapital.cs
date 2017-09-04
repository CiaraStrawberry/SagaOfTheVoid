using UnityEngine;
using System.Collections;

public class _SuperCapital : _Ship {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public _SuperCapital() {
        base.HullType = eHullType.SuperCapital;
        // Use medium shields
        base.ShieldMaterial = "Assets/_Content/Materials/ForceField_SpaceShip_Static_UV1_Big.mat";
    }

}
