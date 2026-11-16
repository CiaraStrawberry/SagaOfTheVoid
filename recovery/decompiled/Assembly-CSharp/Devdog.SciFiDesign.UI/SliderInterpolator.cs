using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(Slider))]
public class SliderInterpolator : MonoBehaviour
{
	[Range(0f, 1f)]
	[SerializeField]
	private float _from;

	[Range(0f, 1f)]
	public float minFrom;

	[Range(0f, 1f)]
	[SerializeField]
	private float _to = 1f;

	[Range(0f, 1f)]
	public float maxTo = 1f;

	public float startDelay;

	public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

	public float speed = 1f;

	private Slider _slider;

	private float _timer;

	private WaitForSeconds _startDelay;

	public float from => Mathf.Max(_from, minFrom);

	public float to => Mathf.Min(_to, maxTo);

	public Slider slider => _slider;

	protected void Awake()
	{
		_slider = GetComponent<Slider>();
		_startDelay = new WaitForSeconds(startDelay);
		InterpolateValue();
	}

	public void InterpolateValue(float to)
	{
		_to = to;
		InterpolateValue();
	}

	public void InterpolateValue(float from, float to)
	{
		_from = from;
		_to = to;
		InterpolateValue();
	}

	public void InterpolateValue()
	{
		StartCoroutine(_InterpolateValue());
	}

	protected IEnumerator _InterpolateValue()
	{
		_timer = 0f;
		_slider.value = _from;
		yield return _startDelay;
		while (_timer < 1f)
		{
			_timer += Time.deltaTime * speed;
			float val = Mathf.Lerp(_from, _to, _timer);
			_slider.value = animationCurve.Evaluate(_timer) * val;
			yield return null;
		}
	}
}
