namespace DFNetwork.Networking.PacketHandling;

public interface iPacketHandler
{
	short ID { get; set; }

	string CommandName { get; }

	void Initialize();

	void OnEditorGUI();

	byte[] Serialize(params object[] param);

	object[] Deserialize(byte[] data);
}
