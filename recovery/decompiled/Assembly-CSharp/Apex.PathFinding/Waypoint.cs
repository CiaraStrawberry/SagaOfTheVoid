using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public struct Waypoint(Vector3 pos) : IPositioned
{
	private Vector3 _pos = pos;

	public Vector3 position => _pos;

	public static implicit operator Vector3(Waypoint pos)
	{
		return pos.position;
	}

	public static implicit operator Waypoint(Vector3 pos)
	{
		return new Waypoint(pos);
	}
}
