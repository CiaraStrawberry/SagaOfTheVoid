using UnityEngine;

public class SDP01_DriftObject : MonoBehaviour
{
	private Transform myTransform;

	public bool Setup;

	public float DriftSpeedX;

	public float DriftSpeedY;

	public float DriftSpeedZ;

	public float DriftRotateSpeedX;

	public float DriftRotateSpeedY;

	public float DriftRotateSpeedZ;

	private Vector3 currentLocalPosition;

	private Vector3 currentLocalRotationEulers;

	private void Start()
	{
		myTransform = gameObject.transform;
		currentLocalPosition = myTransform.localPosition;
		currentLocalRotationEulers = myTransform.localRotation.eulerAngles;
	}

	private void Update()
	{
		if (Setup)
		{
			currentLocalPosition.x += DriftSpeedX * Time.deltaTime;
			currentLocalPosition.y += DriftSpeedY * Time.deltaTime;
			currentLocalPosition.z += DriftSpeedZ * Time.deltaTime;
			currentLocalRotationEulers.x += DriftRotateSpeedX * Time.deltaTime;
			currentLocalRotationEulers.y += DriftRotateSpeedY * Time.deltaTime;
			currentLocalRotationEulers.z += DriftRotateSpeedZ * Time.deltaTime;
			myTransform.localPosition = currentLocalPosition;
			myTransform.localRotation = Quaternion.Euler(currentLocalRotationEulers);
		}
	}
}
