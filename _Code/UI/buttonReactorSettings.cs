using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class buttonReactorSettings : MonoBehaviour {

    public GameObject go;
    public Transform dispenseLocation;
    public bool High;
    public bool Medium;
    public bool Low;

    SettingsController shipparent;

    private void Start()
    {
        GetComponent<VRTK_Button>().events.OnPush.AddListener(handlePush);
        shipparent = GameObject.Find("SettingsParent").GetComponent<SettingsController>();
    }

    private void handlePush()
    {
        Debug.Log("Pushed");
        if (High) { shipparent.High(); Debug.Log("high"); } 
        if (Medium) shipparent.Medium();
        if (Low) shipparent.Low();
    }
}
