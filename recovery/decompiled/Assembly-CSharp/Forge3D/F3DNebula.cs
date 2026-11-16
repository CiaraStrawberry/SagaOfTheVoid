using UnityEngine;

namespace Forge3D;

public class F3DNebula : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		transform.position -= Vector3.forward * Time.deltaTime * 4000f;
		if (transform.position.z < -9150f)
		{
			Vector3 position = transform.position;
			position.z = 9150f;
			transform.position = position;
			transform.rotation = Random.rotation;
			transform.localScale = new Vector3(1f, 1f, 1f) * Random.Range(200, 800);
		}
	}
}
