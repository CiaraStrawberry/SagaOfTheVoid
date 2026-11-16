using Apex.DataStructures;
using Apex.GameState;
using Apex.LoadBalancing;
using Apex.Services;
using Apex.Units;
using Apex.Utilities;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Steering/Steering Scanner", 1038)]
[ApexComponent("Steering")]
public class SteeringScanner : ExtendedMonoBehaviour, ILoadBalanced, ISupportRuntimeStateChange
{
	[MinCheck(0.1f, label = "Scan Interval", tooltip = "How often the load balancer executes this scanner. Lower values grant better behaviour, but higher values grant better performance")]
	public float scanInterval = 0.2f;

	[MinCheck(1f, label = "Scan Radius", tooltip = "How large a radius this scanner scans units in, i.e. how far away other units and blocks can be and still be perceived. Higher values grant better behaviour, lower values better performance.")]
	public float scanRadius = 9f;

	[Label("Forecast Distance", "If not 0, the position used as origin for the overlap sphere cast is projected in velocity direction by this magnitude. This distance is always capped by scanRadius.")]
	public float forecastDistance = 1f;

	[Label("Sort Units With Distance", "If true, the scanner sorts all scanned units according to their distance to this unit (nearest first). Must be true if used in conjunction with UnitAvoidance without accumulating avoid vectors.")]
	public bool sortUnitsWithDistance = true;

	[Label("Filter Away Units in Same Group", "If true, the scanner does not return units in the same transient group as this unit, if false returns all units within scanRadius.")]
	public bool filterAwayUnitsInSameGroup;

	private Collider[] _blocks = new Collider[0];

	private DynamicArray<IUnitFacade> _units;

	private IUnitFacade _unitData;

	private DistanceComparer<IUnitFacade> _unitComparer = new DistanceComparer<IUnitFacade>(ascending: true);

	bool ILoadBalanced.repeat => enabled;

	public IIterable<IUnitFacade> units => _units;

	public Collider[] blocks => _blocks;

	protected override void OnStartAndEnable()
	{
		NavLoadBalancer.scanners.Add(this, scanInterval, delayFirstUpdate: false);
		_unitData = this.GetUnitFacade();
		_units = new DynamicArray<IUnitFacade>(5);
		if (forecastDistance > scanRadius)
		{
			forecastDistance = scanRadius;
		}
	}

	private void OnDisable()
	{
		NavLoadBalancer.scanners.Remove(this);
	}

	float? ILoadBalanced.ExecuteUpdate(float deltaTime, float nextInterval)
	{
		_units.Clear();
		Vector3 position = _unitData.position;
		Vector3 position2 = ((forecastDistance == 0f) ? position : (position + _unitData.velocity.normalized * forecastDistance));
		_blocks = UnityServices.physics.OverlapSphere(position2, scanRadius, Layers.blocks);
		Collider[] array = UnityServices.physics.OverlapSphere(position2, scanRadius, Layers.units);
		int num = array.Length;
		if (num == 0)
		{
			return null;
		}
		float y = _unitData.basePosition.y;
		GameStateManager gameStateManager = GameServices.gameStateManager;
		for (int i = 0; i < num; i++)
		{
			Collider collider = array[i];
			if (collider == null || collider.Equals(_unitData.collider))
			{
				continue;
			}
			IUnitFacade unitFacade = gameStateManager.GetUnitFacade(collider.gameObject);
			float y2 = unitFacade.basePosition.y;
			if (y + _unitData.height < y2 || y > y2 + unitFacade.height)
			{
				continue;
			}
			if (filterAwayUnitsInSameGroup)
			{
				TransientGroup<IUnitFacade> transientGroup = unitFacade.transientGroup;
				if (transientGroup != null && object.ReferenceEquals(transientGroup, _unitData.transientGroup))
				{
					continue;
				}
			}
			_units.Add(unitFacade);
		}
		if (sortUnitsWithDistance)
		{
			_unitComparer.compareTo = position;
			_units.Sort(_unitComparer);
		}
		return null;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(transform.position, scanRadius);
	}

	public void ReevaluateState()
	{
		if (forecastDistance > scanRadius)
		{
			forecastDistance = scanRadius;
		}
	}
}
