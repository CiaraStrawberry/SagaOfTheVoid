using System;
using Apex.DataStructures;
using Apex.Messages;
using Apex.PathFinding;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Examples/Steer for Point", 1024)]
public class SteerForPointComponent : ArrivalBase, IMovable
{
	private Vector3 _point;

	public Path currentPath => null;

	public IIterable<Vector3> currentWaypoints => null;

	public Vector3? finalDestination => _point;

	public Vector3? nextNodePosition => _point;

	public UnitNavigationEventMessage.Event lastNavigationEvent => UnitNavigationEventMessage.Event.None;

	public void MoveTo(Vector3 position, bool append)
	{
		_point = position;
	}

	public void MoveAlong(Path path)
	{
		throw new NotImplementedException();
	}

	public void MoveAlong(Path path, ReplanCallback onReplan)
	{
		throw new NotImplementedException();
	}

	public void EnableMovementOrders()
	{
		throw new NotImplementedException();
	}

	public void DisableMovementOrders()
	{
		throw new NotImplementedException();
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		Vector3 desiredAcceleration = Arrive(_point, input);
		if (!hasArrived)
		{
			output.desiredAcceleration = desiredAcceleration;
		}
	}

	public override void Stop()
	{
		_point = transform.position;
	}
}
