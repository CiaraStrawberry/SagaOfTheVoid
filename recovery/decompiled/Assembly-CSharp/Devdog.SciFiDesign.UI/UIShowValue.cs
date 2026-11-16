using System;
using Devdog.General;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

[Serializable]
public class UIShowValue
{
	[Header("Text")]
	public Text textField;

	public string textFormat = "{0}/{1}";

	public int roundToDecimals = 1;

	public bool clearTextWhenZero = true;

	[Header("Slider")]
	public Slider slider;

	public bool hideSliderWhenZero;

	[Header("Image fill")]
	public Image imageFill;

	[Header("Audio")]
	public AudioClipInfo activationClip;

	public void Repaint(float current, float max)
	{
		if (textField != null)
		{
			if (current < 0.0001f && clearTextWhenZero)
			{
				textField.text = string.Empty;
				SetActive(textField, set: false);
			}
			else
			{
				textField.text = string.Format(textFormat, Math.Round(current, roundToDecimals), Math.Round(max, roundToDecimals), Math.Round(current / max, roundToDecimals));
				SetActive(textField, set: true);
			}
		}
		if (slider != null)
		{
			slider.minValue = 0f;
			slider.maxValue = max;
			if (current <= 0.0001f)
			{
				if (hideSliderWhenZero)
				{
					SetActive(slider, set: false);
				}
				else
				{
					SetActive(slider, set: true);
				}
			}
			else
			{
				SetActive(slider, set: true);
			}
			if (current != slider.value)
			{
				slider.value = current;
			}
		}
		if (imageFill != null)
		{
			float num = current / max;
			SetActive(imageFill, num > 0.001f);
			if (num != imageFill.fillAmount)
			{
				imageFill.fillAmount = num;
			}
		}
	}

	private void SetActive(MonoBehaviour b, bool set)
	{
		if (b.enabled != set)
		{
			b.enabled = set;
		}
	}

	public void HideAll()
	{
		if (textField != null)
		{
			SetActive(textField, set: false);
		}
		if (slider != null)
		{
			SetActive(slider, set: false);
		}
		if (imageFill != null)
		{
			SetActive(imageFill, set: false);
		}
	}

	public void Activate()
	{
		AudioManager.AudioPlayOneShot(activationClip);
	}
}
