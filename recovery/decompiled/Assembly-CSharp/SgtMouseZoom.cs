using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Mouse Zoom")]
public class SgtMouseZoom : MonoBehaviour
{
	[Tooltip("The camera that will be zoomed (default = MainCamera)")]
	public Camera Camera;

	[Tooltip("The key that needs to be held down to zoom")]
	public KeyCode Require;

	[Tooltip("How quickly this rotates relative to the mouse movement")]
	public float Sensitivity = 0.1f;

	[Tooltip("The zoom value")]
	public float Zoom;

	[Tooltip("The minimum zoom value")]
	public float ZoomMin = 1f;

	[Tooltip("The maximum zoom value")]
	public float ZoomMax = 90f;

	[Tooltip("The speed at which this approaches the target rotation")]
	public float Dampening = 10f;

	private float currentZoom;

	protected virtual void Update()
	{
		Camera camera = Camera;
		if (camera == null)
		{
			camera = Camera.main;
		}
		if (!(camera != null))
		{
			return;
		}
		if (currentZoom == 0f)
		{
			if (camera.orthographic)
			{
				currentZoom = camera.orthographicSize;
			}
			else
			{
				currentZoom = camera.fieldOfView;
			}
			Zoom = currentZoom;
		}
		if (Require == KeyCode.None || Input.GetKey(Require))
		{
			float y = Input.mouseScrollDelta.y;
			if (y > 0f)
			{
				Zoom *= 1f - Sensitivity;
			}
			if (y < 0f)
			{
				Zoom *= 1f + Sensitivity;
			}
		}
		Zoom = Mathf.Clamp(Zoom, ZoomMin, ZoomMax);
		currentZoom = SgtHelper.Dampen(currentZoom, Zoom, Dampening, Time.deltaTime, 0.1f);
		if (camera.orthographic)
		{
			camera.orthographicSize = currentZoom;
		}
		else
		{
			camera.fieldOfView = currentZoom;
		}
	}
}
