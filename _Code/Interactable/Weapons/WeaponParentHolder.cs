using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParentHolder : MonoBehaviour {

    public GameObject Parentstart;
    void Start ()
    {
        Parentstart = transform.parent.gameObject;
        if  (transform.parent.GetComponent<WeaponAttachPoint>() == null) transform.parent = transform.parent.parent;
        
    }
    void Update ()
    {
        if (Parentstart.gameObject == null) Destroy(this.gameObject);
    }
}
