using UnityEngine;
using UnityEngine.UI;

namespace Devdog.General.UI;

public class UIWindowPage : UIWindow
{
	[Header("Page specific")]
	public bool isDefaultPage = true;

	[SerializeField]
	protected bool _isEnabled = true;

	public Button myButton;

	private UIWindow _windowParent;

	public bool isEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			_isEnabled = value;
			myButton.enabled = !isEnabled;
			if (!_isEnabled)
			{
				Hide();
			}
		}
	}

	public UIWindow windowParent
	{
		get
		{
			if (_windowParent == null)
			{
				_windowParent = transform.parent.GetComponentInParent<UIWindow>();
			}
			return _windowParent;
		}
	}

	protected override void Awake()
	{
		base.Awake();
	}

	protected override void LevelStart()
	{
		if (windowParent != null && !windowParent.pages.Contains(this))
		{
			windowParent.pages.Add(this);
		}
		base.LevelStart();
	}

	protected override void DoShow(bool resetCurrentPage)
	{
		if (!isVisible)
		{
			base.DoShow(resetCurrentPage);
			windowParent.NotifyChildShown(this);
		}
	}

	protected override void DoHide()
	{
		base.DoHide();
	}
}
