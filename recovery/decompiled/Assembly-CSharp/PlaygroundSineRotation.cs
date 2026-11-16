using UnityEngine;

public class PlaygroundSineRotation : MonoBehaviour
{
	private Transform thisTransform;

	private void Start()
	{
		thisTransform = transform;
	}

	private void Update()
	{
		thisTransform.Rotate(new Vector3(Mathf.Sin(Time.time), Mathf.Cos(Time.time), Mathf.Sin(Time.time)));
	}
}
