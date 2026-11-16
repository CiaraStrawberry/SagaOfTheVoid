using System;
using System.Collections.Generic;
using Devdog.General.ThirdParty.UniLinq;
using UnityEngine;

namespace Devdog.General;

public class InputManager : ManagerBase<InputManager>
{
	protected static List<GameObject> limitUIInputTo = new List<GameObject>(8);

	protected static List<GameObject> limitPlayerInputTo = new List<GameObject>(8);

	public static event Action<bool> OnUIInputLimitChanged;

	public static event Action<bool> OnPlayerInputChanged;

	private static void NotifyUIInputChanged(bool b)
	{
		if (OnUIInputLimitChanged != null)
		{
			OnUIInputLimitChanged(b);
		}
	}

	public static void LimitUIInputTo(GameObject obj)
	{
		limitUIInputTo.Add(obj);
		NotifyUIInputChanged(b: true);
	}

	public static void RemoveUILimitInput(GameObject obj)
	{
		GameObject obj2 = limitUIInputTo.LastOrDefault();
		limitUIInputTo.Remove(obj);
		if (limitUIInputTo.Count == 0)
		{
			NotifyUIInputChanged(b: false);
		}
		else if (IsUIInputLimitedTo(obj2))
		{
			NotifyUIInputChanged(b: true);
		}
	}

	public static void ClearUIInputLimits(GameObject obj)
	{
		limitUIInputTo.Clear();
		NotifyUIInputChanged(b: false);
	}

	public static GameObject GetLimitedUIInputObject()
	{
		if (limitUIInputTo.Count > 0)
		{
			return limitUIInputTo[limitUIInputTo.Count - 1];
		}
		return null;
	}

	public static bool IsUIInputLimitedTo(GameObject obj)
	{
		return GetLimitedUIInputObject() == obj;
	}

	public static bool CanReceiveUIInput(GameObject obj)
	{
		return CanReceiveInput(obj, GetLimitedUIInputObject());
	}

	private static void NotifyPlayerInputChanged(bool b)
	{
		if (OnPlayerInputChanged != null)
		{
			OnPlayerInputChanged(b);
		}
	}

	public static void LimitPlayerInputTo(GameObject obj)
	{
		limitPlayerInputTo.Add(obj);
		NotifyPlayerInputChanged(b: true);
	}

	public static void RemovePlayerLimitInput(GameObject obj)
	{
		GameObject obj2 = limitPlayerInputTo.LastOrDefault();
		limitPlayerInputTo.Remove(obj);
		if (limitPlayerInputTo.Count == 0)
		{
			NotifyPlayerInputChanged(b: false);
		}
		else if (IsPlayerInputLimitedTo(obj2))
		{
			NotifyPlayerInputChanged(b: true);
		}
	}

	public static void ClearPlayerInputLimits(GameObject obj)
	{
		limitPlayerInputTo.Clear();
		NotifyPlayerInputChanged(b: false);
	}

	public static GameObject GetLimitedPlayerInputObject()
	{
		if (limitPlayerInputTo.Count > 0)
		{
			return limitPlayerInputTo[limitPlayerInputTo.Count - 1];
		}
		return null;
	}

	public static bool IsPlayerInputLimitedTo(GameObject obj)
	{
		return GetLimitedPlayerInputObject() == obj;
	}

	public static bool CanReceivePlayerInput(GameObject obj)
	{
		return CanReceiveInput(obj, GetLimitedPlayerInputObject());
	}

	private static bool CanReceiveInput(GameObject obj, GameObject limitObject)
	{
		if (obj == null)
		{
			return false;
		}
		if (!obj.activeInHierarchy)
		{
			return false;
		}
		if (limitObject == null)
		{
			return true;
		}
		if (obj == limitObject || obj.transform.IsChildOf(limitObject.transform))
		{
			return true;
		}
		return false;
	}
}
