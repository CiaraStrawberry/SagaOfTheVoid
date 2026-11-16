using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerName_Get : MonoBehaviour
{
	private new string name;

	private bool count;

	private void Start()
	{
		StartCoroutine(Example());
	}

	private void Update()
	{
	}

	private void Load()
	{
		name = GameObject.Find("Controllers").transform.Find("JoinMultiplayer").GetComponent<JoinMultiplayerGame>().roomName;
		GetComponent<Text>().text = name;
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(2f);
		Load();
	}
}
