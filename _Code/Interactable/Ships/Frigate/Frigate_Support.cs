using UnityEngine;
using System.Collections;

public class Frigate_Support : _Frigate {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Frigate_Support() {
        base.ShipClass = eShipClass.Support;

        ShipMesh = "ScifiFrigateBarracudaHullA.fbx";
        ShipMaterialRoot = "ScifiFrigateBarracuda";

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
        cc.center = new Vector3(0f, 0f, 0.36f);
        cc.radius = 2.5f;
        cc.height = 8f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
