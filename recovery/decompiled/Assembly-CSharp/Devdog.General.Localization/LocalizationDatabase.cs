using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization;

[Serializable]
[CreateAssetMenu(menuName = "Devdog/Localization Database")]
public class LocalizationDatabase : BetterScriptableObject
{
	[SerializeField]
	private string _lang = "en-US";

	[SerializeField]
	private Dictionary<string, string> _localizedStrings = new Dictionary<string, string> { 
	{
		"NO_KEY",
		string.Empty
	} };

	[SerializeField]
	private Dictionary<string, UnityEngine.Object> _localizedObjects = new Dictionary<string, UnityEngine.Object>();

	public string lang
	{
		get
		{
			return _lang;
		}
		set
		{
			_lang = value;
		}
	}

	public bool ContainsString(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		return _localizedStrings.ContainsKey(key);
	}

	public string GetString(string key, string notFound = "")
	{
		if (string.IsNullOrEmpty(key))
		{
			return notFound;
		}
		if (_localizedStrings.TryGetValue(key, out var value))
		{
			return value;
		}
		return notFound;
	}

	public void SetString(string key, string msg)
	{
		if (!string.IsNullOrEmpty(key))
		{
			if (string.IsNullOrEmpty(msg))
			{
				DeleteString(key);
			}
			else
			{
				_localizedStrings[key] = msg;
			}
		}
	}

	public void DeleteString(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			_localizedStrings.Remove(key);
		}
	}

	public bool ContainsObject(string key)
	{
		if (string.IsNullOrEmpty(key))
		{
			return false;
		}
		return _localizedObjects.ContainsKey(key);
	}

	public T GetObject<T>(string key, T notFound = null) where T : UnityEngine.Object
	{
		if (string.IsNullOrEmpty(key))
		{
			return notFound;
		}
		if (_localizedObjects.TryGetValue(key, out var value))
		{
			return (T)value;
		}
		return notFound;
	}

	public void SetObject<T>(string key, T obj) where T : UnityEngine.Object
	{
		if (!string.IsNullOrEmpty(key))
		{
			if (obj == null)
			{
				DeleteObject(key);
			}
			else
			{
				_localizedObjects[key] = obj;
			}
		}
	}

	public void DeleteObject(string key)
	{
		if (!string.IsNullOrEmpty(key))
		{
			_localizedObjects.Remove(key);
		}
	}
}
