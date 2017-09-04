using UnityEngine;
using System.Collections;

public class SuperCapital_Defense : _SuperCapital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SuperCapital_Defense() {
        base.ShipClass = eShipClass.Defense;

        ShipMesh = "ScifiCruiserProtectorHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiCruiserProtector";

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
