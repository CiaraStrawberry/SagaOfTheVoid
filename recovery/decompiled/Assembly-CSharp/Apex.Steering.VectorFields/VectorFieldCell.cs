using Apex.DataStructures;

namespace Apex.Steering.VectorFields;

public struct VectorFieldCell
{
	public PlaneVector direction;

	public int pathPortalIndex;

	public VectorFieldCell(PlaneVector direction, int portalIndex)
	{
		this.direction = direction;
		pathPortalIndex = portalIndex;
	}

	public VectorFieldCell(PlaneVector direction)
	{
		this.direction = direction;
		pathPortalIndex = -1;
	}

	public VectorFieldCell(int portalIndex)
	{
		direction = PlaneVector.zero;
		pathPortalIndex = portalIndex;
	}
}
