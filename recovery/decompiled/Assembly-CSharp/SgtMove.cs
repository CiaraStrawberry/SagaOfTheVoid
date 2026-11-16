using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Move")]
public class SgtMove : MonoBehaviour
{
	[Tooltip("The speed this GameObject will be moved each second")]
	public Vector3 Speed = Vector3.forward;

	protected virtual void Update()
	{
		transform.Translate(Speed * Time.deltaTime);
	}
}
