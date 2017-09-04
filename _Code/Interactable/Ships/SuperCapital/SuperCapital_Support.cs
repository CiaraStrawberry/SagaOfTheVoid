using UnityEngine;
using System.Collections;

public class SuperCapital_Support : _SuperCapital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SuperCapital_Support() {
        base.ShipClass = eShipClass.Support;

        ShipMesh = "";
        ShipMaterialRoot = "";

    }

    /// <summary>
    /// Set the ship meshes / materials
    /// </summary>
    /// <remarks>This only runs on adding a script in editor</remarks>
    public override void Reset()
    {
        // Base setup
        base.Reset();

        // Set the collider sizing

    }

    void Update()
    {

    }
}
