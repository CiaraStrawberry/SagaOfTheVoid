using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.PathFinding;

public struct Position(Vector3 pos) : IPositioned
{
	private Vector3 _pos = pos;

	public Vector3 position => _pos;

	public static implicit operator Vector3(Position pos)
	{
		return pos.position;
	}

	public static implicit operator Position(Vector3 pos)
	{
		return new Position(pos);
	}

	public override string ToString()
	{
		return "Position: " + _pos.ToString();
	}
}
