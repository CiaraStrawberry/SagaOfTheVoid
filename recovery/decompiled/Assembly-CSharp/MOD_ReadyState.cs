using System;
using System.Collections.Generic;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using DFNetwork.Simulation;
using DFNetwork.Simulation.Modules;
using UnityEngine;

[Serializable]
public class MOD_ReadyState : ScriptableObject, iModule
{
	private Dictionary<short, bool> PlayersReady = new Dictionary<short, bool>();

	private short MinimumPlayers = 1;

	private aNetworkLayer NetLayer;

	public string ModuleName => "Ready State";

	public string[] UsesPackets => new string[1] { "Ready" };

	public bool AllPlayersReady
	{
		get
		{
			foreach (bool value in PlayersReady.Values)
			{
				if (!value)
				{
					return false;
				}
			}
			return MinimumPlayers <= NetLayer.GetPlayerList().Length;
		}
	}

	public void Initialize(aNetworkLayer netLayer, SimulationLayer simLayer)
	{
		NetLayer = netLayer;
		NetLayer.OnRoomJoined += OnRoomJoined;
		NetLayer.OnRoomLeft += OnRoomLeft;
		NetLayer.OnPlayerConnected += PlayerConnect;
		NetLayer.OnPlayerDisconnected += PlayerDisconnected;
	}

	public bool ProcessCommand(Packet cmd)
	{
		if (!PlayersReady.ContainsKey(cmd.Owner))
		{
			PlayersReady.Add(cmd.Owner, (bool)cmd.Params[0]);
		}
		else
		{
			PlayersReady[cmd.Owner] = (bool)cmd.Params[0];
		}
		return AllPlayersReady;
	}

	private void PlayerConnect(short ID)
	{
		Debug.Log("Player Connected: " + ID);
		PlayersReady.Add(ID, value: false);
	}

	private void PlayerDisconnected(short ID)
	{
		PlayersReady.Remove(ID);
	}

	private void OnRoomJoined()
	{
		PlayersReady.Clear();
		short[] playerList = NetLayer.GetPlayerList();
		foreach (short num in playerList)
		{
			if (!PlayersReady.ContainsKey(num))
			{
				Debug.Log("Player Connected: " + num);
				PlayersReady.Add(num, value: false);
			}
		}
	}

	private void OnRoomLeft()
	{
		PlayersReady.Clear();
	}

	public void SendReady(bool ready)
	{
		NetLayer.SendPacket(NetLayer.GeneratePacket("Ready", ready));
	}

	public bool GetReadyStateForPlayer(short ID)
	{
		return PlayersReady.ContainsKey(ID) && PlayersReady[ID];
	}
}
