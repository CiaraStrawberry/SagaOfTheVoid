using System;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
public class LocalizedTexture2D : LocalizedObjectBase<Texture2D>
{
	public LocalizedTexture2D()
	{
	}

	public LocalizedTexture2D(string key)
		: base(key)
	{
	}
}
