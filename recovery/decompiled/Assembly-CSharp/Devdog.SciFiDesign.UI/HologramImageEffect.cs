using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign.UI;

[ExecuteInEditMode]
public class HologramImageEffect : MonoBehaviour
{
	[Required]
	public Shader hologramShader;

	[Required]
	public Texture2D hologramTiledImage;

	private Material _hologramMaterial;

	protected virtual void CheckResources()
	{
		if (hologramShader == null)
		{
			hologramShader = Shader.Find("Hidden/HologramImageEffect");
		}
		if (!hologramShader.isSupported)
		{
			Debug.LogWarning("Shader " + hologramShader.name + " is not supported on this platform.", gameObject);
		}
		if (_hologramMaterial == null)
		{
			_hologramMaterial = new Material(hologramShader);
			_hologramMaterial.hideFlags = HideFlags.DontSave;
			_hologramMaterial.SetTexture("_HologramTexture", hologramTiledImage);
		}
	}

	private void OnRenderImage(RenderTexture source, RenderTexture destination)
	{
		CheckResources();
		Graphics.Blit(source, destination, _hologramMaterial);
	}
}
