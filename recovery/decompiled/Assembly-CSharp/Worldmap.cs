using UnityEngine;

public class Worldmap : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(Vector3.up * 3f * Time.deltaTime);
	}
}
