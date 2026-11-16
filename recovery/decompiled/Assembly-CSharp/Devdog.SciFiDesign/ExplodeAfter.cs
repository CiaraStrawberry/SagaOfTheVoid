using System.Collections;
using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class ExplodeAfter : MonoBehaviour
{
	public float minTime = 10f;

	public float maxTime = 20f;

	[Required]
	public GameObject explosion;

	private GameObject _inst;

	protected void Start()
	{
		StartCoroutine(WaitAndDestroy(Random.Range(minTime, maxTime)));
	}

	private IEnumerator WaitAndDestroy(float waitTime)
	{
		yield return new WaitForSeconds(waitTime);
		_inst = Object.Instantiate(explosion);
		_inst.transform.position = transform.position;
		_inst.transform.rotation = transform.rotation;
		_inst.SetActive(value: true);
		Object.Destroy(gameObject);
	}
}
