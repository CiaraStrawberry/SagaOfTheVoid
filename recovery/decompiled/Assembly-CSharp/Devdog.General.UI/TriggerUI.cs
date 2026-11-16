using System;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.General.UI;

[RequireComponent(typeof(UIWindow))]
public class TriggerUI : MonoBehaviour
{
	[Serializable]
	public class KeyIconBinding
	{
		public KeyCode keyCode;

		public Sprite sprite;
	}

	public Image imageIcon;

	public Text shortcutText;

	public bool moveToTriggerLocation = true;

	public KeyIconBinding[] keyIconBindings = new KeyIconBinding[0];

	private UIWindow _window;

	protected virtual void Awake()
	{
		_window = GetComponent<UIWindow>();
	}

	protected virtual void Start()
	{
		ManagerBase<PlayerManager>.instance.OnPlayerChanged += OnPlayerChanged;
		if (ManagerBase<PlayerManager>.instance.currentPlayer != null)
		{
			OnPlayerChanged(null, ManagerBase<PlayerManager>.instance.currentPlayer);
		}
	}

	protected void LateUpdate()
	{
		if (moveToTriggerLocation && ManagerBase<PlayerManager>.instance.currentPlayer != null)
		{
			UpdatePosition(ManagerBase<PlayerManager>.instance.currentPlayer.triggerHandler.selectedTrigger);
		}
	}

	private void OnPlayerChanged(Player oldPlayer, Player newPlayer)
	{
		if (oldPlayer != null)
		{
			oldPlayer.triggerHandler.OnSelectedTriggerChanged -= BestTriggerChanged;
		}
		newPlayer.triggerHandler.OnSelectedTriggerChanged += BestTriggerChanged;
		BestTriggerChanged(null, newPlayer.triggerHandler.selectedTrigger);
	}

	private void BestTriggerChanged(TriggerBase old, TriggerBase newBest)
	{
		if (newBest != null)
		{
			_window.Show();
			Repaint(newBest);
			if (moveToTriggerLocation)
			{
				UpdatePosition(newBest);
			}
		}
		else
		{
			_window.Hide();
		}
	}

	protected virtual Sprite GetIcon(ITriggerInputHandler inputHandler, TriggerBase trigger)
	{
		KeyIconBinding keyIconBinding = keyIconBindings.FirstOrDefault((KeyIconBinding o) => o.keyCode.ToString() == inputHandler.ToString());
		if (keyIconBinding == null)
		{
			DevdogLogger.LogWarning("Couldn't find binding for keycode : " + inputHandler.ToString(), this);
			return null;
		}
		return keyIconBinding.sprite;
	}

	protected virtual void UpdatePosition(TriggerBase trigger)
	{
		if (trigger != null)
		{
			transform.position = Camera.main.WorldToScreenPoint(trigger.transform.position);
		}
	}

	public virtual void Repaint(TriggerBase trigger)
	{
		_window.Show();
		Sprite sprite = null;
		string text = string.Empty;
		if (trigger != null)
		{
			ITriggerInputHandler component = trigger.GetComponent<ITriggerInputHandler>();
			if (component != null)
			{
				sprite = GetIcon(component, trigger);
				text = component.ToString();
			}
		}
		if (imageIcon != null && imageIcon.sprite != sprite)
		{
			imageIcon.sprite = sprite;
		}
		if (shortcutText != null && shortcutText.text != text)
		{
			shortcutText.text = text;
		}
	}
}
