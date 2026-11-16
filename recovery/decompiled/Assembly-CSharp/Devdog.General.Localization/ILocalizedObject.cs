using UnityEngine;

namespace Devdog.General.Localization;

public interface ILocalizedObject<T> : ILocalizedObject where T : Object
{
	T val { get; set; }
}
public interface ILocalizedObject
{
	Object objectVal { get; set; }
}
