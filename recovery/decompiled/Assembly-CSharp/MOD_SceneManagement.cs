using System;
using System.Collections.Generic;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using DFNetwork.Simulation;
using DFNetwork.Simulation.Modules;
using UnityEngine;

[Serializable]
public class MOD_SceneManagement : ScriptableObject, iModule
{
	private Dictionary<short, bool> PlayersLoaded = new Dictionary<short, bool>();

	private aNetworkLayer NetLayer;

	[SerializeField]
	private List<string> availableMaps = new List<string>();

	public string ModuleName => "Scene Management";

	public string[] UsesPackets => new string[2] { "LoadScene", "SceneLoaded" };

	public List<string> AvailableMaps => availableMaps;

	public bool AllPlayersLoaded
	{
		get
		{
			foreach (bool value in PlayersLoaded.Values)
			{
				if (!value)
				{
					return value;
				}
			}
			return true;
		}
	}

	public event Action OnMasterInstructLoad;

	public void Initialize(aNetworkLayer netLayer, SimulationLayer simLayer)
	{
		NetLayer = netLayer;
	}

	public bool ProcessCommand(Packet cmd)
	{
		if (cmd.CommandName == "LoadScene")
		{
			if (OnMasterInstructLoad != null)
			{
				OnMasterInstructLoad();
			}
			short[] playerList = NetLayer.GetPlayerList();
			foreach (short key in playerList)
			{
				PlayersLoaded.Add(key, value: false);
			}
			Application.LoadLevelAdditive((string)cmd.Params[0]);
			NetLayer.SendPacket(NetLayer.GeneratePacket("SceneLoaded", (string)cmd.Params[0]));
		}
		else
		{
			PlayersLoaded[cmd.Owner] = true;
		}
		return true;
	}

	public void SendLevelToLoad(string name)
	{
		NetLayer.SendPacket(NetLayer.GeneratePacket("LoadScene", name));
	}

	public bool GetPlayerStillLoading(short ID)
	{
		return PlayersLoaded[ID];
	}
}
