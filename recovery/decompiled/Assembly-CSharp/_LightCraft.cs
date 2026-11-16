using EasyEditor;
using UnityEngine;

public class _LightCraft : _Ship
{
	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private GameObject _CraftPrefab;

	[Inspector(group = "Engine Settings")]
	[SerializeField]
	private int _ShipsInSquad;

	public GameObject CraftPrefab
	{
		get
		{
			return _CraftPrefab;
		}
		set
		{
			_CraftPrefab = value;
		}
	}

	public int ShipsInSquad
	{
		get
		{
			return _ShipsInSquad;
		}
		set
		{
			_ShipsInSquad = value;
		}
	}

	public _LightCraft()
	{
		HullType = eHullType.Light;
	}
}
