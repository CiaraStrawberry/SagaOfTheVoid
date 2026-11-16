using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;

namespace DFNetwork.Simulation.Modules;

public interface iModule
{
	string ModuleName { get; }

	string[] UsesPackets { get; }

	bool ProcessCommand(Packet cmd);

	void Initialize(aNetworkLayer netLayer, SimulationLayer simLayer);
}
