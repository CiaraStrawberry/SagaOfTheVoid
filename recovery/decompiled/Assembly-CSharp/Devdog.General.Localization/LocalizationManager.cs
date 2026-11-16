using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General.Localization;

[ExecuteInEditMode]
public class LocalizationManager : ManagerBase<LocalizationManager>
{
	[SerializeField]
	[Required]
	private LocalizationDatabase _defaultDatabase;

	[SerializeField]
	private LocalizationDatabase[] _databases = new LocalizationDatabase[0];

	private static LocalizationDatabase _currentDatabase;

	private static string[] _availableLanguageNames = new string[0];

	public const string NoKeyConstant = "NO_KEY";

	public static LocalizationDatabase defaultDatabase
	{
		get
		{
			if (ManagerBase<LocalizationManager>.instance == null)
			{
				return null;
			}
			return ManagerBase<LocalizationManager>.instance._defaultDatabase;
		}
		set
		{
			if (ManagerBase<LocalizationManager>.instance == null)
			{
				DevdogLogger.LogError("Trying to set default database, but no manager instance was found (add the manager to the _Managers object!");
			}
			else
			{
				ManagerBase<LocalizationManager>.instance._defaultDatabase = value;
			}
		}
	}

	public static LocalizationDatabase currentDatabase => _currentDatabase;

	public static string currentLanguage
	{
		get
		{
			if (_currentDatabase != null)
			{
				return _currentDatabase.lang;
			}
			return string.Empty;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		defaultDatabase = _defaultDatabase;
		SetLanguage(defaultDatabase);
		ResetNoKeyOnDatabases();
		NotifyLanguageDatabasesChanged();
	}

	private void ResetNoKeyOnDatabases()
	{
		LocalizationDatabase[] databases = _databases;
		foreach (LocalizationDatabase localizationDatabase in databases)
		{
			if (localizationDatabase != null)
			{
				localizationDatabase.SetString("NO_KEY", string.Empty);
			}
		}
	}

	private static void NotifyLanguageDatabasesChanged()
	{
		UpdateAvailableLanguageNames();
	}

	private static void UpdateAvailableLanguageNames()
	{
		List<string> list = new List<string>();
		if (ManagerBase<LocalizationManager>.instance != null)
		{
			LocalizationDatabase[] databases = ManagerBase<LocalizationManager>.instance._databases;
			foreach (LocalizationDatabase localizationDatabase in databases)
			{
				if (!(localizationDatabase == null))
				{
					list.Add(localizationDatabase.lang);
				}
			}
		}
		_availableLanguageNames = list.ToArray();
	}

	public static string[] GetAvailableLanguageNames()
	{
		return _availableLanguageNames;
	}

	public static LocalizationDatabase[] GetAvailableLanguageDatabases()
	{
		if (ManagerBase<LocalizationManager>.instance != null)
		{
			return ManagerBase<LocalizationManager>.instance._databases;
		}
		return new LocalizationDatabase[0];
	}

	public static LocalizationDatabase FindLocalizationDatabaseByName(string lang)
	{
		if (ManagerBase<LocalizationManager>.instance == null)
		{
			return null;
		}
		for (int i = 0; i < ManagerBase<LocalizationManager>.instance._databases.Length; i++)
		{
			if (ManagerBase<LocalizationManager>.instance._databases[i] != null && ManagerBase<LocalizationManager>.instance._databases[i].lang == lang)
			{
				return ManagerBase<LocalizationManager>.instance._databases[i];
			}
		}
		return null;
	}

	public static void SetLanguage(string lang)
	{
		_currentDatabase = FindLocalizationDatabaseByName(lang);
		if (_currentDatabase == null)
		{
			DevdogLogger.LogWarning("Couldn't find localization database with language identifier \"" + lang + "\"");
		}
	}

	public static void SetLanguage(LocalizationDatabase db)
	{
		_currentDatabase = db;
	}

	public static string GetString(string key, string notFound = "")
	{
		if (currentDatabase != null && currentDatabase.ContainsString(key))
		{
			return currentDatabase.GetString(key, notFound);
		}
		if (defaultDatabase != null)
		{
			return defaultDatabase.GetString(key, notFound);
		}
		return notFound;
	}

	public static T GetObject<T>(string key, T notFound = null) where T : UnityEngine.Object
	{
		if (currentDatabase != null && currentDatabase.ContainsObject(key))
		{
			return currentDatabase.GetObject(key, notFound);
		}
		if (defaultDatabase != null)
		{
			return defaultDatabase.GetObject(key, notFound);
		}
		return notFound;
	}

	public static string CreateNewStringKey(string defaultValue = "")
	{
		string text = Guid.NewGuid().ToString();
		defaultDatabase.SetString(text, defaultValue);
		if (currentDatabase != null)
		{
			currentDatabase.SetString(text, defaultValue);
		}
		return text;
	}

	public static string CreateNewObjectKey<T>(T defaultValue = null) where T : UnityEngine.Object
	{
		string text = Guid.NewGuid().ToString();
		defaultDatabase.SetObject(text, defaultValue);
		if (currentDatabase != null)
		{
			currentDatabase.SetObject(text, defaultValue);
		}
		return text;
	}
}
