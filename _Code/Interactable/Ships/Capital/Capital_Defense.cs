using UnityEngine;
using System.Collections;

public class Capital_Defense : _Capital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Capital_Defense() {
        base.ShipClass = eShipClass.Defense;

        ShipMesh = "ScifiHeavyDestroyerWolfhoundHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiHeavyDestroyerWolfhound";

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
        cc.center = new Vector3(0f, 0f, 2.33f);
        cc.radius = 12f;
        cc.height = 40f;
        cc.direction = 2;

        // Set the collision radius
        Apex.Units.UnitComponent uc = this.GetComponent<Apex.Units.UnitComponent>();
        uc.fieldOfView = 200;
        uc.radius = 12f;
        uc.determination = 1;

    }

    void Update()
    {

    }

}
