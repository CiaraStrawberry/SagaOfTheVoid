using UnityEngine;

public class SDP01_DebrisCameraController : MonoBehaviour
{
	private Transform myTransform;

	private Vector3 currentRotationEulers;

	private float currentYRotation;

	public Transform CameraHeightTransform;

	private Vector3 heightEulers;

	public Transform CameraTransform;

	private Vector3 cameraLocalPosition;

	public Camera MainCamera;

	public float ZoomMIN = 5f;

	public float ZoomMAX = 35f;

	private float currentZoomLevel = -10f;

	public float UpDownMIN = -90f;

	public float UpDownMAX = 90f;

	private float currentUpDownLevel;

	public float LerpSpeed = 1f;

	public KeyCode ZoomOutKey = KeyCode.Q;

	public KeyCode ZoomInKey = KeyCode.E;

	public KeyCode RotateLeftKey = KeyCode.A;

	public KeyCode RotateRightKey = KeyCode.D;

	public KeyCode RotateUpKey = KeyCode.W;

	public KeyCode RotateDownKey = KeyCode.S;

	public float ZoomInOutSpeed = 15f;

	public float RotateUpDownSpeed = 10f;

	public float RotateLeftRightSpeed = 25f;

	private void Start()
	{
		myTransform = gameObject.transform;
		if (CameraTransform != null)
		{
			cameraLocalPosition = CameraTransform.localPosition;
			currentYRotation = myTransform.rotation.eulerAngles.y;
			currentZoomLevel = Mathf.Abs(cameraLocalPosition.z);
			currentUpDownLevel = CameraHeightTransform.rotation.eulerAngles.x;
		}
	}

	private void Update()
	{
		if (Input.GetKey(ZoomInKey) && currentZoomLevel > ZoomMIN)
		{
			currentZoomLevel -= ZoomInOutSpeed * Time.deltaTime;
		}
		if (Input.GetKey(ZoomOutKey) && currentZoomLevel < ZoomMAX)
		{
			currentZoomLevel += ZoomInOutSpeed * Time.deltaTime;
		}
		if (Input.GetKey(RotateLeftKey))
		{
			currentYRotation += RotateLeftRightSpeed * Time.deltaTime;
		}
		if (Input.GetKey(RotateRightKey))
		{
			currentYRotation -= RotateLeftRightSpeed * Time.deltaTime;
		}
		if (Input.GetKey(RotateDownKey) && currentUpDownLevel > UpDownMIN)
		{
			currentUpDownLevel -= RotateUpDownSpeed * Time.deltaTime;
		}
		if (Input.GetKey(RotateUpKey) && currentUpDownLevel < UpDownMAX)
		{
			currentUpDownLevel += RotateUpDownSpeed * Time.deltaTime;
		}
		heightEulers = new Vector3(currentUpDownLevel, 0f, 0f);
		CameraHeightTransform.localRotation = Quaternion.LerpUnclamped(CameraHeightTransform.localRotation, Quaternion.Euler(heightEulers), LerpSpeed * Time.deltaTime);
		currentRotationEulers = new Vector3(0f, currentYRotation, 0f);
		myTransform.rotation = Quaternion.LerpUnclamped(myTransform.rotation, Quaternion.Euler(currentRotationEulers), LerpSpeed * Time.deltaTime);
		cameraLocalPosition.z = 0f - currentZoomLevel;
		CameraTransform.localPosition = Vector3.LerpUnclamped(CameraTransform.localPosition, cameraLocalPosition, LerpSpeed * Time.deltaTime);
	}
}
