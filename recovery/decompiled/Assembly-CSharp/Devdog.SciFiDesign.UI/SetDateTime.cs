using System;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

public class SetDateTime : MonoBehaviour
{
	[SerializeField]
	private Text _textTimeHour;

	[SerializeField]
	private Text _textTimeMinute;

	[SerializeField]
	private Text _textColon;

	[SerializeField]
	private Text _textDay;

	public bool blinkColon;

	public float blinkColonInterval = 0.5f;

	protected void Start()
	{
		InvokeRepeating("UpdateTime", 0f, 60f);
		if (blinkColon)
		{
			InvokeRepeating("UpdateColon", 0f, blinkColonInterval);
		}
	}

	protected void UpdateTime()
	{
		DateTime now = DateTime.Now;
		if (_textTimeHour != null)
		{
			_textTimeHour.text = ((now.Hour >= 10) ? now.Hour.ToString() : ("0" + now.Hour));
		}
		if (_textTimeHour != null)
		{
			_textTimeMinute.text = ((now.Minute >= 10) ? now.Minute.ToString() : ("0" + now.Minute));
		}
		if (_textDay != null)
		{
			_textDay.text = now.DayOfWeek.ToString().ToUpper();
		}
	}

	protected void UpdateColon()
	{
		if (string.IsNullOrEmpty(_textColon.text))
		{
			_textColon.text = ":";
		}
		else
		{
			_textColon.text = string.Empty;
		}
	}
}
