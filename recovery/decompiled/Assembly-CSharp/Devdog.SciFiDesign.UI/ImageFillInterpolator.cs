using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class ImageFillInterpolator : MonoBehaviour
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

	private Image _image;

	private float _timer;

	public float from => Mathf.Max(_from, minFrom);

	public float to => Mathf.Min(_to, maxTo);

	public Image image => _image;

	protected void Awake()
	{
		_image = GetComponent<Image>();
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
		_image.fillAmount = _from;
		yield return new WaitForSeconds(startDelay);
		while (_timer < 1f)
		{
			_timer += Time.deltaTime * speed;
			float val = Mathf.Lerp(_from, _to, _timer);
			_image.fillAmount = animationCurve.Evaluate(_timer) * val;
			yield return null;
		}
	}
}
