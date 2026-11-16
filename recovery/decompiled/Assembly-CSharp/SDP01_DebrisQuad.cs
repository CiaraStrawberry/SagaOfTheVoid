using UnityEngine;

public class SDP01_DebrisQuad : MonoBehaviour
{
	private Transform myTransform;

	public Transform Quad2Transform;

	private void Start()
	{
		myTransform = gameObject.transform;
		Vector3 zero = Vector3.zero;
		zero.x = Random.Range(-360f, 360f);
		zero.y = Random.Range(-360f, 360f);
		zero.z = Random.Range(-360f, 360f);
		myTransform.rotation = Quaternion.Euler(zero);
		zero.x = Random.Range(-360f, 360f);
		zero.y = Random.Range(-360f, 360f);
		zero.z = Random.Range(-360f, 360f);
		if (Quad2Transform != null)
		{
			Quad2Transform.localRotation = Quaternion.Euler(zero);
			float num = Random.Range(0.75f, 1.25f);
			Quad2Transform.localScale = myTransform.localScale * num;
		}
	}
}
