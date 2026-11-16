using UnityEngine;

public class PlaygroundPingPongMove : MonoBehaviour
{
	public float speed = 1f;

	public Vector3 moveLength;

	private Vector3 startPos;

	private Transform thisTransform;

	private void Start()
	{
		thisTransform = transform;
		startPos = transform.position;
	}

	private void Update()
	{
		Vector3 vector = new Vector3((moveLength.x != 0f) ? Mathf.PingPong(Time.time * speed, moveLength.x) : 0f, (moveLength.y != 0f) ? Mathf.PingPong(Time.time * speed, moveLength.y) : 0f, (moveLength.z != 0f) ? Mathf.PingPong(Time.time * speed, moveLength.z) : 0f);
		thisTransform.position = startPos + vector;
	}
}
