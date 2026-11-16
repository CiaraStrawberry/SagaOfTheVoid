using System;
using Devdog.General;
using Devdog.SciFiDesign.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Devdog.SciFiDesign;

public class ShipVisuals : MonoBehaviour
{
	[Serializable]
	public class Element
	{
		public float displayValueMultiplier = 1f;

		[Required]
		public ImageFillInterpolator fill;

		public float increaseFactor = 1f;

		public float decreaseFactor = 1f;

		public float startValue;

		[Header("UI")]
		public Text text;

		public string textFormat = "{0} AU/S";

		public Color fromColor = Color.green;

		public Color toColor = Color.green;

		public AnimationCurve blendCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

		public float value { get; protected set; }

		public float maxValue { get; set; }

		public Element()
		{
			value = startValue;
		}

		public void Increase()
		{
			value += Time.deltaTime * increaseFactor;
			value = Mathf.Clamp(value, startValue, fill.minFrom + (fill.maxTo - fill.minFrom) * maxValue + startValue);
			Repaint();
		}

		public void Decrease()
		{
			value -= Time.deltaTime * decreaseFactor;
			value = Mathf.Max(value, startValue);
			Repaint();
		}

		public void Repaint()
		{
			float num = fill.minFrom + (fill.maxTo - fill.minFrom) * (value / (fill.minFrom + (fill.maxTo - fill.minFrom) * maxValue));
			fill.image.fillAmount = Mathf.Clamp(num, fill.minFrom, fill.maxTo);
			float t = blendCurve.Evaluate(value / (fill.minFrom + (fill.maxTo - fill.minFrom) * maxValue));
			fill.image.color = Color.Lerp(fromColor, toColor, t);
			text.text = string.Format(textFormat, Math.Round(value * displayValueMultiplier, 2));
		}
	}

	[Header("Particle systems")]
	[Required]
	public ParticleSystem small;

	[Required]
	public ParticleSystem medium;

	[Header("Speed")]
	public Element speed = new Element();

	[Header("G-Force")]
	public Element gForce = new Element();

	public UnityEvent onGForceWarningAboveEvent = new UnityEvent();

	public UnityEvent onGForceWarningBelowEvent = new UnityEvent();

	private float _warningGForceSpeed = 10f;

	private bool _boost;

	private bool _aboveGForceLimit;

	protected virtual void Start()
	{
		speed.maxValue = 5f;
		gForce.maxValue = 32f;
		small.Play();
	}

	protected void Update()
	{
		if (Input.GetKeyDown(KeyCode.LeftShift))
		{
			StartBoost();
		}
		else if (Input.GetKeyUp(KeyCode.LeftShift))
		{
			StopBoost();
		}
		if (_boost)
		{
			DoBoost();
		}
		else
		{
			RestoreBoost();
		}
	}

	protected virtual void RestoreBoost()
	{
		if (speed.value > 0f)
		{
			speed.Decrease();
		}
		if (gForce.value > 0f)
		{
			gForce.Decrease();
			if (_aboveGForceLimit)
			{
				_aboveGForceLimit = false;
				onGForceWarningBelowEvent.Invoke();
			}
		}
	}

	protected virtual void DoBoost()
	{
		speed.Increase();
		gForce.Increase();
		if (gForce.value > _warningGForceSpeed && !_aboveGForceLimit)
		{
			_aboveGForceLimit = true;
			onGForceWarningAboveEvent.Invoke();
		}
	}

	protected virtual void StartBoost()
	{
		medium.Play();
		_boost = true;
	}

	protected virtual void StopBoost()
	{
		medium.Stop();
		_boost = false;
	}
}
