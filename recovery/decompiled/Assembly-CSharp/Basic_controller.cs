using UnityEngine;

public class Basic_controller : MonoBehaviour
{
	private CharacterController controller;

	public float speed = 6f;

	public float turnSpeed = 5f;

	public float gravity = 20f;

	public float runSpeed = 1.7f;

	private Vector3 moveDirection = Vector3.zero;

	private void Start()
	{
		controller = GetComponent<CharacterController>();
	}

	private void Update()
	{
		if (controller.isGrounded)
		{
			moveDirection = transform.forward * Input.GetAxis("Vertical") * speed * runSpeed;
		}
		float axis = Input.GetAxis("Horizontal");
		transform.Rotate(0f, axis * turnSpeed * Time.deltaTime, 0f);
		controller.Move(moveDirection * Time.deltaTime);
		moveDirection.y -= gravity * Time.deltaTime;
	}
}
