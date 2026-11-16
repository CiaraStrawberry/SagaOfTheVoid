using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to Jump", 1036)]
[ApexComponent("Steering")]
public class SteerToJumpComponent : SteeringComponent
{
	[Tooltip("The default height that jumping reaches.")]
	public float defaultHeight = 3f;

	[Tooltip("The default force power used for jumping.")]
	public float defaultForce = 200f;

	private IUnitFacade _unit;

	private float _force;

	private float _targetHeight;

	private bool _jumping;

	protected override void OnStartAndEnable()
	{
		base.OnStartAndEnable();
		_unit = this.GetUnitFacade();
	}

	public void Jump()
	{
		Jump(defaultHeight, defaultForce);
	}

	public void Jump(float toHeight, float force)
	{
		if (!_jumping && _unit.isGrounded)
		{
			_jumping = true;
			_force = force;
			_targetHeight = _unit.position.y + toHeight;
		}
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (_jumping)
		{
			_jumping = _unit.position.y < _targetHeight;
			output.overrideHeightNavigation = true;
			output.verticalForce = _force;
		}
	}
}
