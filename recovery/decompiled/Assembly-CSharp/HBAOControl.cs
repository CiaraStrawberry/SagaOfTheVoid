using UnityEngine;
using UnityEngine.UI;

public class HBAOControl : MonoBehaviour
{
	public HBAO hbao;

	public Slider aoRadiusSlider;

	public void ToggleShowAO()
	{
		if (hbao.generalSettings.displayMode != HBAO.DisplayMode.Normal)
		{
			HBAO.GeneralSettings generalSettings = hbao.generalSettings;
			generalSettings.displayMode = HBAO.DisplayMode.Normal;
			hbao.generalSettings = generalSettings;
		}
		else
		{
			HBAO.GeneralSettings generalSettings2 = hbao.generalSettings;
			generalSettings2.displayMode = HBAO.DisplayMode.AOOnly;
			hbao.generalSettings = generalSettings2;
		}
	}

	public void UpdateAoRadius()
	{
		HBAO.AOSettings aoSettings = hbao.aoSettings;
		aoSettings.radius = aoRadiusSlider.value;
		hbao.aoSettings = aoSettings;
	}
}
