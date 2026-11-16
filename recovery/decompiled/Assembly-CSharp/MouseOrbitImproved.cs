using UnityEngine;

public class MouseOrbitImproved : MonoBehaviour
{
	public Transform target;

	public float distance = 5f;

	public float xSpeed = 120f;

	public float ySpeed = 120f;

	public float yMinLimit = -20f;

	public float yMaxLimit = 80f;

	public float distanceMin = 0.5f;

	public float distanceMax = 15f;

	public float smoothTime = 2f;

	private float rotationYAxis;

	private float rotationXAxis;

	private float velocityX;

	private float velocityY;

	private void Start()
	{
		Vector3 eulerAngles = transform.eulerAngles;
		rotationYAxis = eulerAngles.y;
		rotationXAxis = eulerAngles.x;
		if ((bool)GetComponent<Rigidbody>())
		{
			GetComponent<Rigidbody>().freezeRotation = true;
		}
	}

	private void LateUpdate()
	{
		if ((bool)target)
		{
			if (Input.GetMouseButton(0))
			{
				velocityX += xSpeed * Input.GetAxis("Mouse X") * distance * 0.02f;
				velocityY += ySpeed * Input.GetAxis("Mouse Y") * 0.02f;
			}
			rotationYAxis += velocityX;
			rotationXAxis -= velocityY;
			rotationXAxis = ClampAngle(rotationXAxis, yMinLimit, yMaxLimit);
			Quaternion quaternion = Quaternion.Euler(rotationXAxis, rotationYAxis, 0f);
			Quaternion quaternion2 = quaternion;
			distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5f, distanceMin, distanceMax);
			if (Physics.Linecast(target.position, transform.position, out var hitInfo))
			{
				distance -= hitInfo.distance;
			}
			Vector3 vector = new Vector3(0f, 0f, 0f - distance);
			Vector3 position = quaternion2 * vector + target.position;
			transform.rotation = quaternion2;
			transform.position = position;
			velocityX = Mathf.Lerp(velocityX, 0f, Time.deltaTime * smoothTime);
			velocityY = Mathf.Lerp(velocityY, 0f, Time.deltaTime * smoothTime);
		}
	}

	public static float ClampAngle(float angle, float min, float max)
	{
		if (angle < -360f)
		{
			angle += 360f;
		}
		if (angle > 360f)
		{
			angle -= 360f;
		}
		return Mathf.Clamp(angle, min, max);
	}
}
