using System;
using System.Collections;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Devdog.General.UI;

[RequireComponent(typeof(Animator))]
public class UIWindow : MonoBehaviour
{
	[Serializable]
	public class UIWindowActionEvent : UnityEvent
	{
	}

	[Header("Behavior")]
	public string windowName = "MyWindow";

	public bool hideOnStart = true;

	public bool resetPositionOnStart = true;

	public bool blockUIInput;

	public bool blockPlayerInput;

	[Header("Audio & Visuals")]
	[SerializeField]
	private MotionInfo _showAnimation;

	private int _showAnimationHash;

	[SerializeField]
	private MotionInfo _hideAnimation;

	private int _hideAnimationHash;

	public AudioClipInfo showAudioClip;

	public AudioClipInfo hideAudioClip;

	[Header("Actions")]
	public UIWindowActionEvent onShowActions = new UIWindowActionEvent();

	public UIWindowActionEvent onHideActions = new UIWindowActionEvent();

	private List<UIWindowPage> _pages = new List<UIWindowPage>();

	private IEnumerator _animationCoroutine;

	private Animator _animator;

	private RectTransform _rectTransform;

	public bool isVisible { get; protected set; }

	public virtual UIWindowPage defaultPage => pages.FirstOrDefault((UIWindowPage o) => o.isDefaultPage);

	public List<UIWindowPage> pages => _pages;

	public UIWindowPage currentPage { get; set; }

	public Animator animator
	{
		get
		{
			if (_animator == null)
			{
				_animator = GetComponent<Animator>();
			}
			return _animator;
		}
	}

	protected RectTransform rectTransform
	{
		get
		{
			if (_rectTransform == null)
			{
				_rectTransform = GetComponent<RectTransform>();
			}
			return _rectTransform;
		}
	}

	public event Action OnHide;

	public event Action OnShow;

	protected virtual void Awake()
	{
		if (_showAnimation.motion != null)
		{
			_showAnimationHash = Animator.StringToHash(_showAnimation.motion.name);
		}
		if (_hideAnimation.motion != null)
		{
			_hideAnimationHash = Animator.StringToHash(_hideAnimation.motion.name);
		}
		if (resetPositionOnStart)
		{
			rectTransform.anchoredPosition = Vector2.zero;
		}
		SceneManager.sceneLoaded += SceneLoaded;
	}

	private void SceneLoaded(Scene scene, LoadSceneMode mode)
	{
		LevelStart();
	}

	protected void OnDestroy()
	{
		SceneManager.sceneLoaded -= SceneLoaded;
	}

	protected virtual void Start()
	{
		LevelStart();
	}

	protected virtual void LevelStart()
	{
		if (hideOnStart)
		{
			HideFirst();
		}
		else
		{
			isVisible = true;
		}
	}

	public void NotifyChildShown(UIWindowPage page)
	{
		currentPage = page;
		HideAllPagesExceptCurrent();
		DoShow(resetCurrentPage: false);
	}

	protected void HideAllPagesExceptCurrent(float waitTime = 0f)
	{
		UIWindowPage uIWindowPage = currentPage;
		foreach (UIWindowPage page in pages)
		{
			if (!(page == uIWindowPage))
			{
				page.Hide(waitTime);
			}
		}
	}

	protected void NotifyWindowHidden()
	{
		if (currentPage != null && currentPage.isVisible)
		{
			currentPage.NotifyWindowHidden();
		}
		if (blockUIInput)
		{
			InputManager.RemoveUILimitInput(gameObject);
		}
		if (blockPlayerInput)
		{
			InputManager.RemovePlayerLimitInput(gameObject);
		}
		onHideActions.Invoke();
		if (OnHide != null)
		{
			OnHide();
		}
	}

	protected void NotifyWindowShown()
	{
		if (blockUIInput)
		{
			InputManager.LimitUIInputTo(gameObject);
		}
		if (blockPlayerInput)
		{
			InputManager.LimitPlayerInputTo(gameObject);
		}
		onShowActions.Invoke();
		if (OnShow != null)
		{
			OnShow();
		}
	}

	protected virtual void SetChildrenActive(bool active)
	{
		foreach (Transform item in base.transform)
		{
			item.gameObject.SetActive(active);
		}
		Graphic[] components = gameObject.GetComponents<Graphic>();
		Graphic[] array = components;
		foreach (Graphic graphic in array)
		{
			graphic.enabled = active;
		}
	}

	public void PlayAudio(AudioClipInfo clip)
	{
		AudioSource component = GetComponent<AudioSource>();
		if (component != null)
		{
			component.Play(clip);
		}
	}

	private void PlayAnimation(MotionInfo clip, int hash, Action callback)
	{
		if (clip.motion != null)
		{
			if (_animationCoroutine != null)
			{
				StopCoroutine(_animationCoroutine);
			}
			_animationCoroutine = _PlayAnimationAndDisableAnimator(clip.motion.averageDuration + 0.1f, hash, callback);
			StartCoroutine(_animationCoroutine);
		}
		else
		{
			animator.enabled = false;
			callback?.Invoke();
		}
	}

	public void Toggle()
	{
		if (isVisible)
		{
			Hide();
		}
		else
		{
			Show();
		}
	}

	public void Show()
	{
		DoShow();
	}

	public void Show(float waitTime)
	{
		if (waitTime > 0f)
		{
			StartCoroutine(_Show(waitTime));
		}
		else
		{
			DoShow();
		}
	}

	protected IEnumerator _Show(float waitTime)
	{
		if (!isVisible)
		{
			if (defaultPage != null)
			{
				defaultPage.Show();
			}
			yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));
			DoShow();
		}
	}

	protected void DoShow()
	{
		DoShow(resetCurrentPage: true);
	}

	protected virtual void DoShow(bool resetCurrentPage)
	{
		if (isVisible)
		{
			return;
		}
		isVisible = true;
		SetChildrenActive(active: true);
		if (resetCurrentPage)
		{
			currentPage = defaultPage;
			if (currentPage != null)
			{
				currentPage.Show();
			}
		}
		PlayAnimation(_showAnimation, _showAnimationHash, null);
		PlayAudio(showAudioClip);
		NotifyWindowShown();
	}

	public virtual void HideFirst()
	{
		isVisible = false;
		animator.enabled = false;
		SetChildrenActive(active: false);
	}

	public void Hide()
	{
		DoHide();
	}

	public void Hide(float waitTime)
	{
		if (waitTime > 0f)
		{
			StartCoroutine(_Hide(waitTime));
		}
		else
		{
			DoHide();
		}
	}

	protected IEnumerator _Hide(float waitTime)
	{
		if (isVisible)
		{
			yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));
			DoHide();
		}
	}

	protected virtual void DoHide()
	{
		if (!isVisible)
		{
			return;
		}
		isVisible = false;
		PlayAnimation(_hideAnimation, _hideAnimationHash, () =>
		{
			if (!isVisible)
			{
				SetChildrenActive(active: false);
			}
		});
		PlayAudio(hideAudioClip);
		NotifyWindowHidden();
	}

	protected IEnumerator _PlayAnimationAndDisableAnimator(float waitTime, int hash, Action callback)
	{
		yield return null;
		IEnumerator before = _animationCoroutine;
		animator.enabled = true;
		animator.Play(hash);
		yield return StartCoroutine(CoroutineUtility.WaitRealtime(waitTime));
		animator.enabled = false;
		callback?.Invoke();
		if (before == _animationCoroutine)
		{
			_animationCoroutine = null;
		}
	}
}
