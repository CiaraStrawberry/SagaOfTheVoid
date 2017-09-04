using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuShipListController : MonoBehaviour {
    public GameObject fleet1;
    public GameObject fleet2;
    public GameObject fleet3;
    private TextMesh textmesh;
	// Use this for initialization
	void Start () {
        textmesh = GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void Update () {
        string displaystring = "";
        if (fleet1.GetActive() == true) displaystring = updatedisplay(fleet1);
        if (fleet2.GetActive() == true) displaystring = updatedisplay(fleet2);
        if (fleet3.GetActive() == true) displaystring = updatedisplay(fleet3);
        textmesh.text = displaystring;
    }
    string updatedisplay (GameObject displaytoupdate)
    {
        string output = "";
        foreach (Transform child in displaytoupdate.transform)
        {
            if (child.childCount > 0)
                output =child.GetChild(0).name + "\n" + output;
        }
        return output;
    }
}
