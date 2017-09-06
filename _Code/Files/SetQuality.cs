using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using simple;

/// <summary>
/// This function manually sets the quality level of the game.
/// </summary>
public class SetQuality : MonoBehaviour
{

    private GameObject basehol;
    private IOComponent FleetBuilder;
    private GameObject Serializationmanager;

    void Awake()
    {
        FleetBuilder = IORoot.findIO("fleet1");
    }

    void Start()
    {
        if (name == "high")
        {
            QualitySettings.SetQualityLevel(5);
        }
        if (name == "medium")
        {
            QualitySettings.SetQualityLevel(3);
        }
        if (name == "low")
        {
            QualitySettings.SetQualityLevel(1);
        }
        if (FleetBuilder == null)
            return;
    }


}

    

