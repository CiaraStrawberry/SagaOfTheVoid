using UnityEngine;
using System.Collections;

public class Frigate_Tank : _Frigate {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Frigate_Tank() {
        base.ShipClass = eShipClass.Tank;

        // Set mesh types
        ShipMesh = "ScifiFrigateWyrmHull.fbx";
        ShipMaterialRoot = "ScifiFrigateWyrm";

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
        CapsuleCollider cc = this.GetComponent<CapsuleCollider>();
        cc.center = new Vector3(0f, 0f, 0.51f);
        cc.radius = 1.2f;
        cc.height = 9f;
        cc.direction = 2;

    }

    void Update()
    {

    }
}
