using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class BetterMonoBehaviour : MonoBehaviour, ISerializationCallbackReceiver
{
	[SerializeField]
	[IgnoreCustomSerialization]
	private string _serializedJsonString = "{}";

	[SerializeField]
	[IgnoreCustomSerialization]
	private List<UnityEngine.Object> _objectReferences;

	[NonSerialized]
	private readonly BetterSerializationModel _serializer = new BetterSerializationModel();

	public virtual void Save()
	{
		_serializer.Save(ref _objectReferences, ref _serializedJsonString, this);
	}

	public virtual void Load()
	{
		_serializer.Load(ref _objectReferences, ref _serializedJsonString, this);
	}

	public void OnBeforeSerialize()
	{
		Save();
	}

	public void OnAfterDeserialize()
	{
		Load();
	}
}
