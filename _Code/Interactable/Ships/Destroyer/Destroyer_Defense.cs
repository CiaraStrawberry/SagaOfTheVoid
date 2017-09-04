using UnityEngine;
using System.Collections;

public class Destroyer_Defense : _Destroyer {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Destroyer_Defense() {
        base.ShipClass = eShipClass.Defense;

        ShipMesh = "ScifiDestroyerTitanHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiDestroyerTitan";

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
        cc.radius = 6f;
        cc.height = 25f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
