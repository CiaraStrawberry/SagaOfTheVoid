using System;
using DFNetwork.Helpers;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

public class packet_LoadingProgress : ScriptableObject, iPacketHandler
{
	[SerializeField]
	private short id;

	public short ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string CommandName => "LoadingProgress";

	public void Initialize()
	{
	}

	public void OnEditorGUI()
	{
	}

	public byte[] Serialize(params object[] param)
	{
		if (param.Length == 0 || !(param[0] is float))
		{
			throw new InvalidCastException("This module requires a float sent as param[0]");
		}
		Serializer serializer = new Serializer();
		serializer.AddBits((float)param[0]);
		return serializer.ToArray();
	}

	public object[] Deserialize(byte[] data)
	{
		object[] array = new object[0];
		Deserializer deserializer = new Deserializer(data);
		array[0] = deserializer.ReadNextFloat();
		return array;
	}
}
