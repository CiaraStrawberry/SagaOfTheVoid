using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachPointsController : MonoBehaviour {
    public GameObject[] attachpoints;
    public int countthrough = 0;
    private Vector3 startpos;
    // Use this for initialization
    void Start () {
        disableall();
        InvokeRepeating("rep", 0, 0.3f);
        disableall();
       if(attachpoints[0]) attachpoints[0].SetActive(true);
        startpos = transform.localPosition;
        rep();

    }
    void disableall() { foreach (GameObject point in attachpoints) if(point) point.SetActive(false); }
    
	// Update is called once per frame
	void Update () {
        transform.localPosition = startpos;
	}
    void rep ()
    {
        if (attachpoints[countthrough] != null && attachpoints[countthrough].GetActive() != false && attachpoints[countthrough].transform.childCount > 0)
        {
            countthrough++; 
        }
        if (countthrough < 1) countthrough = 1;
        if (attachpoints[countthrough] != null && attachpoints[countthrough - 1].transform.childCount < 1)
        {
            countthrough--;
        }
        disableall();
        for (int i = 0; i < countthrough + 1; i++) if(attachpoints[i] != null)  attachpoints[i].SetActive(true);

        int a = 0;
        foreach (GameObject point in attachpoints) {
            a++;       
            if (point && point.transform.childCount > 0 && a > countthrough)
            {
                countthrough++;
            }       
        }
    }
    public void AttachShip (GameObject ship)
    {
        ship.transform.parent = attachpoints[countthrough].transform;
        ship.transform.localPosition = new Vector3(0, 0, 0);
        ship.transform.rotation = new Quaternion(0, 0, 0, 0);
        rep();
    }
}
