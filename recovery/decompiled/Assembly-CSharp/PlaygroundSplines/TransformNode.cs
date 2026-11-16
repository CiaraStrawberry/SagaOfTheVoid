using System;
using UnityEngine;

namespace PlaygroundSplines;

[Serializable]
public class TransformNode
{
	public bool enabled;

	public Transform transform;

	private bool isAvailable;

	private Vector3 position;

	private Vector3 inversePosition;

	private Vector3 previousPosition;

	public bool Update(Transform splineTransform)
	{
		if (enabled && transform != null)
		{
			previousPosition = position;
			position = transform.position;
			inversePosition = splineTransform.InverseTransformPoint(transform.position);
			isAvailable = true;
			return true;
		}
		isAvailable = false;
		return false;
	}

	public bool IsAvailable()
	{
		return enabled && isAvailable;
	}

	public Vector3 GetPosition()
	{
		return position;
	}

	public Vector3 GetInvsersePosition()
	{
		return inversePosition;
	}

	public void SetPosition(Vector3 newPosition)
	{
		if (!(transform == null))
		{
			transform.position = newPosition;
		}
	}

	public void Translate(Vector3 translation)
	{
		if (!(transform == null))
		{
			transform.position += translation;
		}
	}

	public Vector3 GetPositionDelta()
	{
		return previousPosition - position;
	}
}
