using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice;

public class FirstPersonController : BaseController
{
	[SerializeField]
	private MouseLookHelper mouseLook = new MouseLookHelper();

	private float oldYRotation;

	private Quaternion velRotation;

	public Vector3 Velocity => rigidBody.velocity;

	protected override void SetCamera()
	{
		base.SetCamera();
		mouseLook.Init(transform, camTrans);
	}

	protected override void Move(float h, float v)
	{
		Vector3 velocity = camTrans.forward * v + camTrans.right * h;
		velocity.x *= speed;
		velocity.z *= speed;
		velocity.y = 0f;
		rigidBody.velocity = velocity;
	}

	private void Update()
	{
		RotateView();
	}

	private void RotateView()
	{
		oldYRotation = transform.eulerAngles.y;
		mouseLook.LookRotation(transform, camTrans);
		velRotation = Quaternion.AngleAxis(transform.eulerAngles.y - oldYRotation, Vector3.up);
		rigidBody.velocity = velRotation * rigidBody.velocity;
	}
}
