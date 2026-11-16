using System;
using UnityEngine;

namespace Apex.Steering.Props;

[AddComponentMenu("Apex/Legacy/Navigation/Patrol Route (OLD)", 1022)]
public class PatrolRoute : MonoBehaviour
{
	public bool drawGizmosAlways;

	public Color gizmoColor = new Color(0f, 150f / 255f, 211f / 255f);

	private PatrolPoint[] _patrolPoints;

	public PatrolPoint[] patrolPoints => _patrolPoints;

	private void Awake()
	{
		_patrolPoints = GetComponentsInChildren<PatrolPoint>();
		Array.Sort(_patrolPoints, (PatrolPoint a, PatrolPoint b) =>
		{
			int num = a.orderIndex.CompareTo(b.orderIndex);
			return (num == 0) ? a.gameObject.name.CompareTo(b.gameObject.name) : num;
		});
	}
}
