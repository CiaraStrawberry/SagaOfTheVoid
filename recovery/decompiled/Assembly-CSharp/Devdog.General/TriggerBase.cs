using System;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

[DisallowMultipleComponent]
public abstract class TriggerBase : MonoBehaviour
{
	public bool useWhenPlayerComesInRange;

	public bool blockPlayerInput;

	private bool _inRange;

	private ITriggerRangeHandler _rangeHandler;

	private static List<ITriggerCallbacks> _callbacks = new List<ITriggerCallbacks>();

	public bool isInUse => TriggerManager.currentActiveTrigger == this;

	[Obsolete("Use TriggerManager.currentActiveTrigger instead.")]
	protected static TriggerBase currentTrigger
	{
		get
		{
			return TriggerManager.currentActiveTrigger;
		}
		set
		{
			TriggerManager.currentActiveTrigger = value;
		}
	}

	public virtual bool inRange
	{
		get
		{
			if (rangeHandler == null || rangeHandler.Equals(null))
			{
				return _inRange;
			}
			return rangeHandler.IsPlayerInRange(ManagerBase<PlayerManager>.instance.currentPlayer);
		}
		protected set
		{
			_inRange = value;
		}
	}

	public ITriggerRangeHandler rangeHandler
	{
		get
		{
			if (_rangeHandler == null || _rangeHandler.Equals(null))
			{
				_rangeHandler = GetComponentInChildren<ITriggerRangeHandler>();
			}
			return _rangeHandler;
		}
	}

	protected virtual void Awake()
	{
	}

	protected virtual void OnDestroy()
	{
		if (isInUse && !(ManagerBase<PlayerManager>.instance == null))
		{
			UnUse();
			UnityEngine.Object.Destroy(GetComponent<ITriggerInputHandler>() as Component);
			UnityEngine.Object.Destroy(GetComponent<ITriggerRangeHandler>() as Component);
		}
	}

	public virtual bool Toggle()
	{
		return Toggle(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	public virtual bool Toggle(Player player)
	{
		if (isInUse)
		{
			return UnUse(player);
		}
		return Use(player);
	}

	protected bool CanUse()
	{
		return CanUse(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	public virtual bool CanUse(Player player)
	{
		if (!enabled || !inRange)
		{
			return false;
		}
		if (isInUse)
		{
			return true;
		}
		if (!InputManager.CanReceiveUIInput(gameObject))
		{
			return false;
		}
		return true;
	}

	protected bool CanUnUse()
	{
		return CanUnUse(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	public virtual bool CanUnUse(Player player)
	{
		if (!enabled || !inRange)
		{
			return false;
		}
		if (!isInUse)
		{
			return false;
		}
		if (!InputManager.CanReceiveUIInput(gameObject))
		{
			return false;
		}
		return true;
	}

	public bool Use()
	{
		return Use(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	public abstract bool Use(Player player);

	public bool UnUse()
	{
		return UnUse(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	public abstract bool UnUse(Player player);

	public abstract void DoVisuals();

	public abstract void UndoVisuals();

	public virtual void NotifyCameInRange(Player player)
	{
		inRange = true;
		if (useWhenPlayerComesInRange)
		{
			Use(player);
		}
	}

	public virtual void NotifyWentOutOfRange(Player player)
	{
		if (isInUse)
		{
			UnUse(player);
		}
		inRange = false;
	}

	private void UpdateTriggerCallbacks()
	{
		GetComponents(_callbacks);
	}

	protected virtual void NotifyTriggerUsed(Player player)
	{
		if (blockPlayerInput)
		{
			InputManager.LimitPlayerInputTo(gameObject);
		}
		UpdateTriggerCallbacks();
		for (int i = 0; i < _callbacks.Count; i++)
		{
			ITriggerCallbacks triggerCallbacks = _callbacks[i];
			Behaviour behaviour = triggerCallbacks as Behaviour;
			if ((!(behaviour != null) || behaviour.isActiveAndEnabled) && triggerCallbacks.OnTriggerUsed(player))
			{
				break;
			}
		}
	}

	protected virtual void NotifyTriggerUnUsed(Player player)
	{
		if (blockPlayerInput)
		{
			InputManager.RemovePlayerLimitInput(gameObject);
		}
		UpdateTriggerCallbacks();
		for (int i = 0; i < _callbacks.Count; i++)
		{
			ITriggerCallbacks triggerCallbacks = _callbacks[i];
			Behaviour behaviour = triggerCallbacks as Behaviour;
			if ((!(behaviour != null) || behaviour.isActiveAndEnabled) && triggerCallbacks.OnTriggerUnUsed(player))
			{
				break;
			}
		}
	}
}
