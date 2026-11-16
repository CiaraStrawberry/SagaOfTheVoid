using System;

namespace ExitGames.Client.Photon.Voice;

public class LocalVoiceShort : LocalVoice<short>
{
	internal LocalVoiceShort()
	{
		levelMeter = new LevelMeterShort(0, 0);
		voiceDetector = new VoiceDetectorShort(0, 0);
	}

	internal LocalVoiceShort(VoiceClient voiceClient, byte id, IAudioStream<short> audioStream, VoiceInfo voiceInfo, int channelId)
		: base(voiceClient, id, audioStream, voiceInfo, channelId)
	{
		levelMeter = new LevelMeterShort(sourceSamplingRateHz, info.Channels);
		voiceDetector = new VoiceDetectorShort(sourceSamplingRateHz, info.Channels);
	}

	protected override ArraySegment<byte> compress(short[] buffer)
	{
		return opusEncoder.Encode(buffer);
	}
}
