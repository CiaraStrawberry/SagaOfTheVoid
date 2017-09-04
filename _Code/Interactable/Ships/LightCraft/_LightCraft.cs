using UnityEngine;
using System.Collections;
using EasyEditor;

public class _LightCraft : _Ship {

    /// <summary>
    /// Default Constructor
    /// </summary>
    public _LightCraft() {
        base.HullType = eHullType.Light;
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private GameObject _CraftPrefab;
    /// <summary>
    /// Prefab to instance the ships
    /// </summary>
    public GameObject CraftPrefab {
        get { return _CraftPrefab; }
        set { _CraftPrefab = value; }
    }

    [Inspector(group = "Engine Settings")]
    [SerializeField]
    private int _ShipsInSquad;
    /// <summary>
    /// How many ships are allowed in the squad
    /// </summary>
    public int ShipsInSquad {
        get { return _ShipsInSquad; }
        set { _ShipsInSquad = value; }
    }

}
