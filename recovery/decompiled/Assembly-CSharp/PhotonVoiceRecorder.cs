using System.Linq;
using System.Text;
using ExitGames.Client.Photon.Voice;
using Photon;
using UnityEngine;

[RequireComponent(typeof(PhotonVoiceSpeaker))]
[DisallowMultipleComponent]
public class PhotonVoiceRecorder : Photon.MonoBehaviour
{
	public LocalVoice voice = LocalVoice.Dummy;

	private string microphoneDevice;

	public AudioClip AudioClip;

	public bool LoopAudioClip = true;

	public bool istransmittingdehbug;

	public float levelmeter;

	public string microphonedevice;

	private string log0;

	private string log1;

	public VoiceDetector VoiceDetector => (!photonView.isMine) ? null : voice.VoiceDetector;

	public string MicrophoneDevice
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
			if (voice != LocalVoice.Dummy && AudioClip == null)
			{
				PhotonVoiceSettings instance = PhotonVoiceSettings.Instance;
				Application.RequestUserAuthorization(UserAuthorization.Microphone);
				string text = ((MicrophoneDevice == null) ? PhotonVoiceNetwork.MicrophoneDevice : MicrophoneDevice);
				if (PhotonVoiceSettings.Instance.DebugInfo)
				{
					Debug.LogFormat("PUNVoice: Setting recorder's microphone device to {0}", text);
				}
				voice.RemoveSelf();
				MicWrapper micWrapper = new MicWrapper(text, (int)instance.SamplingRate);
				bool debugEchoMode = PhotonVoiceNetwork.Client.DebugEchoMode;
				PhotonVoiceNetwork.Client.DebugEchoMode = false;
				VoiceInfo voiceInfo = new VoiceInfo((int)instance.SamplingRate, micWrapper.Channels, (int)instance.FrameDuration, instance.Bitrate, photonView.viewID);
				LocalVoice localVoice = voice;
				voice = PhotonVoiceNetwork.CreateLocalVoice(micWrapper, voiceInfo);
				voice.AudioGroup = localVoice.AudioGroup;
				voice.Transmit = localVoice.Transmit;
				voice.VoiceDetector.On = localVoice.VoiceDetector.On;
				voice.VoiceDetector.Threshold = localVoice.VoiceDetector.Threshold;
				PhotonVoiceNetwork.Client.DebugEchoMode = debugEchoMode;
			}
		}
	}

	public byte AudioGroup
	{
		get
		{
			return voice.AudioGroup;
		}
		set
		{
			voice.AudioGroup = value;
		}
	}

	public bool IsTransmitting => voice.IsTransmitting;

	public LevelMeter LevelMeter => voice.LevelMeter;

	public bool Transmit
	{
		get
		{
			return voice.Transmit;
		}
		set
		{
			voice.Transmit = value;
		}
	}

	public bool Detect
	{
		get
		{
			return voice.VoiceDetector.On;
		}
		set
		{
			voice.VoiceDetector.On = value;
		}
	}

	public bool VoiceDetectorCalibrating => voice.VoiceDetectorCalibrating;

	private void Awake()
	{
		if (Microphone.devices.Count() > 0)
		{
			MicrophoneDevice = Microphone.devices[0];
		}
	}

	private void Start()
	{
		if (Microphone.devices.Length < 1 || !photonView.isMine)
		{
			return;
		}
		PhotonVoiceSettings instance = PhotonVoiceSettings.Instance;
		Application.RequestUserAuthorization(UserAuthorization.Microphone);
		int num = 0;
		IAudioStreamFloat audioStreamFloat;
		if (AudioClip == null)
		{
			string text = ((MicrophoneDevice == null) ? PhotonVoiceNetwork.MicrophoneDevice : MicrophoneDevice);
			Debug.LogFormat("PUNVoice: Setting recorder's microphone device to {0}", text);
			MicWrapper micWrapper = new MicWrapper(text, (int)instance.SamplingRate);
			num = micWrapper.Channels;
			audioStreamFloat = micWrapper;
		}
		else
		{
			audioStreamFloat = new AudioClipWrapper(AudioClip);
			num = AudioClip.channels;
			if (LoopAudioClip)
			{
				((AudioClipWrapper)audioStreamFloat).Loop = true;
			}
		}
		VoiceInfo voiceInfo = new VoiceInfo((int)instance.SamplingRate, num, (int)instance.FrameDuration, instance.Bitrate, photonView.viewID);
		voice = PhotonVoiceNetwork.CreateLocalVoice(audioStreamFloat, voiceInfo);
		VoiceDetector.On = PhotonVoiceSettings.Instance.VoiceDetection;
		VoiceDetector.Threshold = PhotonVoiceSettings.Instance.VoiceDetectionThreshold;
		if (voice != LocalVoice.Dummy)
		{
			voice.Transmit = PhotonVoiceSettings.Instance.AutoTransmit;
		}
		else if (PhotonVoiceSettings.Instance.AutoTransmit)
		{
			Debug.LogWarning("PUNVoice: Cannot Transmit.");
		}
	}

	public void Update()
	{
		istransmittingdehbug = IsTransmitting;
		levelmeter = voice.LevelMeter.CurrentAvgAmp;
	}

	private void OnDestroy()
	{
		if (voice != LocalVoice.Dummy)
		{
			voice.RemoveSelf();
		}
	}

	private void OnEnable()
	{
		Application.RequestUserAuthorization(UserAuthorization.Microphone);
	}

	private void OnJoinedVoiceRoom()
	{
		Debug.Log(PhotonVoiceNetwork.CurrentRoomName + " joined");
	}

	public void VoiceDetectorCalibrate(int durationMs)
	{
		if (photonView.isMine)
		{
			voice.VoiceDetectorCalibrate(durationMs);
		}
	}

	private string tostr<T>(T[] x, int lim = 10)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < ((x.Length >= lim) ? lim : x.Length); i++)
		{
			stringBuilder.Append("-");
			stringBuilder.Append(x[i]);
		}
		return stringBuilder.ToString();
	}

	public string ToStringFull()
	{
		int minFreq = 0;
		int maxFreq = 0;
		Microphone.GetDeviceCaps(MicrophoneDevice, out minFreq, out maxFreq);
		return $"Mic '{MicrophoneDevice}': {minFreq}..{maxFreq} Hz";
	}
}
