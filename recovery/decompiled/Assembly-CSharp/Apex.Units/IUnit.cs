using Apex.Common;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public interface IUnit : IPositioned, IHaveAttributes
{
	float radius { get; }

	float fieldOfView { get; }

	Vector3 forward { get; }
}
