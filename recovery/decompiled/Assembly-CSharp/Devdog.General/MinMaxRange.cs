using System;
using UnityEngine;

namespace Devdog.General;

[Serializable]
public struct MinMaxRange
{
	public int min;

	public int max;

	public int Generate()
	{
		return UnityEngine.Random.Range(min, max);
	}
}
