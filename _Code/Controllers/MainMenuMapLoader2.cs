using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuMapLoader2 : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	int a;
	// Update is called once per frame
	void Update () {
        a++;
        if( a == 1)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
	}
}
