using UnityEngine;

public class F3DSkysphere : MonoBehaviour
{
	public Transform Cam;

	private void Start()
	{
	}

	private void LateUpdate()
	{
		transform.position = Cam.position;
	}
}
