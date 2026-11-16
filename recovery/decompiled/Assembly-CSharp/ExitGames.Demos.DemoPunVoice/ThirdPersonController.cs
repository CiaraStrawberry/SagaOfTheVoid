using UnityEngine;

namespace ExitGames.Demos.DemoPunVoice;

public class ThirdPersonController : BaseController
{
	[SerializeField]
	private float movingTurnSpeed = 360f;

	protected override void Move(float h, float v)
	{
		rigidBody.velocity = v * speed * transform.forward;
		transform.rotation *= Quaternion.AngleAxis(movingTurnSpeed * h * Time.deltaTime, Vector3.up);
	}
}
