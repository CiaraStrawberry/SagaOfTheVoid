using System.Linq;
using ExitGames.Client.Photon.LoadBalancing;
using ExitGames.Client.Photon.Voice;
using UnityEngine;

public class PhotonVoiceNetwork : MonoBehaviour
{
	private static PhotonVoiceNetwork _instance;

	private static GameObject _singleton;

	private static object instanceLock = new object();

	private static bool destroyed = false;

	internal UnityVoiceFrontend client;

	private static string microphoneDevice = null;

	internal static PhotonVoiceNetwork instance => getInstance();

	public static UnityVoiceFrontend Client => instance.client;

	public static ExitGames.Client.Photon.LoadBalancing.ClientState ClientState => instance.client.State;

	public static string CurrentRoomName => (instance.client.CurrentRoom != null) ? instance.client.CurrentRoom.Name : string.Empty;

	public static string MicrophoneDevice
	{
		get
		{
			return microphoneDevice;
		}
		set
		{
			if (value != null && !Microphone.devices.Contains(value))
			{
				Debug.LogError("PUNVoice: " + value + " is not a valid microphone device");
				return;
			}
			microphoneDevice = value;
			if (PhotonVoiceSettings.Instance.DebugInfo)
			{
			}
			PhotonVoiceRecorder[] array = Object.FindObjectsOfType<PhotonVoiceRecorder>();
			foreach (PhotonVoiceRecorder photonVoiceRecorder in array)
			{
				if (photonVoiceRecorder.photonView.isMine && photonVoiceRecorder.MicrophoneDevice == null)
				{
					photonVoiceRecorder.MicrophoneDevice = null;
				}
			}
		}
	}

	private PhotonVoiceNetwork()
	{
		client = new UnityVoiceFrontend(this);
	}

	internal static PhotonVoiceNetwork getInstance()
	{
		lock (instanceLock)
		{
			if (destroyed)
			{
				return null;
			}
			if (_instance == null)
			{
				_singleton = new GameObject();
				_instance = _singleton.AddComponent<PhotonVoiceNetwork>();
				_singleton.name = "PhotonVoiceNetworkSingleton";
				Object.DontDestroyOnLoad(_singleton);
			}
			return _instance;
		}
	}

	private void OnDestroy()
	{
		if (!(this != _instance))
		{
			destroyed = true;
			client.Dispose();
		}
	}

	[RuntimeInitializeOnLoadMethod]
	public static void RuntimeInitializeOnLoad()
	{
		getInstance();
	}

	public void Awake()
	{
		if (Microphone.devices.Length < 1)
		{
			Debug.LogError("PUNVoice: No microphone device found");
		}
		MicrophoneDevice = Microphone.devices[0];
	}

	public static bool Connect()
	{
		instance.client.AppId = PhotonNetwork.PhotonServerSettings.VoiceAppID;
		instance.client.AppVersion = "1.0";
		if (PhotonNetwork.PhotonServerSettings.HostType == ServerSettings.HostingOption.SelfHosted)
		{
			string text = $"{PhotonNetwork.PhotonServerSettings.ServerAddress}:{PhotonNetwork.PhotonServerSettings.VoiceServerPort}";
			Debug.LogFormat("PUNVoice: connecting to master {0}", text);
			return instance.client.Connect(text, null, null, null, null);
		}
		Debug.LogFormat("PUNVoice: connecting to region {0}", PhotonNetwork.networkingPeer.CloudRegion.ToString());
		return instance.client.ConnectToRegionMaster(PhotonNetwork.networkingPeer.CloudRegion.ToString());
	}

	public static void Disconnect()
	{
		instance.client.Disconnect();
	}

	protected void OnEnable()
	{
		if (!(this != _instance))
		{
			Application.RequestUserAuthorization(UserAuthorization.Microphone);
		}
	}

	protected void OnApplicationQuit()
	{
		if (!(this != _instance))
		{
			client.Disconnect();
		}
	}

	protected void Update()
	{
		if (!(this != _instance))
		{
			client.DebugLostPercent = PhotonVoiceSettings.Instance.DebugLostPercent;
			client.Service();
		}
	}

	public static LocalVoice CreateLocalVoice(IAudioStreamFloat audioClip, VoiceInfo voiceInfo)
	{
		return instance.client.CreateLocalVoice(audioClip, voiceInfo);
	}

	private void OnJoinedRoom()
	{
		if (this != _instance)
		{
			return;
		}
		ExitGames.Client.Photon.LoadBalancing.ClientState state = client.State;
		if (state == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
		{
			if (PhotonVoiceSettings.Instance.AutoConnect)
			{
				client.OpLeaveRoom();
			}
		}
		else if (PhotonVoiceSettings.Instance.AutoConnect)
		{
			client.Reconnect();
		}
	}

	private void OnLeftRoom()
	{
		if (!(this != _instance) && PhotonVoiceSettings.Instance.AutoDisconnect)
		{
			client.Disconnect();
		}
	}

	private void OnDisconnectedFromPhoton()
	{
		if (!(this != _instance) && PhotonVoiceSettings.Instance.AutoDisconnect)
		{
			client.Disconnect();
		}
	}

	internal static void LinkSpeakerToRemoteVoice(PhotonVoiceSpeaker speaker)
	{
		if ((bool)speaker && (bool)instance && instance.client != null)
		{
			instance.client.LinkSpeakerToRemoteVoice(speaker);
		}
	}

	internal static void UnlinkSpeakerFromRemoteVoice(PhotonVoiceSpeaker speaker)
	{
		if (!destroyed)
		{
			instance.client.UnlinkSpeakerFromRemoteVoice(speaker);
		}
	}
}
