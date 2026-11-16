using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundEventC
{
	public bool enabled = true;

	public bool sendToManager;

	[NonSerialized]
	public bool initializedTarget;

	[NonSerialized]
	public bool initializedEvent;

	public PlaygroundParticlesC target;

	public EVENTBROADCASTC broadcastType;

	public EVENTTYPEC eventType;

	public EVENTINHERITANCEC eventInheritancePosition;

	public EVENTINHERITANCEC eventInheritanceVelocity;

	public EVENTINHERITANCEC eventInheritanceColor;

	public Vector3 eventPosition;

	public VALUEMETHOD velocityValueMethod;

	public MINMAXVECTOR3METHOD velocityRandomMethod;

	public Vector3 eventVelocity;

	public Vector3 eventVelocityMin;

	public Color32 eventColor = Color.white;

	public int eventEmitCount = 1;

	public float eventTime = 1f;

	public float collisionThreshold = 10f;

	public float velocityMultiplier = 1f;

	private float timer;

	public event OnPlaygroundParticle particleEvent;

	public void Initialize()
	{
		initializedTarget = target != null && target.gameObject.activeSelf && target.gameObject.activeInHierarchy && target.enabled;
		initializedEvent = particleEvent != null;
	}

	public bool UpdateTime()
	{
		bool result = false;
		timer += PlaygroundC.globalDeltaTime;
		if (timer >= eventTime)
		{
			result = true;
			timer = 0f;
		}
		return result;
	}

	public void SetTimer(float newTime)
	{
		timer = newTime;
	}

	public void SendParticleEvent(PlaygroundEventParticle eventParticle)
	{
		if (initializedEvent)
		{
			particleEvent(eventParticle);
		}
	}

	public PlaygroundEventC Clone()
	{
		PlaygroundEventC playgroundEventC = new PlaygroundEventC();
		playgroundEventC.enabled = enabled;
		playgroundEventC.target = target;
		playgroundEventC.eventType = eventType;
		playgroundEventC.eventInheritancePosition = eventInheritancePosition;
		playgroundEventC.eventInheritanceVelocity = eventInheritanceVelocity;
		playgroundEventC.eventInheritanceColor = eventInheritanceColor;
		playgroundEventC.eventPosition = eventPosition;
		playgroundEventC.eventVelocity = eventVelocity;
		playgroundEventC.eventVelocityMin = eventVelocityMin;
		playgroundEventC.velocityRandomMethod = velocityRandomMethod;
		playgroundEventC.velocityValueMethod = velocityValueMethod;
		playgroundEventC.eventColor = eventColor;
		playgroundEventC.eventEmitCount = eventEmitCount;
		playgroundEventC.eventTime = eventTime;
		playgroundEventC.collisionThreshold = collisionThreshold;
		playgroundEventC.velocityMultiplier = velocityMultiplier;
		playgroundEventC.particleEvent = particleEvent;
		playgroundEventC.broadcastType = broadcastType;
		return playgroundEventC;
	}
}
