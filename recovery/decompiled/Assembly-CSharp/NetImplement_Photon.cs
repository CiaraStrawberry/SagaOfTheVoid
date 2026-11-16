using System;
using System.Collections;
using DFNetwork.Debugging;
using DFNetwork.Networking;
using DFNetwork.Networking.PacketHandling;
using EasyEditor;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(PhotonLagSimulationGui))]
[RequireComponent(typeof(PhotonStatsGui))]
public class NetImplement_Photon : aNetworkLayer
{
	[Inspector(displayHeader = true, foldable = true, group = "System Settings", groupDescription = "General system settings.  The only time you will need to change version is if you change packet types or ID's")]
	[SerializeField]
	private string currentVersion = "v1.0 Alpha";

	[Inspector(group = "System Settings")]
	[SerializeField]
	private PhotonLogLevel logLevel = PhotonLogLevel.Full;

	[Inspector(displayHeader = true, foldable = true, group = "Server Settings", groupDescription = "Settings used only when not using the default properties of Photon Cloud")]
	[Comment("This doesn't need to be set at the moment")]
	[SerializeField]
	private bool useMasterServer;

	[Inspector(group = "Server Settings")]
	[SerializeField]
	private string ipAddress = string.Empty;

	[Inspector(group = "Server Settings")]
	[SerializeField]
	private string port = string.Empty;

	[Inspector(group = "System Settings")]
	[SerializeField]
	private string gameID = string.Empty;

	private PacketTypeRegistry _packetRegistry;

	public string CurrentVersion
	{
		get
		{
			return currentVersion;
		}
		set
		{
			currentVersion = value;
		}
	}

	public PhotonLogLevel LogLevel
	{
		get
		{
			return logLevel;
		}
		set
		{
			logLevel = value;
		}
	}

	public bool UserMasterServer
	{
		get
		{
			return useMasterServer;
		}
		set
		{
			useMasterServer = value;
		}
	}

	public string IpAddress
	{
		get
		{
			return ipAddress;
		}
		set
		{
			ipAddress = value;
		}
	}

	public string Port
	{
		get
		{
			return port;
		}
		set
		{
			port = value;
		}
	}

	public string GameID
	{
		get
		{
			return gameID;
		}
		set
		{
			gameID = value;
		}
	}

	public override short GetCurrentPlayerID => (short)PhotonNetwork.player.ID;

	public override void Awake()
	{
		base.Awake();
	}

	public override void Start()
	{
		base.Start();
	}

	public override Packet GeneratePacket(short CommandID, params object[] param)
	{
		return GeneratePacket(_packetRegistry.GetCommandName(CommandID), param);
	}

	public override Packet GeneratePacket(string CommandName, params object[] param)
	{
		return new Packet(CommandName, (short)PhotonNetwork.player.ID, param);
	}

	public override void SendPacket(Packet packet)
	{
		PhotonNetwork.RPC(PhotonView.Get(this), "RecievePacket", PhotonTargets.AllBuffered, false, ProcessOutboundPacket(packet));
	}

	[PunRPC]
	public void RecievePacket(byte[] data)
	{
		ProcessIncommingPacket(data);
	}

	public override void DoNetworkConnect()
	{
		PhotonNetwork.autoJoinLobby = true;
		if (!PhotonNetwork.connected)
		{
			PhotonNetwork.ConnectUsingSettings(currentVersion);
		}
	}

	public override void DoNetworkDisconnect()
	{
		if (PhotonNetwork.connected)
		{
			PhotonNetwork.Disconnect();
		}
	}

	public override short GetPlayerID(string name)
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (photonPlayer.name == name)
			{
				return (short)photonPlayer.ID;
			}
		}
		throw new Exception("Player not found");
	}

	public override short[] GetPlayerList()
	{
		short[] array = new short[PhotonNetwork.playerList.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = (short)PhotonNetwork.playerList[i].ID;
		}
		return array;
	}

	public override string GetPlayerName(short ID)
	{
		PhotonPlayer[] playerList = PhotonNetwork.playerList;
		foreach (PhotonPlayer photonPlayer in playerList)
		{
			if (photonPlayer.ID == ID)
			{
				return photonPlayer.name;
			}
		}
		throw new Exception("Player not found");
	}

	public override bool IsHost()
	{
		return PhotonNetwork.player.isMasterClient;
	}

	private void OnConnectedToPhoton()
	{
		RaiseOnNetworkConnect();
	}

	private void OnDisconnectedFromPhoton()
	{
		RaiseOnNetworkDisconnect();
	}

	private void OnJoinedRoom()
	{
		Debug.Log("Joined room: " + GetComponent<PhotonView>().viewID);
		RaiseOnRoomJoined();
	}

	private IEnumerator OnLeftRoom()
	{
		while (PhotonNetwork.room != null || !PhotonNetwork.connected)
		{
			yield return 0;
		}
		RaiseOnRoomLeft();
	}

	private void OnPhotonPlayerConnected(PhotonPlayer player)
	{
		Debug.Log("Player Connected");
		RaiseOnPlayerConnected((short)player.ID);
	}

	private void OnPhotonPlayerDisconnected(PhotonPlayer player)
	{
		RaiseOnPlayerDisconnected((short)player.ID);
	}

	private void OnMasterClientSwitched(PhotonPlayer newMasterClient)
	{
		Logging.AddLogItem("Photon Implementation", "Switched to new master client");
	}
}
