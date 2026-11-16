using UnityEngine;

namespace DynamicFogAndMist;

public class PillarManager : MonoBehaviour
{
	private float initialX;

	private void Start()
	{
		initialX = transform.position.x;
		RepositionPillar();
	}

	private void Update()
	{
		if (transform.position.z < Camera.main.transform.position.z - transform.localScale.z)
		{
			RepositionPillar();
		}
	}

	private void RepositionPillar()
	{
		Vector3 vector = transform.position + Vector3.forward * 200f;
		transform.position = new Vector3(initialX + Random.value * 6f - 3f, vector.y, vector.z);
	}

	private void OnCollisionEnter(Collision collision)
	{
		GetComponent<AudioSource>().Play();
	}
}
