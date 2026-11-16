using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundGradientC
{
	public Gradient gradient = new Gradient();

	public void CopyTo(PlaygroundGradientC copy)
	{
		copy.gradient.SetKeys(gradient.colorKeys, gradient.alphaKeys);
	}
}
