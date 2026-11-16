using UnityEngine;

namespace Apex.Steering.Components;

[AddComponentMenu("Apex/Unit/Navigation/Humanoid Speed", 1027)]
[ApexComponent("Steering")]
public class HumanoidSpeedComponent : SpeedComponent
{
	public float crawlSpeed = 1f;

	public float crouchedSpeed = 1.5f;

	public float walkSpeed = 3f;

	public float runSpeed = 6f;

	public float strafeMaxSpeed = 4f;

	public float backPedalMaxSpeed = 3f;

	private Transform _transform;

	public override float maximumSpeed
	{
		get
		{
			return runSpeed;
		}
		set
		{
			runSpeed = value;
		}
	}

	private void Awake()
	{
		this.WarnIfMultipleInstances();
		_transform = transform;
		Walk();
	}

	public void Crawl()
	{
		_preferredSpeed = crawlSpeed;
	}

	public void Crouch()
	{
		_preferredSpeed = crouchedSpeed;
	}

	public void Walk()
	{
		_preferredSpeed = walkSpeed;
	}

	public void Run()
	{
		_preferredSpeed = runSpeed;
	}

	public override float GetPreferredSpeed(Vector3 currentMovementDirection)
	{
		float num = Vector3.Dot(currentMovementDirection, _transform.forward);
		if (num < 0.5f)
		{
			if (_preferredSpeed >= strafeMaxSpeed && num > -0.5f)
			{
				return strafeMaxSpeed;
			}
			if (_preferredSpeed > backPedalMaxSpeed)
			{
				return backPedalMaxSpeed;
			}
		}
		return _preferredSpeed;
	}

	public override void CloneFrom(IDefineSpeed speedComponent)
	{
		base.CloneFrom(speedComponent);
		HumanoidSpeedComponent humanoidSpeedComponent = speedComponent as HumanoidSpeedComponent;
		if (humanoidSpeedComponent != null)
		{
			crawlSpeed = humanoidSpeedComponent.crawlSpeed;
			crouchedSpeed = humanoidSpeedComponent.crouchedSpeed;
			walkSpeed = humanoidSpeedComponent.walkSpeed;
			runSpeed = humanoidSpeedComponent.runSpeed;
			strafeMaxSpeed = humanoidSpeedComponent.strafeMaxSpeed;
			backPedalMaxSpeed = humanoidSpeedComponent.backPedalMaxSpeed;
		}
	}
}
