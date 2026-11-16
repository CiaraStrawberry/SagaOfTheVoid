using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundTransformC
{
	public Transform transform;

	public int instanceID;

	public bool available;

	public Vector3 position;

	public Vector3 localPosition;

	public Vector3 previousPosition;

	public Vector3 forward;

	public Vector3 up;

	public Vector3 right;

	public Quaternion rotation;

	public Quaternion inverseRotation;

	public ScaleMethod scaleMethod;

	public Vector3 localScale;

	public Vector3 lossyScale;

	public Matrix4x4 transformMatrix = default;

	public bool unfolded;

	public bool Update()
	{
		if (transform != null)
		{
			instanceID = transform.GetInstanceID();
			previousPosition = position;
			position = transform.position;
			forward = transform.forward;
			up = transform.up;
			right = transform.right;
			rotation = transform.rotation;
			inverseRotation = rotation;
			inverseRotation.x = 0f - inverseRotation.x;
			inverseRotation.y = 0f - inverseRotation.y;
			inverseRotation.z = 0f - inverseRotation.z;
			localScale = transform.localScale;
			lossyScale = transform.lossyScale;
			available = true;
		}
		else
		{
			available = false;
		}
		return available;
	}

	public void UpdateMatrix()
	{
		if (available)
		{
			transformMatrix.SetTRS(position, rotation, (scaleMethod != ScaleMethod.Local) ? lossyScale : localScale);
		}
	}

	public bool IsSameAs(Transform comparer)
	{
		return transform == comparer;
	}

	public void SetZeroRotation()
	{
		rotation = Quaternion.identity;
	}

	public void SetFromTransform(Transform otherTransform)
	{
		transform = otherTransform;
		Update();
	}

	public void GetFromTransform(Transform otherTransform)
	{
		otherTransform.position = position;
		otherTransform.rotation = rotation;
		otherTransform.localScale = localScale;
	}

	public void SetLocalPosition(Transform otherTransform)
	{
		if (available)
		{
			localPosition = otherTransform.InverseTransformPoint(transform.position);
		}
	}

	public void SetPostitionAsLocal()
	{
		position = localPosition;
	}

	public int GetInstanceID()
	{
		return instanceID;
	}

	public PlaygroundTransformC Clone()
	{
		PlaygroundTransformC playgroundTransformC = new PlaygroundTransformC();
		playgroundTransformC.transform = transform;
		playgroundTransformC.available = available;
		playgroundTransformC.position = position;
		playgroundTransformC.forward = forward;
		playgroundTransformC.up = up;
		playgroundTransformC.right = right;
		playgroundTransformC.rotation = rotation;
		return playgroundTransformC;
	}
}
