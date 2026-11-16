using Apex.Services;
using Apex.Units;
using Apex.Utilities;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steer for Containment", 1022)]
[ApexComponent("Steering")]
public class SteerForContainmentComponent : SteeringComponent
{
	[MinCheck(0.1f, label = "Buffer Distance", tooltip = "How far away in meters, from the unit, it height samples in 4 directions. Note that this value is added to the unit's radius, so small values are recommended")]
	public float bufferDistance = 0.3f;

	[Tooltip("If true, draws debug information, including the latest containment vector.")]
	public bool drawGizmos;

	private ISampleHeights _heightSampler;

	private Vector3 _lastContainVector;

	private IUnitFacade _unitData;

	private HeightNavigationCapabilities _heightCaps;

	protected override void Start()
	{
		base.Start();
		_heightSampler = GameServices.heightStrategy.heightSampler;
		IUnitFacade unitFacade = this.GetUnitFacade();
		_heightCaps = unitFacade.heightNavigationCapability;
		bufferDistance += unitFacade.radius;
	}

	public override void GetDesiredSteering(SteeringInput input, SteeringOutput output)
	{
		_lastContainVector = Vector3.zero;
		if (input.grid == null)
		{
			return;
		}
		_unitData = input.unit;
		if (_unitData.isGrounded)
		{
			Vector3 position = _unitData.position;
			Vector3 vector = Vector3.zero;
			float y = _unitData.basePosition.y;
			Vector3 position2 = position + Vector3.right * bufferDistance;
			Vector3 position3 = position + Vector3.left * bufferDistance;
			bool flag = _heightSampler.TrySampleHeight(position2, out var height);
			bool flag2 = _heightSampler.TrySampleHeight(position3, out var height2);
			if (!flag || y - height > _heightCaps.maxDropHeight || height - y > _heightCaps.maxClimbHeight)
			{
				vector = Vector3.left;
			}
			else if (!flag2 || y - height2 > _heightCaps.maxDropHeight || height2 - y > _heightCaps.maxClimbHeight)
			{
				vector = Vector3.right;
			}
			Vector3 position4 = position + Vector3.forward * bufferDistance;
			Vector3 position5 = position + Vector3.back * bufferDistance;
			bool flag3 = _heightSampler.TrySampleHeight(position4, out var height3);
			bool flag4 = _heightSampler.TrySampleHeight(position5, out var height4);
			if (!flag3 || y - height3 > _heightCaps.maxDropHeight || height3 - y > _heightCaps.maxClimbHeight)
			{
				vector = ((vector.sqrMagnitude == 0f) ? Vector3.back : (vector + Vector3.back).normalized);
			}
			else if (!flag4 || y - height4 > _heightCaps.maxDropHeight || height4 - y > _heightCaps.maxClimbHeight)
			{
				vector = ((vector.sqrMagnitude == 0f) ? Vector3.forward : (vector + Vector3.forward).normalized);
			}
			if (vector.sqrMagnitude != 0f)
			{
				output.desiredAcceleration = (_lastContainVector = vector * input.maxAcceleration);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (drawGizmos && _lastContainVector.sqrMagnitude == 0f)
		{
			Vector3 position = _unitData.position;
			Gizmos.color = Color.red;
			Gizmos.DrawLine(position, position + _lastContainVector);
		}
	}
}
