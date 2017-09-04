using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR

using UnityEditor;

#endif

/// <summary>
/// Instructs the users to load a scene, is also handed a random number
/// </summary>
[System.Serializable()]
public class MOD_SceneManagement : ScriptableObject, DFNetwork.Simulation.Modules.iModule {
	
	/// <summary>
	/// Occurs when on scene load.
	/// </summary>
	public event System.Action OnMasterInstructLoad;
	
	/// <summary>
	/// The players that have loaded the map.
	/// </summary>
	private Dictionary<short, bool> PlayersLoaded = new Dictionary<short, bool>();
	
	#region Public Properties
	
	/// <summary>
	/// Friendly name of this module
	/// </summary>
	/// <value>
	/// The name of the module.
	/// </value>
	public string ModuleName {
		get {
			return "Scene Management";
		}
	}
	
	/// <summary>
	/// Packets that this module uses
	/// </summary>
	/// <value>
	/// The uses packets.
	/// </value>
	public string[] UsesPackets {
		get {
			return new string[]{"LoadScene", "SceneLoaded"};					
		}
	}
	
	/// <summary>
	/// The net layer.
	/// </summary>
	DFNetwork.Networking.aNetworkLayer NetLayer;
	
	[SerializeField]
	private List<string> availableMaps = new List<string>();
	/// <summary>
	/// Gets a list of available maps.
	/// </summary>
	/// <value>
	/// The available maps.
	/// </value>
	public List<string> AvailableMaps {
		get { return availableMaps; }
	}
	
	#endregion
	
	#region Module Implementations
	
	/// <summary>
	/// Register the scene loader module
	/// </summary>
	public void Initialize (DFNetwork.Networking.aNetworkLayer netLayer, DFNetwork.Simulation.SimulationLayer simLayer)
	{
		NetLayer = netLayer;
	}
	
	/// <summary>
	/// Processes a single command
	/// </summary>
	/// <returns>
	/// The command.
	/// </returns>
	/// <param name='cmd'>
	/// If set to <c>true</c> cmd.
	/// </param>
	public bool ProcessCommand (DFNetwork.Networking.PacketHandling.Packet cmd)
	{
		
		// Check if the packet is "LoadScene"
		if (cmd.CommandName == "LoadScene") {
			
			// Pop the event
			if (OnMasterInstructLoad != null) { OnMasterInstructLoad(); }
			
			// Set up the player registry
			foreach (short s in NetLayer.GetPlayerList()) {
				
				PlayersLoaded.Add(s, false);
				
			}

			// get the scene to load
            //UnityEngine.SceneManagement.SceneManager.LoadScene()
			Application.LoadLevelAdditive((string)cmd.Params[0]);
		
			// Now send a packet saying that it has been loaded
			NetLayer.SendPacket( NetLayer.GeneratePacket("SceneLoaded", (string)cmd.Params[0]) );
			
		} else {
			
			// Means we got a packet for "scene loaded"
			PlayersLoaded[cmd.Owner] = true;
			
		}
		
		return true;
		
	}
	
	/// <summary>
	/// Sends the level to load.
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	public void SendLevelToLoad(string name) {
		
		NetLayer.SendPacket( NetLayer.GeneratePacket("LoadScene", name) );
		
	}

	/// <summary>
	/// Gets a player to check if they are still loading.
	/// </summary>
	/// <returns>
	/// The player still loading.
	/// </returns>
	/// <param name='id'>
	/// If set to <c>true</c> identifier.
	/// </param>
	public bool GetPlayerStillLoading(short ID) {
		
		return PlayersLoaded[ID];
		
	}
	
	
	/// <summary>
	/// Gets a value indicating whether this <see cref="MOD_SceneManagement"/> all players loaded the named scene
	/// </summary>
	/// <value>
	/// <c>true</c> if all players loaded; otherwise, <c>false</c>.
	/// </value>
	public bool AllPlayersLoaded {
		
		get {
			
			foreach (bool b in PlayersLoaded.Values) {
			
				if (!b) { return b; }
				
			}
			
			return true;
			
		}
		
	}
	
	#endregion
	
}
