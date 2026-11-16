using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.LoadBalancing;

namespace ExitGames.Client.Photon.Voice;

public class LoadBalancingFrontend : LoadBalancingClient, IVoiceFrontend, IDisposable
{
	private const int DefaultVoiceChannel = 1;

	protected VoiceClient voiceClient;

	private bool debugEchoMode;

	public int FramesLost => voiceClient.FramesLost;

	public int FramesReceived => voiceClient.FramesReceived;

	public int FramesSent => voiceClient.FramesSent;

	public int FramesSentBytes => voiceClient.FramesSentBytes;

	public int RoundTripTime => voiceClient.RoundTripTime;

	public int RoundTripTimeVariance => voiceClient.RoundTripTimeVariance;

	public int DebugLostPercent
	{
		get
		{
			return voiceClient.DebugLostPercent;
		}
		set
		{
			voiceClient.DebugLostPercent = value;
		}
	}

	public IEnumerable<LocalVoice> LocalVoices => voiceClient.LocalVoices;

	public IEnumerable<RemoteVoiceInfo> RemoteVoiceInfos => voiceClient.RemoteVoiceInfos;

	public IEnumerable<object> RemoteVoiceLocalUserObjects => voiceClient.RemoteVoiceLocalUserObjects;

	public bool DebugEchoMode
	{
		get
		{
			return debugEchoMode;
		}
		set
		{
			debugEchoMode = value;
			if (State == ClientState.Joined)
			{
				if (debugEchoMode)
				{
					voiceClient.sendChannelVoicesInfo(1, LocalPlayer.ID);
					return;
				}
				object[] customEventContent = new object[2]
				{
					(byte)0,
					EventSubcode.DebugEchoRemoveMyVoices
				};
				RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
				raiseEventOptions.TargetActors = new int[1] { LocalPlayer.ID };
				OpRaiseEvent(201, customEventContent, sendReliable: true, raiseEventOptions);
			}
		}
	}

	public new Action<EventData> OnEventAction { get; set; }

	public new Action<ClientState> OnStateChangeAction { get; set; }

	public byte GlobalAudioGroup
	{
		get
		{
			return voiceClient.GlobalAudioGroup;
		}
		set
		{
			voiceClient.GlobalAudioGroup = value;
			if (State == ClientState.Joined)
			{
				if (voiceClient.GlobalAudioGroup != 0)
				{
					loadBalancingPeer.OpChangeGroups(new byte[0], new byte[1] { voiceClient.GlobalAudioGroup });
				}
				else
				{
					loadBalancingPeer.OpChangeGroups(new byte[0], null);
				}
			}
		}
	}

	public bool SupportsArraySegmentSerialization => true;

	public LoadBalancingFrontend()
	{
		base.OnEventAction += onEventActionVoiceClient;
		base.OnStateChangeAction += onStateChangeVoiceClient;
		voiceClient = new VoiceClient(this);
	}

	public void LogError(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.ERROR, string.Format(fmt, args));
	}

	public void LogWarning(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.WARNING, string.Format(fmt, args));
	}

	public void LogInfo(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.INFO, string.Format(fmt, args));
	}

	public void LogDebug(string fmt, params object[] args)
	{
		DebugReturn(DebugLevel.ALL, string.Format(fmt, args));
	}

	public IEnumerable<LocalVoice> LocalVoicesInChannel(int channelId)
	{
		return voiceClient.LocalVoicesInChannel(channelId);
	}

	public bool IsChannelJoined(int channelId)
	{
		return State == ClientState.Joined;
	}

	public new void Service()
	{
		base.Service();
		voiceClient.Service();
	}

	public LocalVoice CreateLocalVoice(IAudioStreamBase audioStream, VoiceInfo voiceInfo)
	{
		return voiceClient.CreateLocalVoice(audioStream, voiceInfo, 1);
	}

	public virtual bool ChangeAudioGroups(byte[] groupsToRemove, byte[] groupsToAdd)
	{
		return loadBalancingPeer.OpChangeGroups(groupsToRemove, groupsToAdd);
	}

	public void SendVoicesInfo(object content, int channelId, int targetPlayerId)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		if (targetPlayerId != 0)
		{
			raiseEventOptions.TargetActors = new int[1] { targetPlayerId };
		}
		else if (DebugEchoMode)
		{
			raiseEventOptions.Receivers = ReceiverGroup.All;
		}
		OpRaiseEvent(201, content, sendReliable: true, raiseEventOptions);
	}

	public void SendVoiceRemove(object content, int channelId)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		if (DebugEchoMode)
		{
			raiseEventOptions.Receivers = ReceiverGroup.All;
		}
		OpRaiseEvent(201, content, sendReliable: true, raiseEventOptions);
	}

	public void SendFrame(object content, int channelId, byte audioGroup)
	{
		RaiseEventOptions raiseEventOptions = new RaiseEventOptions();
		if (DebugEchoMode)
		{
			raiseEventOptions.Receivers = ReceiverGroup.All;
		}
		raiseEventOptions.InterestGroup = audioGroup;
		OpRaiseEvent(201, content, sendReliable: false, raiseEventOptions);
		loadBalancingPeer.SendOutgoingCommands();
	}

	public string ChannelIdStr(int channelId)
	{
		return null;
	}

	public string PlayerIdStr(int playerId)
	{
		return null;
	}

	private void onEventActionVoiceClient(EventData ev)
	{
		switch (ev.Code)
		{
		case byte.MaxValue:
		{
			int num = (int)ev[254];
			if (num != LocalPlayer.ID)
			{
				voiceClient.sendChannelVoicesInfo(1, num);
			}
			break;
		}
		case 254:
		{
			int num = (int)ev[254];
			if (num == LocalPlayer.ID)
			{
				voiceClient.clearRemoteVoices();
			}
			else
			{
				onPlayerLeave(num);
			}
			break;
		}
		case 201:
			voiceClient.onVoiceEvent(ev[245], 1, (int)ev[254], LocalPlayer.ID);
			break;
		}
		if (OnEventAction != null)
		{
			OnEventAction(ev);
		}
	}

	private void onStateChangeVoiceClient(ClientState state)
	{
		if (state == ClientState.Joined)
		{
			voiceClient.clearRemoteVoices();
			voiceClient.sendChannelVoicesInfo(1, 0);
			if (voiceClient.GlobalAudioGroup != 0)
			{
				loadBalancingPeer.OpChangeGroups(new byte[0], new byte[1] { voiceClient.GlobalAudioGroup });
			}
		}
		if (OnStateChangeAction != null)
		{
			OnStateChangeAction(state);
		}
	}

	private void onPlayerLeave(int playerId)
	{
		if (voiceClient.removePlayerVoices(1, playerId))
		{
			DebugReturn(DebugLevel.INFO, "[PV] Player " + playerId + " voices removed on leave");
		}
		else
		{
			DebugReturn(DebugLevel.WARNING, "[PV] Voices of player " + playerId + " not found when trying to remove on player leave");
		}
	}

	public void Dispose()
	{
		voiceClient.Dispose();
	}
}
