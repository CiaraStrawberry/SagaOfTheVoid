namespace ExitGames.Client.Photon.LoadBalancing;

public enum ClientState
{
	Uninitialized,
	Queued,
	Authenticated,
	JoinedLobby,
	DisconnectingFromMasterserver,
	ConnectingToGameserver,
	ConnectedToGameserver,
	Joining,
	Joined,
	Leaving,
	DisconnectingFromGameserver,
	ConnectingToMasterserver,
	QueuedComingFromGameserver,
	Disconnecting,
	Disconnected,
	ConnectedToMaster,
	ConnectingToNameServer,
	ConnectedToNameServer,
	DisconnectingFromNameServer,
	Authenticating
}
