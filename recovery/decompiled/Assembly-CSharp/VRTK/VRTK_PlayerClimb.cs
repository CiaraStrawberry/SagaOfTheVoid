using UnityEngine;
using VRTK.GrabAttachMechanics;

namespace VRTK;

[RequireComponent(typeof(VRTK_BodyPhysics))]
[AddComponentMenu("VRTK/Scripts/Locomotion/VRTK_PlayerClimb")]
public class VRTK_PlayerClimb : MonoBehaviour
{
	[Header("Climb Settings")]
	[Tooltip("Will scale movement up and down based on the player transform's scale.")]
	public bool usePlayerScale = true;

	[Header("Custom Settings")]
	[Tooltip("The VRTK Body Physics script to use for dealing with climbing and falling. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_BodyPhysics bodyPhysics;

	[Tooltip("The VRTK Teleport script to use when snapping to nearest floor on release. If this is left blank then a Teleport script will need to be applied to the same GameObject.")]
	public VRTK_BasicTeleport teleporter;

	[Tooltip("The VRTK Headset Collision script to use for determining if the user is climbing inside a collidable object. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_HeadsetCollision headsetCollision;

	[Tooltip("The VRTK Position Rewind script to use for dealing resetting invalid positions. If this is left blank then the script will need to be applied to the same GameObject.")]
	public VRTK_PositionRewind positionRewind;

	protected Transform playArea;

	protected Vector3 startControllerScaledLocalPosition;

	protected Vector3 startGrabPointLocalPosition;

	protected Vector3 startPlayAreaWorldOffset;

	protected GameObject grabbingController;

	protected GameObject climbingObject;

	protected Quaternion climbingObjectLastRotation;

	protected bool isClimbing;

	protected bool useGrabbedObjectRotation;

	public event PlayerClimbEventHandler PlayerClimbStarted;

	public event PlayerClimbEventHandler PlayerClimbEnded;

	protected virtual void Awake()
	{
		bodyPhysics = ((!(bodyPhysics != null)) ? GetComponentInChildren<VRTK_BodyPhysics>() : bodyPhysics);
		teleporter = ((!(teleporter != null)) ? GetComponentInChildren<VRTK_BasicTeleport>() : teleporter);
		headsetCollision = ((!(headsetCollision != null)) ? GetComponentInChildren<VRTK_HeadsetCollision>() : headsetCollision);
		positionRewind = ((!(positionRewind != null)) ? GetComponentInChildren<VRTK_PositionRewind>() : positionRewind);
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		playArea = VRTK_DeviceFinder.PlayAreaTransform();
		InitListeners(state: true);
	}

	protected virtual void OnDisable()
	{
		Ungrab(carryMomentum: false, null, climbingObject);
		InitListeners(state: false);
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void Update()
	{
		if (isClimbing)
		{
			Vector3 vector = GetScaledLocalPosition(grabbingController.transform) - startControllerScaledLocalPosition;
			Vector3 vector2 = climbingObject.transform.TransformPoint(startGrabPointLocalPosition);
			playArea.position = vector2 + startPlayAreaWorldOffset - vector;
			if (useGrabbedObjectRotation)
			{
				Vector3 vector3 = climbingObjectLastRotation * Vector3.forward;
				Vector3 vector4 = climbingObject.transform.rotation * Vector3.forward;
				Vector3 axis = Vector3.Cross(vector3, vector4);
				float angle = Vector3.Angle(vector3, vector4);
				playArea.RotateAround(vector2, axis, angle);
				climbingObjectLastRotation = climbingObject.transform.rotation;
			}
			if (!IsHeadsetColliding())
			{
				positionRewind.SetLastGoodPosition();
			}
		}
	}

	protected virtual void OnPlayerClimbStarted(PlayerClimbEventArgs e)
	{
		if (PlayerClimbStarted != null)
		{
			PlayerClimbStarted(this, e);
		}
	}

	protected virtual void OnPlayerClimbEnded(PlayerClimbEventArgs e)
	{
		if (PlayerClimbEnded != null)
		{
			PlayerClimbEnded(this, e);
		}
	}

	protected virtual PlayerClimbEventArgs SetPlayerClimbEvent(VRTK_ControllerReference controllerReference, GameObject target)
	{
		PlayerClimbEventArgs result = default;
		result.controllerIndex = VRTK_ControllerReference.GetRealIndex(controllerReference);
		result.controllerReference = controllerReference;
		result.target = target;
		return result;
	}

	protected virtual void InitListeners(bool state)
	{
		InitControllerListeners(VRTK_DeviceFinder.GetControllerLeftHand(), state);
		InitControllerListeners(VRTK_DeviceFinder.GetControllerRightHand(), state);
		InitTeleportListener(state);
	}

	protected virtual void InitTeleportListener(bool state)
	{
		if (teleporter != null)
		{
			if (state)
			{
				teleporter.Teleporting += OnTeleport;
			}
			else
			{
				teleporter.Teleporting -= OnTeleport;
			}
		}
	}

	protected virtual void OnTeleport(object sender, DestinationMarkerEventArgs e)
	{
		if (isClimbing)
		{
			Ungrab(carryMomentum: false, e.controllerReference, e.target.gameObject);
		}
	}

	protected virtual Vector3 GetScaledLocalPosition(Transform objTransform)
	{
		if (usePlayerScale)
		{
			return playArea.localRotation * Vector3.Scale(objTransform.localPosition, playArea.localScale);
		}
		return playArea.localRotation * objTransform.localPosition;
	}

	protected virtual void OnGrabObject(object sender, ObjectInteractEventArgs e)
	{
		if (IsClimbableObject(e.target))
		{
			GameObject givenController = ((VRTK_InteractGrab)sender).gameObject;
			GameObject actualController = VRTK_DeviceFinder.GetActualController(givenController);
			Grab(actualController, e.controllerReference, e.target);
		}
	}

	protected virtual void OnUngrabObject(object sender, ObjectInteractEventArgs e)
	{
		GameObject givenController = ((VRTK_InteractGrab)sender).gameObject;
		GameObject actualController = VRTK_DeviceFinder.GetActualController(givenController);
		if ((bool)e.target && IsClimbableObject(e.target) && IsActiveClimbingController(actualController))
		{
			Ungrab(carryMomentum: true, e.controllerReference, e.target);
		}
	}

	protected virtual void Grab(GameObject currentGrabbingController, VRTK_ControllerReference controllerReference, GameObject target)
	{
		bodyPhysics.ResetFalling();
		bodyPhysics.TogglePreventSnapToFloor(state: true);
		bodyPhysics.enableBodyCollisions = false;
		bodyPhysics.ToggleOnGround(state: false);
		isClimbing = true;
		climbingObject = target;
		grabbingController = currentGrabbingController;
		startControllerScaledLocalPosition = GetScaledLocalPosition(grabbingController.transform);
		startGrabPointLocalPosition = climbingObject.transform.InverseTransformPoint(grabbingController.transform.position);
		startPlayAreaWorldOffset = playArea.transform.position - grabbingController.transform.position;
		climbingObjectLastRotation = climbingObject.transform.rotation;
		useGrabbedObjectRotation = climbingObject.GetComponent<VRTK_ClimbableGrabAttach>().useObjectRotation;
		OnPlayerClimbStarted(SetPlayerClimbEvent(controllerReference, climbingObject));
	}

	protected virtual void Ungrab(bool carryMomentum, VRTK_ControllerReference controllerReference, GameObject target)
	{
		isClimbing = false;
		if (IsHeadsetColliding())
		{
			positionRewind.RewindPosition();
		}
		if (IsBodyColliding() && !IsHeadsetColliding())
		{
			bodyPhysics.ForceSnapToFloor();
		}
		bodyPhysics.enableBodyCollisions = true;
		if (carryMomentum)
		{
			Vector3 velocity = Vector3.zero;
			if (VRTK_ControllerReference.IsValid(controllerReference))
			{
				velocity = -VRTK_DeviceFinder.GetControllerVelocity(controllerReference);
				velocity = ((!usePlayerScale) ? playArea.TransformDirection(velocity) : playArea.TransformVector(velocity));
			}
			bodyPhysics.ApplyBodyVelocity(velocity, forcePhysicsOn: true, applyMomentum: true);
		}
		grabbingController = null;
		climbingObject = null;
		OnPlayerClimbEnded(SetPlayerClimbEvent(controllerReference, target));
	}

	protected virtual bool IsActiveClimbingController(GameObject controller)
	{
		return controller == grabbingController;
	}

	protected virtual bool IsClimbableObject(GameObject obj)
	{
		VRTK_InteractableObject component = obj.GetComponent<VRTK_InteractableObject>();
		return (bool)component && (bool)component.grabAttachMechanicScript && component.grabAttachMechanicScript.IsClimbable();
	}

	protected virtual void InitControllerListeners(GameObject controller, bool state)
	{
		if (!controller)
		{
			return;
		}
		VRTK_InteractGrab component = controller.GetComponent<VRTK_InteractGrab>();
		if ((bool)component)
		{
			if (state)
			{
				component.ControllerGrabInteractableObject += OnGrabObject;
				component.ControllerUngrabInteractableObject += OnUngrabObject;
			}
			else
			{
				component.ControllerGrabInteractableObject -= OnGrabObject;
				component.ControllerUngrabInteractableObject -= OnUngrabObject;
			}
		}
	}

	protected virtual bool IsBodyColliding()
	{
		return bodyPhysics != null && bodyPhysics.GetCurrentCollidingObject() != null;
	}

	protected virtual bool IsHeadsetColliding()
	{
		return headsetCollision != null && headsetCollision.IsColliding();
	}
}
