using System.Collections;
using UnityEngine;

public class F3DDespawn : MonoBehaviour
{
	private void Start()
	{
		StartCoroutine("Despawn");
	}

	private IEnumerator Despawn()
	{
		yield return new WaitForSeconds(2f);
		Object.Destroy(gameObject);
	}

	private void Update()
	{
	}
}
