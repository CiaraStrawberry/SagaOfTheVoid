using System;
using DFNetwork.Helpers;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

public class packet_SpawnObjects : ScriptableObject, iPacketHandler
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

	public string CommandName => "SpawnObjects";

	public void Initialize()
	{
	}

	public void OnEditorGUI()
	{
	}

	public byte[] Serialize(params object[] param)
	{
		if (param.Length == 0 || !(param[0] is short) || !(param[1] is Vector3) || !(param[2] is Quaternion))
		{
			throw new InvalidCastException("This module requires a short sent as param[0] and a vector3 sent as param[1] and a quaternion as param[2]");
		}
		Serializer serializer = new Serializer();
		serializer.AddBits((short)param[0]);
		serializer.AddBits((Vector3)param[1]);
		serializer.AddBits((Quaternion)param[2]);
		serializer.AddBits((int)param[3]);
		return serializer.ToArray();
	}

	public object[] Deserialize(byte[] data)
	{
		object[] array = new object[4];
		Deserializer deserializer = new Deserializer(data);
		array[0] = deserializer.ReadNextShort();
		array[1] = deserializer.ReadNextVector3();
		array[2] = deserializer.ReadNextQuaternion();
		array[3] = deserializer.ReadNextInt();
		return array;
	}
}
