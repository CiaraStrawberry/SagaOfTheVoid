using UnityEngine;

public class F3DSolarCamera : MonoBehaviour
{
	private Transform focus;

	private F3DPlanet[] pObjects;

	private int index;

	[Range(0.1f, 5f)]
	public float mouseSensitivity;

	[Range(20f, 200f)]
	public float zoomSpeed;

	private float DefaultZoom = 100f;

	private Vector3 cameraRotation;

	private Vector3 offsetVector;

	private float zoomLevel;

	private Transform camera;

	private void Awake()
	{
		camera = GetComponent<Transform>();
	}

	private void Start()
	{
		zoomLevel += DefaultZoom;
		index = 0;
		pObjects = Object.FindObjectsOfType<F3DPlanet>();
		focus = pObjects[0].transform;
	}

	public Vector2 GetMouse()
	{
		return new Vector2(Input.GetAxis("Mouse X"), 0f - Input.GetAxis("Mouse Y"));
	}

	private void LateUpdate()
	{
		if (Input.GetMouseButtonDown(0))
		{
			index++;
			if (index > pObjects.Length - 1)
			{
				index = 0;
			}
			focus = pObjects[index].transform;
			zoomLevel = focus.localScale.x * 3f;
		}
		if (Input.GetMouseButtonDown(1))
		{
			index--;
			if (index < 0)
			{
				index = pObjects.Length - 1;
			}
			focus = pObjects[index].transform;
			zoomLevel = focus.localScale.x * 3f;
		}
		cameraRotation += new Vector3(GetMouse().x, GetMouse().y, 0f) * mouseSensitivity;
		cameraRotation.y = Mathf.Clamp(cameraRotation.y, -85f, 85f);
		zoomLevel += Input.GetAxis("Mouse ScrollWheel") * (0f - zoomSpeed);
		zoomLevel = Mathf.Clamp(zoomLevel, focus.localScale.x + focus.localScale.x / 3f, 150f);
		offsetVector = -Vector3.forward * zoomLevel;
		offsetVector = Quaternion.AngleAxis(cameraRotation.x, Vector3.up) * offsetVector;
		Vector3 normalized = Vector3.Cross(offsetVector, Vector3.up).normalized;
		offsetVector = Quaternion.AngleAxis(cameraRotation.y, normalized) * offsetVector;
		camera.position = focus.position + offsetVector;
		camera.rotation = Quaternion.LookRotation(focus.transform.position - transform.position, Vector3.up);
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
