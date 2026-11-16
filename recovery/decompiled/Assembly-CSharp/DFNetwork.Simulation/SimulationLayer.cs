using System;
using System.Collections.Generic;
using System.Linq;
using DFNetwork.Debugging;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using DFNetwork.Simulation.Modules;
using EasyEditor;
using UnityEngine;

namespace DFNetwork.Simulation;

[Serializable]
public class SimulationLayer : MonoBehaviour
{
	public Action OnSimulationStart;

	public Action OnSimulationStop;

	[Inspector(displayHeader = true, foldable = true, group = "Simulation Settings", groupDescription = "Simulation settings for the system")]
	[SerializeField]
	private bool simdebugging;

	[Inspector(displayHeader = true, foldable = true, group = "Simulation Settings")]
	[SerializeField]
	private bool showLogToConsole;

	[HideInInspector]
	[SerializeField]
	private List<ResourceItem> _Resources = new List<ResourceItem>();

	private Dictionary<string, List<string>> PacketHandlingGraph = new Dictionary<string, List<string>>();

	private aNetworkLayer netLayer;

	public bool Debugging
	{
		get
		{
			return simdebugging;
		}
		set
		{
			simdebugging = value;
		}
	}

	public bool ShowLogToConsole
	{
		get
		{
			return showLogToConsole;
		}
		set
		{
			showLogToConsole = value;
		}
	}

	public List<ResourceItem> Resources
	{
		get
		{
			return _Resources;
		}
		set
		{
			_Resources = value;
		}
	}

	public ModuleTypeRegistry ModuleRegistry => ModuleTypeRegistry.GetInstance;

	private void Awake()
	{
		ModuleRegistry.RefreshCache();
	}

	private void Start()
	{
		if (gameObject.GetComponent<aNetworkLayer>() == null)
		{
			throw new MissingComponentException("The game object \"" + gameObject.name + "\" needs to have an implementation of aNetworkLayer");
		}
		netLayer = gameObject.GetComponent<aNetworkLayer>();
		netLayer.OnPacketArrived += PacketArrived;
		ModuleRegistry.RefreshCache();
		ModuleRegistry.GetAllModules.ForEach((iModule n) =>
		{
			Logging.AddLogItem("Sim Layer", "Registering module handler " + n.ModuleName);
			n.Initialize(netLayer, this);
			string[] usesPackets = n.UsesPackets;
			foreach (string text in usesPackets)
			{
				if (!PacketHandlingGraph.ContainsKey(text))
				{
					PacketHandlingGraph.Add(text, new List<string>());
				}
				Logging.AddLogItem("Sim Layer", "Adding handler for packet: " + text + " and module: " + n.ModuleName);
				PacketHandlingGraph[text].Add(n.ModuleName);
			}
		});
	}

	public bool ContainsResource(string prefab)
	{
		return Enumerable.Count(_Resources, (ResourceItem x) => x.PrefabName == prefab) > 0;
	}

	public bool ContainsResource(short prefabID)
	{
		return Enumerable.Count(_Resources, (ResourceItem x) => x.PrefabID == prefabID) > 0;
	}

	public short GetResourceID(string prefab)
	{
		if (Enumerable.Count(_Resources, (ResourceItem x) => x.PrefabName == prefab) == 0)
		{
			Debug.Log(prefab);
			throw new KeyNotFoundException("Prefab does not exist in the list, refresh the list?");
		}
		return _Resources.Where((ResourceItem x) => x.PrefabName == prefab).First().PrefabID;
	}

	public string GetResourcePrefab(short id)
	{
		if (Enumerable.Count(_Resources, (ResourceItem x) => x.PrefabID == id) == 0)
		{
			throw new KeyNotFoundException("Prefab does not exist in the list, refresh the list?");
		}
		return _Resources.Where((ResourceItem x) => x.PrefabID == id).First().PrefabName;
	}

	private void PacketArrived(Packet packet)
	{
		if (simdebugging)
		{
			Logging.AddLogItem("Sim Layer|Packet Arrived", "Owner: " + packet.Owner + ", command: " + packet.CommandName);
		}
		if (!PacketHandlingGraph.ContainsKey(packet.CommandName))
		{
			return;
		}
		foreach (string item in PacketHandlingGraph[packet.CommandName])
		{
			ModuleRegistry.GetModuleByName(item).ProcessCommand(packet);
		}
	}

	private void OnApplicationQuit()
	{
		Logging.AddLogItem("Simulation", "Ending Simulation");
	}
}
