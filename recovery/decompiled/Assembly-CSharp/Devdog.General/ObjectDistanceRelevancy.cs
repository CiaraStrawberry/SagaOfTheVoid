using System;
using UnityEngine;

namespace Devdog.General;

[RequireComponent(typeof(SphereCollider))]
public sealed class ObjectDistanceRelevancy : MonoBehaviour, IObjectRelevancy
{
	private bool _inTrigger;

	public event Action OnBecameRelevant;

	public event Action OnBecameIrrelevant;

	private void Awake()
	{
		OnValidate();
	}

	private void OnValidate()
	{
		SphereCollider orAddComponent = gameObject.GetOrAddComponent<SphereCollider>();
		orAddComponent.radius = Mathf.Max(10f, orAddComponent.radius);
		orAddComponent.isTrigger = true;
		orAddComponent.gameObject.layer = 2;
	}

	public bool IsRelevant(GameObject obj)
	{
		return _inTrigger;
	}

	private void OnTriggerEnter(Collider other)
	{
		DoOnTriggerEnter(other.gameObject);
	}

	private void OnTriggerEnter2D(Collider2D other)
	{
		DoOnTriggerEnter(other.gameObject);
	}

	private void DoOnTriggerEnter(GameObject obj)
	{
		if (obj.GetComponent<Player>() != null)
		{
			bool inTrigger = _inTrigger;
			_inTrigger = true;
			if (!inTrigger && OnBecameRelevant != null)
			{
				OnBecameRelevant();
			}
		}
	}

	private void OnTriggerExit(Collider other)
	{
		DoOnTriggerExit(other.gameObject);
	}

	private void OnTriggerExit2D(Collider2D other)
	{
		DoOnTriggerExit(other.gameObject);
	}

	private void DoOnTriggerExit(GameObject obj)
	{
		if (obj.GetComponent<Player>() != null)
		{
			bool inTrigger = _inTrigger;
			_inTrigger = false;
			if (inTrigger && OnBecameIrrelevant != null)
			{
				OnBecameIrrelevant();
			}
		}
	}
}
