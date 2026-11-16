using UnityEngine;

namespace VRTK;

public abstract class VRTK_DestinationMarker : MonoBehaviour
{
	[Header("Destination Marker Settings", order = 1)]
	[Tooltip("If this is checked then the teleport flag is set to true in the Destination Set event so teleport scripts will know whether to action the new destination.")]
	public bool enableTeleport = true;

	protected VRTK_PolicyList invalidListPolicy;

	protected float navMeshCheckDistance;

	protected bool headsetPositionCompensation;

	protected bool forceHoverOnRepeatedEnter = true;

	protected Collider existingCollider;

	public event DestinationMarkerEventHandler DestinationMarkerEnter;

	public event DestinationMarkerEventHandler DestinationMarkerExit;

	public event DestinationMarkerEventHandler DestinationMarkerHover;

	public event DestinationMarkerEventHandler DestinationMarkerSet;

	public virtual void OnDestinationMarkerEnter(DestinationMarkerEventArgs e)
	{
		if (DestinationMarkerEnter != null && (!forceHoverOnRepeatedEnter || e.raycastHit.collider != existingCollider))
		{
			existingCollider = e.raycastHit.collider;
			DestinationMarkerEnter(this, e);
		}
		if (forceHoverOnRepeatedEnter && e.raycastHit.collider == existingCollider)
		{
			OnDestinationMarkerHover(e);
		}
	}

	public virtual void OnDestinationMarkerExit(DestinationMarkerEventArgs e)
	{
		if (DestinationMarkerExit != null)
		{
			DestinationMarkerExit(this, e);
			existingCollider = null;
		}
	}

	public virtual void OnDestinationMarkerHover(DestinationMarkerEventArgs e)
	{
		if (DestinationMarkerHover != null)
		{
			DestinationMarkerHover(this, e);
		}
	}

	public virtual void OnDestinationMarkerSet(DestinationMarkerEventArgs e)
	{
		if (DestinationMarkerSet != null)
		{
			DestinationMarkerSet(this, e);
		}
	}

	public virtual void SetInvalidTarget(VRTK_PolicyList list = null)
	{
		invalidListPolicy = list;
	}

	public virtual void SetNavMeshCheckDistance(float distance)
	{
		navMeshCheckDistance = distance;
	}

	public virtual void SetHeadsetPositionCompensation(bool state)
	{
		headsetPositionCompensation = state;
	}

	public virtual void SetForceHoverOnRepeatedEnter(bool state)
	{
		forceHoverOnRepeatedEnter = state;
	}

	protected virtual void OnEnable()
	{
		VRTK_ObjectCache.registeredDestinationMarkers.Add(this);
	}

	protected virtual void OnDisable()
	{
		VRTK_ObjectCache.registeredDestinationMarkers.Remove(this);
	}

	protected virtual DestinationMarkerEventArgs SetDestinationMarkerEvent(float distance, Transform target, RaycastHit raycastHit, Vector3 position, VRTK_ControllerReference controllerReference, bool forceDestinationPosition = false, Quaternion? rotation = null)
	{
		DestinationMarkerEventArgs result = default;
		result.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		result.controllerReference = controllerReference;
		result.distance = distance;
		result.target = target;
		result.raycastHit = raycastHit;
		result.destinationPosition = position;
		result.destinationRotation = rotation;
		result.enableTeleport = enableTeleport;
		result.forceDestinationPosition = forceDestinationPosition;
		return result;
	}
}
