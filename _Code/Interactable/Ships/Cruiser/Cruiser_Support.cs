using UnityEngine;
using System.Collections;

public class Cruiser_Support : _Cruiser {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Cruiser_Support() {
        base.ShipClass = eShipClass.Support;

        ShipMesh = "ScifiFrigateBarracudaHullB.fbx";
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
        cc.center = new Vector3(0f, 0f, 0.93f);
        cc.radius = 5f;
        cc.height = 11f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
