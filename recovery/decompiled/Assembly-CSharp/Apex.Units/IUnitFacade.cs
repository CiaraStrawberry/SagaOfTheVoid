using Apex.Common;
using Apex.PathFinding;
using Apex.Steering;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public interface IUnitFacade : IUnitProperties, IMovable, IMovingObject, IDefineSpeed, IGameObjectComponent, IGroupable<IUnitFacade>, IPositioned, IHaveAttributes
{
	Collider collider { get; }

	Transform transform { get; }

	Vector3 forward { get; }

	Vector3? lookAt { get; set; }

	bool isMovable { get; }

	bool isAlive { get; set; }

	IPathFinderOptions pathFinderOptions { get; }

	IPathNavigationOptions pathNavigationOptions { get; }

	IPositioned formationPos { get; set; }

	bool hasArrivedAtDestination { get; set; }

	void Initialize(GameObject unitObject);
}
