using Apex.DataStructures;

namespace Apex.WorldGeometry;

public static class MatrixDirection
{
	public static readonly VectorXZ None = new VectorXZ(0, 0);

	public static readonly VectorXZ Down = new VectorXZ(0, -1);

	public static readonly VectorXZ Up = new VectorXZ(0, 1);

	public static readonly VectorXZ DownLeft = new VectorXZ(-1, -1);

	public static readonly VectorXZ DownRight = new VectorXZ(1, -1);

	public static readonly VectorXZ UpLeft = new VectorXZ(-1, 1);

	public static readonly VectorXZ UpRight = new VectorXZ(1, 1);

	public static readonly VectorXZ Left = new VectorXZ(-1, 0);

	public static readonly VectorXZ Right = new VectorXZ(1, 0);

	public static readonly VectorXZ[] Directions = new VectorXZ[9] { DownLeft, Down, DownRight, Left, None, Right, UpLeft, Up, UpRight };
}
