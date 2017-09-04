using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionScaleController : MonoBehaviour {
    public GameObject World;
    private ParticleSystem particle;
    private ParticleSystem.ShapeModule shape;
    private float startsize;
    private ParticleSystem.MainModule main;
    void Start()
    {
        World = GameObject.Find("WorldScaleBase");
        particle = GetComponent<ParticleSystem>();
        shape = particle.shape;
        main = particle.main;
       // startsize = main.startSize.c;
    }
	
	// Update is called once per frame
	void Update () {
     //   shape.radius = World.transform.localScale.x /50;
       // main.startSize = startsize
	}
}
