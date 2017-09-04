using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections;

[System.Serializable()]
public class MOD_MoveObjects : ScriptableObject, DFNetwork.Simulation.Modules.iModule {
	
	public string ModuleName {
		get {
			return "Move Objects";
		}
	}
	
	public string[] UsesPackets {
		get {
			return new string[]{"Ready"};					
		}
	}
	
	public void Initialize (DFNetwork.Networking.aNetworkLayer netLayer, DFNetwork.Simulation.SimulationLayer simLayer)
	{
		
		// Register the "MoveObject" command
		//LS_NETSIM.NetworkLayer.Commands.LS_CommandRegistry.RegisterCommand("MoveObject", 3, Serialize, Deserialize, ProcessCommand, ProcessCommands);
		// Register the "MoveObjects" command
		//LS_NETSIM.NetworkLayer.Commands.LS_CommandRegistry.RegisterCommand("MoveObjects", 3, Serialize, Deserialize, ProcessCommand, ProcessCommands);
		
	}
	
	public bool ProcessCommand (DFNetwork.Networking.PacketHandling.Packet cmd)
	{
		return false;
	}
	
}
