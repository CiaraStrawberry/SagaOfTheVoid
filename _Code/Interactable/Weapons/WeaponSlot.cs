using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyEditor;

public class WeaponSlot : MonoBehaviour {

    [Inspector(displayHeader = true, group = "Weapon Settings", groupDescription = "Settings for the engine", order = 1)]
    [SerializeField]
    private eHullType _MaximumHull;
    /// <summary>
    /// Minimum hull required for weapon
    /// </summary>
    public eHullType MaximumHull {
        get { return _MaximumHull; }
        set { _MaximumHull = value; }
    }

    [Inspector(group = "Weapon Settings", order = 1)]
    [SerializeField]
    private _Weapon _WeaponToPlace;
    /// <summary>
    /// Weapon object to place
    /// </summary>
    public _Weapon WeaponToPlace {
        get { return _WeaponToPlace; }
        set { _WeaponToPlace = value; }
    }

    [Inspector(group = "Weapon Settings", order = 1)]
    [SerializeField]
    private GameObject[] _Placements;
    /// <summary>
    /// Clone positions
    /// </summary>
    public GameObject[] Placements {
        get { return _Placements; }
        set { _Placements = value; }
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

}
