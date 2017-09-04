using UnityEngine;
using System.Collections;

public class Cruiser_Defense : _Cruiser {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Cruiser_Defense() {
        base.ShipClass = eShipClass.Defense;

        ShipMesh = "ScifiDestroyerVanguardHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiDestroyerVanguard";

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
        cc.radius = 2.5f;
        cc.height = 16f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
