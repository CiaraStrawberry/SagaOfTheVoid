using System.Collections.Generic;
using Apex.PathFinding;
using Apex.Steering.VectorFields;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Debugging;

[AddComponentMenu("Apex/Unit/Debugging/Path Visualizer", 1024)]
[ApexComponent("Debugging")]
public class PathVisualizer : Visualizer
{
	public Color routeColor = new Color(148f / 255f, 214f / 255f, 53f / 255f);

	public Color waypointColor = new Color(0f, 150f / 255f, 211f / 255f);

	public bool showSegmentMarkers;

	private IUnitFacade _unit;

	public bool drawVectorField = true;

	protected override void Start()
	{
		_unit = this.GetUnitFacade();
	}

	protected override void DrawVisualization()
	{
		DrawVisualizationPartial();
		if (_unit == null || !_unit.isMovable)
		{
			return;
		}
		Vector3 vector = new Vector3(0f, 0.2f, 0f);
		Gizmos.color = routeColor;
		Vector3 vector2 = _unit.position;
		if (_unit.nextNodePosition.HasValue)
		{
			vector2 = _unit.nextNodePosition.Value;
			Gizmos.DrawLine(_unit.position + vector, vector2 + vector);
		}
		if (_unit.currentPath != null)
		{
			foreach (IPositioned item in (IEnumerable<IPositioned>)_unit.currentPath)
			{
				if (!(item is IPortalNode))
				{
					if (showSegmentMarkers)
					{
						Gizmos.DrawSphere(vector2, 0.2f);
					}
					Gizmos.DrawLine(vector2 + vector, item.position + vector);
					vector2 = item.position;
				}
			}
		}
		Gizmos.color = waypointColor;
		if (_unit.currentWaypoints == null)
		{
			return;
		}
		vector.y = 1f;
		foreach (Vector3 currentWaypoint in _unit.currentWaypoints)
		{
			Vector3 vector3 = currentWaypoint + vector;
			Gizmos.DrawLine(currentWaypoint, vector3);
			Gizmos.DrawSphere(vector3, 0.3f);
		}
	}

	private void DrawVisualizationPartial()
	{
		if (_unit == null || !(_unit.transientGroup is DefaultSteeringTransientUnitGroup defaultSteeringTransientUnitGroup) || _unit != defaultSteeringTransientUnitGroup.modelUnit)
		{
			return;
		}
		IVectorField vectorField = defaultSteeringTransientUnitGroup.vectorField;
		if (vectorField == null || vectorField.currentPath == null || vectorField.currentPath.count == 0)
		{
			return;
		}
		if (drawVectorField)
		{
			defaultSteeringTransientUnitGroup.vectorField.DebugRender();
		}
		Path currentPath = vectorField.currentPath;
		Vector3 vector = new Vector3(0f, 0.2f, 0f);
		Gizmos.color = routeColor;
		Vector3 position = currentPath.Peek().position;
		foreach (IPositioned item in (IEnumerable<IPositioned>)currentPath)
		{
			if (!(item is IPortalNode))
			{
				if (showSegmentMarkers)
				{
					Gizmos.DrawSphere(position, 0.2f);
				}
				Gizmos.DrawLine(position + vector, item.position + vector);
				position = item.position;
			}
		}
	}

	public void CloneFrom(PathVisualizer pathViz)
	{
		drawAlways = pathViz.drawAlways;
		showSegmentMarkers = pathViz.showSegmentMarkers;
		drawVectorField = pathViz.drawVectorField;
		routeColor = pathViz.routeColor;
		waypointColor = pathViz.waypointColor;
	}
}
