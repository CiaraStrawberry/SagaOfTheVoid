using System;
using System.Linq;

namespace VRTK;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public sealed class SDK_DescriptionAttribute : Attribute
{
	public readonly string prettyName;

	public readonly string symbol;

	public readonly string vrDeviceName;

	public readonly int index;

	public bool describesFallbackSDK => prettyName == "Fallback";

	public SDK_DescriptionAttribute(string prettyName, string symbol, string vrDeviceName, string buildTargetGroupName, int index = 0)
	{
		if (prettyName == null)
		{
			VRTK_Logger.Fatal(new ArgumentNullException("prettyName"));
			return;
		}
		if (prettyName == string.Empty)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("prettyName", prettyName, "An empty string isn't allowed."));
			return;
		}
		this.prettyName = prettyName;
		this.symbol = symbol;
		this.vrDeviceName = ((!string.IsNullOrEmpty(vrDeviceName)) ? vrDeviceName : "None");
		this.index = index;
		if (string.IsNullOrEmpty(buildTargetGroupName))
		{
			buildTargetGroupName = "Unknown";
		}
	}

	public SDK_DescriptionAttribute(Type typeToCopyExistingDescriptionFrom, int index = 0)
	{
		if (typeToCopyExistingDescriptionFrom == null)
		{
			VRTK_Logger.Fatal(new ArgumentNullException("typeToCopyExistingDescriptionFrom"));
			return;
		}
		Type typeFromHandle = typeof(SDK_DescriptionAttribute);
		SDK_DescriptionAttribute[] descriptions = GetDescriptions(typeToCopyExistingDescriptionFrom);
		if (descriptions.Length == 0)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("typeToCopyExistingDescriptionFrom", typeToCopyExistingDescriptionFrom, $"'{typeToCopyExistingDescriptionFrom.Name}' doesn't specify any SDK descriptions via '{typeFromHandle.Name}' to copy."));
			return;
		}
		if (descriptions.Length <= index)
		{
			VRTK_Logger.Fatal(new ArgumentOutOfRangeException("index", index, $"'{typeToCopyExistingDescriptionFrom.Name}' has no '{typeFromHandle.Name}' at that index."));
			return;
		}
		SDK_DescriptionAttribute sDK_DescriptionAttribute = descriptions[index];
		prettyName = sDK_DescriptionAttribute.prettyName;
		symbol = sDK_DescriptionAttribute.symbol;
		vrDeviceName = sDK_DescriptionAttribute.vrDeviceName;
		this.index = index;
	}

	public static SDK_DescriptionAttribute[] GetDescriptions(Type type)
	{
		return (from SDK_DescriptionAttribute attribute in type.GetCustomAttributes(typeof(SDK_DescriptionAttribute), inherit: false)
			orderby attribute.index
			select attribute).ToArray();
	}
}
