using UnityEngine;

namespace Valve.VR.InteractionSystem;

public class LinearDisplacement : MonoBehaviour
{
	public Vector3 displacement;

	public LinearMapping linearMapping;

	private Vector3 initialPosition;

	private void Start()
	{
		initialPosition = transform.localPosition;
		if (linearMapping == null)
		{
			linearMapping = GetComponent<LinearMapping>();
		}
	}

	private void Update()
	{
		if ((bool)linearMapping)
		{
			transform.localPosition = initialPosition + linearMapping.value * displacement;
		}
	}
}
