using Apex.Common;
using Apex.Steering;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Units;

public interface IUnitProperties : IPositioned, IHaveAttributes
{
	float radius { get; }

	float fieldOfView { get; }

	float groundOffset { get; }

	float baseToPositionOffset { get; }

	float height { get; }

	Vector3 basePosition { get; }

	HeightNavigationCapabilities heightNavigationCapability { get; }

	bool isSelectable { get; }

	bool isSelected { get; set; }

	int determination { get; set; }

	void RecalculateBasePosition();

	void MarkSelectPending(bool pending);
}
