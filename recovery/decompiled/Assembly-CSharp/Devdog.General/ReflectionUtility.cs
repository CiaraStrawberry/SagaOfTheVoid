using System;
using System.Collections.Generic;
using System.Reflection;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General;

public static class ReflectionUtility
{
	public static Type[] GetAllClassesWithAttribute(Type attribute, bool inherit = true)
	{
		Type[] allTypesThatImplement = GetAllTypesThatImplement(typeof(object));
		return allTypesThatImplement.Where((Type o) => o.GetCustomAttributes(attribute, inherit).Length > 0).ToArray();
	}

	public static FieldInfo[] GetFieldsWithAttributeInherited(object obj, Type attribute)
	{
		List<FieldInfo> list = new List<FieldInfo>();
		GetAllFieldsInherited(obj.GetType(), list);
		List<FieldInfo> list2 = new List<FieldInfo>();
		FieldInfo[] array = list.Where((FieldInfo o) => o.GetCustomAttributes(attribute, inherit: true).Length > 0).ToArray();
		foreach (FieldInfo item in array)
		{
			list2.Add(item);
		}
		return list2.ToArray();
	}

	public static void GetAllSerializableFieldsInherited(Type startType, List<FieldInfo> appendList)
	{
		GetAllFieldsInherited(startType, appendList);
		appendList.RemoveAll((FieldInfo o) => ((o.IsPrivate || o.IsFamily) && o.GetCustomAttributes(typeof(SerializeField), inherit: true).Length == 0 && o.GetCustomAttributes(typeof(CustomSerializationAttribute), inherit: true).Length == 0) || o.GetCustomAttributes(typeof(IgnoreCustomSerializationAttribute), inherit: true).Length > 0 || o.GetCustomAttributes(typeof(NonSerializedAttribute), inherit: true).Length > 0 || o.GetCustomAttributes(typeof(HideInInspector), inherit: true).Length > 0);
	}

	public static void GetAllFieldsInherited(Type startType, List<FieldInfo> appendList)
	{
		GetAllFieldsInherited(startType, appendList, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public static void GetAllFieldsInherited(Type startType, List<FieldInfo> appendList, BindingFlags flags)
	{
		if (startType == typeof(MonoBehaviour) || startType == null || startType == typeof(object))
		{
			return;
		}
		FieldInfo[] fields = startType.GetFields(flags);
		FieldInfo[] array = fields;
		foreach (FieldInfo fieldInfo in array)
		{
			if (!appendList.Any((FieldInfo o) => o.Name == fieldInfo.Name))
			{
				appendList.Add(fieldInfo);
			}
		}
		GetAllFieldsInherited(startType.BaseType, appendList, flags);
	}

	public static FieldInfo GetFieldInherited(Type startType, string fieldName)
	{
		if (startType == typeof(MonoBehaviour) || startType == null || startType == typeof(object))
		{
			return null;
		}
		FieldInfo field = startType.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
		if (field != null)
		{
			return field;
		}
		return GetFieldInherited(startType.BaseType, fieldName);
	}

	public static MethodInfo GetMethodByName(Type type, string methodName)
	{
		return GetAllMethodsFromType(type).FirstOrDefault((MethodInfo o) => o.Name == methodName);
	}

	public static MethodInfo[] GetAllMethodsFromType(Type type)
	{
		return GetAllMethodsFromType(type, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
	}

	public static MethodInfo[] GetAllMethodsFromType(Type type, BindingFlags flags)
	{
		return type.GetMethods(flags);
	}

	public static Type[] GetAllTypesThatImplement(Type type)
	{
		return GetAllTypesThatImplement(type, creatableTypesOnly: true);
	}

	public static Type[] GetAllTypesThatImplement(Type type, bool creatableTypesOnly)
	{
		Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
		List<Assembly> list = new List<Assembly>();
		Assembly[] array = assemblies;
		foreach (Assembly assembly in array)
		{
			if (!assembly.FullName.StartsWith("Mono.Cecil") && !assembly.FullName.StartsWith("UnityScript") && !assembly.FullName.StartsWith("Boo.Lan") && !assembly.FullName.StartsWith("System") && !assembly.FullName.StartsWith("JetBrains") && !assembly.FullName.StartsWith("nunit") && !assembly.FullName.StartsWith("NUnit") && !assembly.FullName.StartsWith("I18N") && !assembly.FullName.StartsWith("UnityEditor") && !assembly.FullName.StartsWith("mscorlib"))
			{
				list.Add(assembly);
			}
		}
		IEnumerable<Type> source = list.SelectMany((Assembly o) => o.GetTypes());
		source = source.Where((Type o) => type.IsAssignableFrom(o));
		if (creatableTypesOnly)
		{
			source = source.Where((Type o) => !o.IsAbstract && !o.IsInterface);
		}
		return source.ToArray();
	}

	public static void CopySerializableValues(object from, object to)
	{
		List<FieldInfo> list = new List<FieldInfo>();
		GetAllSerializableFieldsInherited(from.GetType(), list);
		List<FieldInfo> list2 = new List<FieldInfo>();
		GetAllSerializableFieldsInherited(to.GetType(), list2);
		foreach (FieldInfo fromField in list)
		{
			FieldInfo fieldInfo = list2.FirstOrDefault((FieldInfo o) => o.Name == fromField.Name);
			if (fieldInfo != null)
			{
				try
				{
					fieldInfo.SetValue(to, fromField.GetValue(from));
				}
				catch (Exception)
				{
				}
			}
		}
	}

	public static bool IsBuiltInUnityObjectType(Type type)
	{
		return type.Namespace != null && type.Name.Contains("UnityEngine");
	}
}
