using UnityEngine;

namespace VRTK.Examples;

public class AutoRotation : MonoBehaviour
{
	[Tooltip("Angular velocity in degrees per seconds")]
	public float degPerSec = 60f;

	[Tooltip("Rotation axis")]
	public Vector3 rotAxis = Vector3.up;

	private void Start()
	{
		rotAxis.Normalize();
	}

	private void Update()
	{
		transform.Rotate(rotAxis, degPerSec * Time.deltaTime);
	}
}
