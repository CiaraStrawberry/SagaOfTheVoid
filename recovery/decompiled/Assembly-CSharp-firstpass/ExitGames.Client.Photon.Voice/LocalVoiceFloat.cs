using System;

namespace ExitGames.Client.Photon.Voice;

public class LocalVoiceFloat : LocalVoice<float>
{
	internal LocalVoiceFloat()
	{
		levelMeter = new LevelMeterFloat(0, 0);
		voiceDetector = new VoiceDetectorFloat(0, 0);
	}

	internal LocalVoiceFloat(VoiceClient voiceClient, byte id, IAudioStream<float> audioStream, VoiceInfo voiceInfo, int channelId)
		: base(voiceClient, id, audioStream, voiceInfo, channelId)
	{
		levelMeter = new LevelMeterFloat(sourceSamplingRateHz, info.Channels);
		voiceDetector = new VoiceDetectorFloat(sourceSamplingRateHz, info.Channels);
	}

	protected override ArraySegment<byte> compress(float[] buffer)
	{
		return opusEncoder.Encode(buffer);
	}
}
