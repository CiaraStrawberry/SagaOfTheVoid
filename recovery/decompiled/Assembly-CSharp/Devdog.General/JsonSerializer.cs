using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.FullSerializer;
using Devdog.General.ThirdParty.FullSerializer.Internal;
using UnityEngine;

namespace Devdog.General;

public static class JsonSerializer
{
	private static readonly fsSerializer _serializer;

	private static readonly object _lockObject;

	private static readonly fsUnityEngineObjectConverter _engineObjectConvertor;

	public static Type currentRootType { get; private set; }

	static JsonSerializer()
	{
		_lockObject = new object();
		_serializer = new fsSerializer();
		_serializer.Config = new fsConfig
		{
			SerializeEnumsAsInteger = true,
			IgnoreSerializeAttributes = new Type[3]
			{
				typeof(IgnoreCustomSerializationAttribute),
				typeof(NonSerializedAttribute),
				typeof(fsIgnoreAttribute)
			},
			SerializeAttributes = new Type[3]
			{
				typeof(CustomSerializationAttribute),
				typeof(SerializeField),
				typeof(fsPropertyAttribute)
			}
		};
		_serializer.RemoveProcessor<fsSerializationCallbackReceiverProcessor>();
		_engineObjectConvertor = new fsUnityEngineObjectConverter();
		_serializer.AddConverter(_engineObjectConvertor);
	}

	private static void SetObjectReferences(List<UnityEngine.Object> objectReferences)
	{
		if (objectReferences != null)
		{
			_serializer.Context.Set(objectReferences);
		}
	}

	public static string Serialize<T>(T value, List<UnityEngine.Object> objectReferences)
	{
		return Serialize(value, typeof(T), objectReferences);
	}

	public static string Serialize(object value, Type type, List<UnityEngine.Object> objectReferences)
	{
		lock (_lockObject)
		{
			try
			{
				SetObjectReferences(objectReferences);
				currentRootType = type;
				_serializer.TrySerialize(type, value, out var data).AssertSuccessWithoutWarnings();
				return fsJsonPrinter.CompressedJson(data);
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat("Couldn't serialize type ", type, " - ", ex.Message, "\n", ex.StackTrace));
			}
		}
		return null;
	}

	public static void DeserializeTo(object obj, Type type, string json, List<UnityEngine.Object> objectReferences)
	{
		DeserializeTo(ref obj, type, json, objectReferences);
	}

	public static void DeserializeTo(ref object obj, Type type, string json, List<UnityEngine.Object> objectReferences)
	{
		lock (_lockObject)
		{
			try
			{
				fsData data = fsJsonParser.Parse(json);
				SetObjectReferences(objectReferences);
				currentRootType = type;
				_serializer.TryDeserialize(data, type, ref obj).AssertSuccessWithoutWarnings();
			}
			catch (Exception ex)
			{
				DevdogLogger.LogError(ex.Message + "\n" + ex.StackTrace);
			}
		}
	}

	public static void DeserializeTo<T>(T obj, string json, List<UnityEngine.Object> objectReferences)
	{
		DeserializeTo(ref obj, json, objectReferences);
	}

	public static void DeserializeTo<T>(ref T obj, string json, List<UnityEngine.Object> objectReferences)
	{
		lock (_lockObject)
		{
			try
			{
				fsData data = fsJsonParser.Parse(json);
				SetObjectReferences(objectReferences);
				currentRootType = typeof(T);
				_serializer.TryDeserialize(data, ref obj).AssertSuccessWithoutWarnings();
			}
			catch (Exception ex)
			{
				Debug.LogError(string.Concat("Couldn't deserialize type ", typeof(T), " - ", ex.Message));
			}
		}
	}
}
