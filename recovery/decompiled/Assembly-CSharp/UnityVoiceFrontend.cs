using System;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon.Voice;
using UnityEngine;

public class UnityVoiceFrontend : LoadBalancingFrontend
{
	private Dictionary<KeyValuePair<int, byte>, PhotonVoiceSpeaker> voiceSpeakers = new Dictionary<KeyValuePair<int, byte>, PhotonVoiceSpeaker>();

	private bool reconnect;

	public Action<int, byte, VoiceInfo> OnRemoteVoiceInfoAction { get; set; }

	public Action<int, byte> OnRemoteVoiceRemoveAction { get; set; }

	public Action<int, byte, float[]> OnAudioFrameAction { get; set; }

	public new Action<ExitGames.Client.Photon.LoadBalancing.ClientState> OnStateChangeAction { get; set; }

	public new Action<OperationResponse> OnOpResponseAction { get; set; }

	internal UnityVoiceFrontend(PhotonVoiceNetwork network)
	{
		VoiceClient voiceClient = base.voiceClient;
		voiceClient.OnRemoteVoiceInfoAction = (VoiceClient.RemoteVoiceInfoDelegate)Delegate.Combine(voiceClient.OnRemoteVoiceInfoAction, new VoiceClient.RemoteVoiceInfoDelegate(OnRemoteVoiceInfo));
		VoiceClient voiceClient2 = base.voiceClient;
		voiceClient2.OnRemoteVoiceRemoveAction = (VoiceClient.RemoteVoiceRemoveDelegate)Delegate.Combine(voiceClient2.OnRemoteVoiceRemoveAction, new VoiceClient.RemoteVoiceRemoveDelegate(OnRemoteVoiceRemove));
		VoiceClient voiceClient3 = base.voiceClient;
		voiceClient3.OnAudioFrameFloatAction = (VoiceClient.AudioFrameFloatDelegate)Delegate.Combine(voiceClient3.OnAudioFrameFloatAction, new VoiceClient.AudioFrameFloatDelegate(OnAudioFrame));
		base.OnStateChangeAction = (Action<ExitGames.Client.Photon.LoadBalancing.ClientState>)Delegate.Combine(base.OnStateChangeAction, new Action<ExitGames.Client.Photon.LoadBalancing.ClientState>(OnStateChange));
		base.OnOpResponseAction += OnOpResponse;
		loadBalancingPeer.DebugOut = DebugLevel.INFO;
	}

	public void Reconnect()
	{
		if (State == ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected || State == ExitGames.Client.Photon.LoadBalancing.ClientState.Uninitialized)
		{
			PhotonVoiceNetwork.Connect();
			return;
		}
		reconnect = true;
		Disconnect();
	}

	public override void DebugReturn(DebugLevel level, string message)
	{
		message = $"PUNVoice: {message}";
		switch (level)
		{
		case DebugLevel.ERROR:
			Debug.LogError(message);
			return;
		case DebugLevel.WARNING:
			Debug.LogWarning(message);
			return;
		case DebugLevel.INFO:
			if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
			{
				Debug.Log(message);
				return;
			}
			break;
		}
		if (level == DebugLevel.ALL && PhotonNetwork.logLevel == PhotonLogLevel.Full)
		{
			Debug.Log(message);
		}
	}

	public void OnOpResponse(OperationResponse resp)
	{
		if (resp.ReturnCode == 0)
		{
			byte operationCode = resp.OperationCode;
			if (operationCode == 226)
			{
				PhotonVoiceRecorder[] array = UnityEngine.Object.FindObjectsOfType<PhotonVoiceRecorder>();
				PhotonVoiceRecorder[] array2 = array;
				foreach (PhotonVoiceRecorder photonVoiceRecorder in array2)
				{
					photonVoiceRecorder.SendMessage("OnJoinedVoiceRoom");
				}
			}
		}
		if (OnOpResponseAction != null)
		{
			OnOpResponseAction(resp);
		}
	}

	private void linkVoice(int playerId, byte voiceId, VoiceInfo voiceInfo, PhotonVoiceSpeaker speaker)
	{
		speaker.OnVoiceLinked(voiceInfo.SamplingRate, voiceInfo.Channels, voiceInfo.FrameDurationSamples, PhotonVoiceSettings.Instance.PlayDelayMs);
		KeyValuePair<int, byte> key = new KeyValuePair<int, byte>(playerId, voiceId);
		if (voiceSpeakers.TryGetValue(key, out var value))
		{
			if (value == speaker)
			{
				return;
			}
			Debug.LogFormat("PUNVoice: Player {0} voice #{1} speaker replaced.", playerId, voiceId);
		}
		else
		{
			Debug.LogFormat("PUNVoice: Player {0} voice #{1} speaker created.", playerId, voiceId);
		}
		voiceSpeakers[key] = speaker;
	}

