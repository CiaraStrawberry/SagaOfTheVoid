using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Mouse Look")]
public class SgtMouseLookFree : MonoBehaviour
{
	[Tooltip("The camera whose FOV will be used to scale the sensitivity (default = MainCamera)")]
	public Camera Camera;

	[Tooltip("The key that needs to be held down to look")]
	public KeyCode Require = KeyCode.Mouse0;

	[Tooltip("How quickly this rotates relative to the mouse movement")]
	public float Sensitivity = 2f;

	[Tooltip("The speed at which this approaches the target rotation")]
	public float Dampening = 10f;

	public Vector3 Remaining;

	protected virtual void Update()
	{
		float num = Sensitivity;
		Camera camera = Camera;
		if (camera == null)
		{
			camera = Camera.main;
		}
		if (camera != null && !camera.orthographic)
		{
			num *= camera.fieldOfView / 60f;
		}
		if (Require == KeyCode.None || Input.GetKey(Require))
		{
			Remaining.x -= Input.GetAxisRaw("Mouse Y") * num;
			Remaining.y += Input.GetAxisRaw("Mouse X") * num;
		}
		Vector3 vector = SgtHelper.Dampen3(Remaining, Vector3.zero, Dampening, Time.deltaTime, 0.1f);
		Vector3 eulerAngles = Remaining - vector;
		Remaining = vector;
		transform.Rotate(eulerAngles);
	}
}
