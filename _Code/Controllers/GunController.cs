using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour {
    public List<GameObject> notspecials = new List<GameObject>();
    public List<GameObject> specials = new List<GameObject>(); 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void reset (float size)
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Pickup");
        foreach( GameObject ship in ships)
        {
            if (ship.transform.parent && ship.transform.parent.GetComponent<WeaponAttachPoint>())  ship.GetComponent<WeaponBaseClass>().reset(size);
        }
    }
    public void supporton ()
    {
        foreach (GameObject gun in notspecials)
        {
            gun.GetComponent<BoxCollider>().enabled = false;
            foreach (Transform t in gun.transform) t.gameObject.SetActive(false);
        }
        foreach (GameObject gun in specials)
        {
            gun.GetComponent<BoxCollider>().enabled = true;
            foreach (Transform t in gun.transform) t.gameObject.SetActive(true);
        }
    }
    public void supportoff ()
    {
        foreach (GameObject gun in notspecials)
        {
            gun.GetComponent<BoxCollider>().enabled = true;
            foreach (Transform t in gun.transform) t.gameObject.SetActive(true);
        }
        foreach (GameObject gun in specials)
        {
            gun.GetComponent<BoxCollider>().enabled = false;
            foreach (Transform t in gun.transform) t.gameObject.SetActive(false);
        }
    }
}
