using UnityEngine;
using System.Collections;

namespace DFNetwork.Simulation.Modules {
	
	public interface iModule {
		
		/// <summary>
		/// Gets the name of the module.
		/// </summary>
		/// <value>
		/// The name of the module.
		/// </value>
		string ModuleName{ get; }
		
		/// <summary>
		/// Return the packets that this module will use
		/// </summary>
		/// <value>
		/// The uses packets.
		/// </value>
		string[] UsesPackets { get; }
		
		/// <summary>
		/// Processes one command, used for instant processing
		/// </summary>
		/// <returns>
		/// The command.
		/// </returns>
		/// <param name='cmd'>
		/// If set to <c>true</c> cmd.
		/// </param>
		bool ProcessCommand(DFNetwork.Networking.PacketHandling.Packet cmd);
		
		/// <summary>
		/// Initialize this instance.
		/// </summary>
		void Initialize(Networking.aNetworkLayer netLayer, SimulationLayer simLayer);
		
	}

}