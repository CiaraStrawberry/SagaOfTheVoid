using System.Collections.Generic;
using UnityEngine;

namespace VRTK;

[RequireComponent(typeof(VRTK_BodyPhysics))]
[AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_MoveInPlace")]
public class VRTK_MoveInPlace : MonoBehaviour
{
	public enum ControlOptions
	{
		HeadsetAndControllers,
		ControllersOnly,
		HeadsetOnly
	}

	public enum DirectionalMethod
	{
		Gaze,
		ControllerRotation,
		DumbDecoupling,
		SmartDecoupling,
		EngageControllerRotationOnly,
		LeftControllerRotationOnly,
		RightControllerRotationOnly
	}

	[Header("Control Settings")]
	[Tooltip("If this is checked then the left controller touchpad will be enabled to move the play area.")]
	public bool leftController = true;

	[Tooltip("If this is checked then the right controller touchpad will be enabled to move the play area.")]
	public bool rightController = true;

	[Tooltip("Select which button to hold to engage Move In Place.")]
	public VRTK_ControllerEvents.ButtonAlias engageButton = VRTK_ControllerEvents.ButtonAlias.TouchpadPress;

	[Tooltip("Select which trackables are used to determine movement.")]
	public ControlOptions controlOptions;

	[Tooltip("How the user's movement direction will be determined.  The Gaze method tends to lead to the least motion sickness.  Smart decoupling is still a Work In Progress.")]
	public DirectionalMethod directionMethod;

	[Header("Speed Settings")]
	[Tooltip("Lower to decrease speed, raise to increase.")]
	public float speedScale = 1f;

	[Tooltip("The max speed the user can move in game units. (If 0 or less, max speed is uncapped)")]
	public float maxSpeed = 4f;

	[Tooltip("The speed in which the play area slows down to a complete stop when the user is no longer pressing the engage button. This deceleration effect can ease any motion sickness that may be suffered.")]
	public float deceleration = 0.1f;

	[Tooltip("The speed in which the play area slows down to a complete stop when the user is falling.")]
	public float fallingDeceleration = 0.01f;

	[Header("Advanced Settings")]
	[Tooltip("The degree threshold that all tracked objects (controllers, headset) must be within to change direction when using the Smart Decoupling Direction Method.")]
	public float smartDecoupleThreshold = 30f;

	[Tooltip("The maximum amount of movement required to register in the virtual world.  Decreasing this will increase acceleration, and vice versa.")]
	public float sensitivity = 0.02f;

	protected Transform playArea;

	protected GameObject controllerLeftHand;

	protected GameObject controllerRightHand;

	protected VRTK_ControllerReference engagedController;

	protected Transform headset;

	protected bool leftSubscribed;

	protected bool rightSubscribed;

	protected bool previousLeftControllerState;

	protected bool previousRightControllerState;

	protected VRTK_ControllerEvents.ButtonAlias previousEngageButton;

	protected VRTK_BodyPhysics bodyPhysics;

	protected bool currentlyFalling;

	protected int averagePeriod;

	protected List<Transform> trackedObjects;

	protected Dictionary<Transform, List<float>> movementList;

	protected Dictionary<Transform, float> previousYPositions;

	protected Vector3 initalGaze;

	protected float currentSpeed;

	protected Vector3 direction;

	protected Vector3 previousDirection;

	protected bool active;

	public virtual void SetControlOptions(ControlOptions givenControlOptions)
	{
		controlOptions = givenControlOptions;
		trackedObjects.Clear();
		if ((bool)controllerLeftHand && (bool)controllerRightHand && (controlOptions.Equals(ControlOptions.HeadsetAndControllers) || controlOptions.Equals(ControlOptions.ControllersOnly)))
		{
			trackedObjects.Add(VRTK_DeviceFinder.GetActualController(controllerLeftHand).transform);
			trackedObjects.Add(VRTK_DeviceFinder.GetActualController(controllerRightHand).transform);
		}
		if ((bool)headset && (controlOptions.Equals(ControlOptions.HeadsetAndControllers) || controlOptions.Equals(ControlOptions.HeadsetOnly)))
		{
			trackedObjects.Add(headset.transform);
		}
	}

	public virtual Vector3 GetMovementDirection()
	{
		return direction;
	}

	public virtual float GetSpeed()
	{
		return currentSpeed;
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		trackedObjects = new List<Transform>();
		movementList = new Dictionary<Transform, List<float>>();
		previousYPositions = new Dictionary<Transform, float>();
		initalGaze = Vector3.zero;
		direction = Vector3.zero;
		previousDirection = Vector3.zero;
		averagePeriod = 60;
		currentSpeed = 0f;
		active = false;
		previousEngageButton = engageButton;
		bodyPhysics = GetComponent<VRTK_BodyPhysics>();
		controllerLeftHand = VRTK_DeviceFinder.GetControllerLeftHand();
		controllerRightHand = VRTK_DeviceFinder.GetControllerRightHand();
		SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed);
		SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed);
		headset = VRTK_DeviceFinder.HeadsetTransform();
		SetControlOptions(controlOptions);
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		foreach (Transform trackedObject in trackedObjects)
		{
			movementList.Add(trackedObject, new List<float>());
			previousYPositions.Add(trackedObject, trackedObject.transform.localPosition.y);
		}
		if (!playArea)
		{
			VRTK_Logger.Error(VRTK_Logger.GetCommonMessage(VRTK_Logger.CommonMessageKeys.SDK_OBJECT_NOT_FOUND, "PlayArea", "Boundaries SDK"));
		}
	}

	protected virtual void OnDisable()
	{
		bodyPhysics = null;
		SetControllerListeners(controllerLeftHand, leftController, ref leftSubscribed, forceDisabled: true);
		SetControllerListeners(controllerRightHand, rightController, ref rightSubscribed, forceDisabled: true);
		controllerLeftHand = null;
		controllerRightHand = null;
		headset = null;
		playArea = null;
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		CheckControllerState(controllerLeftHand, leftController, ref leftSubscribed, ref previousLeftControllerState);
		CheckControllerState(controllerRightHand, rightController, ref leftSubscribed, ref previousRightControllerState);
		previousEngageButton = engageButton;
	}

	protected virtual void FixedUpdate()
	{
		HandleFalling();
		if (MovementActivated() && !currentlyFalling)
		{
			float num = Mathf.Clamp(speedScale * 350f * (CalculateListAverage() / (float)trackedObjects.Count), 0f, maxSpeed);
			previousDirection = direction;
			direction = SetDirection();
			currentSpeed = num;
		}
		else if (currentSpeed > 0f)
		{
			currentSpeed -= ((!currentlyFalling) ? deceleration : fallingDeceleration);
		}
		else
		{
			currentSpeed = 0f;
			direction = Vector3.zero;
			previousDirection = Vector3.zero;
		}
		SetDeltaTransformData();
		MovePlayArea(direction, currentSpeed);
	}

	protected virtual bool MovementActivated()
	{
		return active || engageButton == VRTK_ControllerEvents.ButtonAlias.Undefined;
	}

	protected virtual void CheckControllerState(GameObject controller, bool controllerState, ref bool subscribedState, ref bool previousState)
	{
		if (controllerState != previousState || engageButton != previousEngageButton)
		{
			SetControllerListeners(controller, controllerState, ref subscribedState);
		}
		previousState = controllerState;
	}

	protected virtual float CalculateListAverage()
	{
		float num = 0f;
		foreach (Transform trackedObject in trackedObjects)
		{
			float num2 = Mathf.Abs(previousYPositions[trackedObject] - trackedObject.transform.localPosition.y);
			List<float> list = movementList[trackedObject];
			if (num2 > sensitivity)
			{
				list.Add(sensitivity);
			}
			else
			{
				list.Add(num2);
			}
			if (list.Count > averagePeriod)
			{
				list.RemoveAt(0);
			}
			float num3 = 0f;
			foreach (float item in list)
			{
				float num4 = item;
				num3 += num4;
			}
			float num5 = num3 / (float)averagePeriod;
			num += num5;
		}
		return num;
	}

	protected virtual Vector3 SetDirection()
	{
		Vector3 result = Vector3.zero;
		if (directionMethod == DirectionalMethod.SmartDecoupling || directionMethod == DirectionalMethod.DumbDecoupling)
		{
			if (initalGaze.Equals(Vector3.zero))
			{
				initalGaze = new Vector3(headset.forward.x, 0f, headset.forward.z);
			}
			if (directionMethod == DirectionalMethod.SmartDecoupling)
			{
				bool flag = true;
				float num = headset.rotation.eulerAngles.y;
				if (num <= smartDecoupleThreshold)
				{
					num += 360f;
				}
				if (flag && Mathf.Abs(num - controllerLeftHand.transform.rotation.eulerAngles.y) <= smartDecoupleThreshold && Mathf.Abs(num - controllerRightHand.transform.rotation.eulerAngles.y) <= smartDecoupleThreshold)
				{
					initalGaze = new Vector3(headset.forward.x, 0f, headset.forward.z);
				}
			}
			result = initalGaze;
		}
		else if (directionMethod.Equals(DirectionalMethod.ControllerRotation))
		{
			Vector3 calculatedControllerDirection = DetermineAverageControllerRotation() * Vector3.forward;
			result = CalculateControllerRotationDirection(calculatedControllerDirection);
		}
		else if (directionMethod.Equals(DirectionalMethod.LeftControllerRotationOnly))
		{
			Vector3 calculatedControllerDirection2 = ((!(controllerLeftHand != null)) ? Quaternion.identity : controllerLeftHand.transform.rotation) * Vector3.forward;
			result = CalculateControllerRotationDirection(calculatedControllerDirection2);
		}
		else if (directionMethod.Equals(DirectionalMethod.RightControllerRotationOnly))
		{
			Vector3 calculatedControllerDirection3 = ((!(controllerRightHand != null)) ? Quaternion.identity : controllerRightHand.transform.rotation) * Vector3.forward;
			result = CalculateControllerRotationDirection(calculatedControllerDirection3);
		}
		else if (directionMethod.Equals(DirectionalMethod.EngageControllerRotationOnly))
		{
			Vector3 calculatedControllerDirection4 = ((engagedController == null) ? Quaternion.identity : engagedController.scriptAlias.transform.rotation) * Vector3.forward;
			result = CalculateControllerRotationDirection(calculatedControllerDirection4);
		}
		else if (directionMethod.Equals(DirectionalMethod.Gaze))
		{
			result = new Vector3(headset.forward.x, 0f, headset.forward.z);
		}
		return result;
	}

	protected virtual Vector3 CalculateControllerRotationDirection(Vector3 calculatedControllerDirection)
	{
		return (!(Vector3.Angle(previousDirection, calculatedControllerDirection) <= 90f)) ? previousDirection : calculatedControllerDirection;
	}

	protected virtual void SetDeltaTransformData()
	{
		foreach (Transform trackedObject in trackedObjects)
		{
			previousYPositions[trackedObject] = trackedObject.transform.localPosition.y;
		}
	}

	protected virtual void MovePlayArea(Vector3 moveDirection, float moveSpeed)
	{
		Vector3 vector = moveDirection * moveSpeed * Time.fixedDeltaTime;
		if ((bool)playArea)
		{
			playArea.position = new Vector3(vector.x + playArea.position.x, playArea.position.y, vector.z + playArea.position.z);
		}
	}

	protected virtual void HandleFalling()
	{
		if ((bool)bodyPhysics && bodyPhysics.IsFalling())
		{
			currentlyFalling = true;
		}
		if ((bool)bodyPhysics && !bodyPhysics.IsFalling() && currentlyFalling)
		{
			currentlyFalling = false;
			currentSpeed = 0f;
		}
	}

	protected virtual void EngageButtonPressed(object sender, ControllerInteractionEventArgs e)
	{
		engagedController = e.controllerReference;
		active = true;
	}

	protected virtual void EngageButtonReleased(object sender, ControllerInteractionEventArgs e)
	{
		foreach (Transform trackedObject in trackedObjects)
		{
			movementList[trackedObject].Clear();
		}
		initalGaze = Vector3.zero;
		active = false;
		engagedController = null;
	}

	protected virtual Quaternion DetermineAverageControllerRotation()
	{
		if (controllerLeftHand != null && controllerRightHand != null)
		{
			return AverageRotation(controllerLeftHand.transform.rotation, controllerRightHand.transform.rotation);
		}
		if (controllerLeftHand != null && controllerRightHand == null)
		{
			return controllerLeftHand.transform.rotation;
		}
		if (controllerRightHand != null && controllerLeftHand == null)
		{
			return controllerRightHand.transform.rotation;
		}
		return Quaternion.identity;
	}

	protected virtual Quaternion AverageRotation(Quaternion rot1, Quaternion rot2)
	{
		return Quaternion.Slerp(rot1, rot2, 0.5f);
	}

	protected virtual void SetControllerListeners(GameObject controller, bool controllerState, ref bool subscribedState, bool forceDisabled = false)
	{
		if ((bool)controller)
		{
			bool toggle = !forceDisabled && controllerState;
			ToggleControllerListeners(controller, toggle, ref subscribedState);
		}
	}

	protected virtual void ToggleControllerListeners(GameObject controller, bool toggle, ref bool subscribed)
	{
		VRTK_ControllerEvents component = controller.GetComponent<VRTK_ControllerEvents>();
		if ((bool)component)
		{
			if (engageButton != previousEngageButton && subscribed)
			{
				component.UnsubscribeToButtonAliasEvent(previousEngageButton, startEvent: true, EngageButtonPressed);
				component.UnsubscribeToButtonAliasEvent(previousEngageButton, startEvent: false, EngageButtonReleased);
				subscribed = false;
			}
			if (toggle && !subscribed)
			{
				component.SubscribeToButtonAliasEvent(engageButton, startEvent: true, EngageButtonPressed);
				component.SubscribeToButtonAliasEvent(engageButton, startEvent: false, EngageButtonReleased);
				subscribed = true;
			}
			else if (!toggle && subscribed)
			{
				component.UnsubscribeToButtonAliasEvent(engageButton, startEvent: true, EngageButtonPressed);
				component.UnsubscribeToButtonAliasEvent(engageButton, startEvent: false, EngageButtonReleased);
				subscribed = false;
			}
		}
	}
}
