using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttachPoint : MonoBehaviour {
    public bool equipment;
    private MeshRenderer meshren;
    public bool special;
   
	// Use this for initialization
	void Start () {
        meshren = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
    //    if (transform.childCount > 0 && transform.GetChild(0).GetComponent<WeaponParentHolder>() == null) meshren.enabled = false;
     //   else meshren.enabled = true; 
            
    }
    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Pickup")
        {
    //        col.transform.parent = transform;
        //    col.transform.localPosition = new Vector3(0, 0, 0);
         //   col.GetComponent<WeaponParentHolder>().Parentstart.GetComponent<WeaponBaseClass>().respawn();  
        }
    }
}
