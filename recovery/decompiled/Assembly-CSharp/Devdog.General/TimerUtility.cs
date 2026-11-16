using UnityEngine;

namespace Devdog.General;

public static class TimerUtility
{
	private static ITimerHelper _root;

	private static GameObject _obj;

	static TimerUtility()
	{
	}

	public static ITimerHelper GetTimer()
	{
		if (_root == null || _obj == null)
		{
			_obj = new GameObject("_QuestTimer");
			_root = _obj.AddComponent<TimerHelper>();
		}
		return _root;
	}
}
