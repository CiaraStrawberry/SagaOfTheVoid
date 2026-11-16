namespace DFNetwork.Networking.PacketHandling;

public class Packet
{
	public readonly string CommandName;

	public readonly short PacketType;

	public readonly short Priority;

	public readonly short Owner;

	public object[] Params;

	public Packet(string commandName, short owner, params object[] param)
	{
		PacketType = PacketTypeRegistry.GetInstance.GetCommandID(commandName);
		CommandName = commandName;
		Owner = owner;
		Params = param;
	}
}
