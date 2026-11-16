using Apex.Common;

namespace Apex.WorldGeometry;

public interface IDynamicObstacle : IPositioned
{
	AttributeMask exceptionsMask { get; }

	void ActivateUpdates(float? interval, bool repeat);

	void Toggle(bool active);
}
