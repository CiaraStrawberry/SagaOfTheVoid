using System;
using UnityEngine;

namespace Devdog.General;

[Serializable]
public struct FMinMaxRange
{
	public float min;

	public float max;

	public float Generate()
	{
		return UnityEngine.Random.Range(min, max);
	}
}
