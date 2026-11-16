using UnityEngine;

public class WaypointManager : MonoBehaviour
{
	public Waypoint[] Waypoints;

	private void Start()
	{
		Waypoints = (Waypoint[])Object.FindObjectsOfType(typeof(Waypoint));
	}

	private void Update()
	{
	}
}
