using Devdog.General.UI;
using UnityEngine;

namespace Devdog.General;

public class Trigger : TriggerBase
{
	[Header("The window")]
	[SerializeField]
	private UIWindowField _window;

	[Header("Animations & Audio")]
	public MotionInfo useAnimation;

	public MotionInfo unUseAnimation;

	public AudioClipInfo useAudioClip;

	public AudioClipInfo unUseAudioClip;

	protected Animator animator;

	protected ITriggerWindowContainer windowContainer;

	public bool handleWindowDirectly
	{
		get
		{
			if (windowContainer != null && !windowContainer.Equals(null))
			{
				return false;
			}
			return true;
		}
	}

	public UIWindowField window
	{
		get
		{
			return _window;
		}
		set
		{
			_window = value;
		}
	}

	public UIWindow windowToUse
	{
		get
		{
			if (windowContainer != null)
			{
				return windowContainer.window;
			}
			return window.window;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		animator = GetComponent<Animator>();
		windowContainer = GetComponent<ITriggerWindowContainer>();
	}

	protected virtual void WindowOnHide()
	{
		DoUnUse(ManagerBase<PlayerManager>.instance.currentPlayer);
	}

	protected virtual void WindowOnShow()
	{
	}

	public override void DoVisuals()
	{
		if (useAnimation.motion != null && animator != null)
		{
			animator.Play(useAnimation);
		}
		AudioManager.AudioPlayOneShot(useAudioClip);
	}

	public override void UndoVisuals()
	{
		if (unUseAnimation.motion != null && animator != null)
		{
			animator.Play(unUseAnimation);
		}
		AudioManager.AudioPlayOneShot(unUseAudioClip);
	}

	private void SubscribeToWindowEvents(UIWindow window)
	{
		if (window != null)
		{
			window.OnShow += WindowOnShow;
			window.OnHide += WindowOnHide;
			if (handleWindowDirectly)
			{
				window.Show();
			}
		}
	}

	private void UnSubscribeFromWindowEvents(UIWindow window)
	{
		if (window != null)
		{
			window.OnShow -= WindowOnShow;
			window.OnHide -= WindowOnHide;
			if (handleWindowDirectly)
			{
				window.Hide();
			}
		}
	}

	public override bool Use(Player player)
	{
		if (!CanUse(player))
		{
			return false;
		}
		if (isInUse)
		{
			return true;
		}
		DoUse(player);
		return true;
	}

	protected virtual void DoUse(Player player)
	{
		if (TriggerManager.currentActiveTrigger != null)
		{
			TriggerManager.currentActiveTrigger.UnUse(player);
		}
		SubscribeToWindowEvents(windowToUse);
		DoVisuals();
		TriggerManager.currentActiveTrigger = this;
		NotifyTriggerUsed(player);
	}

	public virtual void ForceUseWithoutStateAndUI(GameObject user)
	{
		DoVisuals();
		NotifyTriggerUsed(null);
	}

	public override bool UnUse(Player player)
	{
		if (!CanUnUse(player))
		{
			return false;
		}
		DoUnUse(player);
		return true;
	}

	protected virtual void DoUnUse(Player player)
	{
		UnSubscribeFromWindowEvents(windowToUse);
		UndoVisuals();
		TriggerManager.currentActiveTrigger = null;
		NotifyTriggerUnUsed(player);
	}

	public virtual void ForceUnUseWithoutStateAndUI(GameObject user)
	{
		UndoVisuals();
		NotifyTriggerUnUsed(null);
	}
}
