using UnityEngine;

public class RandomTorque : MonoBehaviour
{
	private void Start()
	{
		gameObject.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-20f, 20f), 0f, Random.Range(-20f, 20f)) * 2f);
		gameObject.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), Random.Range(-20f, 20f)));
	}
}
