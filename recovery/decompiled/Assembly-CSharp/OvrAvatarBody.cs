using System.Collections.Generic;
using UnityEngine;

public class OvrAvatarBody : MonoBehaviour, IAvatarPart
{
	private float alpha = 1f;

	private void Start()
	{
	}

	public virtual void UpdatePose(float voiceAmplitude)
	{
	}

	public void SetAlpha(float alpha)
	{
		this.alpha = alpha;
		List<Renderer> list = new List<Renderer>();
		gameObject.GetComponentsInChildren(includeInactive: true, list);
		foreach (Renderer item in list)
		{
			Material[] sharedMaterials = item.sharedMaterials;
			foreach (Material material in sharedMaterials)
			{
				material.SetFloat("_Alpha", alpha);
			}
		}
	}

	public void OnAssetsLoaded()
	{
		SetAlpha(alpha);
	}
}
