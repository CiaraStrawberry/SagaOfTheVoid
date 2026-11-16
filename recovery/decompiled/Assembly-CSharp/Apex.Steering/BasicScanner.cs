using Apex.LoadBalancing;
using Apex.Services;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering;

[AddComponentMenu("")]
[ApexComponent("Steering")]
public class BasicScanner : ExtendedMonoBehaviour, ILoadBalanced
{
	public float scanInterval = 1f;

	public float scanRadius = 6f;

	private Collider[] _units = new Collider[0];

	bool ILoadBalanced.repeat => enabled;

	public Collider[] Units => _units;

	protected override void OnStartAndEnable()
	{
		NavLoadBalancer.scanners.Add(this, scanInterval);
	}

	private void OnDisable()
	{
		NavLoadBalancer.scanners.Remove(this);
	}

	float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		_units = UnityServices.physics.OverlapSphere(transform.position, scanRadius, Layers.units);
		return null;
	}
}
