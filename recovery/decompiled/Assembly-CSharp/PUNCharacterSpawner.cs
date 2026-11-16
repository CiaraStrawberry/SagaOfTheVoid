using UnityEngine;
using UnityEngine.SceneManagement;

public class PUNCharacterSpawner : MonoBehaviour
{
	public GameObject gam;

	private void Start()
	{
		SceneManager.sceneLoaded += OnLevelFinishedLoading;
	}

	public void Update()
	{
	}

	public void OnJoinedRoom()
	{
		PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f), 0);
	}

	public void disconnect()
	{
		PhotonVoiceNetwork.Disconnect();
	}

	private void OnLevelFinishedLoading(Scene scene, LoadSceneMode scenemode)
	{
		PhotonVoiceNetwork.Connect();
		if (PhotonNetwork.inRoom)
		{
			PhotonNetwork.Instantiate("PlayerPrefab", new Vector3(0f, 0f, 0f), new Quaternion(0f, 0f, 0f, 0f), 0);
		}
	}
}
