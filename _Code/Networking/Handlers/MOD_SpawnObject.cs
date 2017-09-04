using UnityEngine;
using TrueSync;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections;

/// <summary>
/// Mod_SpawnObject.  This class is used to spawn objects in the game
/// </summary>
[System.Serializable()]
public class MOD_SpawnObject : ScriptableObject, DFNetwork.Simulation.Modules.iModule {

    /// <summary>
    /// Game controller
    /// </summary>
    private GameController _gc;

    /// <summary>
    /// The net layer.
    /// </summary>
    DFNetwork.Networking.aNetworkLayer NetLayer;

	/// <summary>
	/// The sim layer.
	/// </summary>
	DFNetwork.Simulation.SimulationLayer SimLayer;

	/// <summary>
	/// Gets the name of the module.
	/// </summary>
	/// <value>The name of the module.</value>
	public string ModuleName {
		get {
			return "Spawn Object";
		}
	}

	/// <summary>
	/// Return the packets that this module will use
	/// </summary>
	/// <value>The uses packets.</value>
	public string[] UsesPackets {
		get {
			return new string[]{"SpawnObjects"};					
		}
	}

	/// <summary>
	/// Initialize this instance.
	/// </summary>
	/// <param name="netLayer">Net layer.</param>
	/// <param name="simLayer">Sim layer.</param>
	public void Initialize (DFNetwork.Networking.aNetworkLayer netLayer, DFNetwork.Simulation.SimulationLayer simLayer)
	{

        _gc = GameObject.FindObjectOfType<GameController>();
        NetLayer = netLayer;
		SimLayer = simLayer;

	}

	/// <summary>
	/// Processes one command, used for instant processing
	/// </summary>
	/// <returns>The command.</returns>
	/// <param name="cmd">Cmd.</param>
	public bool ProcessCommand (DFNetwork.Networking.PacketHandling.Packet cmd)
	{

        string prefab = SimLayer.GetResourcePrefab((short)cmd.Params[0]);

        // Create the object based on the ID.  This should require a listing somewhere
        GameObject go = LoadResource(((short)cmd.Params[0]), ((Vector3)cmd.Params[1]), ((Quaternion)cmd.Params[2]));
        go.GetComponent<PhotonView>().viewID = (int)cmd.Params[3];

        // Set parent
        if (_gc.WorldContainerObject)
        {
        go.transform.SetParent(_gc.WorldContainerObject.transform);
        go.transform.localScale = Vector3.one;
        }
        


        return false;
        
	}	

	/// <summary>
	/// Sends the spawn object packet
	/// </summary>
	/// <param name="id">Identifier.</param>
	/// <param name="pos">Position.</param>
	/// <param name="rot">Rot.</param>
	public void SendSpawnObject(string prefabName, Vector3 pos, Quaternion rot, int viewID) {

        Debug.Log(NetLayer);
        Debug.Log(SimLayer.GetResourceID(prefabName));
         NetLayer.SendPacket( NetLayer.GeneratePacket("SpawnObjects", SimLayer.GetResourceID(prefabName), pos, rot, viewID) );

    }

    /// <summary>
    /// Loads a resource.
    /// </summary>
    /// <returns>The resource.</returns>
    /// <param name="id">Identifier.</param>
    public GameObject LoadResource(short id, Vector3 pos, Quaternion rot) {

        // Check if the object exists
        if (!SimLayer.ContainsResource(id)) {
            DFNetwork.Debugging.Logging.AddLogItem("Resource Loading", "Resource ID \"" + id + "\" does not exist, maybe you forgot to refresh the list?");
        }

        //Debug.Log("Loading resource \"" + id + "\" with prefab \"" + GetResourcePrefab(id) + "\"");
        //  if(TrueSyncManager.SyncedStartCoroutine)
        GameObject go = null;
        if(SimLayer.GetResourcePrefab(id) != null)
        {
            go= GameObject.Instantiate(Resources.Load(SimLayer.GetResourcePrefab(id)) as GameObject, pos, rot);
          go.GetComponent<TSTransform>().position = pos.ToTSVector();
        }


        return go;

    }

}
