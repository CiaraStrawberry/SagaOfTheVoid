using UnityEngine;
using UnityEngine.Serialization;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Ring Shadow")]
public class SgtRingShadow : SgtShadow
{
	[Tooltip("The texture of the shadow (left = inside, right = outside)")]
	public Texture Texture;

	[Tooltip("The ring that this shadow is being cast from")]
	public SgtRingMesh RingMesh;

	[Tooltip("The inner radius of the ring casting this shadow (auto set if Ring is set)")]
	[FormerlySerializedAs("InnerRadius")]
	public float RadiusMin = 1f;

	[Tooltip("The outer radius of the ring casting this shadow (auto set if Ring is set)")]
	[FormerlySerializedAs("OuterRadius")]
	public float RadiusMax = 2f;

	public override Texture GetTexture()
	{
		return Texture;
	}

	public override bool CalculateShadow(ref Matrix4x4 matrix, ref float ratio)
	{
		if (base.CalculateShadow(ref matrix, ref ratio) && Texture != null)
		{
			if (SgtHelper.Enabled(RingMesh))
			{
				RadiusMin = RingMesh.RadiusMin;
				RadiusMax = RingMesh.RadiusMax;
			}
			Vector3 direction = default;
			Vector3 position = default;
			Color color = default;
			SgtHelper.CalculateLight(Light, transform.position, null, null, ref position, ref direction, ref color);
			Quaternion quaternion = Quaternion.FromToRotation(direction, Vector3.back);
			float num = Vector3.Dot(direction, transform.up);
			float x = transform.lossyScale.x * RadiusMax;
			float num2 = transform.lossyScale.z * RadiusMax;
			Vector3 vector = quaternion * transform.up;
			Quaternion quaternion2 = Quaternion.LookRotation(Vector3.forward, new Vector2(0f - vector.x, vector.y));
			Vector3 xyz = SgtHelper.Reciprocal3(new Vector3(x, num2 * Mathf.Abs(num), 1f));
			float y = Mathf.Tan(SgtHelper.Acos(0f - num));
			Matrix4x4 matrix4x = SgtHelper.Translation(-transform.position);
			Matrix4x4 matrix4x2 = SgtHelper.Rotation(quaternion2 * quaternion);
			Matrix4x4 matrix4x3 = SgtHelper.Scaling(xyz);
			Matrix4x4 matrix4x4 = SgtHelper.ShearingZ(new Vector2(0f, y));
			matrix = matrix4x3 * matrix4x4 * matrix4x2 * matrix4x;
			ratio = SgtHelper.Divide(RadiusMax, RadiusMax - RadiusMin);
			return true;
		}
		return false;
	}
}
