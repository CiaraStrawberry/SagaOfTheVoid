using System;
using UnityEngine;
using EasyEditor;

#if UNITY_EDITOR

using UnityEditor;

#endif

using System.Collections;

[RequireComponent (typeof (PhotonView))]
[RequireComponent (typeof (PhotonLagSimulationGui))]
[RequireComponent (typeof (PhotonStatsGui))]
public class NetImplement_Photon : DFNetwork.Networking.aNetworkLayer {

    #region Properties

    [Inspector(displayHeader = true, foldable = true, group = "System Settings", groupDescription = "General system settings.  The only time you will need to change version is if you change packet types or ID's" )]
    [SerializeField]
	private string currentVersion = "v1.0 Alpha";
	/// <summary>
	/// Gets or sets the current version that photon uses in making sure the
	/// same clients connect with each other
	/// </summary>
	/// <value>
	/// The current version.
	/// </value>
	public string CurrentVersion {
		get { return currentVersion; }
		set { currentVersion = value; }
	}

    [Inspector(group = "System Settings")]
    [SerializeField]
	private PhotonLogLevel logLevel = PhotonLogLevel.Full;
	/// <summary>
	/// Gets or sets the log level.
	/// </summary>
	/// <value>
	/// The log level.
	/// </value>
	public PhotonLogLevel LogLevel {
		get { return logLevel; }
		set { logLevel = value; }
	}

    [Inspector(displayHeader = true, foldable = true, group = "Server Settings", groupDescription = "Settings used only when not using the default properties of Photon Cloud")]
    [Comment("This doesn't need to be set at the moment")]
    [SerializeField]
	private bool useMasterServer = false;
	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="LS_Simulator"/> user a master server.
	/// </summary>
	/// <value>
	/// <c>true</c> if user master server; otherwise, master client<c>false</c>.
	/// </value>
	public bool UserMasterServer {
		get { return useMasterServer; }
		set { useMasterServer = value; }
	}

    [Inspector(group = "Server Settings")]
    [SerializeField]
	private string ipAddress = "";
	/// <summary>
	/// Gets or sets the ip address.
	/// </summary>
	/// <value>
	/// The ip address.
	/// </value>
	public string IpAddress {
		get { return ipAddress; }
		set { ipAddress = value; }
	}

    [Inspector(group = "Server Settings")]
    [SerializeField]
	private string port = "";
	/// <summary>
	/// Gets or sets the port.
	/// </summary>
	/// <value>
	/// The port.
	/// </value>
	public string Port {
		get { return port; }
		set { port = value; }
	}

    [Inspector(group = "System Settings")]
    [SerializeField]
	private string gameID = "";
	/// <summary>
	/// Gets or sets the game ID
	/// </summary>
	/// <value>
	/// The game ID
	/// </value>
	public string GameID {
		get { return gameID; }
		set { gameID = value; }
	}

    #endregion

    /// <summary>
    /// Instance of packet registry
    /// </summary>
    private DFNetwork.Networking.PacketHandling.PacketTypeRegistry _packetRegistry;

    public override void Awake() 
    {
        base.Awake();
    }

    public override void Start()
	{
		base.Start();
	}
	
	#region Implemented Methods 
	
	#region Networking Implementations
	
	/// <summary>
	/// Generates the packet.
	/// </summary>
	/// <returns>
	/// The packet.
	/// </returns>
	/// <param name='CommandID'>
	/// Command Id
	/// </param>
	/// <param name='param'>
	/// Parameter.
	/// </param>
	public override DFNetwork.Networking.PacketHandling.Packet GeneratePacket (short CommandID, params object[] param)
	{
		// Generate a new packet
		return GeneratePacket(_packetRegistry.GetCommandName(CommandID), param);
	}
	
	/// <summary>
	/// Generates the packet.
	/// </summary>
	/// <returns>
	/// The packet.
	/// </returns>
	/// <param name='CommandName'>
	/// Command name.
	/// </param>
	/// <param name='turnToProcess'>
	/// Turn to process.
	/// </param>
	/// <param name='param'>
	/// Parameter.
	/// </param>
	public override DFNetwork.Networking.PacketHandling.Packet GeneratePacket (string CommandName, params object[] param)
	{
		// Generate a new packet		
		return new DFNetwork.Networking.PacketHandling.Packet(CommandName, (short)PhotonNetwork.player.ID, param);
	}
	
	/// <summary>
	/// Processes the outbound packet.
	/// </summary>
	/// <returns>
	/// The outbound packet.
	/// </returns>
	/// <param name='packet'>
	/// Packet.
	/// </param>
	public override void SendPacket (DFNetwork.Networking.PacketHandling.Packet packet)
	{
        // Send the packet
        PhotonNetwork.RPC(PhotonView.Get(this), "RecievePacket", PhotonTargets.AllBuffered, false, base.ProcessOutboundPacket(packet));
    }
	
