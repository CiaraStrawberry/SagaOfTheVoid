using System;
using UnityEngine;

namespace VRTK;

[Serializable]
[AttributeUsage(AttributeTargets.Method, AllowMultiple = true, Inherited = false)]
public sealed class SDK_ScriptingDefineSymbolPredicateAttribute : Attribute, ISerializationCallbackReceiver
{
	public const string RemovableSymbolPrefix = "VRTK_DEFINE_";

	public string symbol;

	[SerializeField]
	private string buildTargetGroupName;

	private SDK_ScriptingDefineSymbolPredicateAttribute()
	{
	}

	public SDK_ScriptingDefineSymbolPredicateAttribute(string symbol, string buildTargetGroupName)
	{
		if (symbol == null)
		{
			VRTK_Logger.Fatal(new ArgumentNullException("symbol"));
			return;
		}
		if (symbol == string.Empty)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("symbol", symbol, "An empty string isn't allowed."));
			return;
		}
		this.symbol = symbol;
		if (buildTargetGroupName == null)
		{
			VRTK_Logger.Fatal(new ArgumentNullException("buildTargetGroupName"));
		}
		else if (buildTargetGroupName == string.Empty)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("buildTargetGroupName", buildTargetGroupName, "An empty string isn't allowed."));
		}
		else
		{
			SetBuildTarget(buildTargetGroupName);
		}
	}

	public SDK_ScriptingDefineSymbolPredicateAttribute(SDK_ScriptingDefineSymbolPredicateAttribute attributeToCopy)
	{
		symbol = attributeToCopy.symbol;
		SetBuildTarget(attributeToCopy.buildTargetGroupName);
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		SetBuildTarget(buildTargetGroupName);
	}

	private void SetBuildTarget(string groupName)
	{
		buildTargetGroupName = groupName;
	}
}
