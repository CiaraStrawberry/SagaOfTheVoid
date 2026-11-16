using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Gravity Source")]
public class SgtGravitySource : MonoBehaviour
{
	public static List<SgtGravitySource> AllGravitySources = new List<SgtGravitySource>();

	[Tooltip("The mass of this gravity source (automatically set if there is a Rigidbody)")]
	public float Mass = 100f;

	[NonSerialized]
	private Rigidbody body;

	protected virtual void OnEnable()
	{
		AllGravitySources.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllGravitySources.Remove(this);
	}

	protected virtual void Update()
	{
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
		if (body != null)
		{
			Mass = body.mass;
		}
	}
}
