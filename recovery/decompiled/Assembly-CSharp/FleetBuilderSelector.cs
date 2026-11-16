using UnityEngine;

public class FleetBuilderSelector : MonoBehaviour
{
	public bool right;

	public bool left;

	private void Start()
	{
	}

	private void Update()
	{
	}

	private void OnTriggerEnter(Collider col)
	{
		if (col.name == "Controller (right)" || col.name == "Controller (left)")
		{
			if (right)
			{
				transform.parent.parent.gameObject.GetComponent<FleetNumControl>().addone();
			}
			if (left)
			{
				transform.parent.parent.gameObject.GetComponent<FleetNumControl>().backone();
			}
		}
	}

	public void disableall()
	{
		right = false;
		left = false;
	}
}
