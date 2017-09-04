using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This module is used to allow players to select if they are ready or not
/// </summary>
[System.Serializable()]
public class MOD_ReadyState : ScriptableObject, DFNetwork.Simulation.Modules.iModule {
	
	// User registry for users who marked ready or not
	private Dictionary<short, bool> PlayersReady = new Dictionary<short, bool>();
	
	// Notes, we will need to get a list of players in the room
	// We will need to have a state that says "I am connected"
	// User will only be able to change their own state.
	
	short MinimumPlayers = 1;
	
	/// <summary>
	/// The net layer.
	/// </summary>
	DFNetwork.Networking.aNetworkLayer NetLayer;
	
	/// <summary>
	/// Gets the name of the module.
	/// </summary>
	/// <value>
	/// The name of the module.
	/// </value>
	public string ModuleName {
		get {
			return "Ready State"; 
		}
	}
	
	/// <summary>
	/// Returns a list of packets that this module is built to handle
	/// </summary>
	/// <value>
	/// The uses packets.
	/// </value>
	public string[] UsesPackets {
		get {
			return new string[]{"Ready"};					
		}
	}
	
	/// <summary>
	/// Initialize this instance
	/// </summary>
	/// <param name='netLayer'>
	/// Net layer.
	/// </param>
	/// <param name='simLayer'>
	/// Sim layer.
	/// </param>
	public void Initialize (DFNetwork.Networking.aNetworkLayer netLayer, DFNetwork.Simulation.SimulationLayer simLayer)
	{
		
		NetLayer = netLayer;
		
		// Grab the room join / left event
		NetLayer.OnRoomJoined += OnRoomJoined;
		NetLayer.OnRoomLeft += OnRoomLeft;
		
		// We need to know when a player connects / disconnects
		NetLayer.OnPlayerConnected += PlayerConnect;
		NetLayer.OnPlayerDisconnected += PlayerDisconnected;
		
	}
	
	/// <summary>
	/// Processes a command
	/// </summary>
	/// <returns>
	/// The command.
	/// </returns>
	/// <param name='cmd'>
	/// If set to <c>true</c> cmd.
	/// </param>
	public bool ProcessCommand (DFNetwork.Networking.PacketHandling.Packet cmd)
	{
	
		// Set the owners ready state
		if (!PlayersReady.ContainsKey(cmd.Owner)) {
			
			// Add the player to the list
			PlayersReady.Add(cmd.Owner, (bool)cmd.Params[0]);
			
		} else {
			
			// Player exists, set new state
			PlayersReady[cmd.Owner] = (bool)cmd.Params[0];
			
		}
		
		return AllPlayersReady;
		
	}	
	
	#region Network Events
	
	/// <summary>
	/// Player connect event
	/// </summary>
	/// <param name='ID'>
	/// Id
	/// </param>
	private void PlayerConnect(short ID) {
		
		Debug.Log ("Player Connected: " + ID);
		// Add this new player to the player list and set them to false
		PlayersReady.Add(ID, false);
		
	}
	
	/// <summary>
	/// Player disconnect event
	/// </summary>
	/// <param name='ID'>
	/// Id
	/// </param>
	private void PlayerDisconnected(short ID) {
		
		// Remove the player that isn't ready
		PlayersReady.Remove(ID);
		
	}
	
	/// <summary>
	/// Raises the room joined event.
	/// </summary>
	private void OnRoomJoined() {
		
		// When a user joins the room, clear current states
		PlayersReady.Clear();
		
		// Get a list of current players, add them to the state recorder, mark them as "not ready"
		// so, get the player list
		foreach(short p in NetLayer.GetPlayerList()) {
			
			// Make sure we have a current list of all the players
			if (!PlayersReady.ContainsKey(p)){
                Debug.Log("Player Connected: " + p);
                PlayersReady.Add(p, false);
			}
			
		}
		
	}
	
	/// <summary>
	/// Raises the room left event.
	/// </summary>
	private void OnRoomLeft() {
		
		// I don't think anything needs to be done with this event
	
		// Clear the registered players
		PlayersReady.Clear();
		
	}
	
	#endregion
	
	/// <summary>
	/// Sends the ready packet
	/// </summary>
	/// <param name='ready'>
	/// Ready.
	/// </param>
	public void SendReady(bool ready) {
		
		// Generate and send the packet based on "ready" or not
		NetLayer.SendPacket( NetLayer.GeneratePacket("Ready", ready) );
		
	}
	
	/// <summary>
	/// Gets the ready state for player.
	/// </summary>
	/// <returns>
	/// The ready state for player.
	/// </returns>
	/// <param name='ID'>
	/// If set to <c>true</c> I.
	/// </param>
	public bool GetReadyStateForPlayer(short ID) {

        return PlayersReady.ContainsKey(ID) ? PlayersReady[ID] : false;
		
	}
	
	/// <summary>
	/// Ares all players ready.
	/// </summary>
	/// <returns>
	/// The all players ready.
	/// </returns>
	public bool AllPlayersReady {
		
		get { 
			
			
			// Check if all players are ready, if not, return false
			foreach (bool readyState in PlayersReady.Values) {
				
				// if any false, return false
				if (!readyState) { return false; }
				
			}
			
			// Check to see if the criteria matches the minimum room count
			return (MinimumPlayers <= NetLayer.GetPlayerList().Length);
			
		}
		
	}
	
}
