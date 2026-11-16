using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DFNetwork.Helpers;
using UnityEngine;

namespace DFNetwork.Networking.PacketHandling;

public class PacketTypeRegistry
{
	internal struct PacketHandler(short commandID, Type objectType, iPacketHandler instance, Func<object[], byte[]> serialize, Func<byte[], object[]> deserialize)
	{
		public short CommandID = commandID;

		public Type ObjectType = objectType;

		public iPacketHandler Instance = instance;

		public Func<object[], byte[]> Serialize = serialize;

		public Func<byte[], object[]> Deserialize = deserialize;
	}

	private Dictionary<string, PacketHandler> _packetHandlers = new Dictionary<string, PacketHandler>();

	private static object _LockObject = new object();

	private static PacketTypeRegistry _instance;

	public static PacketTypeRegistry GetInstance
	{
		get
		{
			lock (_LockObject)
			{
				if (_instance == null)
				{
					_instance = new PacketTypeRegistry();
				}
			}
			return _instance;
		}
	}

	public List<string> GetCommandNameList => _packetHandlers.Select((KeyValuePair<string, PacketHandler> x) => x.Key).ToList();

	private PacketTypeRegistry()
	{
	}

	public void RefreshCache()
	{
		_packetHandlers.Clear();
		Type[] array = (from m in Assembly.GetAssembly(typeof(iPacketHandler)).GetTypes()
			where typeof(iPacketHandler).IsAssignableFrom(m)
			select m).ToArray();
		Type[] array2 = array;
		foreach (Type t in array2)
		{
			if (t.Name != "iPacketHandler" && _packetHandlers.Where((KeyValuePair<string, PacketHandler> x) => x.Key == t.Name).Count() == 0)
			{
				RegisterCommand((iPacketHandler)ScriptableObject.CreateInstance(t));
			}
		}
		_packetHandlers = _packetHandlers.OrderBy((KeyValuePair<string, PacketHandler> key) => key.Key).ToDictionary((KeyValuePair<string, PacketHandler> x) => x.Key, (KeyValuePair<string, PacketHandler> y) => y.Value);
	}

	public void RegisterCommand(iPacketHandler packet)
	{
		PacketHandler value = new PacketHandler((short)_packetHandlers.Count, packet.GetType(), packet, packet.Serialize, packet.Deserialize);
		_packetHandlers.Add(packet.CommandName, value);
	}

	internal PacketHandler GetCommandByName(string name)
	{
		if (!_packetHandlers.ContainsKey(name))
		{
			throw new IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")");
		}
		return _packetHandlers[name];
	}

	public short GetCommandID(string name)
	{
		if (!_packetHandlers.ContainsKey(name))
		{
			throw new IndexOutOfRangeException("Make sure the module you are trying to access is enabled (" + name + ")");
		}
		return _packetHandlers[name].CommandID;
	}

	public string GetCommandName(short id)
	{
		foreach (KeyValuePair<string, PacketHandler> packetHandler in _packetHandlers)
		{
			if (packetHandler.Value.CommandID == id)
			{
				return packetHandler.Key;
			}
		}
		return string.Empty;
	}

	public bool CommandExists(short id)
	{
		bool result = false;
		foreach (KeyValuePair<string, PacketHandler> packetHandler in _packetHandlers)
		{
			if (packetHandler.Value.CommandID == id)
			{
				result = true;
			}
		}
		return result;
	}

	public bool CommandExists(string name)
	{
		return _packetHandlers.ContainsKey(name);
	}

	public byte[] SerializePacket(Packet cmd)
	{
		if (!CommandExists(cmd.CommandName))
		{
			throw new MissingReferenceException("Packet type \"" + cmd.CommandName + "\" does not exist, please make sure the module is enabled");
		}
		Serializer serializer = new Serializer();
		serializer.Serialize();
		serializer.AddBits(cmd.PacketType);
		serializer.AddBits(cmd.Owner);
		byte[] array = serializer.ToArray();
		byte[] array2 = _packetHandlers[cmd.CommandName].Serialize(cmd.Params);
		byte[] array3 = new byte[array.Length + array2.Length];
		array.CopyTo(array3, 0);
		array2.CopyTo(array3, array.Length);
		return array3;
	}

	public Packet DeserializePacket(byte[] data)
	{
		Deserializer deserializer = new Deserializer(data);
		short num = deserializer.ReadNextShort();
		if (!CommandExists(num))
		{
			throw new MissingReferenceException("Packet type \"" + num + "\" does not exist, please make sure the module is enabled");
		}
		string commandName = GetCommandName(num);
		short owner = deserializer.ReadNextShort();
		byte[] array = new byte[data.Length - deserializer.index];
		array = data.Skip(deserializer.index).Take(data.Length - deserializer.index).ToArray();
		object[] param = _packetHandlers[commandName].Deserialize(array);
		return new Packet(commandName, owner, param);
	}
}
