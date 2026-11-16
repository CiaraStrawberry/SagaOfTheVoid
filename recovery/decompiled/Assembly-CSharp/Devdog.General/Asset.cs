using System;
using UnityEngine;

namespace Devdog.General;

[Serializable]
public class Asset<T> : IAsset where T : UnityEngine.Object
{
	public T val;

	public UnityEngine.Object objectVal
	{
		get
		{
			return val;
		}
		set
		{
			val = (T)value;
		}
	}

	public Asset()
	{
	}

	public Asset(T val)
	{
		this.val = val;
	}
}
