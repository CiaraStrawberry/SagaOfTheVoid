using UnityEngine;

public class WaypointRider : MonoBehaviour
{
	private int _targetWaypoint;

	private Transform _waypoints;

	public float movementSpeed = 3f;

	private void Start()
	{
		_waypoints = GameObject.Find("Waypoints").transform;
	}

	private void Update()
	{
	}

	private void FixedUpdate()
	{
		handleWalkWaypoints();
	}

	private void handleWalkWaypoints()
	{
		Transform child = _waypoints.GetChild(_targetWaypoint);
		Vector3 value = child.position - transform.position;
		Vector3 vector = Vector3.Normalize(value);
		float magnitude = value.magnitude;
		if ((double)magnitude < 0.1)
		{
			if (_targetWaypoint + 1 >= _waypoints.childCount)
			{
				Object.Destroy(gameObject);
				return;
			}
			_targetWaypoint++;
		}
		else
		{
			transform.position += vector * movementSpeed * Time.fixedDeltaTime;
		}
		Quaternion b = Quaternion.LookRotation(vector);
		transform.rotation = Quaternion.Lerp(transform.rotation, b, 0.5f);
	}
}
