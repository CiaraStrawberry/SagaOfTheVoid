using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Forge3D;

public class DespawnOnTimer : MonoBehaviour {
    public float destroyontime = 1;
    private float timepassed;
    public bool destroynormal;
    private GameObject World;
    private float StartScale;
    public bool loadtest;
	// Use this for initialization
	public void Start () {
        timepassed = 0;
        World = GameObject.Find("WorldScaleBase");
        StartScale = World.transform.localScale.x;
        if(transform.parent && transform.parent.name != "Working")
        {
           if(loadtest == false) transform.parent = World.transform.Find("World").Find("Objects").Find("Working");
        }
	}
	
	// Update is called once per frame
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
