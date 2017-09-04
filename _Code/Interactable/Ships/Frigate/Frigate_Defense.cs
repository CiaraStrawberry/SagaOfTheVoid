using UnityEngine;
using System.Collections;

public class Frigate_Defense : _Frigate {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Frigate_Defense() {
        base.ShipClass = eShipClass.Defense;

        ShipMesh = "ScifiFrigateHammerheadHull.fbx";
        ShipMaterialRoot = "ScifiFrigateHammerhead";

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
        cc.center = new Vector3(0f, 0f, 0f);
        cc.radius = 1.5f;
        cc.height = 9f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
