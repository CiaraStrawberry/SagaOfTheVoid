using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Box")]
public class SgtBox : SgtShape
{
	[Tooltip("The min/max size of the cube")]
	public Vector3 Extents = Vector3.one;

	[Tooltip("The transtion style between minimum and maximum density")]
	public SgtEase.Type Ease = SgtEase.Type.Smoothstep;

	[Tooltip("How quickly the density increases when inside the sphere")]
	public float Power = 2f;

	public override float GetDensity(Vector3 worldPoint)
	{
		Vector3 vector = transform.InverseTransformPoint(worldPoint);
		float a = Mathf.InverseLerp(Extents.x, 0f, Mathf.Abs(vector.x));
		float a2 = Mathf.InverseLerp(Extents.y, 0f, Mathf.Abs(vector.y));
		float b = Mathf.InverseLerp(Extents.z, 0f, Mathf.Abs(vector.z));
		float t = Mathf.Min(a, Mathf.Min(a2, b));
		return SgtHelper.Pow(SgtEase.Evaluate(Ease, t), Power);
	}

	public static SgtBox CreateBox(int layer = 0, Transform parent = null)
	{
		return CreateBox(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtBox CreateBox(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Cube", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtBox>();
	}
}
