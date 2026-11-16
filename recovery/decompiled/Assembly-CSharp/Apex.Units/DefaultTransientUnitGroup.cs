using System.Collections.Generic;
using Apex.DataStructures;
using Apex.PathFinding;
using UnityEngine;

namespace Apex.Units;

public class DefaultTransientUnitGroup : TransientGroup<IUnitFacade>, IGrouping<IUnitFacade>
{
	int IGrouping<IUnitFacade>.groupCount => 1;

	int IGrouping<IUnitFacade>.memberCount => count;

	TransientGroup<IUnitFacade> IGrouping<IUnitFacade>.this[int index] => this;

	public override Vector3 velocity => modelUnit?.velocity ?? Vector3.zero;

	public override Vector3 actualVelocity => modelUnit?.actualVelocity ?? Vector3.zero;

	public override Path currentPath => modelUnit?.currentPath;

	public override IIterable<Vector3> currentWaypoints => modelUnit?.currentWaypoints;

	public override Vector3? finalDestination => modelUnit?.finalDestination;

	public override Vector3? nextNodePosition => modelUnit?.nextNodePosition;

	public override bool isGrounded => modelUnit.isGrounded;

	public DefaultTransientUnitGroup(int capacity)
		: base(capacity)
	{
	}

	public DefaultTransientUnitGroup(IUnitFacade[] members)
		: base(members)
	{
	}

	public DefaultTransientUnitGroup(IEnumerable<IUnitFacade> members)
		: base(members)
	{
	}

	public override void Wait(float? seconds)
	{
		for (int i = 0; i < count; i++)
		{
			base[i].Wait(seconds);
		}
	}

	public override void Resume()
	{
		for (int i = 0; i < count; i++)
		{
			base[i].Resume();
		}
	}

	public override void Stop()
	{
		for (int i = 0; i < count; i++)
		{
			base[i].Stop();
		}
	}

	public override void EnableMovementOrders()
	{
		for (int i = 0; i < count; i++)
		{
			base[i].EnableMovementOrders();
		}
	}

	public override void DisableMovementOrders()
	{
		for (int i = 0; i < count; i++)
		{
			base[i].DisableMovementOrders();
		}
	}

	public override void SetPreferredSpeed(float speed)
	{
		int num = count;
		for (int i = 0; i < num; i++)
		{
			base[i].SetPreferredSpeed(speed);
		}
		modelUnit?.SetPreferredSpeed(speed);
	}

	void IGrouping<IUnitFacade>.Remove(IUnitFacade member)
	{
		Remove(member);
	}

	void IGrouping<IUnitFacade>.Add(IUnitFacade member)
	{
		Add(member);
	}

	protected override void MoveToInternal(Vector3 position, bool append)
	{
		for (int i = 0; i < count; i++)
		{
			base[i].MoveTo(position, append);
		}
	}

	protected override void MoveAlongInternal(Path path, ReplanCallback onReplan)
	{
		for (int i = 0; i < count; i++)
		{
			Path path2 = path.Clone();
			base[i].MoveAlong(path2, onReplan);
		}
	}
}
