using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipController : MonoBehaviour {

    // Use this for initialization
    void Awake ()
    {
        GameObject.Find("MainMenuHolder").GetComponent<MainMenuValueHolder>().TutorialObjs.Add(this.gameObject);
    }
    void OnEnable()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 0 || UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 1)
        {
            this.gameObject.SetActive(false);
        }
    }
	// Update is called once per frame
	void Update () {
		
	}
}
