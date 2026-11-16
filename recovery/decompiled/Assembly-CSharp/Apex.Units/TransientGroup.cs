using System;
using System.Collections.Generic;
using Apex.DataStructures;
using Apex.Messages;
using Apex.PathFinding;
using Apex.Steering;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public abstract class TransientGroup<T> : IMovable, IMovingObject, ISortable<T> where T : IGroupable<T>
{
	private IndexableSet<T> _members;

	public virtual Vector3 centerOfGravity => GetGroupCenterOfGravity();

	public virtual T modelUnit => (_members.count <= 0) ? default(T) : _members[0];

	public int count => _members.count;

	public bool hasArrived { get; set; }

	public virtual Path currentPath
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual IIterable<Vector3> currentWaypoints
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual Vector3? finalDestination
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public UnitNavigationEventMessage.Event lastNavigationEvent
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual Vector3? nextNodePosition
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual bool isGrounded
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual Vector3 velocity
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	public virtual Vector3 actualVelocity
	{
		get
		{
			throw new NotSupportedException();
		}
	}

	internal bool keepAlive { get; set; }

	public T this[int index] => _members[index];

	public virtual IFormation currentFormation => null;

	public TransientGroup(int capacity)
	{
		_members = new IndexableSet<T>(capacity);
	}

	public TransientGroup(T[] members)
	{
		Ensure.ArgumentNotNull(members, "members");
		_members = new IndexableSet<T>(members);
	}

	public TransientGroup(IEnumerable<T> members)
	{
		Ensure.ArgumentNotNull(members, "members");
		_members = new IndexableSet<T>(members);
	}

	public virtual void Add(T member)
	{
		_members.Add(member);
	}

	public virtual void Remove(T member)
	{
		_members.Remove(member);
		if (object.ReferenceEquals(member.transientGroup, this))
		{
			member.transientGroup = null;
		}
	}

	public virtual void Sort()
	{
		_members.Sort();
	}

	public virtual void Sort(IComparer<T> comparer)
	{
		_members.Sort(comparer);
	}

	public virtual void Sort(int index, int length)
	{
		_members.Sort(index, length);
	}

	public virtual void Sort(int index, int length, IComparer<T> comparer)
	{
		_members.Sort(index, length, comparer);
	}

	public virtual void Dissolve()
	{
		foreach (T item in (IEnumerable<T>)_members)
		{
			T current = item;
			if (object.ReferenceEquals(current.transientGroup, this))
			{
				current.transientGroup = null;
			}
		}
		_members.Clear();
	}

	public virtual Vector3 GetGroupCenterOfGravity()
	{
		Vector3 zero = Vector3.zero;
		int num = _members.count;
		if (num == 0)
		{
			return zero;
		}
		for (int i = 0; i < num; i++)
		{
			if (_members[i] is IPositioned positioned)
			{
				zero += positioned.position;
			}
		}
		return zero / num;
	}

	public void MoveTo(Vector3 position, bool append)
	{
		PrepareForAction();
		MoveToInternal(position, append);
	}

	public virtual void MoveAlong(Path path)
	{
		MoveAlong(path, null);
	}

	public void MoveAlong(Path path, ReplanCallback onReplan)
	{
		PrepareForAction();
		MoveAlongInternal(path, onReplan);
	}

	public virtual void SetPreferredSpeed(float speed)
	{
	}

	public abstract void Wait(float? seconds);

	public abstract void Resume();

	public abstract void Stop();

	public abstract void EnableMovementOrders();

	public abstract void DisableMovementOrders();

	protected abstract void MoveToInternal(Vector3 position, bool append);

	protected abstract void MoveAlongInternal(Path path, ReplanCallback onReplan);

	protected virtual void PrepareForAction()
	{
		for (int i = 0; i < _members.count; i++)
		{
			T member = _members[i];
			TransientGroup<T> transientGroup = member.transientGroup;
			if (transientGroup == null || !object.ReferenceEquals(transientGroup, this))
			{
				if (transientGroup != null && !transientGroup.keepAlive)
				{
					transientGroup.Remove(member);
				}
				member.transientGroup = this;
			}
		}
	}

	public virtual void SetFormation(IFormation formation)
	{
	}

	public virtual void ClearFormation()
	{
		SetFormation(null);
	}
}
