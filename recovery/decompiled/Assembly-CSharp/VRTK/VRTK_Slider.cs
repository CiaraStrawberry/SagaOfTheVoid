using UnityEngine;
using VRTK.GrabAttachMechanics;
using VRTK.SecondaryControllerGrabActions;

namespace VRTK;

[AddComponentMenu("VRTK/Scripts/Controls/3D/VRTK_Slider")]
public class VRTK_Slider : VRTK_Control
{
	[Tooltip("An optional game object to which the wheel will be connected. If the game object moves the wheel will follow along.")]
	public GameObject connectedTo;

	[Tooltip("The axis on which the slider should move. All other axis will be frozen.")]
	public Direction direction;

	[Tooltip("The collider to specify the minimum limit of the slider.")]
	public Collider minimumLimit;

	[Tooltip("The collider to specify the maximum limit of the slider.")]
	public Collider maximumLimit;

	[Tooltip("The minimum value of the slider.")]
	public float minimumValue;

	[Tooltip("The maximum value of the slider.")]
	public float maximumValue = 100f;

	[Tooltip("The increments in which slider values can change.")]
	public float stepSize = 0.1f;

	[Tooltip("If this is checked then when the slider is released, it will snap to the nearest value position.")]
	public bool snapToStep;

	[Tooltip("The amount of friction the slider will have when it is released.")]
	public float releasedFriction = 50f;

	protected Direction finalDirection;

	protected Rigidbody sliderRigidbody;

	protected ConfigurableJoint sliderJoint;

	protected bool sliderJointCreated;

	protected Vector3 minimumLimitDiff;

	protected Vector3 maximumLimitDiff;

	protected Vector3 snapPosition;

