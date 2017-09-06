using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Spawns the player prefab once someone has joined the room.
/// </summary>
public class PUNCharacterSpawner : MonoBehaviour
{

    /// <summary>
    /// Initialise the function deligates.
    /// </summary>
    void Start() {  SceneManager.sceneLoaded += OnLevelFinishedLoading;  }
   

   /// <summary>
   /// Spawn the prefab.
   /// </summary>
    public void OnJoinedRoom() { PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0); }
   
    /// <summary>
    /// Disconnect the voice player.
    /// </summary>
    public void disconnect()
    {
        PhotonVoiceNetwork.Disconnect();
    }

    /// <summary>
    /// Check if a player prefab needs to be spawned.
    /// </summary>
    /// <param name="scene"></param>
    /// <param name="scenemode"></param>
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        PhotonVoiceNetwork.Connect();
        if(PhotonNetwork.inRoom) PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
    }
}
