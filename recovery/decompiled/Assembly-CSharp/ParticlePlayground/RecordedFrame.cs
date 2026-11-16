using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class RecordedFrame
{
	[HideInInspector]
	public PlaybackParticle[] particles;

	[HideInInspector]
	public float timeStamp;

	[HideInInspector]
	public float keyframeInterval;

	[HideInInspector]
	public FrameType frameType = FrameType.Middle;

	public RecordedFrame()
	{
	}

	public RecordedFrame(PlaygroundParticlesC playgroundParticles, float keyframeInterval)
	{
		particles = new PlaybackParticle[playgroundParticles.particleCache.Length];
		for (int i = 0; i < particles.Length; i++)
		{
			ref PlaybackParticle reference = ref particles[i];
			reference = new PlaybackParticle(playgroundParticles.playgroundCache.position[i], playgroundParticles.playgroundCache.velocity[i], playgroundParticles.playgroundCache.rotation[i], playgroundParticles.playgroundCache.size[i], playgroundParticles.particleCache[i].remainingLifetime, playgroundParticles.particleCache[i].startLifetime, playgroundParticles.playgroundCache.life[i], playgroundParticles.playgroundCache.birth[i], playgroundParticles.playgroundCache.death[i], playgroundParticles.playgroundCache.lifetimeSubtraction[i], playgroundParticles.playgroundCache.color[i], playgroundParticles.playgroundCache.targetPosition[i], playgroundParticles.playgroundCache.initialSize[i]);
		}
		timeStamp = Time.realtimeSinceStartup;
		this.keyframeInterval = keyframeInterval;
	}

	public RecordedFrame Clone()
	{
		RecordedFrame recordedFrame = new RecordedFrame();
		recordedFrame.particles = (PlaybackParticle[])particles.Clone();
		recordedFrame.timeStamp = timeStamp;
		recordedFrame.frameType = frameType;
		return recordedFrame;
	}

	public ParticleSystem.Particle[] CloneAsParticles()
	{
		ParticleSystem.Particle[] array = new ParticleSystem.Particle[particles.Length];
		for (int i = 0; i < array.Length; i++)
		{
			ref ParticleSystem.Particle reference = ref array[i];
			reference = particles[i].CloneAsParticle();
		}
		return array;
	}

	public SerializedFrame CloneAsSerializedFrame()
	{
		SerializedFrame serializedFrame = new SerializedFrame();
		serializedFrame.serializedParticles = CloneAsSerializedParticles();
		serializedFrame.keyframeInterval = keyframeInterval;
		serializedFrame.timeStamp = timeStamp;
		serializedFrame.frameType = frameType;
		return serializedFrame;
	}

	public SerializedParticle[] CloneAsSerializedParticles()
	{
		SerializedParticle[] array = new SerializedParticle[particles.Length];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = particles[i].CloneAsSerializedParticle();
		}
		return array;
	}
}
