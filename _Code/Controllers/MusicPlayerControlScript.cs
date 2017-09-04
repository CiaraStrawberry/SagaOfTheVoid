using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayerControlScript : MonoBehaviour {

    // Use this for initialization
    void Start()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        else
        {

            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
    }
     
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if (this != null && this.gameObject != null)  Destroy(this.gameObject);
        }
    }
}
