using System;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
public class LocalizedAudioClip : LocalizedObjectBase<AudioClip>
{
	public LocalizedAudioClip()
	{
	}

	public LocalizedAudioClip(string key)
		: base(key)
	{
	}
}
