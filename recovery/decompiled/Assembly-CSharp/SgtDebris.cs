using System;
using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Debris")]
public class SgtDebris : MonoBehaviour
{
	public enum StateType
	{
		Hide,
		Fade,
		Show
	}

	public Action OnSpawn;

	public Action OnDespawn;

	[Tooltip("Can this particle be pooled?")]
	public bool Pool;

	[Tooltip("The current state of the scaling")]
	public StateType State;

	[Tooltip("The prefab this was instantiated from")]
	public SgtDebris Prefab;

	[Tooltip("This gets automatically copied when spawning debris")]
	public Vector3 Scale;

	[Tooltip("The cell this debris was spawned in")]
	public SgtVector3L Cell;

	public float Show;
}
