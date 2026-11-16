using ExitGames.UtilityScripts;
using Photon;
using UnityEngine;

public class ColorPerPlayerApply : PunBehaviour
{
	private static ColorPerPlayer colorPickerCache;

	private Renderer rendererComponent;

	public void Awake()
	{
		if (colorPickerCache == null)
		{
			colorPickerCache = Object.FindObjectOfType<ColorPerPlayer>();
		}
		if (colorPickerCache == null)
		{
			enabled = false;
		}
		if (photonView.isSceneView)
		{
			enabled = false;
		}
		rendererComponent = GetComponent<Renderer>();
	}

	public override void OnPhotonInstantiate(PhotonMessageInfo info)
	{
		ApplyColor();
	}

	public void ApplyColor()
	{
		if (photonView.owner != null)
		{
			rendererComponent.material.color = colorPickerCache.Colors[photonView.owner.GetRoomIndex()];
		}
	}
}
