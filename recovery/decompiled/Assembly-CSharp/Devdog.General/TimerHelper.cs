using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Devdog.General;

public class TimerHelper : MonoBehaviour, ITimerHelper, IDisposable
{
	[NonSerialized]
	private Dictionary<int, Coroutine> _lookups = new Dictionary<int, Coroutine>();

	[NonSerialized]
	private int _IDCounter;

	public virtual void StopAllTimers()
	{
		StopAllCoroutines();
		_lookups.Clear();
	}

	public virtual void StopTimer(int id)
	{
		if (_lookups.ContainsKey(id))
		{
			StopCoroutine(_lookups[id]);
			_lookups.Remove(id);
		}
	}

	public int StartTimer(float time, Action callbackWhenTimeIsOver)
	{
		return StartTimer(time, null, callbackWhenTimeIsOver);
	}

	public virtual int StartTimer(float time, Action callbackContinous, Action callbackWhenTimeIsOver)
	{
		_IDCounter++;
		_lookups[_IDCounter] = StartCoroutine(_StartTimer(_IDCounter, time, callbackContinous, callbackWhenTimeIsOver));
		return _IDCounter;
	}

	protected virtual IEnumerator _StartTimer(int timerID, float time, Action callbackContinous, Action callbackWhenTimeIsOver)
	{
		float timer = 0f;
		while (timer < time)
		{
			timer += Time.deltaTime;
			callbackContinous?.Invoke();
			yield return null;
		}
		_lookups.Remove(timerID);
		callbackWhenTimeIsOver?.Invoke();
	}

	public void Dispose()
	{
		StopAllTimers();
		UnityEngine.Object.Destroy(gameObject);
	}
}
