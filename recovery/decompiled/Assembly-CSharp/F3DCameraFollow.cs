using UnityEngine;

public class F3DCameraFollow : MonoBehaviour
{
	public Transform BGCamera;

	public Transform CameraOrigin;

	public float Pos;

	public float Rot;

	private void Start()
	{
	}

	private void Update()
	{
		transform.rotation = Quaternion.Lerp(transform.rotation, CameraOrigin.rotation, Time.deltaTime * Rot);
	}

	private void LateUpdate()
	{
		transform.position = CameraOrigin.position;
	}
}