	/// <summary>
	/// Processes the incomming packet.
	/// </summary>
	/// <param name='data'>
	/// Data.
	/// </param>
	[PunRPC]
	public void RecievePacket (byte[] data)
	{
		// We only need to send it to the base
		base.ProcessIncommingPacket(data);
	}
	
	#endregion
	
	#region Overriden methods

	/// <summary>
	/// Instruct photon to connect
	/// </summary>
	public override void DoNetworkConnect ()
	{

        // Need this to pull the rooms
        PhotonNetwork.autoJoinLobby = true;

		// Connect to the network
		if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(currentVersion);
        }
		
	}
	
	/// <summary>
	/// Instruct photon to disconnect
	/// </summary>
	public override void DoNetworkDisconnect ()
	{
		
		// Disconnect from the network
		if (PhotonNetwork.connected) { PhotonNetwork.Disconnect(); }
		
	}
	
	/// <summary>
	/// Gets the get current player Id
	/// </summary>
	/// <value>
	/// The get current player Id
	/// </value>
	public override short GetCurrentPlayerID {
		get {
			return (short)PhotonNetwork.player.ID;	
		}
	}
	
	/// <summary>
	/// Gets the player ID
	/// </summary>
	/// <returns>
	/// The player ID
	/// </returns>
	/// <param name='name'>
	/// Name.
	/// </param>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public override short GetPlayerID (string name)
	{
		
		foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
			
			if (pp.name == name) { return (short)pp.ID; }
			
		}
		
		throw new Exception("Player not found");
		
	}
	
	/// <summary>
	/// Gets the player list of ID's
	/// </summary>
	/// <returns>
	/// The player list.
	/// </returns>
	public override short[] GetPlayerList ()
	{
		
		// This is developed as short because you will never have over
		// 255 players in the room, that is too many
		short[] t = new short[PhotonNetwork.playerList.Length];
		
		for(int a = 0; a < t.Length; a++) {
			t[a] = (short)PhotonNetwork.playerList[a].ID;
		}
		
		return t;
		
	}
	
	/// <summary>
	/// Gets the name of the player.
	/// </summary>
	/// <returns>
	/// The player name.
	/// </returns>
	/// <param name='ID'>
	/// ID
	/// </param>
	/// <exception cref='Exception'>
	/// Represents errors that occur during application execution.
	/// </exception>
	public override string GetPlayerName (short ID)
	{
		
		foreach (PhotonPlayer pp in PhotonNetwork.playerList) {
			
			if (pp.ID == ID) { return pp.name; }
			
		}
		
		throw new Exception("Player not found");
		
	}
	
	/// <summary>
	/// Determines whether this instance is host.
	/// </summary>
	/// <returns>
	/// <c>true</c> if this instance is host; otherwise, <c>false</c>.
	/// </returns>
	public override bool IsHost ()
	{
		return PhotonNetwork.player.isMasterClient;
	}
	
	#endregion
	
	#endregion
	
	#region Photon Callbacks
	
	void OnConnectedToPhoton() {

        RaiseOnNetworkConnect();
		
	}
	
    void OnDisconnectedFromPhoton()
    {

		RaiseOnNetworkDisconnect();

	}    
	
	/// <summary>
	/// Raises the joined room event, called when the user joins the room
	/// </summary>
	void OnJoinedRoom()
    {

        Debug.Log("Joined room: " + GetComponent<PhotonView>().viewID);

        // Get a view ID for our photon view
        //GetComponent<PhotonView>().viewID = PhotonNetwork.AllocateViewID();

        // Send event that room was joined
        base.RaiseOnRoomJoined();

        // Raise player connected
        //RaiseOnPlayerConnected(GetCurrentPlayerID);
        	
    }
    
    IEnumerator OnLeftRoom()
    {

		//Wait untill Photon is properly disconnected (empty room, and connected back to main server)
        while(PhotonNetwork.room!=null || PhotonNetwork.connected==false)
            yield return 0;
		
		base.RaiseOnRoomLeft();

    }
	
	/// <summary>
	/// Raises the photon player connected event.
	/// </summary>
	/// <param name='player'>
	/// Player.
	/// </param>
	void OnPhotonPlayerConnected(PhotonPlayer player) {

        Debug.Log("Player Connected");

		RaiseOnPlayerConnected((short)player.ID);
		
	}
	
	/// <summary>
	/// Raises the photon player disconnected event.
	/// </summary>
	/// <param name='player'>
	/// Player.
	/// </param>
	void OnPhotonPlayerDisconnected(PhotonPlayer player) {
		
		RaiseOnPlayerDisconnected((short)player.ID);
		
	}
	
	void OnMasterClientSwitched(PhotonPlayer newMasterClient) {
		
		DFNetwork.Debugging.Logging.AddLogItem("Photon Implementation", "Switched to new master client");
		
	}
	
	#endregion
	
}
