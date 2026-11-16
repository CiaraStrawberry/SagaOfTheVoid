using UnityEngine;

namespace VolumetricFogAndMist;

[ExecuteInEditMode]
[RequireComponent(typeof(Camera), typeof(VolumetricFog))]
public class VolumetricFogPosT : MonoBehaviour, IVolumetricFogRenderComponent
{
	public VolumetricFog fog { get; set; }

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		if (fog != null)
		{
			fog.DoOnRenderImage(source, destination);
		}
	}

	public void DestroySelf()
	{
		Object.DestroyImmediate(this);
	}
}
