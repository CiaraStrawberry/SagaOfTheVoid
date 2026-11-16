using Apex.DataStructures;
using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("Apex/Game World/Dynamic Obstacle", 1030)]
[ApexComponent("Behaviours")]
public class DynamicObstacle : DynamicObstacleBase
{
	private interface IActualBounds
	{
		MatrixBounds Prepare(CellMatrix matrix, bool block);

		bool Update(Cell c);

		void Render();
	}

	private sealed class AxisBounds : IActualBounds
	{
		private readonly Collider _collider;

		private readonly DynamicObstacle _parent;

		private MatrixBounds _lastCoverage;

		private MatrixBounds _newCoverage;

		public AxisBounds(Collider collider, DynamicObstacle parent)
		{
			_collider = collider;
			_parent = parent;
			_lastCoverage = (_newCoverage = MatrixBounds.nullBounds);
		}

		public MatrixBounds Prepare(CellMatrix matrix, bool block)
		{
			_lastCoverage = _newCoverage;
			if (!block)
			{
				_newCoverage = MatrixBounds.nullBounds;
				return _lastCoverage;
			}
			Vector3 velocity = _parent.GetVelocity();
			float minOverlapToInclude = matrix.cellSize / 2f - ((!_parent.useGridObstacleSensitivity) ? _parent.customSensitivity : matrix.obstacleSensitivityRange);
			Bounds b = GrowBoundsByVelocity(_collider.bounds, velocity);
			_newCoverage = matrix.GetMatrixBounds(b, minOverlapToInclude, adjustToBounds: true);
			return MatrixBounds.Combine(_lastCoverage, _newCoverage);
		}

		public bool Update(Cell c)
		{
			int matrixPosX = c.matrixPosX;
			int matrixPosZ = c.matrixPosZ;
			if (_lastCoverage.Contains(matrixPosX, matrixPosZ) && !_newCoverage.Contains(matrixPosX, matrixPosZ))
			{
				return c.RemoveDynamicObstacle(_parent);
			}
			if (!_lastCoverage.Contains(matrixPosX, matrixPosZ) && _newCoverage.Contains(matrixPosX, matrixPosZ))
			{
				return c.AddDynamicObstacle(_parent);
			}
			return false;
		}

		public void Render()
		{
		}

		private static Bounds GrowBoundsByVelocity(Bounds bounds, Vector3 velocity)
		{
			if (velocity.x != 0f || velocity.z != 0f)
			{
				Vector3 min = bounds.min;
				Vector3 max = bounds.max;
				if (velocity.x < 0f)
				{
					min.x += velocity.x;
				}
				else if (velocity.x > 0f)
				{
					max.x += velocity.x;
				}
				if (velocity.z < 0f)
				{
					min.z += velocity.z;
				}
				else if (velocity.z > 0f)
				{
					max.z += velocity.z;
				}
				bounds.SetMinMax(min, max);
			}
			return bounds;
		}
	}

	private IActualBounds _actualBounds;

	private IGrid _lastGrid;

	protected override void OnAwake()
	{
		if (_actualBounds == null)
		{
			_actualBounds = new AxisBounds(GetComponent<Collider>(), this);
		}
	}

	public override void RenderVisualization()
	{
		if (_actualBounds != null)
		{
			_actualBounds.Render();
		}
	}

	protected override void UpdateCells(bool block)
	{
		if (!block)
		{
			if (_lastGrid != null)
			{
				UpdateCells(_lastGrid, block: false);
				_lastGrid = null;
			}
			return;
		}
		IGrid grid = GridManager.instance.GetGrid(_transform.position);
		if (grid != _lastGrid && _lastGrid != null)
		{
			UpdateCells(_lastGrid, block: false);
		}
		_lastGrid = grid;
		if (grid != null)
		{
			UpdateCells(grid, block);
		}
	}

	private void UpdateCells(IGrid grid, bool block)
	{
		CellMatrix cellMatrix = grid.cellMatrix;
		MatrixBounds b = _actualBounds.Prepare(cellMatrix, block);
		Cell[,] rawMatrix = cellMatrix.rawMatrix;
		bool flag = false;
		for (int i = b.minColumn; i <= b.maxColumn; i++)
		{
			for (int j = b.minRow; j <= b.maxRow; j++)
			{
				Cell c = rawMatrix[i, j];
				flag |= _actualBounds.Update(c);
			}
		}
		if (flag && causesReplanning)
		{
			grid.TouchSections(b);
		}
	}
}
