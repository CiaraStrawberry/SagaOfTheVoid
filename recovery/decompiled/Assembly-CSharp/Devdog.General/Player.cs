using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

[DisallowMultipleComponent]
public class Player : MonoBehaviour
{
	public IPlayerTriggerHandler triggerHandler;

	public BestTriggerSelectorBase triggerSelector;

	private List<IPlayerInputCallbacks> _playerInputCallbacks = new List<IPlayerInputCallbacks>();

	protected virtual void Awake()
	{
		SetTriggerHandler();
		if (ManagerBase<PlayerManager>.instance != null)
		{
			ManagerBase<PlayerManager>.instance.currentPlayer = this;
		}
	}

	protected virtual void Start()
	{
		InputManager.OnPlayerInputChanged += OnPlayerInputChanged;
	}

	protected virtual void OnDestroy()
	{
		InputManager.OnPlayerInputChanged -= OnPlayerInputChanged;
	}

	protected virtual void OnPlayerInputChanged(bool isInputLimited)
	{
		GetComponentsInChildren(includeInactive: true, _playerInputCallbacks);
		foreach (IPlayerInputCallbacks playerInputCallback in _playerInputCallbacks)
		{
			playerInputCallback.SetInputActive(!isInputLimited);
		}
	}

	protected virtual void SetTriggerHandler()
	{
		GameObject gameObject = new GameObject("_TriggerHandler");
		gameObject.transform.SetParent(transform);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localRotation = Quaternion.identity;
		gameObject.transform.localScale = Vector3.one;
		PlayerTriggerHandler playerTriggerHandler = gameObject.AddComponent<PlayerTriggerHandler>();
		playerTriggerHandler.player = this;
		playerTriggerHandler.selector = triggerSelector;
		triggerHandler = playerTriggerHandler;
	}
}
