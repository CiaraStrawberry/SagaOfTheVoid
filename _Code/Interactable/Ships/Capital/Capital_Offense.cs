using UnityEngine;
using System.Collections;
using Apex.Steering;
using Apex.Units;

public class Capital_Offense : _Capital {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Capital_Offense()
    {
        base.ShipClass = eShipClass.Offense;

        ShipMesh = "ScifiCruiserKingswordHullNoHangar.fbx";
        ShipMaterialRoot = "ScifiCruiserKingsword";

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
        cc.radius = 7f;
        cc.height = 47f;
        cc.direction = 2;
    }

    void Update()
    {

    }

}
