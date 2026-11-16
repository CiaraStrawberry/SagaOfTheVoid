using System;
using UnityEngine;

namespace ParticlePlayground;

[Serializable]
public class PlaygroundCache
{
	[HideInInspector]
	public float[] initialSize;

	[HideInInspector]
	public float[] size;

	[HideInInspector]
	public float[] rotation;

	[HideInInspector]
	public Vector3[] rotation3d;

	[HideInInspector]
	public float[] life;

	[HideInInspector]
	public float[] lifetimeSubtraction;

	[HideInInspector]
	public float[] birth;

	[HideInInspector]
	public float[] birthDelay;

	[HideInInspector]
	public float[] death;

	[HideInInspector]
	public bool[] emission;

	[HideInInspector]
	public bool[] rebirth;

	[HideInInspector]
	public float[] lifetimeOffset;

	[HideInInspector]
	public Vector3[] velocity;

	[HideInInspector]
	public Vector3[] initialVelocity;

	[HideInInspector]
	public Vector3[] initialLocalVelocity;

	[HideInInspector]
	public Vector3[] position;

	[HideInInspector]
	public Vector3[] targetPosition;

	[HideInInspector]
	public Vector3[] targetDirection;

	[HideInInspector]
	public Vector3[] previousTargetPosition;

	[HideInInspector]
	public Vector3[] previousParticlePosition;

	[HideInInspector]
	public Vector3[] collisionParticlePosition;

	[HideInInspector]
	public Vector3[] localSpaceMovementCompensation;

	[HideInInspector]
	public Vector3[] scatterPosition;

	[HideInInspector]
	public float[] initialRotation;

	[HideInInspector]
	public float[] rotationSpeed;

	[HideInInspector]
	public Vector3[] initialRotation3d;

	[HideInInspector]
	public Vector3[] rotationSpeed3d;

	[HideInInspector]
	public Color32[] color;

	[HideInInspector]
	public Color32[] scriptedColor;

	[HideInInspector]
	public Color32[] initialColor;

	[HideInInspector]
	public int[] lifetimeColorId;

	[HideInInspector]
	public bool[] noForce;

	[HideInInspector]
	public bool[] isNonBirthed;

	[HideInInspector]
	public bool[] isFirstLoop;

	[HideInInspector]
	public bool[] simulate;

	[HideInInspector]
	public bool[] isCalculatedThisFrame;

	[HideInInspector]
	public bool[] changedByProperty;

	[HideInInspector]
	public bool[] changedByPropertyColor;

	[HideInInspector]
	public bool[] changedByPropertyColorLerp;

	[HideInInspector]
	public bool[] changedByPropertyColorKeepAlpha;

	[HideInInspector]
	public bool[] changedByPropertySize;

	[HideInInspector]
	public bool[] changedByPropertyTarget;

	[HideInInspector]
	public bool[] changedByPropertyDeath;

	[HideInInspector]
	public int[] propertyTarget;

	[HideInInspector]
	public int[] propertyId;

	[HideInInspector]
	public int[] propertyColorId;

	[HideInInspector]
	public int[] manipulatorId;

	[HideInInspector]
	public int[] excludeFromManipulatorId;

	[HideInInspector]
	public float[] lifetimeLoss;

	[HideInInspector]
	public bool[] isMasked;

	[HideInInspector]
	public float[] maskAlpha;

	[HideInInspector]
	public int[] maskSorting;

