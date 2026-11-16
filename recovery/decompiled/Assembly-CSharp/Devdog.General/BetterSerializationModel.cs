using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class BetterSerializationModel
{
	[NonSerialized]
	private bool _isSerializing;

	public void Save(ref List<UnityEngine.Object> objectReferences, ref string json, UnityEngine.Object obj)
	{
		if (!_isSerializing && !Application.isPlaying)
		{
			_isSerializing = true;
			objectReferences = new List<UnityEngine.Object>();
			json = JsonSerializer.Serialize(obj, obj.GetType(), objectReferences);
			_isSerializing = false;
		}
	}

	public void Load(ref List<UnityEngine.Object> objectReferences, ref string json, UnityEngine.Object obj)
	{
		if (!_isSerializing)
		{
			_isSerializing = true;
			JsonSerializer.DeserializeTo(obj, obj.GetType(), json, objectReferences);
			_isSerializing = false;
		}
	}
}
