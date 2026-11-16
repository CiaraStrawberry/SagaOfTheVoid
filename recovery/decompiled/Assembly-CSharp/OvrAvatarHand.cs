using UnityEngine;

public class OvrAvatarHand : MonoBehaviour, IAvatarPart
{
	private float alpha = 1f;

	public void UpdatePose(OvrAvatarDriver.ControllerPose pose)
	{
	}

	public void SetAlpha(float alpha)
	{
		this.alpha = alpha;
	}

	public void OnAssetsLoaded()
	{
		SetAlpha(alpha);
	}
}
