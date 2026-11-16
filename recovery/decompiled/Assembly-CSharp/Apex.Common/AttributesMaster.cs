using System;
using System.Linq;
using System.Reflection;

namespace Apex.Common;

public static class AttributesMaster
{
	public static bool attributesEnabled { get; private set; }

	public static Type attributesEnumType { get; private set; }

	public static int highestOrderBit { get; private set; }

	static AttributesMaster()
	{
		Refresh();
	}

	public static void Refresh()
	{
		Type markerAttribute = typeof(EntityAttributesEnumAttribute);
		Assembly assembly = markerAttribute.Assembly;
		attributesEnumType = (from t in assembly.GetTypes()
			where t.IsEnum && Attribute.IsDefined(t, markerAttribute) && t != typeof(DefaultEntityAttributesEnum)
			select t).FirstOrDefault();
		if (attributesEnumType == null)
		{
			attributesEnabled = false;
			highestOrderBit = 0;
			attributesEnumType = typeof(DefaultEntityAttributesEnum);
		}
		else
		{
			Array values = Enum.GetValues(attributesEnumType);
			highestOrderBit = (int)Math.Log((int)values.GetValue(values.Length - 1), 2.0);
			attributesEnabled = true;
		}
	}
}
