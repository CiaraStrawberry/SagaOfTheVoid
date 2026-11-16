using UnityEngine;

public class ViewDrag : MonoBehaviour
{
	private Vector3 hit_position = Vector3.zero;

	private Vector3 current_position = Vector3.zero;

	private Vector3 camera_position = Vector3.zero;

	public bool invert;

	public bool yBecomesZ = true;

	public float speed = 2f;

	private void Start()
	{
	}

	private void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			hit_position = Input.mousePosition;
			camera_position = transform.position;
		}
		if (Input.GetMouseButton(0))
		{
			current_position = Input.mousePosition;
			LeftMouseDrag();
		}
	}

	private void LeftMouseDrag()
	{
		current_position.z = (hit_position.z = camera_position.y);
	}
}
