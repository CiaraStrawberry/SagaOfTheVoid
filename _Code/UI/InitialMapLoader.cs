using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InitialMapLoader : MonoBehaviour {

	// Use this for initialization
	void Start () {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.LoadScene(5);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
