using ExitGames.Client.Photon;
using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice;

public class VoiceDemoUI : MonoBehaviour
{
	public delegate void OnDebugToggle(bool debugMode);

	[SerializeField]
	private Text punState;

	[SerializeField]
	private Text voiceState;

	private Canvas canvas;

	[SerializeField]
	private Button punSwitch;

	private Text punSwitchText;

	[SerializeField]
	private Button voiceSwitch;

	private Text voiceSwitchText;

	[SerializeField]
	private Button calibrateButton;

	private Text calibrateText;

	[SerializeField]
	private Text voiceDebugText;

	private PhotonVoiceRecorder rec;

	[SerializeField]
	private GameObject inGameSettings;

	[SerializeField]
	private GameObject globalSettings;

	[SerializeField]
	private Text devicesInfoText;

	private GameObject debugGO;

	private bool debugMode;

	private float volumeBeforeMute;

	private DebugLevel previousDebugLevel;

	[SerializeField]
	private int calibrationMilliSeconds = 2000;

	public bool DebugMode
	{
		get
		{
			return debugMode;
		}
		set
		{
			debugMode = value;
			debugGO.SetActive(debugMode);
			voiceDebugText.text = string.Empty;
			if (debugMode)
			{
				previousDebugLevel = PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut;
				PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut = DebugLevel.ALL;
			}
			else
			{
				PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut = previousDebugLevel;
			}
			if (DebugToggled != null)
			{
				DebugToggled(debugMode);
			}
		}
	}

	public static event OnDebugToggle DebugToggled;

	private void OnEnable()
	{
		ChangePOV.CameraChanged += OnCameraChanged;
		CharacterInstantiation.CharacterInstantiated += CharacterInstantiation_CharacterInstantiated;
		BetterToggle.ToggleValueChanged += BetterToggle_ToggleValueChanged;
	}

	private void OnDisable()
	{
		ChangePOV.CameraChanged -= OnCameraChanged;
		CharacterInstantiation.CharacterInstantiated -= CharacterInstantiation_CharacterInstantiated;
		BetterToggle.ToggleValueChanged -= BetterToggle_ToggleValueChanged;
	}

	private void InitToggles(Toggle[] toggles)
	{
		if (toggles == null)
		{
			return;
		}
		foreach (Toggle toggle in toggles)
		{
			switch (toggle.name)
			{
			case "Mute":
				toggle.isOn = AudioListener.volume <= 0.001f;
				break;
			case "AutoTransmit":
				toggle.isOn = PhotonVoiceSettings.Instance.AutoTransmit;
				break;
			case "VoiceDetection":
				toggle.isOn = PhotonVoiceSettings.Instance.VoiceDetection;
				break;
			case "AutoConnect":
				toggle.isOn = PhotonVoiceSettings.Instance.AutoConnect;
				break;
			case "AutoDisconnect":
				toggle.isOn = PhotonVoiceSettings.Instance.AutoDisconnect;
				break;
			case "DebugVoice":
				DebugMode = PhotonVoiceSettings.Instance.DebugInfo;
				toggle.isOn = DebugMode;
				break;
			case "Transmit":
				toggle.isOn = rec != null && rec.Transmit;
				break;
			case "DebugEcho":
				toggle.isOn = PhotonVoiceNetwork.Client.DebugEchoMode;
				break;
			}
		}
	}

	private void CharacterInstantiation_CharacterInstantiated(GameObject character)
	{
		rec = character.GetComponent<PhotonVoiceRecorder>();
		rec.enabled = true;
	}

	private void BetterToggle_ToggleValueChanged(Toggle toggle)
	{
		switch (toggle.name)
		{
		case "Mute":
			if (toggle.isOn)
			{
				volumeBeforeMute = AudioListener.volume;
				AudioListener.volume = 0f;
			}
			else
			{
				AudioListener.volume = volumeBeforeMute;
				volumeBeforeMute = 0f;
			}
			break;
		case "Transmit":
			if ((bool)rec)
			{
				rec.Transmit = toggle.isOn;
			}
			break;
		case "VoiceDetection":
			PhotonVoiceSettings.Instance.VoiceDetection = toggle.isOn;
			if ((bool)rec)
			{
				rec.Detect = toggle.isOn;
			}
			break;
		case "DebugEcho":
			PhotonVoiceNetwork.Client.DebugEchoMode = toggle.isOn;
			break;
		case "AutoConnect":
			PhotonVoiceSettings.Instance.AutoConnect = toggle.isOn;
			break;
		case "AutoDisconnect":
			PhotonVoiceSettings.Instance.AutoDisconnect = toggle.isOn;
			break;
		case "AutoTransmit":
			PhotonVoiceSettings.Instance.AutoTransmit = toggle.isOn;
			break;
		case "DebugVoice":
			DebugMode = toggle.isOn;
			PhotonVoiceSettings.Instance.DebugInfo = DebugMode;
			break;
		}
	}

	private void OnCameraChanged(Camera newCamera)
	{
		canvas.worldCamera = newCamera;
	}

