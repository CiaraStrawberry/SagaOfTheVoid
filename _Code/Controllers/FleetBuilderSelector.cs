using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FleetBuilderSelector : MonoBehaviour
{
    public bool right;
    public bool left;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider col)
    {
        if (col.name == "Controller (right)" || col.name == "Controller (left)")
        {
            if (right) { transform.parent.parent.gameObject.GetComponent<FleetNumControl>().addone(); }
            if (left) { transform.parent.parent.gameObject.GetComponent<FleetNumControl>().backone(); }
        }

    }
    public void disableall()
    {
        right = false;
        left = false;
    }
}