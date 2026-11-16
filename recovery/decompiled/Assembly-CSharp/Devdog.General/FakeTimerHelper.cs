using System;

namespace Devdog.General;

public class FakeTimerHelper : ITimerHelper, IDisposable
{
	public virtual void StopAllTimers()
	{
	}

	public virtual void StopTimer(int id)
	{
	}

	public virtual int StartTimer(float time, Action callback)
	{
		return StartTimer(time, null, callback);
	}

	public virtual int StartTimer(float time, Action callbackContinous, Action callback)
	{
		callbackContinous?.Invoke();
		if (time <= 0f)
		{
			callback?.Invoke();
		}
		return 0;
	}

	public void Dispose()
	{
	}
}
