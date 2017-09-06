using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Forge3D;

/// <summary>
/// This script despawns its attached object on a timer.
/// </summary>
public class DespawnOnTimer : MonoBehaviour {

    // the timer for which the object needs to despawn.
    public float destroyontime = 1;
    // the time since the object spawned/
    private float timepassed;
    // Do you destroy the object wholesale or pool it.
    public bool destroynormal;
    // the root world object.
    private GameObject World;
    // the starting scale of the object relative to the world.
    private float StartScale;

    /// <summary>
    /// Reset everything after pooling.
    /// </summary>
	public void Start () {
        timepassed = 0;
        World = GameObject.Find("WorldScaleBase");
        StartScale = World.transform.localScale.x;
        if(transform.parent && transform.parent.name != "Working" && loadtest == false) transform.parent = World.transform.Find("World").Find("Objects").Find("Working");
	}
	
    /// <summary>
    /// Check if the object is ready to despawn and if so despawn it.
    /// </summary>
	void Update () {
        timepassed = timepassed + Time.deltaTime;
        if (timepassed > destroyontime)
        {
            F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
            if (destroynormal) Destroy(this.gameObject);
        }
        if(World.transform.localScale.x != StartScale) F3DPoolManager.Pools["GeneratedPool"].Despawn(this.gameObject.transform);
    }
}
