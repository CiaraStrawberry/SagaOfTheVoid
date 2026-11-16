using UnityEngine;
using UnityEngine.EventSystems;

namespace Devdog.General.UI;

[RequireComponent(typeof(UIWindow))]
public class DraggableWindow : MonoBehaviour, IBeginDragHandler, IDragHandler, IPointerDownHandler, IEventSystemHandler
{
	[Header("Dragging")]
	public float dragSpeed = 1f;

	[Header("Bounds")]
	public bool preventFromGoingOutOfBounds = true;

	public Vector2 spacing = new Vector2(100f, 100f);

	[Header("Bring to foreground")]
	public bool onClickBringToForeground = true;

	public bool onWindowShowBringToForeground = true;

	public int maxForegroundIndex = 10;

	private Vector2 _dragOffset;

	private Vector2 _maxPos;

	private Vector2 _minPos;

	private RectTransform _rectTransform;

	private UIWindow _window;

	public static DraggableWindow currentWindow { get; protected set; }

	public UIWindow window
	{
		get
		{
			if (_window == null)
			{
				_window = GetComponent<UIWindow>();
			}
			return _window;
		}
		set
		{
			_window = value;
		}
	}

	public void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		float num = _rectTransform.rect.width * _rectTransform.pivot.x;
		float num2 = _rectTransform.rect.height * _rectTransform.pivot.y;
		_minPos = new Vector2(num - spacing.x, num2 - spacing.y);
		_maxPos = new Vector2((float)Screen.width + num - spacing.x, (float)Screen.height + num2 - spacing.y);
		if (onWindowShowBringToForeground)
		{
			window.OnShow += MoveToForeground;
		}
	}

	public void OnBeginDrag(PointerEventData eventData)
	{
		_dragOffset = new Vector2(_rectTransform.anchoredPosition.x, _rectTransform.anchoredPosition.y) - eventData.position;
	}

	public void OnDrag(PointerEventData eventData)
	{
		_rectTransform.anchoredPosition = new Vector3(eventData.position.x + _dragOffset.x * dragSpeed, eventData.position.y + _dragOffset.y * dragSpeed, 0f);
		if (preventFromGoingOutOfBounds)
		{
			_rectTransform.position = Clamp(_rectTransform.position, _minPos, _maxPos);
		}
	}

	private Vector2 Clamp(Vector2 a, Vector2 min, Vector2 max)
	{
		a.x = Mathf.Clamp(a.x, min.x, max.x);
		a.y = Mathf.Clamp(a.y, min.y, max.y);
		return a;
	}

	public void OnPointerDown(PointerEventData eventData)
	{
		if (onClickBringToForeground)
		{
			MoveToForeground();
		}
	}

	public virtual void MoveBack()
	{
		transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
	}

	public virtual void MoveToBackground()
	{
		transform.SetAsFirstSibling();
	}

	public virtual void MoveUp()
	{
		transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
	}

	public virtual void MoveToForeground()
	{
		if (!(currentWindow == this))
		{
			transform.SetSiblingIndex(maxForegroundIndex);
			currentWindow = this;
		}
	}
}
