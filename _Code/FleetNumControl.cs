using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetNumControl : MonoBehaviour {

    public int waythrough = 4;
    public GameObject capitals;
    public GameObject cruisers;
    public GameObject frigates;
    public GameObject destroyers;
    public GameObject corvettes;
    public GameObject LightCraft;
    int i = 0;
    
    void Awake ()
    {
        //  disableall();
        enableall();
    }
    void Start() {

        checkupdate();
        
    }
    void Update ()
    {
   
    }
    
       
    
    public void addone ()
    {
        waythrough++;
        if (waythrough > 5) waythrough = 0;
        checkupdate();
    }
    public void backone ()
    {
        waythrough--;
        if (waythrough < 0) waythrough = 5;
        checkupdate();
    }
    
    public void checkupdate ()
    {
        disableall();
        switch (waythrough) {
            case 5: LightCraft.SetActive(true); break;
            case 4: corvettes.SetActive(true); break;
            case 3:capitals.SetActive(true); break;
            case 2: cruisers.SetActive(true); break;
            case 1: frigates.SetActive(true); break;
            case 0: destroyers.SetActive(true); break;
        }
    }
    void disableall ()
    {
        LightCraft.SetActive(false);
        corvettes.SetActive(false);
        capitals.SetActive(false);
        cruisers.SetActive(false);
        frigates.SetActive(false);
        destroyers.SetActive(false);
    }
    void enableall ()
    {
        LightCraft.SetActive(true);
        corvettes.SetActive(true);
        capitals.SetActive(true);
        cruisers.SetActive(true);
        frigates.SetActive(true);
        destroyers.SetActive(true);
    }
    /*
    public void reset ()
    {
        GameObject[] ships = GameObject.FindGameObjectsWithTag("Pickup");
        foreach (GameObject ship in ships)
        {
            if (ship.transform.parent.GetComponent<WeaponAttachPoint>()) ship.GetComponent<WeaponBaseClass>().reset(1);
        }
    }
    */
    
}