	public PlaygroundCache Clone()
	{
		PlaygroundCache playgroundCache = new PlaygroundCache();
		playgroundCache.initialSize = initialSize.Clone() as float[];
		playgroundCache.size = size.Clone() as float[];
		playgroundCache.life = life.Clone() as float[];
		playgroundCache.lifetimeSubtraction = lifetimeSubtraction.Clone() as float[];
		playgroundCache.rotation = rotation.Clone() as float[];
		playgroundCache.birth = birth.Clone() as float[];
		playgroundCache.birthDelay = birthDelay.Clone() as float[];
		playgroundCache.death = death.Clone() as float[];
		playgroundCache.emission = emission.Clone() as bool[];
		playgroundCache.rebirth = rebirth.Clone() as bool[];
		playgroundCache.lifetimeOffset = lifetimeOffset.Clone() as float[];
		playgroundCache.velocity = velocity.Clone() as Vector3[];
		playgroundCache.initialVelocity = initialVelocity.Clone() as Vector3[];
		playgroundCache.initialLocalVelocity = initialLocalVelocity.Clone() as Vector3[];
		playgroundCache.position = position.Clone() as Vector3[];
		playgroundCache.targetPosition = targetPosition.Clone() as Vector3[];
		playgroundCache.targetDirection = targetDirection.Clone() as Vector3[];
		playgroundCache.previousTargetPosition = previousTargetPosition.Clone() as Vector3[];
		playgroundCache.previousParticlePosition = previousParticlePosition.Clone() as Vector3[];
		playgroundCache.collisionParticlePosition = collisionParticlePosition.Clone() as Vector3[];
		playgroundCache.localSpaceMovementCompensation = localSpaceMovementCompensation.Clone() as Vector3[];
		playgroundCache.scatterPosition = scatterPosition.Clone() as Vector3[];
		playgroundCache.initialRotation = initialRotation.Clone() as float[];
		playgroundCache.rotationSpeed = rotationSpeed.Clone() as float[];
		playgroundCache.color = color.Clone() as Color32[];
		playgroundCache.scriptedColor = scriptedColor.Clone() as Color32[];
		playgroundCache.initialColor = initialColor.Clone() as Color32[];
		playgroundCache.lifetimeColorId = lifetimeColorId.Clone() as int[];
		playgroundCache.noForce = noForce.Clone() as bool[];
		playgroundCache.changedByProperty = changedByProperty.Clone() as bool[];
		playgroundCache.changedByPropertyColor = changedByPropertyColor.Clone() as bool[];
		playgroundCache.changedByPropertyColorLerp = changedByPropertyColorLerp.Clone() as bool[];
		playgroundCache.changedByPropertyColorKeepAlpha = changedByPropertyColorKeepAlpha.Clone() as bool[];
		playgroundCache.changedByPropertySize = changedByPropertySize.Clone() as bool[];
		playgroundCache.changedByPropertyTarget = changedByPropertyTarget.Clone() as bool[];
		playgroundCache.changedByPropertyDeath = changedByPropertyDeath.Clone() as bool[];
		playgroundCache.propertyTarget = propertyTarget.Clone() as int[];
		playgroundCache.propertyId = propertyId.Clone() as int[];
		playgroundCache.excludeFromManipulatorId = excludeFromManipulatorId.Clone() as int[];
		playgroundCache.propertyColorId = propertyColorId.Clone() as int[];
		playgroundCache.manipulatorId = manipulatorId.Clone() as int[];
		playgroundCache.isMasked = isMasked.Clone() as bool[];
		playgroundCache.maskAlpha = maskAlpha.Clone() as float[];
		if (maskSorting != null)
		{
			playgroundCache.maskSorting = maskSorting.Clone() as int[];
		}
		playgroundCache.isNonBirthed = isNonBirthed.Clone() as bool[];
		playgroundCache.isFirstLoop = isFirstLoop.Clone() as bool[];
		playgroundCache.simulate = simulate.Clone() as bool[];
		playgroundCache.isCalculatedThisFrame = isCalculatedThisFrame.Clone() as bool[];
		if (playgroundCache.lifetimeLoss == null)
		{
			playgroundCache.lifetimeLoss = new float[0];
		}
		else
		{
			playgroundCache.lifetimeLoss = lifetimeLoss.Clone() as float[];
		}
		return playgroundCache;
	}
}
