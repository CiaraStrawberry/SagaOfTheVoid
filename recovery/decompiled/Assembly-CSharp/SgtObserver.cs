using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
[AddComponentMenu("Space Graphics Toolkit/SGT Observer")]
public class SgtObserver : MonoBehaviour
{
	public static List<SgtObserver> AllObservers = new List<SgtObserver>();

	public static Action<SgtObserver> OnObserverPreCull;

	public static Action<SgtObserver> OnObserverPreRender;

	public static Action<SgtObserver> OnObserverPostRender;

	[Tooltip("The amount of degrees this observer has rolled (used to counteract billboard non-rotation)")]
	public float RollAngle;

	public Quaternion RollQuaternion = Quaternion.identity;

	public Matrix4x4 RollMatrix = Matrix4x4.identity;

	[NonSerialized]
	public Vector3 DeltaPosition;

	[NonSerialized]
	public Vector3 Velocity;

	[NonSerialized]
	public Quaternion OldRotation = Quaternion.identity;

	[NonSerialized]
	public Vector3 OldPosition;

	[NonSerialized]
	public Camera Camera;

	public static SgtObserver Find(Camera camera)
	{
		for (int num = AllObservers.Count - 1; num >= 0; num--)
		{
			SgtObserver sgtObserver = AllObservers[num];
			if (sgtObserver.Camera == camera)
			{
				return sgtObserver;
			}
		}
		return null;
	}

	protected virtual void OnEnable()
	{
		AllObservers.Add(this);
		OldRotation = transform.rotation;
		OldPosition = transform.position;
		if (Camera == null)
		{
			Camera = GetComponent<Camera>();
		}
	}

	protected virtual void OnPreCull()
	{
		if (OnObserverPreCull != null)
		{
			OnObserverPreCull(this);
		}
	}

	protected virtual void OnPreRender()
	{
		if (OnObserverPreRender != null)
		{
			OnObserverPreRender(this);
		}
	}

	protected virtual void OnPostRender()
	{
		if (OnObserverPostRender != null)
		{
			OnObserverPostRender(this);
		}
	}

	protected virtual void LateUpdate()
	{
		Quaternion rotation = transform.rotation;
		Vector3 position = transform.position;
		Quaternion quaternion = Quaternion.Inverse(OldRotation) * rotation;
		Vector3 vector = OldPosition - position;
		OldRotation = rotation;
		OldPosition = position;
		RollAngle = (RollAngle - quaternion.eulerAngles.z) % 360f;
		RollQuaternion = Quaternion.Euler(0f, 0f, RollAngle);
		RollMatrix = SgtHelper.Rotation(RollQuaternion);
		DeltaPosition = vector;
		Velocity = SgtHelper.Reciprocal(Time.deltaTime) * vector;
	}

	protected virtual void OnDisable()
	{
		AllObservers.Remove(this);
	}
}
