using UnityEngine;

public abstract class SgtShadow : MonoBehaviour
{
	[Tooltip("The light that this shadow is being cast away from")]
	public Light Light;

	public abstract Texture GetTexture();

	protected virtual void Start()
	{
	}

	public virtual bool CalculateShadow(ref Matrix4x4 matrix, ref float ratio)
	{
		if (SgtHelper.Enabled(Light) && Light.intensity > 0f)
		{
			return true;
		}
		return false;
	}
}
