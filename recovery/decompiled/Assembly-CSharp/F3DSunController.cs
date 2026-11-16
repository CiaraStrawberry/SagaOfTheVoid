using UnityEngine;

public class F3DSunController : MonoBehaviour
{
	private float mouseX;

	private float mouseY;

	private Vector3 offsetVector;

	private void Start()
	{
	}

	private void Update()
	{
		mouseX += Input.GetAxis("Mouse X") * 3f;
		mouseY += Input.GetAxis("Mouse Y") * 3f;
		offsetVector = Quaternion.AngleAxis(mouseX, Vector3.up) * (Vector3.forward + Vector3.up) * 3f;
		Vector3 normalized = Vector3.Cross(offsetVector, Vector3.up).normalized;
		offsetVector = Quaternion.AngleAxis(mouseY, normalized) * offsetVector;
		transform.position = offsetVector;
	}

	private void OnGUI()
	{
		if (!Screen.fullScreen && GUI.Button(new Rect(10f, 10f, 150f, 100f), "Fullscreen"))
		{
			Resolution[] resolutions = Screen.resolutions;
			Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, fullscreen: true);
		}
	}
}
