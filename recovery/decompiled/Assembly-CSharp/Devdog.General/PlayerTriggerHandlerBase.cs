using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public abstract class PlayerTriggerHandlerBase<T> : MonoBehaviour, IPlayerTriggerHandler where T : Component
{
	[SerializeField]
	private BestTriggerSelectorBase _selector;

	private TriggerBase _selectedTrigger;

	protected readonly List<TriggerBase> triggersInRange = new List<TriggerBase>();

	public Player player { get; set; }

	public BestTriggerSelectorBase selector
	{
		get
		{
			return _selector;
		}
		set
		{
			_selector = value;
		}
	}

	public TriggerBase selectedTrigger
	{
		get
		{
			return _selectedTrigger;
		}
		protected set
		{
			TriggerBase triggerBase = _selectedTrigger;
			_selectedTrigger = value;
			if (triggerBase != _selectedTrigger && OnSelectedTriggerChanged != null)
			{
				OnSelectedTriggerChanged(triggerBase, _selectedTrigger);
			}
		}
	}

	public event Action<TriggerBase, TriggerBase> OnSelectedTriggerChanged;

	protected virtual void Awake()
	{
		gameObject.layer = 2;
		InvokeRepeating("UpdateSelectedTrigger", 0f, 0.2f);
	}

	protected virtual void UpdateSelectedTrigger()
	{
		if (!(selector == null))
		{
			selectedTrigger = selector.GetBestTrigger(player, triggersInRange);
		}
	}

	protected virtual void Update()
	{
		if (selectedTrigger != null)
		{
			ITriggerInputHandler component = selectedTrigger.GetComponent<ITriggerInputHandler>();
			if (component != null && component.AreKeysDown())
			{
				component.Use();
			}
		}
		else
		{
			UpdateSelectedTrigger();
		}
	}

	public virtual bool IsInRangeOfTrigger(TriggerBase trigger)
	{
		return triggersInRange.Contains(trigger);
	}

	protected void NotifyTriggerEnter(T other)
	{
		TriggerBase componentInChildren = other.GetComponentInChildren<TriggerBase>();
		if (componentInChildren != null && (componentInChildren.rangeHandler == null || componentInChildren.rangeHandler.Equals(null)))
		{
			triggersInRange.Add(componentInChildren);
			componentInChildren.NotifyCameInRange(player);
		}
	}

	protected void NotifyTriggerExit(T other)
	{
		TriggerBase componentInChildren = other.GetComponentInChildren<TriggerBase>();
		if (componentInChildren != null && (componentInChildren.rangeHandler == null || componentInChildren.rangeHandler.Equals(null)))
		{
			componentInChildren.NotifyWentOutOfRange(player);
			triggersInRange.Remove(componentInChildren);
		}
	}
}
