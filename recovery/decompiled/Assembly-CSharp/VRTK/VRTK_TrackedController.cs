using UnityEngine;

namespace VRTK;

public class VRTK_TrackedController : MonoBehaviour
{
	public uint index = uint.MaxValue;

	protected GameObject aliasController;

	public event VRTKTrackedControllerEventHandler ControllerEnabled;

	public event VRTKTrackedControllerEventHandler ControllerDisabled;

	public event VRTKTrackedControllerEventHandler ControllerIndexChanged;

	public virtual void OnControllerEnabled(VRTKTrackedControllerEventArgs e)
	{
		if (index < uint.MaxValue && ControllerEnabled != null)
		{
			ControllerEnabled(this, e);
		}
	}

	public virtual void OnControllerDisabled(VRTKTrackedControllerEventArgs e)
	{
		if (index < uint.MaxValue && ControllerDisabled != null)
		{
			ControllerDisabled(this, e);
		}
	}

	public virtual void OnControllerIndexChanged(VRTKTrackedControllerEventArgs e)
	{
		if (index < uint.MaxValue && ControllerIndexChanged != null)
		{
			ControllerIndexChanged(this, e);
		}
	}

	protected virtual VRTKTrackedControllerEventArgs SetEventPayload(uint previousIndex = uint.MaxValue)
	{
		VRTKTrackedControllerEventArgs result = default;
		result.currentIndex = index;
		result.previousIndex = previousIndex;
		return result;
	}

	protected virtual void Awake()
	{
		VRTK_SDKManager.instance.AddBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void OnEnable()
	{
		aliasController = VRTK_DeviceFinder.GetScriptAliasController(gameObject);
		if (aliasController == null)
		{
			aliasController = gameObject;
		}
		index = VRTK_DeviceFinder.GetControllerIndex(gameObject);
		OnControllerEnabled(SetEventPayload());
	}

	protected virtual void OnDisable()
	{
		OnControllerDisabled(SetEventPayload());
	}

	protected virtual void OnDestroy()
	{
		VRTK_SDKManager.instance.RemoveBehaviourToToggleOnLoadedSetupChange(this);
	}

	protected virtual void FixedUpdate()
	{
		VRTK_SDK_Bridge.ControllerProcessFixedUpdate(VRTK_ControllerReference.GetControllerReference(index));
	}

	protected virtual void Update()
	{
		uint controllerIndex = VRTK_DeviceFinder.GetControllerIndex(gameObject);
		if (index < uint.MaxValue && controllerIndex != index)
		{
			uint eventPayload = index;
			OnControllerIndexChanged(SetEventPayload(eventPayload));
		}
		VRTK_SDK_Bridge.ControllerProcessUpdate(VRTK_ControllerReference.GetControllerReference(index));
		if (aliasController != null && gameObject.activeInHierarchy && !aliasController.activeSelf)
		{
			aliasController.SetActive(value: true);
		}
	}
}
