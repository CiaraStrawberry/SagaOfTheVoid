using System.Collections.Generic;
using Devdog.General.Localization;
using UnityEngine;

namespace Devdog.General;

public abstract class IdentifieableScriptableObject : ScriptableObject, IEqualityComparer<IdentifieableScriptableObject>
{
	public abstract int ID { get; protected set; }

	public new abstract LocalizedString name { get; protected set; }

	public bool Equals(IdentifieableScriptableObject x, IdentifieableScriptableObject y)
	{
		return x == y;
	}

	public int GetHashCode(IdentifieableScriptableObject obj)
	{
		return obj.GetHashCode();
	}
}
