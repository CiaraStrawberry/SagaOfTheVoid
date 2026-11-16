using UnityEngine;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Presence/VRTK_PositionRewind")]
public class VRTK_PositionRewind : MonoBehaviour
{
	public enum CollisionDetectors
	{
		HeadsetOnly,
		BodyOnly,
		HeadsetAndBody
	}

	[Header("Rewind Settings")]
	[Tooltip("The colliders to determine if a collision has occured for the rewind to be actioned.")]
	public CollisionDetectors collisionDetector;

	[Tooltip("The amount of time from original headset collision until the rewind to the last good known position takes place.")]
	public float rewindDelay = 0.5f;

	[Tooltip("The additional distance to push the play area back upon rewind to prevent being right next to the wall again.")]
	public float pushbackDistance = 0.5f;

	[Tooltip("The threshold to determine how low the headset has to be before it is considered the user is crouching. The last good position will only be recorded in a non-crouching position.")]
	public float crouchThreshold = 0.5f;

	[Tooltip("The threshold to determind how low the headset can be to perform a position rewind. If the headset Y position is lower than this threshold then a rewind won't occur.")]
	public float crouchRewindThreshold = 0.1f;

	[Tooltip("A specified VRTK_PolicyList to use to determine whether any objects will be acted upon by the Position Rewind.")]
	public VRTK_PolicyList targetListPolicy;

	[Header("Custom Settings")]
	[Tooltip("The VRTK Body Physics script to use for the collisions and rigidbodies. If this is left blank then the first Body Physics script found in the scene will be used.")]
	public VRTK_BodyPhysics bodyPhysics;

	[Tooltip("The VRTK Headset Collision script to use to determine if the headset is colliding. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_HeadsetCollision headsetCollision;

	protected Transform headset;

	protected Transform playArea;

	protected Vector3 lastGoodStandingPosition;

	protected Vector3 lastGoodHeadsetPosition;

	protected float highestHeadsetY;

	protected float lastPlayAreaY;

	protected bool lastGoodPositionSet;

	protected bool hasCollided;

	protected bool isColliding;

	protected bool isRewinding;

	protected float collideTimer;

	public virtual void SetLastGoodPosition()
	{
		if (playArea != null && headset != null)
		{
			lastGoodPositionSet = true;
			lastGoodStandingPosition = playArea.position;
			lastGoodHeadsetPosition = headset.position;
		}
	}

	public virtual void RewindPosition()
	{
		if (headset != null)
		{
			Vector3 vector = lastGoodHeadsetPosition - headset.position;
			Vector3 vector2 = vector.normalized * pushbackDistance;
			playArea.position += vector + vector2;
			if (bodyPhysics != null)
			{
				bodyPhysics.ResetVelocities();
			}
		}
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		lastGoodPositionSet = false;
		headset = VRTK_DeviceFinder.HeadsetTransform();
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		if (playArea == null)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
		}
		bodyPhysics = ((!(bodyPhysics != null)) ? Object.FindObjectOfType<VRTK_BodyPhysics>() : bodyPhysics);
		headsetCollision = ((!(headsetCollision != null)) ? GetComponentInChildren<VRTK_HeadsetCollision>() : headsetCollision);
		ManageListeners(state: true);
	}

	protected virtual void OnDisable()
	{
		ManageListeners(state: false);
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		if (isColliding)
		{
			if (collideTimer > 0f)
			{
				collideTimer -= Time.deltaTime;
				return;
			}
			collideTimer = 0f;
			isColliding = false;
			DoPositionRewind();
		}
	}

	protected virtual bool CrouchThresholdReached()
	{
		float num = 0.005f;
		return playArea.position.y > lastPlayAreaY + num || playArea.position.y < lastPlayAreaY - num;
	}

	protected virtual void SetHighestHeadsetY()
	{
		ref float reference = ref highestHeadsetY;
		float num;
		if (CrouchThresholdReached())
		{
			num = crouchThreshold;
		}
		else
		{
			num = ((!(headset.localPosition.y > highestHeadsetY)) ? highestHeadsetY : headset.localPosition.y);
		}
		reference = num;
	}

	protected virtual void UpdateLastGoodPosition()
	{
		float num = highestHeadsetY - crouchThreshold;
		if (headset.localPosition.y > num && num > crouchThreshold)
		{
			SetLastGoodPosition();
		}
		lastPlayAreaY = playArea.position.y;
	}

	protected virtual void FixedUpdate()
	{
		if (!isColliding && playArea != null)
		{
			SetHighestHeadsetY();
			UpdateLastGoodPosition();
		}
	}

	protected virtual void StartCollision(GameObject target)
	{
		if (!VRTK_PolicyList.Check(target, targetListPolicy))
		{
			isColliding = true;
			if (!hasCollided && collideTimer <= 0f)
			{
				hasCollided = true;
				collideTimer = rewindDelay;
			}
		}
	}

	protected virtual void EndCollision()
	{
		isColliding = false;
		hasCollided = false;
		isRewinding = false;
	}

	protected virtual bool BodyCollisionsEnabled()
	{
		return bodyPhysics == null || bodyPhysics.enableBodyCollisions;
	}

	protected virtual bool CanRewind()
	{
		return !isRewinding && ((playArea != null) & lastGoodPositionSet) && headset.localPosition.y > crouchRewindThreshold && BodyCollisionsEnabled();
	}

	protected virtual void DoPositionRewind()
	{
		if (CanRewind())
		{
			isRewinding = true;
			RewindPosition();
		}
	}

	protected virtual bool HeadsetListen()
	{
		return collisionDetector == CollisionDetectors.HeadsetAndBody || collisionDetector == CollisionDetectors.HeadsetOnly;
	}

	protected virtual bool BodyListen()
	{
		return collisionDetector == CollisionDetectors.HeadsetAndBody || collisionDetector == CollisionDetectors.BodyOnly;
	}

	protected virtual void ManageListeners(bool state)
	{
		if (state)
		{
			if (headsetCollision != null && HeadsetListen())
			{
				headsetCollision.HeadsetCollisionDetect += HeadsetCollisionDetect;
				headsetCollision.HeadsetCollisionEnded += HeadsetCollisionEnded;
			}
			if (bodyPhysics != null && BodyListen())
			{
				bodyPhysics.StartColliding += StartColliding;
				bodyPhysics.StopColliding += StopColliding;
			}
		}
		else
		{
			if (headsetCollision != null && HeadsetListen())
			{
				headsetCollision.HeadsetCollisionDetect -= HeadsetCollisionDetect;
				headsetCollision.HeadsetCollisionEnded -= HeadsetCollisionEnded;
			}
			if (bodyPhysics != null && BodyListen())
			{
				bodyPhysics.StartColliding -= StartColliding;
				bodyPhysics.StopColliding -= StopColliding;
			}
		}
	}

	private void StartColliding(object sender, BodyPhysicsEventArgs e)
	{
		StartCollision(e.target);
	}

	private void StopColliding(object sender, BodyPhysicsEventArgs e)
	{
		EndCollision();
	}

	protected virtual void HeadsetCollisionDetect(object sender, HeadsetCollisionEventArgs e)
	{
		StartCollision(e.collider.gameObject);
	}

	protected virtual void HeadsetCollisionEnded(object sender, HeadsetCollisionEventArgs e)
	{
		EndCollision();
	}
}
