using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class TriggerRangeHandler : MonoBehaviour, ITriggerRangeHandler
{
	[SerializeField]
	private float _useRange = 10f;

	private readonly List<Player> _playersInRange = new List<Player>();

	private SphereCollider _sphereCollider;

	private Rigidbody _rigidbody;

	private TriggerBase _trigger;

	public float useRange => _useRange;

	protected void Awake()
	{
		_trigger = GetComponentInParent<TriggerBase>();
		_rigidbody = gameObject.GetOrAddComponent<Rigidbody>();
		_rigidbody.useGravity = false;
		_rigidbody.isKinematic = true;
		_sphereCollider = gameObject.GetOrAddComponent<SphereCollider>();
		_sphereCollider.isTrigger = true;
		_sphereCollider.radius = useRange;
		gameObject.layer = 2;
	}

	public IEnumerable<Player> GetPlayersInRange()
	{
		return _playersInRange;
	}

	public bool IsPlayerInRange(Player player)
	{
		return _playersInRange.Contains(player);
	}

	protected void OnTriggerEnter(Collider other)
	{
		Player component = other.gameObject.GetComponent<Player>();
		if (component != null)
		{
			_playersInRange.Add(component);
			_trigger.NotifyCameInRange(component);
		}
	}

	protected void OnTriggerExit(Collider other)
	{
		Player component = other.gameObject.GetComponent<Player>();
		if (component != null)
		{
			_trigger.NotifyWentOutOfRange(component);
			_playersInRange.Remove(component);
		}
	}
}
