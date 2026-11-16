namespace ExitGames.Client.Photon.Voice;

internal interface IVoiceFrontend
{
	bool SupportsArraySegmentSerialization { get; }

	void LogError(string fmt, params object[] args);

	void LogWarning(string fmt, params object[] args);

	void LogInfo(string fmt, params object[] args);

	void LogDebug(string fmt, params object[] args);

	bool IsChannelJoined(int channelId);

	void SendVoicesInfo(object content, int channelId, int targetPlayerId);

	void SendVoiceRemove(object content, int channelId);

	void SendFrame(object content, int channelId, byte audioGroup);

	string ChannelIdStr(int channelId);

	string PlayerIdStr(int playerId);
}
