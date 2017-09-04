using UnityEngine;
using System.Collections;

public class Frigate_Offense : _Frigate {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public Frigate_Offense() {
        base.ShipClass = eShipClass.Offense;

        ShipMesh = "ScifiFrigatePredatorHull.fbx";
        ShipMaterialRoot = "ScifiFrigatePredator";

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
        cc.height = 9f;
        cc.direction = 2;
    }

    void Update()
    {

    }
}
