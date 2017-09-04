using UnityEngine;
using System.Collections;

public class SuperCapital_Offense : _SuperCapital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SuperCapital_Offense() {
        base.ShipClass = eShipClass.Offense;

        ShipMesh = "ScifiCruiserArbiterHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiCruiserArbiter";

    }

    /// <summary>
    /// Set the ship meshes / materials
    /// </summary>
    /// <remarks>This only runs on adding a script in editor</remarks>
    public override void Reset()
    {
        // Base setup
        base.Reset();

        // Set the collider

    }

    void Update()
    {

    }
}
