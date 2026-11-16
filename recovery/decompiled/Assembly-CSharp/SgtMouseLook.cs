using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Mouse Look")]
public class SgtMouseLook : MonoBehaviour
{
	[Tooltip("The camera whose FOV will be used to scale the sensitivity (default = MainCamera)")]
	public Camera Camera;

	[Tooltip("The key that needs to be held down to look")]
	public KeyCode Require = KeyCode.Mouse0;

	[Tooltip("How quickly this rotates relative to the mouse movement")]
	public float Sensitivity = 2f;

	[Tooltip("The target X rotation")]
	[FormerlySerializedAs("TargetPitch")]
	public float Pitch;

	[Tooltip("The target Y rotation")]
	[FormerlySerializedAs("TargetYaw")]
	public float Yaw;

	[Tooltip("The speed at which this approaches the target rotation")]
	public float Dampening = 10f;

	private float currentPitch;

	private float currentYaw;

	protected virtual void Awake()
	{
		currentPitch = Pitch;
		currentYaw = Yaw;
	}

	protected virtual void Update()
	{
		Pitch = Mathf.Clamp(Pitch, -89.9f, 89.9f);
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
			Pitch -= Input.GetAxisRaw("Mouse Y") * num;
			Yaw += Input.GetAxisRaw("Mouse X") * num;
		}
		currentPitch = SgtHelper.Dampen(currentPitch, Pitch, Dampening, Time.deltaTime, 0.1f);
		currentYaw = SgtHelper.Dampen(currentYaw, Yaw, Dampening, Time.deltaTime, 0.1f);
		Quaternion q = Quaternion.Euler(currentPitch, currentYaw, 0f);
		SgtHelper.SetLocalRotation(transform, q);
	}
}
