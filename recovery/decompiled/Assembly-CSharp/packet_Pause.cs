using DFNetwork.Networking.PacketHandling;
using UnityEngine;

public class packet_Pause : ScriptableObject, iPacketHandler
{
	[SerializeField]
	private short id;

	public short ID
	{
		get
		{
			return id;
		}
		set
		{
			id = value;
		}
	}

	public string CommandName => "Pause";

	public void Initialize()
	{
	}

	public void OnEditorGUI()
	{
	}

	public byte[] Serialize(params object[] param)
	{
		return new byte[0];
	}

	public object[] Deserialize(byte[] data)
	{
		return new object[0];
	}
}
