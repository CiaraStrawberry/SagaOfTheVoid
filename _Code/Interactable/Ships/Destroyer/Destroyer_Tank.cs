using UnityEngine;
using System.Collections;

public class Destroyer_Tank : _Destroyer {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Destroyer_Tank() {
        base.ShipClass = eShipClass.Tank;

        ShipMesh = "ScifiDestroyerPhoenixHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiDestroyerPhoenix";

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
        cc.radius = 5f;
        cc.height = 20f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
