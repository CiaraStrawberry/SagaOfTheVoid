using UnityEngine;

public class DebugCameraControls : MonoBehaviour
{
	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			transform.rotation *= Quaternion.Euler(0f, 10f, 0f);
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			transform.rotation *= Quaternion.Inverse(Quaternion.Euler(0f, 10f, 0f));
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			transform.position += 5f * transform.forward;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow))
		{
			transform.rotation *= Quaternion.Inverse(Quaternion.Euler(-10f, 0f, 0f));
		}
		if (Input.GetKeyDown(KeyCode.UpArrow))
		{
			transform.position += 5f * Vector3.up;
		}
	}
}
