using Apex.DataStructures;
using Apex.Services;

namespace Apex.WorldGeometry;

public class GridSection
{
	public RectangleXZ bounds { get; private set; }

	public float lastChanged { get; private set; }

	public GridSection(RectangleXZ rect)
	{
		bounds = rect;
		lastChanged = -1f;
	}

	public void Touch()
	{
		lastChanged = UnityServices.time.time;
	}
}
