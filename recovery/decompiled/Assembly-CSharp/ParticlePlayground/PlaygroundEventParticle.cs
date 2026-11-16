using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundEventParticle
{
	[HideInInspector]
	public float initialSize;

	[HideInInspector]
	public float size;

	[HideInInspector]
	public float rotation;

	[HideInInspector]
	public Vector3 rotation3d;

	[HideInInspector]
	public float life;

	[HideInInspector]
	public float totalLifetime;

	[HideInInspector]
	public float birth;

	[HideInInspector]
	public float birthDelay;

	[HideInInspector]
	public float death;

	[HideInInspector]
	public bool emission;

	[HideInInspector]
	public bool rebirth;

	[HideInInspector]
	public float lifetimeOffset;

	[HideInInspector]
	public Vector3 velocity;

	[HideInInspector]
	public Vector3 initialVelocity;

	[HideInInspector]
	public Vector3 initialLocalVelocity;

	[HideInInspector]
	public Vector3 position;

	[HideInInspector]
	public Vector3 targetPosition;

	[HideInInspector]
	public Vector3 targetDirection;

	[HideInInspector]
	public Vector3 previousTargetPosition;

	[HideInInspector]
	public Vector3 previousParticlePosition;

	[HideInInspector]
	public Vector3 collisionParticlePosition;

	[HideInInspector]
	public Vector3 localSpaceMovementCompensation;

	[HideInInspector]
	public Vector3 scatterPosition;

	[HideInInspector]
	public float initialRotation;

	[HideInInspector]
	public Vector3 initialRotation3d;

	[HideInInspector]
	public float rotationSpeed;

	[HideInInspector]
	public Vector3 rotationSpeed3d;

	[HideInInspector]
	public Color32 color;

	[HideInInspector]
	public Color32 scriptedColor;

	[HideInInspector]
	public Color32 initialColor;

	[HideInInspector]
	public int lifetimeColorId;

	[HideInInspector]
	public bool noForce;

	[HideInInspector]
	public bool isNonBirthed;

	[HideInInspector]
	public bool isFirstLoop;

	[HideInInspector]
	public int particleId;

	[HideInInspector]
	public int particleSystemId;

	[HideInInspector]
	public int manipulatorId;

	[HideInInspector]
	public bool changedByProperty;

	[HideInInspector]
	public bool changedByPropertyColor;

	[HideInInspector]
	public bool changedByPropertyColorLerp;

	[HideInInspector]
	public bool changedByPropertyColorKeepAlpha;

	[HideInInspector]
	public bool changedByPropertySize;

	[HideInInspector]
	public bool changedByPropertyTarget;

	[HideInInspector]
	public bool changedByPropertyDeath;

	[HideInInspector]
	public int propertyTarget;

	[HideInInspector]
	public int propertyId;

	[HideInInspector]
	public int propertyColorId;

	[HideInInspector]
	public int excludeFromManipulatorId;

	[HideInInspector]
	public bool isMasked;

	[HideInInspector]
	public float maskAlpha;

	[HideInInspector]
	public Transform collisionTransform;

	[HideInInspector]
	public Collider collisionCollider;

	[HideInInspector]
	public Collider2D collisionCollider2D;

	public PlaygroundEventParticle Clone()
	{
		PlaygroundEventParticle playgroundEventParticle = new PlaygroundEventParticle();
		playgroundEventParticle.initialSize = initialSize;
		playgroundEventParticle.size = size;
		playgroundEventParticle.life = life;
		playgroundEventParticle.totalLifetime = totalLifetime;
		playgroundEventParticle.rotation = rotation;
		playgroundEventParticle.rotation3d = rotation3d;
		playgroundEventParticle.birth = birth;
		playgroundEventParticle.birthDelay = birthDelay;
		playgroundEventParticle.death = death;
		playgroundEventParticle.emission = emission;
		playgroundEventParticle.rebirth = rebirth;
		playgroundEventParticle.lifetimeOffset = lifetimeOffset;
		playgroundEventParticle.velocity = velocity;
		playgroundEventParticle.initialVelocity = initialVelocity;
		playgroundEventParticle.initialLocalVelocity = initialLocalVelocity;
		playgroundEventParticle.position = position;
		playgroundEventParticle.targetPosition = targetPosition;
		playgroundEventParticle.targetDirection = targetDirection;
		playgroundEventParticle.previousTargetPosition = previousTargetPosition;
		playgroundEventParticle.previousParticlePosition = previousParticlePosition;
		playgroundEventParticle.collisionParticlePosition = collisionParticlePosition;
		playgroundEventParticle.localSpaceMovementCompensation = localSpaceMovementCompensation;
		playgroundEventParticle.scatterPosition = scatterPosition;
		playgroundEventParticle.initialRotation = initialRotation;
		playgroundEventParticle.initialRotation3d = initialRotation3d;
		playgroundEventParticle.rotationSpeed = rotationSpeed;
		playgroundEventParticle.rotationSpeed3d = rotationSpeed3d;
		playgroundEventParticle.color = color;
		playgroundEventParticle.scriptedColor = scriptedColor;
		playgroundEventParticle.initialColor = initialColor;
		playgroundEventParticle.lifetimeColorId = lifetimeColorId;
		playgroundEventParticle.noForce = noForce;
		playgroundEventParticle.changedByProperty = changedByProperty;
		playgroundEventParticle.changedByPropertyColor = changedByPropertyColor;
		playgroundEventParticle.changedByPropertyColorLerp = changedByPropertyColorLerp;
		playgroundEventParticle.changedByPropertyColorKeepAlpha = changedByPropertyColorKeepAlpha;
		playgroundEventParticle.changedByPropertySize = changedByPropertySize;
		playgroundEventParticle.changedByPropertyTarget = changedByPropertyTarget;
		playgroundEventParticle.changedByPropertyDeath = changedByPropertyDeath;
		playgroundEventParticle.propertyTarget = propertyTarget;
		playgroundEventParticle.propertyId = propertyId;
		playgroundEventParticle.excludeFromManipulatorId = excludeFromManipulatorId;
		playgroundEventParticle.propertyColorId = propertyColorId;
		playgroundEventParticle.particleId = particleId;
		playgroundEventParticle.particleSystemId = particleSystemId;
		playgroundEventParticle.manipulatorId = manipulatorId;
		playgroundEventParticle.isMasked = isMasked;
		playgroundEventParticle.maskAlpha = maskAlpha;
		playgroundEventParticle.collisionTransform = collisionTransform;
		playgroundEventParticle.collisionCollider = collisionCollider;
		playgroundEventParticle.collisionCollider2D = collisionCollider2D;
		return playgroundEventParticle;
	}
}
