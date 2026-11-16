using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Rotate")]
public class SgtRotate : MonoBehaviour
{
	public Vector3 DegreesPerSecond = new Vector3(0f, 100f, 0f);

	protected virtual void Update()
	{
		transform.Rotate(DegreesPerSecond * Time.deltaTime);
	}
}
