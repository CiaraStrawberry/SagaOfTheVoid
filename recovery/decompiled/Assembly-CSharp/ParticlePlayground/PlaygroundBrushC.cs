using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundBrushC
{
	public Texture2D texture;

	public float scale = 1f;

	public BRUSHDETAILC detail = BRUSHDETAILC.High;

	public float distance = 10000f;

	[HideInInspector]
	public Color32[] color;

	[HideInInspector]
	public int colorLength;

	public void SetTexture(Texture2D newTexture)
	{
		texture = newTexture;
		Construct();
	}

	public void Construct()
	{
		color = PlaygroundC.GetPixels(texture);
		if (color != null)
		{
			colorLength = color.Length;
		}
	}

	public Color32 GetColor(int index)
	{
		index %= colorLength;
		return color[index];
	}

	public void SetColor(Color32 c, int index)
	{
		color[index] = c;
	}

	public PlaygroundBrushC Clone()
	{
		PlaygroundBrushC playgroundBrushC = new PlaygroundBrushC();
		playgroundBrushC.texture = texture;
		playgroundBrushC.scale = scale;
		playgroundBrushC.detail = detail;
		playgroundBrushC.distance = distance;
		if (playgroundBrushC.color != null)
		{
			playgroundBrushC.color = color.Clone() as Color32[];
		}
		playgroundBrushC.colorLength = colorLength;
		return playgroundBrushC;
	}
}
