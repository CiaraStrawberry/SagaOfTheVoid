using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.FullSerializer;
using UnityEngine;

namespace Devdog.General;

public class fsUnityEngineObjectConverter : fsConverter
{
	private bool IsAssetWrapper(Type type)
	{
		if (typeof(IAsset).IsAssignableFrom(type))
		{
			return true;
		}
		return false;
	}

	public override object CreateInstance(fsData data, Type storageType)
	{
		return null;
	}

	public override bool CanProcess(Type type)
	{
		if (IsAssetWrapper(type))
		{
			return true;
		}
		if (JsonSerializer.currentRootType.IsAssignableFrom(type))
		{
			return false;
		}
		if (typeof(UnityEngine.Object).IsAssignableFrom(type))
		{
			return true;
		}
		return false;
	}

	public override bool RequestCycleSupport(Type storageType)
	{
		return false;
	}

	public override bool RequestInheritanceSupport(Type storageType)
	{
		return false;
	}

	protected virtual UnityEngine.Object GetUnityEngineObject(object instance)
	{
		UnityEngine.Object result = null;
		IAsset asset = instance as IAsset;
		if (asset != null)
		{
			result = asset.objectVal;
		}
		if (asset == null)
		{
			result = instance as UnityEngine.Object;
		}
		return result;
	}

	public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
	{
		List<UnityEngine.Object> list = Serializer.Context.Get<List<UnityEngine.Object>>();
		UnityEngine.Object unityEngineObject = GetUnityEngineObject(instance);
		int num = list.IndexOf(unityEngineObject);
		if (num == -1)
		{
			num = list.Count;
			list.Add(unityEngineObject);
		}
		serialized = new fsData(num);
		return fsResult.Success;
	}

	public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
	{
		try
		{
			List<UnityEngine.Object> list = Serializer.Context.Get<List<UnityEngine.Object>>();
			if (!data.IsNull)
			{
				int num = (int)data.AsInt64;
				if (num == -1 || num >= list.Count)
				{
					DevdogLogger.LogError(string.Concat("Couldn't deserialize UnityEngine.Object : ", instance, " - not found in database. (index: ", num, ")"));
					return fsResult.Fail("Index out of range " + num);
				}
				if (IsAssetWrapper(storageType))
				{
					Type typeFromHandle = typeof(Asset<>);
					Type type = typeFromHandle.MakeGenericType(storageType.GetGenericArguments()[0]);
					IAsset asset = (IAsset)Activator.CreateInstance(type);
					asset.objectVal = list[num];
					instance = asset;
				}
				else if (typeof(UnityEngine.Object).IsAssignableFrom(storageType))
				{
					instance = list[num];
				}
			}
			else
			{
				instance = null;
			}
		}
		catch (Exception ex)
		{
			DevdogLogger.LogError(ex.Message + "\n" + ex.StackTrace);
			return fsResult.Fail(ex.Message);
		}
		return fsResult.Success;
	}
}
