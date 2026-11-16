using System;
using System.Collections.Generic;
using POpusCodec;
using POpusCodec.Enums;

namespace ExitGames.Client.Photon.Voice;

public abstract class LocalVoice : IDisposable
{
	public static LocalVoice Dummy = new LocalVoiceFloat();

	protected int voiceDetectorCalibrateCount;

	internal VoiceInfo info;

	protected OpusEncoder opusEncoder;

	internal byte id;

	internal int channelId;

	internal byte evNumber;

	private VoiceClient voiceClient;

	protected int sourceSamplingRateHz;

	internal int sourceFrameSize;

	internal Dictionary<byte, int> eventTimestamps = new Dictionary<byte, int>();

	public byte AudioGroup { get; set; }

	public bool Transmit { get; set; }

	public abstract bool IsTransmitting { get; }

	public int FramesSent { get; private set; }

	public int FramesSentBytes { get; private set; }

	public abstract VoiceDetector VoiceDetector { get; }

	public abstract LevelMeter LevelMeter { get; }

	public bool VoiceDetectorCalibrating => voiceDetectorCalibrateCount > 0;

	internal LocalVoice()
	{
	}

	internal LocalVoice(VoiceClient voiceClient, byte id, IAudioStreamBase audioStream, VoiceInfo voiceInfo, int channelId)
	{
		info = voiceInfo;
		this.channelId = channelId;
		opusEncoder = new OpusEncoder((SamplingRate)voiceInfo.SamplingRate, (Channels)voiceInfo.Channels, voiceInfo.Bitrate, OpusApplicationType.Voip, (Delay)(voiceInfo.FrameDurationUs * 2 / 1000));
		this.voiceClient = voiceClient;
		this.id = id;
		sourceSamplingRateHz = audioStream.SamplingRate;
		sourceFrameSize = info.FrameSize * sourceSamplingRateHz / (int)opusEncoder.InputSamplingRate;
		if (sourceFrameSize != info.FrameSize)
		{
			sourceSamplingRateHz = audioStream.SamplingRate;
			this.voiceClient.frontend.LogWarning("[PV] Local voice #" + this.id + " audio source frequency " + sourceSamplingRateHz + " and encoder sampling rate " + (int)opusEncoder.InputSamplingRate + " do not match. Resampling will occur before encoding.");
		}
	}

	public void VoiceDetectorCalibrate(int durationMs)
	{
		voiceDetectorCalibrateCount = sourceSamplingRateHz * (int)opusEncoder.InputChannels * durationMs / 1000;
		LevelMeter.ResetAccumAvgPeakAmp();
	}

	internal void service()
	{
		while (processStream())
		{
		}
	}

	internal abstract bool readStream();

	internal abstract ArraySegment<byte> compress();

	private bool processStream()
	{
		if (voiceClient.frontend.IsChannelJoined(channelId) && Transmit)
		{
			if (readStream())
			{
				ArraySegment<byte> arraySegment = compress();
				FramesSent++;
				FramesSentBytes += arraySegment.Count;
				object obj = arraySegment;
				if (!voiceClient.frontend.SupportsArraySegmentSerialization)
				{
					byte[] array = new byte[arraySegment.Count];
					Buffer.BlockCopy(arraySegment.Array, arraySegment.Offset, array, 0, arraySegment.Count);
					obj = array;
				}
				object[] content = new object[3] { id, evNumber, obj };
				voiceClient.frontend.SendFrame(content, channelId, AudioGroup);
				eventTimestamps[evNumber] = Environment.TickCount;
				evNumber++;
				return true;
			}
			return false;
		}
		return false;
	}

	public void RemoveSelf()
	{
		voiceClient.RemoveLocalVoice(this);
	}

	public virtual void Dispose()
	{
		if (opusEncoder != null)
		{
			opusEncoder.Dispose();
		}
	}
}
public abstract class LocalVoice<T> : LocalVoice
{
	private IAudioStream<T> audioStream;

	private T[] frameBuffer;

	private T[] sourceFrameBuffer;

	protected VoiceDetector<T> voiceDetector;

	protected LevelMeter<T> levelMeter;

	public override VoiceDetector VoiceDetector => voiceDetector;

	public override LevelMeter LevelMeter => levelMeter;

	public override bool IsTransmitting => Transmit && (!VoiceDetector.On || VoiceDetector.Detected);

	internal LocalVoice()
	{
	}

	internal LocalVoice(VoiceClient voiceClient, byte id, IAudioStream<T> audioStream, VoiceInfo voiceInfo, int channelId)
		: base(voiceClient, id, audioStream, voiceInfo, channelId)
	{
		this.audioStream = audioStream;
		frameBuffer = new T[info.FrameSize];
		if (sourceFrameSize == info.FrameSize)
		{
			sourceFrameBuffer = frameBuffer;
		}
		else
		{
			sourceFrameBuffer = new T[sourceFrameSize];
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		audioStream.Dispose();
	}

	internal override bool readStream()
	{
		if (!audioStream.GetData(sourceFrameBuffer))
		{
			return false;
		}
		levelMeter.process(sourceFrameBuffer);
		if (voiceDetectorCalibrateCount != 0)
		{
			voiceDetectorCalibrateCount -= sourceFrameBuffer.Length;
			if (voiceDetectorCalibrateCount <= 0)
			{
				voiceDetectorCalibrateCount = 0;
				VoiceDetector.Threshold = LevelMeter.AccumAvgPeakAmp * 2f;
			}
		}
		if (VoiceDetector.On)
		{
			voiceDetector.process(sourceFrameBuffer);
			if (!VoiceDetector.Detected)
			{
				return false;
			}
		}
		if (sourceFrameBuffer != frameBuffer)
		{
			VoiceUtil.Resample(sourceFrameBuffer, frameBuffer, (int)opusEncoder.InputChannels);
		}
		return true;
	}

	internal override ArraySegment<byte> compress()
	{
		return compress(frameBuffer);
	}

	protected abstract ArraySegment<byte> compress(T[] buffer);
}
