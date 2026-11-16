using TrueSync;
using UnityEngine;

public class PlayerBehavior : TrueSyncBehaviour
{
	private const byte INPUT_KEY_MOVE = 0;

	private const byte INPUT_KEY_JUMP = 1;

	public int speed;

	public RuntimeAnimatorController[] animatorControllers;

	public Sprite[] sprites;

	private Animator animator;

	private SpriteRenderer spriteRenderer;

	public override void OnSyncedStart()
	{
		animator = GetComponent<Animator>();
		spriteRenderer = GetComponent<SpriteRenderer>();
		if (owner.Id == 1)
		{
			animator.runtimeAnimatorController = animatorControllers[0];
			spriteRenderer.sprite = sprites[0];
			tsRigidBody2D.position = new TSVector2(1, 0);
		}
		else
		{
			animator.runtimeAnimatorController = animatorControllers[1];
			spriteRenderer.sprite = sprites[1];
			spriteRenderer.flipX = true;
			TSVector2 center = new TSVector2(-0.63f, -0.87f);
			tsCollider2D.Center = center;
			tsRigidBody2D.position = new TSVector2(-1 - center.x, 0);
		}
		animator.StopPlayback();
	}

	private void UpdateAnimations()
	{
		animator.Update(Time.deltaTime);
		if (tsRigidBody2D.velocity.LengthSquared() > FP.Half)
		{
			animator.SetBool("running", value: true);
		}
		else
		{
			animator.SetBool("running", value: false);
		}
	}

	public override void OnSyncedInput()
	{
		int value = (int)(Input.GetAxis("Horizontal") * 100f);
		byte value2 = (byte)(Input.GetButton("Jump") ? 1 : 0);
		TrueSyncInput.SetInt(0, value);
		TrueSyncInput.SetByte(1, value2);
	}

	public override void OnSyncedUpdate()
	{
		UpdateAnimations();
		TSVector2 velocity = tsRigidBody2D.velocity;
		velocity.x = (FP)(TrueSyncInput.GetInt(0) * speed) / (FP)100;
		if (TrueSyncInput.GetByte(1) > 0)
		{
			velocity.y = 10;
		}
		tsRigidBody2D.velocity = velocity;
	}
}
