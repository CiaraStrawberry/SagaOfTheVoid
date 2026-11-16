using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Thruster")]
public class SgtThruster : MonoBehaviour
{
	[Tooltip("How active is this thruster? 0 for off, 1 for max power, -1 for max reverse, etc")]
	public float Throttle;

	[Tooltip("The rigidbody you want to apply the thruster forces to")]
	public Rigidbody Rigidbody;

	[Tooltip("The type of force we want to apply to the Rigidbody")]
	public SgtForceType ForceType = SgtForceType.AddForceAtPosition;

	[Tooltip("The force mode used when ading force to the Rigidbody")]
	public ForceMode ForceMode = ForceMode.Acceleration;

	[Tooltip("The maximum amount of force applied to the rigidbody (when the throttle is -1 or 1)")]
	public float ForceMagnitude = 1f;

	public static SgtThruster CreateThruster(int layer = 0, Transform parent = null)
	{
		return CreateThruster(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtThruster CreateThruster(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Thruster", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtThruster>();
	}

	protected virtual void FixedUpdate()
	{
		if (Rigidbody != null)
		{
			Vector3 force = transform.forward * ForceMagnitude * Throttle * Time.fixedDeltaTime;
			switch (ForceType)
			{
			case SgtForceType.AddForce:
				Rigidbody.AddForce(force, ForceMode);
				break;
			case SgtForceType.AddForceAtPosition:
				Rigidbody.AddForceAtPosition(force, transform.position, ForceMode);
				break;
			}
		}
	}
}
