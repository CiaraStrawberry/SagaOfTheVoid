using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selection_Controller : MonoBehaviour {
    private GameObject HealthBar;
    // Use this for initialization
    void Awake() {
       
        HealthBar = transform.Find("HealthBar").gameObject;
        HealthBar.SetActive(false);
    
    }
  

    public void enable()
    {
        if (HealthBar)
        {
            HealthBar.SetActive(true);
        }
        else
        {
            HealthBar = transform.Find("HealthBar").gameObject;
            HealthBar.SetActive(false);
        }
        
    }

    public void Disable()
    {
        if (HealthBar)
        {
            HealthBar.SetActive(true);
        }
        else
        {
            HealthBar = transform.Find("HealthBar").gameObject;
            HealthBar.SetActive(false);
        }
    }

}