	protected override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		if (enabled && setupSuccessful)
		{
			Gizmos.DrawLine(transform.position, minimumLimit.transform.position);
			Gizmos.DrawLine(transform.position, maximumLimit.transform.position);
		}
	}

	protected override void InitRequiredComponents()
	{
		DetectSetup();
		InitRigidbody();
		InitInteractableObject();
		InitJoint();
	}

	protected override bool DetectSetup()
	{
		if (sliderJointCreated && (bool)connectedTo)
		{
			sliderJoint.connectedBody = connectedTo.GetComponent<Rigidbody>();
		}
		finalDirection = direction;
		if (direction == Direction.autodetect)
		{
			bool flag = Physics.Raycast(transform.position, transform.right, out var hitInfo);
			bool flag2 = Physics.Raycast(transform.position, transform.up, out var hitInfo2);
			bool flag3 = Physics.Raycast(transform.position, transform.forward, out var hitInfo3);
			Vector3 vector = transform.localScale / 2f;
			if (flag && hitInfo.collider.gameObject.Equals(minimumLimit.gameObject))
			{
				finalDirection = Direction.x;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.right, minimumLimit.transform.localScale.x, vector.x, addition: false);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.right, maximumLimit.transform.localScale.x, vector.x, addition: true);
			}
			if (flag && hitInfo.collider.gameObject.Equals(maximumLimit.gameObject))
			{
				finalDirection = Direction.x;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.right, minimumLimit.transform.localScale.x, vector.x, addition: true);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.right, maximumLimit.transform.localScale.x, vector.x, addition: false);
			}
			if (flag2 && hitInfo2.collider.gameObject.Equals(minimumLimit.gameObject))
			{
				finalDirection = Direction.y;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.up, minimumLimit.transform.localScale.y, vector.y, addition: false);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.up, maximumLimit.transform.localScale.y, vector.y, addition: true);
			}
			if (flag2 && hitInfo2.collider.gameObject.Equals(maximumLimit.gameObject))
			{
				finalDirection = Direction.y;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.up, minimumLimit.transform.localScale.y, vector.y, addition: true);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.up, maximumLimit.transform.localScale.y, vector.y, addition: false);
			}
			if (flag3 && hitInfo3.collider.gameObject.Equals(minimumLimit.gameObject))
			{
				finalDirection = Direction.z;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.forward, minimumLimit.transform.localScale.y, vector.y, addition: false);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.forward, maximumLimit.transform.localScale.y, vector.y, addition: true);
			}
			if (flag3 && hitInfo3.collider.gameObject.Equals(maximumLimit.gameObject))
			{
				finalDirection = Direction.z;
				minimumLimitDiff = CalculateDiff(minimumLimit.transform.localPosition, Vector3.forward, minimumLimit.transform.localScale.z, vector.z, addition: true);
				maximumLimitDiff = CalculateDiff(maximumLimit.transform.localPosition, Vector3.forward, maximumLimit.transform.localScale.z, vector.z, addition: false);
			}
		}
		return true;
	}

	protected override ControlValueRange RegisterValueRange()
	{
		return new ControlValueRange
		{
			controlMin = minimumValue,
			controlMax = maximumValue
		};
	}

	protected override void HandleUpdate()
	{
		CalculateValue();
		if (snapToStep)
		{
			SnapToValue();
		}
	}

	protected virtual Vector3 CalculateDiff(Vector3 initialPosition, Vector3 givenDirection, float scaleValue, float diffMultiplier, bool addition)
	{
		Vector3 vector = givenDirection * diffMultiplier;
		Vector3 vector2 = givenDirection * (scaleValue / 2f);
		vector2 = ((!addition) ? (initialPosition - vector2) : (initialPosition + vector2));
		Vector3 vector3 = initialPosition - vector2;
		if (addition)
		{
			return vector3 - vector;
		}
		return vector3 + vector;
	}

	protected virtual void InitRigidbody()
	{
		sliderRigidbody = GetComponent<Rigidbody>();
		if (sliderRigidbody == null)
		{
			sliderRigidbody = gameObject.AddComponent<Rigidbody>();
		}
		sliderRigidbody.isKinematic = false;
		sliderRigidbody.useGravity = false;
		sliderRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		sliderRigidbody.drag = releasedFriction;
		if ((bool)connectedTo)
		{
			Rigidbody component = connectedTo.GetComponent<Rigidbody>();
			if (component == null)
			{
				component = connectedTo.AddComponent<Rigidbody>();
				component.useGravity = false;
				component.isKinematic = true;
			}
		}
	}

	protected virtual void InitInteractableObject()
	{
		VRTK_InteractableObject vRTK_InteractableObject = GetComponent<VRTK_InteractableObject>();
		if (vRTK_InteractableObject == null)
		{
			vRTK_InteractableObject = gameObject.AddComponent<VRTK_InteractableObject>();
		}
		vRTK_InteractableObject.isGrabbable = true;
		vRTK_InteractableObject.grabAttachMechanicScript = gameObject.AddComponent<VRTK_TrackObjectGrabAttach>();
		vRTK_InteractableObject.secondaryGrabActionScript = gameObject.AddComponent<VRTK_SwapControllerGrabAction>();
		vRTK_InteractableObject.grabAttachMechanicScript.precisionGrab = true;
		vRTK_InteractableObject.stayGrabbedOnTeleport = false;
	}

	protected virtual void InitJoint()
	{
		sliderJoint = GetComponent<ConfigurableJoint>();
		if (sliderJoint == null)
		{
			sliderJoint = gameObject.AddComponent<ConfigurableJoint>();
		}
		sliderJoint.xMotion = ((finalDirection == Direction.x) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
		sliderJoint.yMotion = ((finalDirection == Direction.y) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
		sliderJoint.zMotion = ((finalDirection == Direction.z) ? ConfigurableJointMotion.Free : ConfigurableJointMotion.Locked);
		sliderJoint.angularXMotion = ConfigurableJointMotion.Locked;
		sliderJoint.angularYMotion = ConfigurableJointMotion.Locked;
		sliderJoint.angularZMotion = ConfigurableJointMotion.Locked;
		ToggleSpring(state: false);
		sliderJointCreated = true;
	}

	protected virtual void CalculateValue()
	{
		Vector3 vector = minimumLimit.transform.localPosition - minimumLimitDiff;
		Vector3 vector2 = maximumLimit.transform.localPosition - maximumLimitDiff;
		float num = Vector3.Distance(vector, vector2);
		float num2 = Vector3.Distance(vector, transform.localPosition);
		float num3 = Mathf.Round((minimumValue + Mathf.Clamp01(num2 / num) * (maximumValue - minimumValue)) / stepSize) * stepSize;
		float num4 = num3 - minimumValue;
		float num5 = maximumValue - minimumValue;
		float num6 = num4 / num5;
		snapPosition = vector + (vector2 - vector) * num6;
		value = num3;
	}

	protected virtual void ToggleSpring(bool state)
	{
		JointDrive jointDrive = new JointDrive
		{
			positionSpring = ((!state) ? 0f : 10000f),
			positionDamper = ((!state) ? 0f : 10f),
			maximumForce = ((!state) ? 0f : 100f)
		};
		sliderJoint.xDrive = jointDrive;
		sliderJoint.yDrive = jointDrive;
		sliderJoint.zDrive = jointDrive;
	}

	protected virtual void SnapToValue()
	{
		ToggleSpring(state: true);
		sliderJoint.targetPosition = snapPosition * -1f;
		sliderJoint.targetVelocity = Vector3.zero;
	}
}
