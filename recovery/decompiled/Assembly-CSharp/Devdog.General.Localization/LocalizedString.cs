using System;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
public class LocalizedString
{
	[SerializeField]
	private string _key = "NO_KEY";

	[IgnoreCustomSerialization]
	public string message
	{
		get
		{
			return LocalizationManager.GetString(_key, string.Empty);
		}
		set
		{
			if (LocalizationManager.currentDatabase != null)
			{
				LocalizationManager.currentDatabase.SetString(_key, value);
			}
		}
	}

	public LocalizedString()
	{
	}

	public LocalizedString(string key)
	{
		_key = key;
	}

	public override string ToString()
	{
		return message;
	}
}
