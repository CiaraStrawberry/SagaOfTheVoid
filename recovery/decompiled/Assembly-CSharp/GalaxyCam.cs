using Galaxia;
using UnityEngine;

public class GalaxyCam : MonoBehaviour
{
	public float MaxZoom = 40f;

	public float MinZoom = 5f;

	[Range(0f, 1f)]
	public float Zoom = 1f;

	public float Sensitivity = 1.5f;

	public float FastSensitivityMultiplyer = 3f;

	public float ZoomSensitivity = 0.65f;

	public float ZoomSmoothing = 2f;

	[CurveRange(0f, 0f, 1f, 1f)]
	public AnimationCurve ZoomRotation = AnimationCurve.Linear(0f, 1f, 1f, 0.8f);

	[CurveRange(0f, 0f, 1f, 1f)]
	public AnimationCurve ZoomMoveSensitivity = AnimationCurve.Linear(0f, 0.3f, 1f, 1f);

	public float Rotation = 50f;

	private void Start()
	{
	}

	private void Update()
	{
		Zoom -= Input.GetAxis("Mouse ScrollWheel") * ZoomSensitivity;
		Zoom = Mathf.Clamp01(Zoom);
		if (Input.GetMouseButton(1))
		{
			float num = Sensitivity;
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				num *= FastSensitivityMultiplyer;
			}
			transform.position -= new Vector3(Input.GetAxis("Mouse X"), 0f, Input.GetAxis("Mouse Y")) * num * ZoomMoveSensitivity.Evaluate(Zoom);
		}
		transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, Zoom * (MaxZoom - MinZoom) + MinZoom, transform.position.z), ZoomSmoothing * Time.deltaTime);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(Rotation * ZoomRotation.Evaluate(Zoom), transform.right), ZoomSmoothing * Time.deltaTime);
	}
}
