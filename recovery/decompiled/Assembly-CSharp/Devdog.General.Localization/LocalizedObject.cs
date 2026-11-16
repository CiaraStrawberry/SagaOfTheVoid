using System;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
public class LocalizedObject : LocalizedObjectBase<UnityEngine.Object>
{
	public LocalizedObject()
	{
	}

	public LocalizedObject(string key)
		: base(key)
	{
	}
}
