using UnityEngine;

namespace VRTK;

public class VRTK_CollisionTracker : MonoBehaviour
{
	public event CollisionTrackerEventHandler CollisionEnter;

	public event CollisionTrackerEventHandler CollisionStay;

	public event CollisionTrackerEventHandler CollisionExit;

	public event CollisionTrackerEventHandler TriggerEnter;

	public event CollisionTrackerEventHandler TriggerStay;

	public event CollisionTrackerEventHandler TriggerExit;

	protected void OnCollisionEnterEvent(CollisionTrackerEventArgs e)
	{
		if (CollisionEnter != null)
		{
			CollisionEnter(this, e);
		}
	}

	protected void OnCollisionStayEvent(CollisionTrackerEventArgs e)
	{
		if (CollisionStay != null)
		{
			CollisionStay(this, e);
		}
	}

	protected void OnCollisionExitEvent(CollisionTrackerEventArgs e)
	{
		if (CollisionExit != null)
		{
			CollisionExit(this, e);
		}
	}

	protected void OnTriggerEnterEvent(CollisionTrackerEventArgs e)
	{
		if (TriggerEnter != null)
		{
			TriggerEnter(this, e);
		}
	}

	protected void OnTriggerStayEvent(CollisionTrackerEventArgs e)
	{
		if (TriggerStay != null)
		{
			TriggerStay(this, e);
		}
	}

	protected void OnTriggerExitEvent(CollisionTrackerEventArgs e)
	{
		if (TriggerExit != null)
		{
			TriggerExit(this, e);
		}
	}

	protected virtual void OnCollisionEnter(Collision collision)
	{
		OnCollisionEnterEvent(SetCollisionTrackerEvent(isTrigger: false, collision, collision.collider));
	}

	protected virtual void OnCollisionStay(Collision collision)
	{
		OnCollisionStayEvent(SetCollisionTrackerEvent(isTrigger: false, collision, collision.collider));
	}

	protected virtual void OnCollisionExit(Collision collision)
	{
		OnCollisionExitEvent(SetCollisionTrackerEvent(isTrigger: false, collision, collision.collider));
	}

	protected virtual void OnTriggerEnter(Collider collider)
	{
		OnTriggerEnterEvent(SetCollisionTrackerEvent(isTrigger: true, null, collider));
	}

	protected virtual void OnTriggerStay(Collider collider)
	{
		OnTriggerStayEvent(SetCollisionTrackerEvent(isTrigger: true, null, collider));
	}

	protected virtual void OnTriggerExit(Collider collider)
	{
		OnTriggerExitEvent(SetCollisionTrackerEvent(isTrigger: true, null, collider));
	}

	protected virtual CollisionTrackerEventArgs SetCollisionTrackerEvent(bool isTrigger, Collision givenCollision, Collider givenCollider)
	{
		CollisionTrackerEventArgs result = default;
		result.isTrigger = isTrigger;
		result.collision = givenCollision;
		result.collider = givenCollider;
		return result;
	}
}
