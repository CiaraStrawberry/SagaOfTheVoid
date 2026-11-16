using Apex.Services;
using Apex.Units;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer to Auto Jump", 1034)]
[ApexComponent("Steering")]
public class SteerToAutoJumpComponent : SteeringComponent
{
	[Tooltip("How far ahead the unit will scan for elevated terrain that it needs to jump onto.")]
	public float scanDistance = 1f;

	[Tooltip("The minimum height of elevated terrain to cause the unit to jump.")]
	public float minimumHeightToJump = 0.4f;

	private IUnitFacade _unit;

	private ISampleHeights _heightSampler;

	private float _force;

	private float _targetHeight;

	private bool _jumping;

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		if (!_jumping)
		{
			if (!_unit.isGrounded)
			{
				return;
			}
			Vector3 position = _unit.position + _unit.forward * scanDistance;
			float y = _unit.position.y;
			_targetHeight = _heightSampler.SampleHeight(position) + _unit.baseToPositionOffset;
			if (_targetHeight - y < minimumHeightToJump || _targetHeight - y - _unit.heightNavigationCapability.maxClimbHeight > 0.0001f)
			{
				return;
			}
			float num = scanDistance / 2f;
			float magnitude = _unit.velocity.magnitude;
			float num2 = num / magnitude;
			float num3 = -0.5f * input.gravity * num2 * num2;
			_targetHeight += num3;
			_force = _targetHeight / (Time.fixedDeltaTime * num2);
		}
		_jumping = _unit.position.y < _targetHeight;
		output.overrideHeightNavigation = true;
		output.verticalForce = _force;
	}

	protected override void OnStartAndEnable()
	{
		_heightSampler = GameServices.heightStrategy.heightSampler;
		if (_unit == null)
		{
			_unit = this.GetUnitFacade();
		}
		base.OnStartAndEnable();
	}
}
