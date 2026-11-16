using System.Collections;
using UnityEngine;

namespace VolumetricFogAndMist;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera), typeof(VolumetricFog))]
public class VolumetricFogPreT : MonoBehaviour, IVolumetricFogRenderComponent
{
	public VolumetricFog fog { get; set; }

	private void OnPreRender()
	{
		if (fog != null)
		{
			fog.DoOnPreRender();
		}
	}

	[ImageEffectOpaque]
	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (fog != null)
		{
			fog.DoOnRenderImage(source, destination);
		}
	}

	private IEnumerator OnPostRender()
	{
		if (fog != null && fog.improveTransparency)
		{
			yield return new WaitForEndOfFrame();
			if (fog != null)
			{
				fog.DoOnPostRender();
			}
		}
	}

	public void DestroySelf()
	{
		Object.DestroyImmediate(this);
	}
}
