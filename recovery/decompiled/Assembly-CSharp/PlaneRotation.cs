using UnityEngine;

public class PlaneRotation : MonoBehaviour
{
	private void Update()
	{
		transform.Rotate(new Vector3(0f, 1f, 0f) * Time.deltaTime);
	}
}
