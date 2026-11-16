using System;
using POpusCodec;
using POpusCodec.Enums;

namespace ExitGames.Client.Photon.Voice;

internal class RemoteVoice : IDisposable
{
	private int channelId;

	private int playerId;

	private byte voiceId;

	internal byte lastEvNumber;

	private OpusDecoder opusDecoder;

	private VoiceClient voiceClient;

	internal VoiceInfo Info { get; private set; }

	internal object LocalUserObject { get; set; }

	internal RemoteVoice(VoiceClient client, int channelId, int playerId, byte voiceId, VoiceInfo info, byte lastEventNumber)
	{
		opusDecoder = new OpusDecoder((SamplingRate)info.SamplingRate, (Channels)info.Channels);
		voiceClient = client;
		this.channelId = channelId;
		this.playerId = playerId;
		this.voiceId = voiceId;
		Info = info;
		lastEvNumber = lastEventNumber;
	}

	internal void receiveBytes(byte[] receivedBytes, byte evNumber)
	{
		if (evNumber != lastEvNumber)
		{
			int num = VoiceUtil.byteDiff(evNumber, lastEvNumber);
			if (num != 0)
			{
				voiceClient.frontend.LogDebug("[PV] evNumer: " + evNumber + " playerVoice.lastEvNumber: " + lastEvNumber + " missing: " + num);
			}
			lastEvNumber = evNumber;
			for (int i = 0; i < num; i++)
			{
				receiveFrame(null);
			}
			voiceClient.FramesLost += num;
		}
		receiveFrame(receivedBytes);
	}

	internal void receiveFrame(byte[] frame)
	{
		if (voiceClient.OnAudioFrameShortAction != null)
		{
			short[] frame2 = decompressShort(frame);
			voiceClient.OnAudioFrameShortAction(channelId, playerId, voiceId, frame2, LocalUserObject);
		}
		if (voiceClient.OnAudioFrameFloatAction != null)
		{
			float[] frame3 = decompressFloat(frame);
			voiceClient.OnAudioFrameFloatAction(channelId, playerId, voiceId, frame3, LocalUserObject);
		}
	}

	internal short[] decompressShort(byte[] buffer)
	{
		short[] array;
		if (buffer == null)
		{
			array = opusDecoder.DecodePacketShort(null);
			voiceClient.frontend.LogDebug("[PV] lost packet decoded length: " + array.Length);
		}
		else
		{
			array = opusDecoder.DecodePacketShort(buffer);
		}
		return array;
	}

	internal float[] decompressFloat(byte[] buffer)
	{
		float[] array;
		if (buffer == null)
		{
			array = opusDecoder.DecodePacketFloat(null);
			voiceClient.frontend.LogDebug("[PV] lost packet decoded length: " + array.Length);
		}
		else
		{
			array = opusDecoder.DecodePacketFloat(buffer);
		}
		return array;
	}

	public void Dispose()
	{
		if (opusDecoder != null)
		{
			opusDecoder.Dispose();
		}
	}
}
