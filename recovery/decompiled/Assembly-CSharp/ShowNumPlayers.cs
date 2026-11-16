using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ShowNumPlayers : MonoBehaviour
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
		GetComponent<Text>().text = "return";
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(2f);
		Load();
	}
}
