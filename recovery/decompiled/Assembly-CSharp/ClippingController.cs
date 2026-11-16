using UnityEngine;

public class ClippingController : MonoBehaviour
{
	[SerializeField]
	private Transform parent;

	private Camera camera1;

	private void Start()
	{
		parent = GameObject.Find("WorldScaleBase").transform;
		camera1 = GetComponent<Camera>();
		camera1.nearClipPlane = 5E-10f;
		camera1.farClipPlane = 10f;
	}

	private void Update()
	{
	}
}
