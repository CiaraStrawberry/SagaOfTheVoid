using System;
using DFNetwork.Helpers;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

public class packet_StartGame : ScriptableObject, iPacketHandler
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

	public string CommandName => "StartGame";

	public void Initialize()
	{
	}

	public void OnEditorGUI()
	{
	}

	public byte[] Serialize(params object[] param)
	{
		if (param.Length == 0 || !(param[0] is int))
		{
			throw new InvalidCastException("This module requires no params");
		}
		Serializer serializer = new Serializer();
		serializer.AddBits((int)param[0]);
		return serializer.ToArray();
	}

	public object[] Deserialize(byte[] data)
	{
		object[] array = new object[1];
		Deserializer deserializer = new Deserializer(data);
		array[0] = deserializer.ReadNextInt();
		return array;
	}
}
