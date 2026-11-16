using System;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using DFNetwork.Simulation;
using DFNetwork.Simulation.Modules;
using UnityEngine;

[Serializable]
public class MOD_MoveObjects : ScriptableObject, iModule
{
	public string ModuleName => "Move Objects";

	public string[] UsesPackets => new string[1] { "Ready" };

	public void Initialize(aNetworkLayer netLayer, SimulationLayer simLayer)
	{
	}

	public bool ProcessCommand(Packet cmd)
	{
		return false;
	}
}
