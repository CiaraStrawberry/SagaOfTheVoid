using ExitGames.Client.Photon.LoadBalancing;
using UnityEngine;
using UnityEngine.UI;

namespace ExitGames.Demos.DemoPunVoice;

[RequireComponent(typeof(Canvas))]
public class Highlighter : MonoBehaviour
{
	private Canvas canvas;

	[SerializeField]
	private PhotonVoiceRecorder recorder;

	[SerializeField]
	private PhotonVoiceSpeaker speaker;

	[SerializeField]
	private Image recorderSprite;

	[SerializeField]
	private Image speakerSprite;

	[SerializeField]
	private Text bufferLagText;

	private bool showSpeakerLag;

	private void OnEnable()
	{
		ChangePOV.CameraChanged += ChangePOV_CameraChanged;
		VoiceDemoUI.DebugToggled += VoiceDemoUI_DebugToggled;
	}

	private void OnDisable()
	{
		ChangePOV.CameraChanged -= ChangePOV_CameraChanged;
		VoiceDemoUI.DebugToggled -= VoiceDemoUI_DebugToggled;
	}

	private void VoiceDemoUI_DebugToggled(bool debugMode)
	{
		showSpeakerLag = debugMode;
	}

	private void ChangePOV_CameraChanged(Camera camera)
	{
		canvas.worldCamera = camera;
	}

	private void Awake()
	{
		canvas = GetComponent<Canvas>();
		if (canvas != null && canvas.worldCamera == null)
		{
			canvas.worldCamera = Camera.main;
		}
	}

	private void Update()
	{
		recorderSprite.enabled = recorder != null && recorder.IsTransmitting && PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined;
		speakerSprite.enabled = speaker != null && speaker.IsPlaying && PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined;
		bufferLagText.enabled = showSpeakerLag && speaker.IsPlaying && speaker.IsVoiceLinked;
		bufferLagText.text = $"{speaker.CurrentBufferLag}";
	}

	private void LateUpdate()
	{
		if (!(canvas == null) && !(canvas.worldCamera == null))
		{
			transform.rotation = Quaternion.Euler(0f, canvas.worldCamera.transform.eulerAngles.y, 0f);
		}
	}
}
