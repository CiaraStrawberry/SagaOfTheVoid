using UnityEngine;

namespace Apex.WorldGeometry;

public interface ICellFactory
{
	Cell Create(CellMatrix parent, Vector3 position, int matrixPosX, int matrixPosZ, bool blocked);
}
