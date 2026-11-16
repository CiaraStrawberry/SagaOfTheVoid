using UnityEngine;

namespace Forge3D;

public class F3DWeaponRangeLightController : MonoBehaviour
{
	public int ControllerID;

	public Light[] LightSources;

	private float[] LightSourcesValues;

	public MeshRenderer[] Lights;

	private float[] LightValues;

	private void Awake()
	{
		if (LightSources.Length > 0)
		{
			LightSourcesValues = new float[LightSources.Length];
			for (int i = 0; i < LightSources.Length; i++)
			{
				LightSourcesValues[i] = LightSources[i].intensity;
			}
		}
		if (Lights.Length > 0)
		{
			LightValues = new float[Lights.Length];
			for (int j = 0; j < Lights.Length; j++)
			{
				LightValues[j] = Lights[j].material.GetFloat("_HDRGlow");
			}
		}
	}

	public void SwitchLights(bool state)
	{
		if (state)
		{
			for (int i = 0; i < LightSources.Length; i++)
			{
				LightSources[i].intensity = LightSourcesValues[i];
			}
			for (int j = 0; j < Lights.Length; j++)
			{
				Lights[j].material.SetFloat("_HDRGlow", LightValues[j]);
			}
		}
		else
		{
			for (int k = 0; k < LightSources.Length; k++)
			{
				LightSources[k].intensity = 0f;
			}
			for (int l = 0; l < Lights.Length; l++)
			{
				Lights[l].material.SetFloat("_HDRGlow", 0f);
			}
		}
	}
}
