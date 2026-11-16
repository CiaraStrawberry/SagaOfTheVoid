using UnityEngine;

public class SphereHover : MonoBehaviour
{
	public float Speed = 0.005f;

	public float Offset = 1f;

	private void Update()
	{
		transform.position += new Vector3(0f, Mathf.Sin(Time.time + Offset) * Speed, 0f);
	}
}
