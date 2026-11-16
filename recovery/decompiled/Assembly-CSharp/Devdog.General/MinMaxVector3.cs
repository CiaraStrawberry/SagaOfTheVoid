using System;
using UnityEngine;

namespace Devdog.General;

[Serializable]
public struct MinMaxVector3
{
	public Vector3 min;

	public Vector3 max;

	public Vector3 Generate()
	{
		return new Vector3(UnityEngine.Random.Range(min.x, max.x), UnityEngine.Random.Range(min.y, max.y), UnityEngine.Random.Range(min.z, max.z));
	}
}
