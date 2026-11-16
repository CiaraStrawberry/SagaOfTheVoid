using System;
using UnityEngine;

[Serializable]
public class SelectorBase : MonoBehaviour
{
	public bool Rotate;

	public bool IsProjector;

	public float RotateDegreesPerSecond = 15f;

	public Vector3 BaseScale = Vector3.one;

	private void Reset()
	{
		Rotate = true;
		IsProjector = false;
		RotateDegreesPerSecond = 15f;
		BaseScale = Vector3.one;
	}

	private void Start()
	{
	}

	private void Update()
	{
		if (Rotate)
		{
			float angle = RotateDegreesPerSecond * Time.deltaTime;
			if (IsProjector)
			{
				transform.Rotate(Vector3.forward, angle);
			}
			else
			{
				transform.Rotate(Vector3.up, angle);
			}
		}
	}
}
