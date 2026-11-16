using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OTween : MonoBehaviour
{
	public enum EaseType
	{
		Linear,
		CubicIn,
		CubicOut,
		BounceIn,
		BounceOut,
		ElasticIn,
		ElasticOut
	}

	private enum VariableType
	{
		Float,
		V3,
		V2
	}

	public string id;

	public GameObject target;

	public EaseType easeType;

	public float delayToStart;

	public float duration;

	public bool isStart;

	public string startCallBack;

	public string updateCallBack;

	public string endCallBack;

	private float startValue1;

	private float startValue2;

	private float startValue3;

	private float endValue1;

	private float endValue2;

	private float endValue3;

	private float startTime;

	private VariableType variableType;

	private static List<OTween> tweens = new List<OTween>();

	private void Update()
	{
		if (!isStart)
		{
			return;
		}
		float num = Time.realtimeSinceStartup - startTime;
		float v = 0f;
		float v2 = 0f;
		float v3 = 0f;
		if (num < duration)
		{
			switch (variableType)
			{
			case VariableType.Float:
				v = UpdateTween(startValue1, endValue1, num);
				break;
			case VariableType.V2:
				v = UpdateTween(startValue1, endValue1, num);
				v2 = UpdateTween(startValue2, endValue2, num);
				break;
			case VariableType.V3:
				v = UpdateTween(startValue1, endValue1, num);
				v2 = UpdateTween(startValue2, endValue2, num);
				v3 = UpdateTween(startValue3, endValue3, num);
				break;
			}
			SendUpdate(v, v2, v3);
		}
		else
		{
			SendUpdate(endValue1, endValue2, endValue3);
			if (!string.IsNullOrEmpty(endCallBack))
			{
				target.SendMessage(endCallBack, SendMessageOptions.DontRequireReceiver);
				tweens.Remove(this);
				UnityEngine.Object.Destroy(this);
			}
		}
	}

	private float UpdateTween(float start, float end, float time)
	{
		float result = 0f;
		switch (easeType)
		{
		case EaseType.Linear:
			result = Linear(time, start, end, duration);
			break;
		case EaseType.BounceIn:
			result = BounceIn(time, start, end, duration);
			break;
		case EaseType.BounceOut:
			result = BounceOut(time, start, end, duration);
			break;
		case EaseType.CubicIn:
			result = CubicIn(time, start, end, duration);
			break;
		case EaseType.CubicOut:
			result = CubicOut(time, start, end, duration);
			break;
		case EaseType.ElasticIn:
			result = ElasticIn(time, start, end, duration);
			break;
		case EaseType.ElasticOut:
			result = ElasticOut(time, start, end, duration);
			break;
		}
		return result;
	}

	private void SendUpdate(float v1, float v2, float v3)
	{
		switch (variableType)
		{
		case VariableType.Float:
			if (!string.IsNullOrEmpty(updateCallBack))
			{
				target.SendMessage(updateCallBack, v1, SendMessageOptions.DontRequireReceiver);
			}
			break;
		case VariableType.V2:
			if (!string.IsNullOrEmpty(updateCallBack))
			{
				target.SendMessage(updateCallBack, new Vector2(v1, v2), SendMessageOptions.DontRequireReceiver);
			}
			break;
		case VariableType.V3:
			if (!string.IsNullOrEmpty(updateCallBack))
			{
				target.SendMessage(updateCallBack, new Vector3(v1, v2, v3), SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}

	private IEnumerator LaunchEase()
	{
		yield return new WaitForSeconds(delayToStart);
		isStart = true;
		startTime = Time.realtimeSinceStartup;
		if (!string.IsNullOrEmpty(startCallBack))
		{
			target.SendMessage(startCallBack, SendMessageOptions.DontRequireReceiver);
		}
	}

	private float Linear(float t, float start, float end, float d)
	{
		return (end - start) * t / d + start;
	}

	private float BounceIn(float t, float start, float end, float d)
	{
		end -= start;
		return end - BounceOut(d - t, 0f, end, d) + start;
	}

	private float BounceOut(float t, float start, float end, float d)
	{
		float num = 7.5625f;
		t /= d;
		end -= start;
		if (t < 372f / 1023f)
		{
			return end * (num * t * t) + start;
		}
		if (t < 744f / 1023f)
		{
			t -= 558f / 1023f;
			return end * (num * t * t + 0.75f) + start;
		}
		if ((double)t < 930.0 / 1023.0)
		{
			t -= 837f / 1023f;
			return end * (num * t * t + 0.9375f) + start;
		}
		t -= 21f / 22f;
		return end * (num * t * t + 63f / 64f) + start;
	}

	private float CubicIn(float t, float start, float end, float d)
	{
		t /= d;
		end -= start;
		return end * t * t * t + start;
	}

	private float CubicOut(float t, float start, float end, float d)
	{
		t /= d;
		t--;
		end -= start;
		return end * (t * t * t + 1f) + start;
	}

	private float ElasticIn(float t, float start, float end, float d)
	{
		end -= start;
		float num = d * 0.3f;
		float num2 = 0f;
		float num3 = 0f;
		if (t == 0f)
		{
			return start;
		}
		if ((t /= d) == 1f)
		{
			return start + end;
		}
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num2 = num / 4f;
		}
		else
		{
			num2 = num / ((float)Math.PI * 2f) * Mathf.Asin(end / num3);
		}
		return 0f - num3 * Mathf.Pow(2f, 10f * --t) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num) + start;
	}

	private float ElasticOut(float t, float start, float end, float d)
	{
		end -= start;
		float num = d * 0.3f;
		float num2 = 0f;
		float num3 = 0f;
		if (t == 0f)
		{
			return start;
		}
		if ((t /= d) == 1f)
		{
			return start + end;
		}
		if (num3 == 0f || num3 < Mathf.Abs(end))
		{
			num3 = end;
			num2 = num * 0.25f;
		}
		else
		{
			num2 = num / ((float)Math.PI * 2f) * Mathf.Asin(end / num3);
		}
		return num3 * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t * d - num2) * ((float)Math.PI * 2f) / num) + end + start;
	}

	private void StartEase()
	{
		StartCoroutine("LaunchEase");
	}

	public static string ValueTo(GameObject obj, EaseType easeType, object startValue, object endValue, float duration, float delaytToStart = 0f, string startCallBack = "", string updateCallBack = "", string endCallBack = "")
	{
		OTween oTween = obj.AddComponent<OTween>();
		oTween.target = obj;
		oTween.easeType = easeType;
		oTween.delayToStart = delaytToStart;
		oTween.duration = duration;
		oTween.isStart = false;
		oTween.startCallBack = startCallBack;
		oTween.updateCallBack = updateCallBack;
		oTween.endCallBack = endCallBack;
		oTween.id = Guid.NewGuid().ToString();
		if (startValue.GetType() == typeof(float))
		{
			oTween.variableType = VariableType.Float;
			oTween.startValue1 = (float)startValue;
			oTween.endValue1 = (float)endValue;
		}
		else if (startValue.GetType() == typeof(Vector2))
		{
			oTween.variableType = VariableType.V2;
			oTween.startValue1 = ((Vector2)startValue).x;
			oTween.startValue2 = ((Vector2)startValue).y;
			oTween.endValue1 = ((Vector2)endValue).x;
			oTween.endValue2 = ((Vector2)endValue).y;
		}
		else if (startValue.GetType() == typeof(Vector3))
		{
			oTween.variableType = VariableType.V3;
			oTween.startValue1 = ((Vector3)startValue).x;
			oTween.startValue2 = ((Vector3)startValue).y;
			oTween.startValue3 = ((Vector3)startValue).z;
			oTween.endValue1 = ((Vector3)endValue).x;
			oTween.endValue2 = ((Vector3)endValue).y;
			oTween.endValue3 = ((Vector3)endValue).z;
		}
		tweens.Add(oTween);
		oTween.StartEase();
		return oTween.id;
	}

	public static void UpdateEndValue(string tweenId, object endValue)
	{
		int num = tweens.FindIndex((OTween t) => t.id == tweenId);
		if (num > -1)
		{
			if (endValue.GetType() == typeof(float))
			{
				tweens[num].endValue1 = (float)endValue;
			}
			else if (endValue.GetType() == typeof(Vector2))
			{
				tweens[num].endValue1 = ((Vector2)endValue).x;
				tweens[num].endValue2 = ((Vector2)endValue).y;
			}
			else if (endValue.GetType() == typeof(Vector3))
			{
				tweens[num].endValue1 = ((Vector3)endValue).x;
				tweens[num].endValue2 = ((Vector3)endValue).y;
				tweens[num].endValue3 = ((Vector3)endValue).z;
			}
		}
	}

	public static void StopTween(GameObject obj)
	{
		for (int i = 0; i < tweens.Count; i++)
		{
			if (tweens[i].target == obj)
			{
				tweens[i].StopAllCoroutines();
				UnityEngine.Object.Destroy(tweens[i]);
				tweens.RemoveAt(i);
				i--;
			}
		}
	}
}
