using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steering Controller", 1037)]
[ApexComponent("Steering")]
public class SteeringController : ExtendedMonoBehaviour
{
	private SteerForFormationComponent _steerForFormation;

	private SteerForPathComponent _steerForPath;

	protected override void Start()
	{
		base.Start();
		_steerForFormation = GetComponent<SteerForFormationComponent>();
		_steerForPath = GetComponent<SteerForPathComponent>();
	}

	public void StartSoloPath()
	{
		if (_steerForFormation != null)
		{
			_steerForFormation.enabled = false;
		}
	}

	public void EndSoloPath()
	{
		if (_steerForFormation != null)
		{
			_steerForFormation.enabled = true;
		}
		if (_steerForPath != null)
		{
			_steerForPath.Stop();
		}
	}
}
