using UnityEngine;
using System.Collections;

public class Capital_Support : _Capital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Capital_Support() {
        base.ShipClass = eShipClass.Support;

        ShipMesh = "ScifiCruiserCerberusHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiCruiserCerberus";

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
        cc.radius = 12f;
        cc.height = 40f;
        cc.direction = 2;

    }

    void Update()
    {

    }

}
