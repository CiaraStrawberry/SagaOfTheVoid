using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class ManipulatorObjectC
{
	public MANIPULATORTYPEC type;

	public ManipulatorPropertyC property = new ManipulatorPropertyC();

	public List<ManipulatorPropertyC> properties = new List<ManipulatorPropertyC>();

	public LayerMask affects;

	public PlaygroundTransformC transform = new PlaygroundTransformC();

	public MANIPULATORSHAPEC shape;

	public float size;

	public Bounds bounds;

	public float strength;

	public float strengthSmoothing = 1f;

	public float strengthDistanceEffect = 1f;

	public bool applyParticleLifetimeStrength;

	public AnimationCurve particleLifetimeStrength = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

	public bool enabled = true;

	public bool inverseBounds;

	public int manipulatorId;

	public bool applyLifetimeFilter;

	public float lifetimeFilterMinimum;

	public float lifetimeFilterMaximum = 1f;

	public bool applyParticleFilter;

	public float particleFilterMinimum;

	public float particleFilterMaximum = 1f;

	public PlaygroundAxisConstraintsC axisConstraints = new PlaygroundAxisConstraintsC();

	public bool unfolded;

	public bool trackParticles;

	public TrackingMethod trackingMethod;

	public bool sendEventEnter = true;

	public bool sendEventExit = true;

	public bool sendEventBirth = true;

	public bool sendEventDeath = true;

	public bool sendEventCollision = true;

	public bool sendEventsUnfolded;

	[NonSerialized]
	public List<ManipulatorParticle> particles = new List<ManipulatorParticle>();

	[NonSerialized]
	public List<ManipulatorParticle> nonAffectedParticles = new List<ManipulatorParticle>();

	public PlaygroundEventParticle manipulatorEventParticle = new PlaygroundEventParticle();

	[NonSerialized]
	private bool initializedEventEnter;

	[NonSerialized]
	private bool initializedEventExit;

	[NonSerialized]
	private bool initializedEventBirth;

	[NonSerialized]
	private bool initializedEventDeath;

	[NonSerialized]
	private bool initializedEventCollision;

	public event OnPlaygroundParticle particleEventEnter;

	public event OnPlaygroundParticle particleEventExit;

	public event OnPlaygroundParticle particleEventBirth;

	public event OnPlaygroundParticle particleEventDeath;

	public event OnPlaygroundParticle particleEventCollision;

	public bool Contains(Vector3 position, Vector3 mPosition)
	{
		if (shape == MANIPULATORSHAPEC.Infinite)
		{
			return true;
		}
		if (shape == MANIPULATORSHAPEC.Box)
		{
			if (!inverseBounds)
			{
				return bounds.Contains(transform.inverseRotation * (position - mPosition));
			}
			return !bounds.Contains(transform.inverseRotation * (position - mPosition));
		}
		if (!inverseBounds)
		{
			return Vector3.Distance(position, mPosition) <= size;
		}
		return Vector3.Distance(position, mPosition) >= size;
	}

	public void SendParticleEventEnter()
	{
		if (initializedEventEnter)
		{
			particleEventEnter(manipulatorEventParticle);
		}
	}

	public void SendParticleEventExit()
	{
		if (initializedEventExit)
		{
			particleEventExit(manipulatorEventParticle);
		}
	}

	public void SendParticleEventBirth()
	{
		if (initializedEventBirth)
		{
			particleEventBirth(manipulatorEventParticle);
		}
	}

	public void SendParticleEventDeath()
	{
		if (initializedEventDeath)
		{
			particleEventDeath(manipulatorEventParticle);
		}
	}

	public void SendParticleEventCollision()
	{
		if (initializedEventCollision)
		{
			particleEventCollision(manipulatorEventParticle);
		}
	}

	public PlaygroundEventParticle GetParticle(int index)
	{
		if (particles == null || particles.Count == 0 || index > particles.Count)
		{
			return null;
		}
		return GetParticle(particles[index].particleSystemId, particles[index].particleId);
	}

	public PlaygroundEventParticle GetParticle(int particleSystemId, int particleId)
	{
		PlaygroundEventParticle playgroundEventParticle = new PlaygroundEventParticle();
		if (PlaygroundC.reference.particleSystems[particleSystemId].UpdateEventParticle(playgroundEventParticle, particleId))
		{
			return playgroundEventParticle;
		}
		return null;
	}

	public List<PlaygroundEventParticle> GetParticles()
	{
		List<PlaygroundEventParticle> list = new List<PlaygroundEventParticle>();
		for (int i = 0; i < particles.Count; i++)
		{
			if (PlaygroundC.reference.particleSystems[particles[i].particleSystemId].UpdateEventParticle(manipulatorEventParticle, particles[i].particleId))
			{
				list.Add(manipulatorEventParticle.Clone());
			}
		}
		return list;
	}

	public bool ContainsParticle(int particleSystemId, int particleId)
	{
		for (int i = 0; i < particles.Count; i++)
		{
			if (particles[i % particles.Count].particleSystemId == particleSystemId && particles[i % particles.Count].particleId == particleId)
			{
				return true;
			}
		}
		return false;
	}

	public bool IsSameId(int compareId)
	{
		if (manipulatorId == compareId)
		{
			return true;
		}
		return false;
	}

	public bool AddParticle(int particleSystemId, int particleId)
	{
		if (particles == null)
		{
			return false;
		}
		for (int i = 0; i < particles.Count; i++)
		{
			if (particles == null || particles[i] == null)
			{
				return false;
			}
			if (particles[i % particles.Count].particleSystemId == particleSystemId && particles[i % particles.Count].particleId == particleId)
			{
				return false;
			}
		}
		particles.Add(new ManipulatorParticle(particleSystemId, particleId));
		return true;
	}

	public bool RemoveParticle(int particleSystemId, int particleId)
	{
		if (particles == null)
		{
			return false;
		}
		bool result = false;
		for (int i = 0; i < particles.Count; i++)
		{
			if (particles == null || particles[i] == null)
			{
				return false;
			}
			if (particles[i % particles.Count].particleSystemId == particleSystemId && particles[i % particles.Count].particleId == particleId)
			{
				particles.RemoveAt(i % particles.Count);
				result = true;
			}
		}
		return result;
	}

	public void KillAllParticles()
	{
		if (particles == null)
		{
			return;
		}
		for (int i = 0; i < particles.Count && i < particles.Count && i >= 0; i++)
		{
			PlaygroundC.reference.particleSystems[particles[i].particleSystemId].Kill(particles[i].particleId);
			particles.RemoveAt(i);
			if (i > 0)
			{
				i--;
			}
		}
	}

	public bool ContainsNonAffectedParticle(int particleSystemId, int particleId)
	{
		for (int i = 0; i < nonAffectedParticles.Count; i++)
		{
			if (nonAffectedParticles[i].particleSystemId == particleSystemId && nonAffectedParticles[i].particleId == particleId)
			{
				return true;
			}
		}
		return false;
	}

	public void AddNonAffectedParticle(int particleSystemId, int particleId)
	{
		nonAffectedParticles.Add(new ManipulatorParticle(particleSystemId, particleId));
	}

	public bool RemoveNonAffectedParticle(int particleSystemId, int particleId)
	{
		for (int i = 0; i < nonAffectedParticles.Count; i++)
		{
			if (nonAffectedParticles[i].particleSystemId == particleSystemId && nonAffectedParticles[i].particleId == particleId)
			{
				nonAffectedParticles.RemoveAt(i);
				return true;
			}
		}
		return false;
	}

	public bool LifetimeFilter(float life, float total)
	{
		if (!applyLifetimeFilter)
		{
			return true;
		}
		float num = life / total;
		return num >= lifetimeFilterMinimum && num <= lifetimeFilterMaximum;
	}

	public bool ParticleFilter(int p, int total)
	{
		if (!applyParticleFilter)
		{
			return true;
		}
		float num = (float)p * 1f / ((float)total * 1f);
		return num >= particleFilterMinimum && num <= particleFilterMaximum;
	}

	public void Update()
	{
		if (!enabled || !transform.Update())
		{
			return;
		}
		manipulatorId = transform.instanceID;
		if (trackParticles)
		{
			initializedEventEnter = particleEventEnter != null;
			initializedEventExit = particleEventExit != null;
			initializedEventBirth = particleEventBirth != null;
			initializedEventDeath = particleEventDeath != null;
			initializedEventCollision = particleEventCollision != null;
		}
		else if (particles.Count > 0)
		{
			particles.Clear();
		}
		if (type != MANIPULATORTYPEC.Property && type != MANIPULATORTYPEC.Combined)
		{
			return;
		}
		property.Update();
		property.SetLocalVelocity(transform.rotation, property.velocity);
		if (property.type == MANIPULATORPROPERTYTYPEC.Math)
		{
			property.UpdateMathProperty(transform.position);
		}
		for (int i = 0; i < properties.Count; i++)
		{
			properties[i].Update();
			properties[i].SetLocalVelocity(transform.rotation, properties[i].velocity);
			if (properties[i].type == MANIPULATORPROPERTYTYPEC.Math)
			{
				properties[i].UpdateMathProperty(transform.position);
			}
		}
	}

	public void SetTransform(Transform transform)
	{
		this.transform.SetFromTransform(transform);
	}

	public void SetLocalTargetsPosition(Transform otherTransform)
	{
		property.SetLocalTargetsPosition(otherTransform);
		for (int i = 0; i < properties.Count; i++)
		{
			properties[i].SetLocalTargetsPosition(otherTransform);
		}
	}

	public ManipulatorObjectC Clone()
	{
		ManipulatorObjectC manipulatorObjectC = new ManipulatorObjectC();
		manipulatorObjectC.type = type;
		manipulatorObjectC.property = property.Clone();
		manipulatorObjectC.affects = affects;
		manipulatorObjectC.transform = transform.Clone();
		manipulatorObjectC.size = size;
		manipulatorObjectC.shape = shape;
		manipulatorObjectC.bounds = bounds;
		manipulatorObjectC.strength = strength;
		manipulatorObjectC.applyParticleLifetimeStrength = applyParticleLifetimeStrength;
		manipulatorObjectC.particleLifetimeStrength = new AnimationCurve(particleLifetimeStrength.keys);
		manipulatorObjectC.strengthSmoothing = strengthSmoothing;
		manipulatorObjectC.strengthDistanceEffect = strengthDistanceEffect;
		manipulatorObjectC.enabled = enabled;
		manipulatorObjectC.applyLifetimeFilter = applyLifetimeFilter;
		manipulatorObjectC.lifetimeFilterMinimum = lifetimeFilterMinimum;
		manipulatorObjectC.lifetimeFilterMaximum = lifetimeFilterMaximum;
		manipulatorObjectC.applyParticleFilter = applyParticleFilter;
		manipulatorObjectC.particleFilterMinimum = particleFilterMinimum;
		manipulatorObjectC.particleFilterMaximum = particleFilterMaximum;
		manipulatorObjectC.inverseBounds = inverseBounds;
		manipulatorObjectC.trackParticles = trackParticles;
		manipulatorObjectC.sendEventEnter = sendEventEnter;
		manipulatorObjectC.sendEventExit = sendEventExit;
		manipulatorObjectC.sendEventBirth = sendEventBirth;
		manipulatorObjectC.sendEventDeath = sendEventDeath;
		manipulatorObjectC.sendEventCollision = sendEventCollision;
		manipulatorObjectC.properties = new List<ManipulatorPropertyC>();
		manipulatorObjectC.axisConstraints = axisConstraints.Clone();
		for (int i = 0; i < properties.Count; i++)
		{
			manipulatorObjectC.properties.Add(properties[i].Clone());
		}
		return manipulatorObjectC;
	}
}