	public void OnRemoteVoiceInfo(int channelId, int playerId, byte voiceId, VoiceInfo voiceInfo, out object localUserObject)
	{
		KeyValuePair<int, byte> key = new KeyValuePair<int, byte>(playerId, voiceId);
		if (voiceSpeakers.ContainsKey(key))
		{
			Debug.LogWarningFormat("PUNVoice: Info duplicate for voice #{0} of player {1}", voiceId, playerId);
		}
		PhotonVoiceSpeaker photonVoiceSpeaker = null;
		PhotonVoiceSpeaker[] array = UnityEngine.Object.FindObjectsOfType<PhotonVoiceSpeaker>();
		PhotonVoiceSpeaker[] array2 = array;
		foreach (PhotonVoiceSpeaker photonVoiceSpeaker2 in array2)
		{
			if (photonVoiceSpeaker2.photonView.viewID == (int)voiceInfo.UserData)
			{
				photonVoiceSpeaker = photonVoiceSpeaker2;
				break;
			}
		}
		if (photonVoiceSpeaker == null)
		{
			Debug.LogWarning("PUNVoice: No PhotonVoiceMic found for info of voice #" + voiceId + " of player " + playerId);
		}
		else
		{
			linkVoice(playerId, voiceId, voiceInfo, photonVoiceSpeaker);
		}
		if (OnRemoteVoiceInfoAction != null)
		{
			OnRemoteVoiceInfoAction(playerId, voiceId, voiceInfo);
		}
		localUserObject = null;
	}

	public void LinkSpeakerToRemoteVoice(PhotonVoiceSpeaker speaker)
	{
		foreach (RemoteVoiceInfo remoteVoiceInfo in RemoteVoiceInfos)
		{
			if (speaker.photonView.viewID == (int)remoteVoiceInfo.Info.UserData)
			{
				linkVoice(remoteVoiceInfo.PlayerId, remoteVoiceInfo.VoiceId, remoteVoiceInfo.Info, speaker);
			}
		}
	}

	public void OnRemoteVoiceRemove(int channelId, int playerId, byte voiceId, object localUserObject)
	{
		KeyValuePair<int, byte> key = new KeyValuePair<int, byte>(playerId, voiceId);
		if (!unlinkSpeaker(key))
		{
			Debug.LogWarningFormat("PUNVoice: Voice #{0} of player {1} not found.", voiceId, playerId);
		}
		else
		{
			Debug.LogFormat("PUNVoice: Player {0} voice # {1} speaker unlinked.", playerId, voiceId);
		}
		if (OnRemoteVoiceRemoveAction != null)
		{
			OnRemoteVoiceRemoveAction(playerId, voiceId);
		}
	}

	private bool unlinkSpeaker(KeyValuePair<int, byte> key)
	{
		if (voiceSpeakers.TryGetValue(key, out var value))
		{
			value.OnVoiceUnlinked();
		}
		return voiceSpeakers.Remove(key);
	}

	public void UnlinkSpeakerFromRemoteVoice(PhotonVoiceSpeaker speaker)
	{
		List<KeyValuePair<int, byte>> list = new List<KeyValuePair<int, byte>>();
		foreach (KeyValuePair<KeyValuePair<int, byte>, PhotonVoiceSpeaker> voiceSpeaker in voiceSpeakers)
		{
			if (voiceSpeaker.Value == speaker)
			{
				list.Add(voiceSpeaker.Key);
				Debug.LogFormat("PUNVoice: Player {0} voice # {1} speaker unlinked.", voiceSpeaker.Key.Key, voiceSpeaker.Key.Value);
			}
		}
		foreach (KeyValuePair<int, byte> item in list)
		{
			unlinkSpeaker(item);
		}
	}

	public void OnAudioFrame(int channelId, int playerId, byte voiceId, float[] frame, object localUserObject)
	{
		PhotonVoiceSpeaker value = null;
		if (voiceSpeakers.TryGetValue(new KeyValuePair<int, byte>(playerId, voiceId), out value))
		{
			value.OnAudioFrame(frame);
		}
		else
		{
			Debug.LogWarningFormat("PUNVoice: Audio Frame event for not existing speaker for voice #{0} of player {1}.", voiceId, playerId);
		}
		if (OnAudioFrameAction != null)
		{
			OnAudioFrameAction(playerId, voiceId, frame);
		}
	}

	public void OnStateChange(ExitGames.Client.Photon.LoadBalancing.ClientState state)
	{
		if (PhotonNetwork.logLevel >= PhotonLogLevel.Informational)
		{
			Debug.LogFormat("PUNVoice: Voice Client state: {0}", state);
		}
		switch (state)
		{
		case ExitGames.Client.Photon.LoadBalancing.ClientState.JoinedLobby:
			if (PhotonNetwork.inRoom)
			{
				OpJoinOrCreateRoom($"{PhotonNetwork.room.Name}_voice_", new ExitGames.Client.Photon.LoadBalancing.RoomOptions
				{
					IsVisible = false
				}, null);
			}
			else
			{
				Debug.LogWarning("PUNVoice: PUN client is not in room yet. Disconnecting voice client.");
				Disconnect();
			}
			break;
		case ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected:
			if (reconnect)
			{
				PhotonVoiceNetwork.Connect();
			}
			reconnect = false;
			break;
		}
		if (OnStateChangeAction != null)
		{
			OnStateChangeAction(state);
		}
	}
}
