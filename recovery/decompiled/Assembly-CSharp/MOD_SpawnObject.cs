using System;
using DFNetwork.Debugging;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using DFNetwork.Simulation;
using DFNetwork.Simulation.Modules;
using TrueSync;
using UnityEngine;

[Serializable]
public class MOD_SpawnObject : ScriptableObject, iModule
{
	private GameController _gc;

	private aNetworkLayer NetLayer;

	private SimulationLayer SimLayer;

	public string ModuleName => "Spawn Object";

	public string[] UsesPackets => new string[1] { "SpawnObjects" };

	public void Initialize(aNetworkLayer netLayer, SimulationLayer simLayer)
	{
		_gc = UnityEngine.Object.FindObjectOfType<GameController>();
		NetLayer = netLayer;
		SimLayer = simLayer;
	}

	public bool ProcessCommand(Packet cmd)
	{
		string resourcePrefab = SimLayer.GetResourcePrefab((short)cmd.Params[0]);
		GameObject gameObject = LoadResource((short)cmd.Params[0], (Vector3)cmd.Params[1], (Quaternion)cmd.Params[2]);
		gameObject.GetComponent<PhotonView>().viewID = (int)cmd.Params[3];
		if ((bool)_gc.WorldContainerObject)
		{
			gameObject.transform.SetParent(_gc.WorldContainerObject.transform);
			gameObject.transform.localScale = Vector3.one;
		}
		return false;
	}

	public void SendSpawnObject(string prefabName, Vector3 pos, Quaternion rot, int viewID)
	{
		Debug.Log(NetLayer);
		Debug.Log(SimLayer.GetResourceID(prefabName));
		NetLayer.SendPacket(NetLayer.GeneratePacket("SpawnObjects", SimLayer.GetResourceID(prefabName), pos, rot, viewID));
	}

	public GameObject LoadResource(short id, Vector3 pos, Quaternion rot)
	{
		if (!SimLayer.ContainsResource(id))
		{
			Logging.AddLogItem("Resource Loading", "Resource ID \"" + id + "\" does not exist, maybe you forgot to refresh the list?");
		}
		GameObject gameObject = null;
		if (SimLayer.GetResourcePrefab(id) != null)
		{
			gameObject = UnityEngine.Object.Instantiate(Resources.Load(SimLayer.GetResourcePrefab(id)) as GameObject, pos, rot);
			gameObject.GetComponent<TSTransform>().position = pos.ToTSVector();
		}
		return gameObject;
	}
}
