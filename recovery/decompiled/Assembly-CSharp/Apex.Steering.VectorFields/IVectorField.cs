using Apex.PathFinding;
using Apex.Units;
using Apex.WorldGeometry;

namespace Apex.Steering.VectorFields;

public interface IVectorField
{
	TransientGroup<IUnitFacade> group { get; }

	Path currentPath { get; }

	IPositioned destination { get; }

	IPositioned nextNodePosition { get; }

	bool isOnFinalApproach { get; }

	void Initialize();

	VectorFieldCell GetFieldCellAtPos(IGridCell cell);

	void SetNewPath(Path path);

	void DebugRender();
}
