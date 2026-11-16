using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(RectTransform))]
public class TraceRectTransformBounds : MonoBehaviour
{
	[Range(-10f, 10f)]
	public float speed = 1f;

	public float offset;

	[Required]
	public RectTransform toTrace;

	public Vector2 multiplier = Vector2.one;

	private RectTransform _rectTransform;

	private float _time;

	protected void Awake()
	{
		_rectTransform = GetComponent<RectTransform>();
		_time = offset;
	}

	protected void Update()
	{
		Vector2 position = toTrace.rect.position;
		position.x += toTrace.rect.width / 2f;
		position.y += toTrace.rect.height / 2f;
		_time += Time.deltaTime * speed;
		position += new Vector2(Mathf.Cos(_time) * (toTrace.rect.width / 2f) * multiplier.x, Mathf.Sin(_time) * (toTrace.rect.height / 2f) * multiplier.y);
		_rectTransform.anchoredPosition = position;
	}
}
