using System.Collections;
using UnityEngine;

public class CreateMatchUI : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine(Example());
	}

	private void Update()
	{
	}

	private void Load()
	{
	}

	private IEnumerator Example()
	{
		yield return new WaitForSeconds(2f);
		Load();
	}
}
