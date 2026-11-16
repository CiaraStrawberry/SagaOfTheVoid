using System;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
public class LocalizedSprite : LocalizedObjectBase<Sprite>
{
	public LocalizedSprite()
	{
	}

	public LocalizedSprite(string key)
		: base(key)
	{
	}
}
