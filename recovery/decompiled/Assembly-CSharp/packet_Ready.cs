using System;
using DFNetwork.Helpers;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

public class packet_Ready : ScriptableObject, iPacketHandler
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

	public string CommandName => "Ready";

	public void Initialize()
	{
	}

	public void OnEditorGUI()
	{
	}

	public byte[] Serialize(params object[] param)
	{
		if (param.Length == 0 || !(param[0] is bool))
		{
			throw new InvalidCastException("This module requires a string sent as param[0]");
		}
		Serializer serializer = new Serializer();
		serializer.AddBits((bool)param[0]);
		return serializer.ToArray();
	}

	public object[] Deserialize(byte[] data)
	{
		object[] array = new object[1];
		Deserializer deserializer = new Deserializer(data);
		array[0] = deserializer.ReadNextBool();
		return array;
	}
}
