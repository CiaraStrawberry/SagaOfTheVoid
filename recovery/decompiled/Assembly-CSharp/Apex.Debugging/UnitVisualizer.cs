using Apex.Units;
using UnityEngine;

namespace Apex.Debugging;

[AddComponentMenu("Apex/Unit/Debugging/Unit Visualizer", 1025)]
[ApexComponent("Debugging")]
public class UnitVisualizer : Visualizer
{
	public Color radiusColor = new Color(98f / 255f, 93f / 255f, 227f / 255f);

	private IUnitProperties _unit;

	private Transform _transform;

	protected override void Start()
	{
		_unit = this.GetUnitFacade();
		_transform = transform;
	}

	protected override void DrawVisualization()
	{
		if (_unit == null)
		{
			_unit = GetComponent<UnitComponent>();
			_transform = transform;
			if (_unit == null)
			{
				return;
			}
		}
		Vector3 position = _transform.position;
		Gizmos.color = radiusColor;
		Gizmos.DrawWireSphere(position, _unit.radius);
	}
}
