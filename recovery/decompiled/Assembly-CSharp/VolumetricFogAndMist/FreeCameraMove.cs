using UnityEngine;

namespace VolumetricFogAndMist;

public class FreeCameraMove : MonoBehaviour
{
	public float cameraSensitivity = 150f;

	public float climbSpeed = 20f;

	public float normalMoveSpeed = 20f;

	public float slowMoveFactor = 0.25f;

	public float fastMoveFactor = 3f;

	private float rotationX;

	private float rotationY;

	private void Update()
	{
		Vector2 vector = Input.mousePosition;
		if (!(vector.x < 0f) && !(vector.x > (float)Screen.width) && !(vector.y < 0f) && !(vector.y > (float)Screen.height))
		{
			rotationX += Input.GetAxis("Mouse X") * cameraSensitivity * Time.deltaTime;
			rotationY += Input.GetAxis("Mouse Y") * cameraSensitivity * Time.deltaTime;
			rotationY = Mathf.Clamp(rotationY, -90f, 90f);
			transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
			transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);
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
				transform.position -= transform.up * climbSpeed * Time.deltaTime;
			}
			if (Input.GetKey(KeyCode.E))
			{
				transform.position += transform.up * climbSpeed * Time.deltaTime;
			}
		}
	}
}
