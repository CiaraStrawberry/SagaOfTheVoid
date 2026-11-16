using System;
using System.Collections;
using Devdog.General;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class FlockGroup : MonoBehaviour
{
	[Required]
	[SerializeField]
	public Transform goals;

	[Required]
	[SerializeField]
	private Transform _units;

	[NonSerialized]
	public FlockUnit[] units = new FlockUnit[0];

	public Transform aimGoal { get; private set; }

	protected void Awake()
	{
		units = _units.GetComponentsInChildren<FlockUnit>(includeInactive: true);
		FlockUnit[] array = units;
		foreach (FlockUnit flockUnit in array)
		{
			flockUnit.parent = this;
		}
		StartCoroutine(RandomUpdate());
	}

	private IEnumerator RandomUpdate()
	{
		while (true)
		{
			PickRandomAimPosition();
			yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 2f));
		}
	}

	private void PickRandomAimPosition()
	{
		aimGoal = goals.GetChild(UnityEngine.Random.Range(0, goals.childCount));
	}
}
