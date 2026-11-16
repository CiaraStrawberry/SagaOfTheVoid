using Apex.PathFinding;
using Apex.Services;
using Apex.Units;
using Apex.WorldGeometry;
using UnityEngine;

namespace Apex;

public static class UnityExtensions
{
	public static IPositioned AsPositioned(this Vector3 pos)
	{
		return new Position(pos);
	}

	public static Vector3 AdjustAxis(this Vector3 target, Vector3 source, Axis targetAxis)
	{
		switch (targetAxis)
		{
		case Axis.Y:
			target.y = source.y;
			break;
		case Axis.X:
			target.x = source.x;
			break;
		case Axis.Z:
			target.z = source.z;
			break;
		}
		return target;
	}

	public static Vector3 AdjustAxis(this Vector3 target, float value, Axis targetAxis)
	{
		switch (targetAxis)
		{
		case Axis.Y:
			target.y = value;
			break;
		case Axis.X:
			target.x = value;
			break;
		case Axis.Z:
			target.z = value;
			break;
		}
		return target;
	}

	public static IUnitFacade GetUnitFacade(this Component c, bool createIfMissing = true)
	{
		return GameServices.gameStateManager.GetUnitFacade(c.gameObject, createIfMissing);
	}

	public static IUnitFacade GetUnitFacade(this GameObject go, bool createIfMissing = true)
	{
		return GameServices.gameStateManager.GetUnitFacade(go, createIfMissing);
	}

	public static IUnitFacade GetUnitFacade(this IGameObjectComponent goc, bool createIfMissing = true)
	{
		return GameServices.gameStateManager.GetUnitFacade(goc.gameObject, createIfMissing);
	}

	public static T GetUnitFacade<T>(this Component c, bool createIfMissing = true) where T : class, IUnitFacade, new()
	{
		return GameServices.gameStateManager.GetUnitFacade<T>(c.gameObject, createIfMissing);
	}

	public static T GetUnitFacade<T>(this GameObject go, bool createIfMissing = true) where T : class, IUnitFacade, new()
	{
		return GameServices.gameStateManager.GetUnitFacade<T>(go, createIfMissing);
	}

	public static T GetUnitFacade<T>(this IGameObjectComponent goc, bool createIfMissing = true) where T : class, IUnitFacade, new()
	{
		return GameServices.gameStateManager.GetUnitFacade<T>(goc.gameObject, createIfMissing);
	}
}
