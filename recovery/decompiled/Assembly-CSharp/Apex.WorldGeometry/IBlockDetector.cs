using UnityEngine;

namespace Apex.WorldGeometry;

public interface IBlockDetector
{
	bool IsBlocked(CellMatrix matrix, Vector3 position, float blockThreshold);
}
