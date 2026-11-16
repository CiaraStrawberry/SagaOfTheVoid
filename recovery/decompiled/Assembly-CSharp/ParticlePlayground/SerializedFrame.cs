using System;

namespace ParticlePlayground;

[Serializable]
public class SerializedFrame
{
	public SerializedParticle[] serializedParticles;

	public float timeStamp;

	public float keyframeInterval;

	public FrameType frameType = FrameType.Middle;

	public RecordedFrame CloneAsRecordedFrame()
	{
		RecordedFrame recordedFrame = new RecordedFrame();
		recordedFrame.particles = CloneAsPlaybackParticles();
		recordedFrame.keyframeInterval = keyframeInterval;
		recordedFrame.timeStamp = timeStamp;
		recordedFrame.frameType = frameType;
		return recordedFrame;
	}

	public PlaybackParticle[] CloneAsPlaybackParticles()
	{
		PlaybackParticle[] array = new PlaybackParticle[serializedParticles.Length];
		for (int i = 0; i < serializedParticles.Length; i++)
		{
			ref PlaybackParticle reference = ref array[i];
			reference = serializedParticles[i].CloneAsPlaybackParticle();
		}
		return array;
	}
}
