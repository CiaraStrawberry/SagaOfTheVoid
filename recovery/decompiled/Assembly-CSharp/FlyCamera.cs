using UnityEngine;

public class FlyCamera : MonoBehaviour
{
	public float cameraSensitivity = 1f;

	public float climbSpeed = 4f;

	public float normalMoveSpeed = 2f;

	public float slowMoveFactor = 0.25f;

	public float fastMoveFactor = 3f;

	public float rotationX;

	public float rotationY;

	public float OrbitSpeed = 1f;

	public float Zoom = 10f;

	public float ZoomSpeed = 10f;

	private Vector3 lastFlyPos;

	private void Start()
	{
	}

	private void Update()
	{
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity;
			rotationY = Mathf.Clamp(rotationY, -90f, 90f);
			if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
			{
				transform.position += transform.forward * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * fastMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			}
			else if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				transform.position += transform.forward * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * (normalMoveSpeed * slowMoveFactor) * Input.GetAxis("Horizontal") * Time.deltaTime;
			}
			else
			{
				transform.position += transform.forward * normalMoveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
				transform.position += transform.right * normalMoveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.Q))
			{
				transform.position += transform.up * climbSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				transform.position -= transform.up * climbSpeed * Time.deltaTime;
			}
			Zoom = transform.position.magnitude;
		}
		else
		{
			if (Input.GetAxis("Mouse ScrollWheel") != 0f)
			{
				Zoom = Mathf.Max(0f, Zoom + Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed);
			}
			transform.position = -transform.forward * Zoom;
			if (Input.GetMouseButton(0))
			{
				rotationX += Input.GetAxis("Mouse X") * cameraSensitivity;
				rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity;
			}
			else
			{
				rotationX += OrbitSpeed * Time.deltaTime;
			}
		}
		if (Input.GetKeyDown(KeyCode.Space))
		{
			if (Cursor.lockState == CursorLockMode.Locked)
			{
				Cursor.lockState = CursorLockMode.None;
			}
			else
			{
				Cursor.lockState = CursorLockMode.Locked;
			}
		}
	}

	private void FixedUpdate()
	{
		transform.rotation = Quaternion.AngleAxis(rotationX, Vector3.up);
		transform.rotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
	}

	private void OnGUI()
	{
		GUIStyle gUIStyle = new GUIStyle(GUI.skin.label);
		gUIStyle.alignment = TextAnchor.MiddleCenter;
		GUI.color = new Color(1f, 1f, 1f, 0.4f);
		if (Cursor.lockState == CursorLockMode.Locked)
		{
			GUI.Label(new Rect(Screen.width / 2 - 256, Screen.height - 37, 512f, 32f), "'Space' - switch orbit camera.", gUIStyle);
		}
		else
		{
			GUI.Label(new Rect(0f, Screen.height - 37, Screen.width, 32f), "'Space' - switch free fly camera. \n Click and Drag to rotate view", gUIStyle);
		}
	}
}
