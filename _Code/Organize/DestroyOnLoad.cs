using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DestroyOnLoad : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
       if(this.gameObject.name != "GameObjectives") if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
       
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
      if(this != null &&this.gameObject != null) Destroy(this.gameObject);
    }
        // Update is called once per frame
    void Update () {
		
	}
}
