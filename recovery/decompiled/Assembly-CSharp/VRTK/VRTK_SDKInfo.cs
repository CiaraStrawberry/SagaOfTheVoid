using System;
using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

[Serializable]
public sealed class VRTK_SDKInfo : ISerializationCallbackReceiver
{
	[SerializeField]
	private string baseTypeName;

	[SerializeField]
	private string fallbackTypeName;

	[SerializeField]
	private string typeName;

	[SerializeField]
	private int descriptionIndex;

	public Type type { get; private set; }

	public string originalTypeNameWhenFallbackIsUsed { get; private set; }

	public SDK_DescriptionAttribute description { get; private set; }

	private VRTK_SDKInfo()
	{
	}

	public VRTK_SDKInfo(VRTK_SDKInfo infoToCopy)
	{
		SetUp(Type.GetType(infoToCopy.baseTypeName), Type.GetType(infoToCopy.fallbackTypeName), infoToCopy.typeName, infoToCopy.descriptionIndex);
	}

	public static VRTK_SDKInfo[] Create<BaseType, FallbackType, ActualType>() where BaseType : SDK_Base where FallbackType : BaseType where ActualType : BaseType
	{
		return Create<BaseType, FallbackType>(typeof(ActualType));
	}

	public static VRTK_SDKInfo[] Create<BaseType, FallbackType>(Type actualType) where BaseType : SDK_Base where FallbackType : BaseType
	{
		string fullName = actualType.FullName;
		SDK_DescriptionAttribute[] descriptions = SDK_DescriptionAttribute.GetDescriptions(actualType);
		if (descriptions.Length == 0)
		{
			VRTK_Logger.Fatal($"'{fullName}' doesn't specify any SDK descriptions via '{typeof(SDK_DescriptionAttribute).Name}'.");
			return new VRTK_SDKInfo[0];
		}
		List<VRTK_SDKInfo> list = new List<VRTK_SDKInfo>(descriptions.Length);
		SDK_DescriptionAttribute[] array = descriptions;
		foreach (SDK_DescriptionAttribute sDK_DescriptionAttribute in array)
		{
			VRTK_SDKInfo vRTK_SDKInfo = new VRTK_SDKInfo();
			vRTK_SDKInfo.SetUp(typeof(BaseType), typeof(FallbackType), fullName, sDK_DescriptionAttribute.index);
			list.Add(vRTK_SDKInfo);
		}
		return list.ToArray();
	}

	private void SetUp(Type baseType, Type fallbackType, string actualTypeName, int descriptionIndex)
	{
		if (baseType == null || fallbackType == null)
		{
			return;
		}
		if (!baseType.IsSubclassOf(typeof(SDK_Base)))
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("baseType", baseType, $"'{baseType.Name}' is not a subclass of the SDK base type '{typeof(SDK_Base).Name}'."));
			return;
		}
		if (!fallbackType.IsSubclassOf(baseType))
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("fallbackType", fallbackType, $"'{fallbackType.Name}' is not a subclass of the SDK base type '{baseType.Name}'."));
			return;
		}
		baseTypeName = baseType.FullName;
		fallbackTypeName = fallbackType.FullName;
		typeName = actualTypeName;
		if (string.IsNullOrEmpty(actualTypeName))
		{
			this.type = fallbackType;
			originalTypeNameWhenFallbackIsUsed = null;
			this.descriptionIndex = -1;
			description = new SDK_DescriptionAttribute(typeof(SDK_FallbackSystem));
			return;
		}
		Type type = Type.GetType(actualTypeName);
		if (type == null)
		{
			this.type = fallbackType;
			originalTypeNameWhenFallbackIsUsed = actualTypeName;
			this.descriptionIndex = -1;
			description = new SDK_DescriptionAttribute(typeof(SDK_FallbackSystem));
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_NOT_FOUND, actualTypeName, fallbackType.Name));
		}
		else if (!type.IsSubclassOf(baseType))
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("actualTypeName", actualTypeName, $"'{actualTypeName}' is not a subclass of the SDK base type '{baseType.Name}'."));
		}
		else
		{
			SDK_DescriptionAttribute[] descriptions = SDK_DescriptionAttribute.GetDescriptions(type);
			if (descriptions.Length <= descriptionIndex)
			{
				VRTK_Logger.Fatal(new ArgumentOutOfRangeException("descriptionIndex", descriptionIndex, $"'{actualTypeName}' has no '{typeof(SDK_DescriptionAttribute).Name}' at that index."));
				return;
			}
			this.type = type;
			originalTypeNameWhenFallbackIsUsed = null;
			this.descriptionIndex = descriptionIndex;
			description = descriptions[descriptionIndex];
		}
	}

	public void OnBeforeSerialize()
	{
	}

	public void OnAfterDeserialize()
	{
		SetUp(Type.GetType(baseTypeName), Type.GetType(fallbackTypeName), typeName, descriptionIndex);
	}

	public override bool Equals(object obj)
	{
		if (!(obj is VRTK_SDKInfo vRTK_SDKInfo))
		{
			return false;
		}
		return this == vRTK_SDKInfo;
	}

	public bool Equals(VRTK_SDKInfo other)
	{
		return this == other;
	}

	public override int GetHashCode()
	{
		return type.GetHashCode();
	}

	public static bool operator ==(VRTK_SDKInfo x, VRTK_SDKInfo y)
	{
		if (object.ReferenceEquals(x, y))
		{
			return true;
		}
		if ((object)x == null || (object)y == null)
		{
			return false;
		}
		return x.type == y.type && x.descriptionIndex == y.descriptionIndex;
	}

	public static bool operator !=(VRTK_SDKInfo x, VRTK_SDKInfo y)
	{
		return !(x == y);
	}
}
