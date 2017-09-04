using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PUNCharacterSpawner : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    public GameObject gam;
    // Update is called once per frame
    public void Update ()
    {
    }
    public void OnJoinedRoom()
    {
        PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
    }
    public void disconnect()
    {
        PhotonVoiceNetwork.Disconnect();
       // foreach(GameObject gam in GameObject.FindGameObjectsWithTag("Voice"))
       // {
       ///     PhotonNetwork.Destroy(gam.GetComponent<PhotonView>());
       // }
    }
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
    {
        PhotonVoiceNetwork.Connect();
        if(PhotonNetwork.inRoom)
        {
            PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0);
        }
      
    }
}
