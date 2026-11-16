using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public interface ICellMatrixConfiguration
{
	Vector3 origin { get; }

	int sizeX { get; }

	int sizeZ { get; }

	float cellSize { get; }

	float obstacleSensitivityRange { get; }

	bool generateHeightmap { get; }

	HeightLookupType heightLookupType { get; }

	int heightLookupMaxDepth { get; }

	float upperBoundary { get; }

	float lowerBoundary { get; }

	ColliderDetectionMode obstacleAndGroundDetection { get; }

	IBlockDetector obstacleAndGroundDetector { get; }

	Bounds bounds { get; }
}
