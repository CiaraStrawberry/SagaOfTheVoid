using System;
using UnityEngine;

[ExecuteInEditMode]
[DisallowMultipleComponent]
[AddComponentMenu("Space Graphics Toolkit/SGT Simple Orbit")]
public class SgtSimpleOrbit : MonoBehaviour
{
	[Tooltip("The radius of the orbit in local coordinates")]
	public float Radius = 1f;

	[Tooltip("How squashed the orbit is")]
	[Range(0f, 1f)]
	public float Oblateness;

	[Tooltip("The local position offset of the orbit")]
	public Vector3 Center;

	[Tooltip("The curent position along the orbit in degrees")]
	public float Angle;

	[Tooltip("The orbit speed")]
	public float DegreesPerSecond = 10f;

	protected virtual void Update()
	{
		if (Application.isPlaying)
		{
			Angle += DegreesPerSecond * Time.deltaTime;
		}
		float radius = Radius;
		float num = Radius * (1f - Oblateness);
		Vector3 center = Center;
		center.x += Mathf.Sin(Angle * ((float)Math.PI / 180f)) * radius;
		center.z += Mathf.Cos(Angle * ((float)Math.PI / 180f)) * num;
		SgtHelper.SetLocalPosition(transform, center);
	}
}
