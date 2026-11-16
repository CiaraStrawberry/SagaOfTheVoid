using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Sphere")]
public class SgtSphere : SgtShape
{
	[Tooltip("The radius of this sphere in local coordinates")]
	public float Radius = 1f;

	[Tooltip("The transtion style between minimum and maximum density")]
	public SgtEase.Type Ease = SgtEase.Type.Smoothstep;

	[Tooltip("How quickly the density increases when inside the sphere")]
	public float Power = 2f;

	public override float GetDensity(Vector3 worldPoint)
	{
		Vector3 vector = transform.InverseTransformPoint(worldPoint);
		float t = Mathf.InverseLerp(Radius, 0f, vector.magnitude);
		return SgtHelper.Pow(SgtEase.Evaluate(Ease, t), Power);
	}

	public static SgtSphere CreateSphere(int layer = 0, Transform parent = null)
	{
		return CreateSphere(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtSphere CreateSphere(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Sphere", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtSphere>();
	}
}
