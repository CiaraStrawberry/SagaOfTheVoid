using UnityEngine;

public class BillboardLaub : MonoBehaviour
{
	private Transform mainCamTransform;

	private Transform cachedTransform;

	private void Start()
	{
		mainCamTransform = Camera.main.transform;
		cachedTransform = transform;
	}

	private void Update()
	{
		Vector3 vector = mainCamTransform.position - cachedTransform.position;
		vector.x = (vector.z = 0f);
		cachedTransform.LookAt(mainCamTransform.position - vector);
	}
}
