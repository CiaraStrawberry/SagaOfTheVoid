using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class you attach to an attach point and it lets the player attach weaponry to a ship.
/// </summary>
public class AttachPointsController : MonoBehaviour {

    //The AttachPoints on this GameObject
    public GameObject[] attachpoints;
    //The number of Points attached to the attach point controller.
    public int countthrough = 0;
    // The Start Position of the Controller
    private Vector3 startpos;

    /// <summary>
    // The start Function
    /// <summary>
    void Start () {
        disableall();
        InvokeRepeating("rep", 0, 0.3f);
        disableall();
       if(attachpoints[0]) attachpoints[0].SetActive(true);
        startpos = transform.localPosition;
        rep();

    }

    /// <summary>
    /// Disable all Attachpoints connected.
    /// <summary>
    void disableall() { foreach (GameObject point in attachpoints) if(point) point.SetActive(false); }

    /// <summary>
    // Update is called once per frame
    /// <summary>
    void Update () {
        transform.localPosition = startpos;
	}

    /// <summary>
    /// The repeating function to keep track of things relating to the attachpoint
    /// <summary>
    void rep ()
    {
        if (attachpoints[countthrough] != null && attachpoints[countthrough].GetActive() != false && attachpoints[countthrough].transform.childCount > 0)   countthrough++; 

        if (countthrough < 1) countthrough = 1;

        if (attachpoints[countthrough] != null && attachpoints[countthrough - 1].transform.childCount < 1)  countthrough--;
        
        disableall();

        for (int i = 0; i < countthrough + 1; i++) if(attachpoints[i] != null)  attachpoints[i].SetActive(true);

        int a = 0;
        // TODO: Change this to a for loop.
        foreach (GameObject point in attachpoints) {
            a++;       
            if (point && point.transform.childCount > 0 && a > countthrough)      countthrough++;    
        }

    }

    /// <summary>
    /// Attach to a ship
    /// </summary>
    /// <param name="ship"> the ship to attach to</param>
    public void AttachShip (GameObject ship)
    {
        ship.transform.parent = attachpoints[countthrough].transform;
        ship.transform.localPosition = new Vector3(0, 0, 0);
        ship.transform.rotation = new Quaternion(0, 0, 0, 0);
        rep();
    }

}
