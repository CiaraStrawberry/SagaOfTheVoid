using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// This is a simple singleton controller that destroys itself if a game starts.
/// </summary>
public class MusicPlayerControlScript : MonoBehaviour {
 
    /// <summary>
    /// initialise.
    /// </summary>
    void Start()
    {
        if (FindObjectsOfType(GetType()).Length > 1) Destroy(gameObject);
        else
        {
            DontDestroyOnLoad(this.gameObject);
            SceneManager.sceneLoaded += OnLevelFinishedLoading;
        }
    }
     
	/// <summary>
    /// Check if the object needs to be destroyed.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        if (this != null && this.gameObject != null && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 0 && UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex != 1 && SceneManager.GetActiveScene().buildIndex != 5 && SceneManager.GetActiveScene().buildIndex != 6)
        {
            if (this != null && this.gameObject != null)  Destroy(this.gameObject);
        }
    }
}
