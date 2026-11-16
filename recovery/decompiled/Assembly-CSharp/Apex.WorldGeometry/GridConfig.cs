using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

public sealed class GridConfig
{
	private float _connectorPortalWidth;

	public string friendlyName { get; set; }

	public Vector3 origin { get; set; }

	public int sizeX { get; set; }

	public int sizeZ { get; set; }

	public float cellSize { get; set; }

	public float connectorPortalWidth
	{
		get
		{
			if (_connectorPortalWidth <= 0f)
			{
				return cellSize;
			}
			return _connectorPortalWidth;
		}
		set
		{
			_connectorPortalWidth = value;
		}
	}

	public float obstacleSensitivityRange { get; set; }

	public ColliderDetectionMode obstacleAndGroundDetection { get; set; }

	public IBlockDetector obstacleAndGroundDetector { get; set; }

	public bool automaticConnections { get; set; }

	public bool generateHeightmap { get; set; }

	public HeightLookupType heightLookupType { get; set; }

	public int heightLookupMaxDepth { get; set; }

	public float upperBoundary { get; set; }

	public float lowerBoundary { get; set; }

	public int subSectionsX { get; set; }

	public int subSectionsZ { get; set; }

	public int subSectionsCellOverlap { get; set; }

	public GridConfig()
	{
		sizeX = 10;
		sizeZ = 10;
		cellSize = 2f;
		generateHeightmap = true;
		heightLookupType = HeightLookupType.Dictionary;
		heightLookupMaxDepth = 5;
		lowerBoundary = 1f;
		upperBoundary = 10f;
		obstacleSensitivityRange = 0.5f;
		obstacleAndGroundDetection = ColliderDetectionMode.Mixed;
		automaticConnections = false;
		subSectionsCellOverlap = 2;
		subSectionsX = 2;
		subSectionsZ = 2;
	}
}
