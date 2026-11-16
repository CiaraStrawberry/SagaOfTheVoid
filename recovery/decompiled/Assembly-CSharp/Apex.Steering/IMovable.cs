using Apex.DataStructures;
using Apex.Messages;
using Apex.PathFinding;
using UnityEngine;

namespace Apex.Steering;

public interface IMovable
{
	Path currentPath { get; }

	IIterable<Vector3> currentWaypoints { get; }

	Vector3? finalDestination { get; }

	Vector3? nextNodePosition { get; }

	UnitNavigationEventMessage.Event lastNavigationEvent { get; }

	void MoveTo(Vector3 position, bool append);

	void MoveAlong(Path path);

	void MoveAlong(Path path, ReplanCallback onReplan);

	void EnableMovementOrders();

	void DisableMovementOrders();
}
