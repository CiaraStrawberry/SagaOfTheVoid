using System;
using DFNetwork.Networking.PacketHandling;
using UnityEngine;

namespace DFNetwork.Networking;

public abstract class aNetworkLayer : MonoBehaviour
{
	private bool isConnected;

	public abstract short GetCurrentPlayerID { get; }

	public bool IsConnected => isConnected;

	public PacketTypeRegistry PacketRegistry => PacketTypeRegistry.GetInstance;

	public event Action<Packet> OnPacketSent;

	public event Action<Packet> OnPacketArrived;

	public event Action OnNetworkConnect;

	public event Action OnNetworkDisconnect;

	public event Action<short> OnPlayerConnected;

	public event Action<short> OnPlayerDisconnected;

	public event Action OnRoomJoined;

	public event Action OnRoomLeft;

	public abstract void DoNetworkConnect();

	public abstract void DoNetworkDisconnect();

	public abstract bool IsHost();

	public abstract short GetPlayerID(string name);

	public abstract short[] GetPlayerList();

	public abstract string GetPlayerName(short ID);

	public abstract Packet GeneratePacket(short CommandID, params object[] param);

	public abstract Packet GeneratePacket(string CommandName, params object[] param);

	public abstract void SendPacket(Packet packet);

	public void RaiseOnNetworkConnect()
	{
		if (OnNetworkConnect != null)
		{
			isConnected = true;
			OnNetworkConnect();
		}
	}

	public void RaiseOnNetworkDisconnect()
	{
		if (OnNetworkDisconnect != null)
		{
			isConnected = false;
			OnNetworkDisconnect();
		}
	}

	public void RaiseOnPlayerConnected(short id)
	{
		if (OnPlayerConnected != null)
		{
			OnPlayerConnected(id);
		}
	}

	public void RaiseOnPlayerDisconnected(short id)
	{
		if (OnPlayerDisconnected != null)
		{
			OnPlayerDisconnected(id);
		}
	}

	public void RaiseOnRoomJoined()
	{
		if (OnRoomJoined != null)
		{
			OnRoomJoined();
		}
	}

	public void RaiseOnRoomLeft()
	{
		if (OnRoomLeft != null)
		{
			OnRoomLeft();
		}
	}

	public virtual void Awake()
	{
		PacketRegistry.RefreshCache();
	}

	public virtual void Start()
	{
		PacketRegistry.RefreshCache();
	}

	public byte[] ProcessOutboundPacket(Packet packet)
	{
		if (OnPacketSent != null)
		{
			OnPacketSent(packet);
		}
		return PacketRegistry.SerializePacket(packet);
	}

	public void ProcessIncommingPacket(byte[] data)
	{
		Packet obj = PacketRegistry.DeserializePacket(data);
		if (OnPacketArrived != null)
		{
			OnPacketArrived(obj);
		}
	}
}
