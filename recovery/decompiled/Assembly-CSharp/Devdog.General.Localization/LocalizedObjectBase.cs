using UnityEngine;

namespace Devdog.General.Localization;

public abstract class LocalizedObjectBase<T> : ILocalizedObject<T>, ILocalizedObject where T : Object
{
	[SerializeField]
	private string _key;

	public Object objectVal
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

	[IgnoreCustomSerialization]
	public T val
	{
		get
		{
			return LocalizationManager.GetObject(_key, (T)null);
		}
		set
		{
			if (LocalizationManager.currentDatabase != null)
			{
				LocalizationManager.currentDatabase.SetObject(_key, value);
			}
		}
	}

	protected LocalizedObjectBase()
	{
	}

	protected LocalizedObjectBase(string key)
	{
		_key = key;
	}

	public override string ToString()
	{
		if (objectVal == null)
		{
			return "null";
		}
		return objectVal.ToString();
	}
}
