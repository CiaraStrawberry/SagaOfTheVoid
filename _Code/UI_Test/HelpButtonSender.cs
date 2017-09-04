using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpButtonSender : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void Help ()
    {
        if(GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs[0].GetActive() == true)   sethelpactive(false);
        else sethelpactive(true);
    }

    void sethelpactive(bool activetoset) {
        foreach (GameObject help in GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.ToArray())
            if (help != null && help.gameObject != null) {
                
                foreach (Transform t in help.GetComponentInChildren<Transform>(true))
                {
                    help.SetActive(true);
                }
                help.SetActive(activetoset);
            }
    }
    
}
