using UnityEngine;
using System.Collections;

public class SuperCapital_Tank : _SuperCapital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public SuperCapital_Tank() {
        base.ShipClass = eShipClass.Tank;

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

        // Set the collider size

    }

    void Update()
    {

    }
}
