using System;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Thruster Scale")]
public class SgtThrusterScale : MonoBehaviour
{
	[Tooltip("The thruster the scale will be based on")]
	public SgtThruster Thruster;

	[Tooltip("The speed at which the scale reaches its target value")]
	public float Dampening = 10f;

	[Tooltip("The scale value that's applied by default")]
	public Vector3 BaseScale;

	[Tooltip("The scale value that's added when the throttle is 1")]
	public Vector3 ThrottleScale = Vector3.one;

	[Tooltip("The amount the ThrottleScale flickers over time")]
	[Range(0f, 1f)]
	public float Flicker = 0.1f;

	[Tooltip("The offset of the flicker animation")]
	public float FlickerOffset;

	[Tooltip("The speed of the flicker animation")]
	public float FlickerSpeed = 5f;

	[SerializeField]
	private float throttle;

	[NonSerialized]
	private float[] points;

	protected virtual void Start()
	{
		if (Thruster == null)
		{
			Thruster = GetComponentInParent<SgtThruster>();
		}
	}

	protected virtual void Update()
	{
		if (!(Thruster != null))
		{
			return;
		}
		if (Application.isPlaying)
		{
			FlickerOffset += FlickerSpeed * Time.deltaTime;
		}
		if (points == null)
		{
			points = new float[128];
			for (int num = points.Length - 1; num >= 0; num--)
			{
				points[num] = UnityEngine.Random.value;
			}
		}
		float num2 = Mathf.Repeat(FlickerOffset, points.Length);
		int num3 = (int)num2;
		float t = num2 % 1f;
		float a = points[num3];
		float b = points[(num3 + 1) % points.Length];
		float c = points[(num3 + 2) % points.Length];
		float d = points[(num3 + 3) % points.Length];
		float num4 = 1f - SgtHelper.CubicInterpolate(a, b, c, d, t) * Flicker;
		throttle = SgtHelper.Dampen(throttle, Thruster.Throttle, Dampening, Time.deltaTime);
		transform.localScale = BaseScale + ThrottleScale * throttle * num4;
	}
}
