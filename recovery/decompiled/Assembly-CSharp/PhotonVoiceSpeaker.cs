using Photon;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[DisallowMultipleComponent]
public class PhotonVoiceSpeaker : Photon.MonoBehaviour
{
	private AudioStreamPlayer player;

	public float LastRecvTime { get; private set; }

	public bool IsPlaying => player.IsPlaying;

	public int CurrentBufferLag => player.CurrentBufferLag;

	public bool IsVoiceLinked => player != null && player.IsStarted;

	private void Awake()
	{
		player = new AudioStreamPlayer(GetComponent<AudioSource>(), "PUNVoice: PhotonVoiceSpeaker:", PhotonVoiceSettings.Instance.DebugInfo);
		PhotonVoiceNetwork.LinkSpeakerToRemoteVoice(this);
	}

	internal void OnVoiceLinked(int frequency, int channels, int frameSamplesPerChannel, int playDelayMs)
	{
		player.Start(frequency, channels, frameSamplesPerChannel, playDelayMs);
	}

	internal void OnVoiceUnlinked()
	{
		player.Stop();
	}

	private void Update()
	{
		player.Update();
	}

	private void OnDestroy()
	{
		PhotonVoiceNetwork.UnlinkSpeakerFromRemoteVoice(this);
		player.Stop();
	}

	private void OnApplicationQuit()
	{
		player.Stop();
	}

	internal void OnAudioFrame(float[] frame)
	{
		LastRecvTime = Time.time;
		player.OnAudioFrame(frame);
	}
}
