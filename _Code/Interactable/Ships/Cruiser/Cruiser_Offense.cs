using UnityEngine;
using System.Collections;

public class Cruiser_Offense : _Cruiser {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Cruiser_Offense() {
        base.ShipClass = eShipClass.Offense;

        ShipMesh = "ScifiFrigateStarmasterHull.fbx";
        ShipMaterialRoot = "ScifiFrigateStarmaster";

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
        cc.radius = 4f;
        cc.height = 12f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
