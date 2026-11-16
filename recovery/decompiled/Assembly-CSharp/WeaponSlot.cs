using EasyEditor;
using UnityEngine;

public class WeaponSlot : MonoBehaviour
{
	[Inspector(displayHeader = true, group = "Weapon Settings", groupDescription = "Settings for the engine", order = 1)]
	[SerializeField]
	private eHullType _MaximumHull;

	[Inspector(group = "Weapon Settings", order = 1)]
	[SerializeField]
	private _Weapon _WeaponToPlace;

	[Inspector(group = "Weapon Settings", order = 1)]
	[SerializeField]
	private GameObject[] _Placements;

	public eHullType MaximumHull
	{
		get
		{
			return _MaximumHull;
		}
		set
		{
			_MaximumHull = value;
		}
	}

	public _Weapon WeaponToPlace
	{
		get
		{
			return _WeaponToPlace;
		}
		set
		{
			_WeaponToPlace = value;
		}
	}

	public GameObject[] Placements
	{
		get
		{
			return _Placements;
		}
		set
		{
			_Placements = value;
		}
	}

	private void Start()
	{
	}

	private void Update()
	{
	}
}