	private void Start()
	{
		canvas = GetComponentInChildren<Canvas>();
		if (punSwitch != null)
		{
			punSwitchText = punSwitch.GetComponentInChildren<Text>();
			punSwitch.onClick.AddListener(PunSwitchOnClick);
		}
		if ((bool)voiceSwitch)
		{
			voiceSwitchText = voiceSwitch.GetComponentInChildren<Text>();
			voiceSwitch.onClick.AddListener(VoiceSwitchOnClick);
		}
		if (calibrateButton != null)
		{
			calibrateButton.onClick.AddListener(CalibrateButtonOnClick);
			calibrateText = calibrateButton.GetComponentInChildren<Text>();
		}
		if (punState != null)
		{
			debugGO = punState.transform.parent.gameObject;
		}
		volumeBeforeMute = AudioListener.volume;
		previousDebugLevel = PhotonVoiceNetwork.Client.loadBalancingPeer.DebugOut;
		if (globalSettings != null)
		{
			globalSettings.SetActive(value: true);
			InitToggles(globalSettings.GetComponentsInChildren<Toggle>());
		}
		if (!(devicesInfoText != null))
		{
			return;
		}
		if (Microphone.devices == null || Microphone.devices.Length == 0)
		{
			devicesInfoText.enabled = true;
			devicesInfoText.color = Color.red;
			devicesInfoText.text = "No microphone device detected!";
			return;
		}
		if (Microphone.devices.Length == 1)
		{
			devicesInfoText.text = $"Mic.: {Microphone.devices[0]}";
			return;
		}
		devicesInfoText.text = $"Multi.Mic.Devices:\n0. {Microphone.devices[0]} (active)\n";
		for (int i = 1; i < Microphone.devices.Length; i++)
		{
			devicesInfoText.text += $"{i}. {Microphone.devices[i]}\n";
		}
	}

	private void PunSwitchOnClick()
	{
		if (PhotonNetwork.connectionStateDetailed == ClientState.Joined)
		{
			PhotonNetwork.Disconnect();
		}
		else if (PhotonNetwork.connectionStateDetailed == ClientState.Disconnected || PhotonNetwork.connectionStateDetailed == ClientState.PeerCreated)
		{
			PhotonNetwork.ConnectUsingSettings($"1.{Application.loadedLevel}");
		}
	}

	private void VoiceSwitchOnClick()
	{
		if (PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined)
		{
			PhotonVoiceNetwork.Disconnect();
		}
		else if (PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Uninitialized || PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected)
		{
			PhotonVoiceNetwork.Connect();
		}
	}

	private void CalibrateButtonOnClick()
	{
		if ((bool)rec && !rec.VoiceDetectorCalibrating)
		{
			rec.VoiceDetectorCalibrate(calibrationMilliSeconds);
		}
	}

	private void Update()
	{
		switch (PhotonNetwork.connectionStateDetailed)
		{
		case ClientState.PeerCreated:
		case ClientState.Disconnected:
			punSwitch.interactable = true;
			punSwitchText.text = "PUN Connect";
			if (rec != null)
			{
				rec.enabled = false;
				rec = null;
			}
			break;
		case ClientState.Joined:
			punSwitch.interactable = true;
			punSwitchText.text = "PUN Disconnect";
			break;
		default:
			punSwitch.interactable = false;
			punSwitchText.text = "PUN busy";
			break;
		}
		switch (PhotonVoiceNetwork.ClientState)
		{
		case ExitGames.Client.Photon.LoadBalancing.ClientState.Joined:
			voiceSwitch.interactable = true;
			voiceSwitchText.text = "Voice Disconnect";
			inGameSettings.SetActive(value: true);
			InitToggles(inGameSettings.GetComponentsInChildren<Toggle>());
			if (rec != null)
			{
				calibrateButton.interactable = !rec.VoiceDetectorCalibrating;
				calibrateText.text = ((!rec.VoiceDetectorCalibrating) ? $"Calibrate ({calibrationMilliSeconds / 1000}s)" : "Calibrating");
			}
			else
			{
				calibrateButton.interactable = false;
				calibrateText.text = "Unavailable";
			}
			break;
		case ExitGames.Client.Photon.LoadBalancing.ClientState.Uninitialized:
		case ExitGames.Client.Photon.LoadBalancing.ClientState.Disconnected:
			if (PhotonNetwork.inRoom)
			{
				voiceSwitch.interactable = true;
				voiceSwitchText.text = "Voice Connect";
				voiceDebugText.text = string.Empty;
			}
			else
			{
				voiceSwitch.interactable = false;
				voiceSwitchText.text = "Voice N/A";
				voiceDebugText.text = string.Empty;
			}
			calibrateButton.interactable = false;
			calibrateText.text = "Unavailable";
			inGameSettings.SetActive(value: false);
			break;
		default:
			voiceSwitch.interactable = false;
			voiceSwitchText.text = "Voice busy";
			break;
		}
		if (debugMode)
		{
			punState.text = $"PUN: {PhotonNetwork.connectionStateDetailed}";
			voiceState.text = $"PhotonVoice: {PhotonVoiceNetwork.ClientState}";
			if (rec != null && rec.LevelMeter != null)
			{
				voiceDebugText.text = string.Format("Amp: avg. {0}, peak {1}", rec.LevelMeter.CurrentAvgAmp.ToString("0.000000"), rec.LevelMeter.CurrentPeakAmp.ToString("0.000000"));
			}
		}
	}
}
