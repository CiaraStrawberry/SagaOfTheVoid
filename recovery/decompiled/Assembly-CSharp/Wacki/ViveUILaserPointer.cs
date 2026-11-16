using UnityEngine;
using Valve.VR;

namespace Wacki;

public class ViveUILaserPointer : IUILaserPointer
{
	public EVRButtonId button = EVRButtonId.k_EButton_Axis1;

	private SteamVR_TrackedObject _trackedObject;

	private bool _connected;

	private int _index;

	protected override void Initialize()
	{
		base.Initialize();
		Debug.Log("Initialize");
		SteamVR_TrackedObject component = GetComponent<SteamVR_TrackedObject>();
		if (component != null)
		{
			_index = (int)component.index;
			_connected = true;
		}
	}

	public override bool ButtonDown()
	{
		if (!_connected)
		{
			return false;
		}
		return SteamVR_Controller.Input(_index)?.GetPressDown(button) ?? false;
	}

	public override bool ButtonUp()
	{
		if (!_connected)
		{
			return false;
		}
		return SteamVR_Controller.Input(_index)?.GetPressUp(button) ?? false;
	}

	public override void OnEnterControl(GameObject control)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input(_index);
		device.TriggerHapticPulse(1000);
	}

	public override void OnExitControl(GameObject control)
	{
		SteamVR_Controller.Device device = SteamVR_Controller.Input(_index);
		device.TriggerHapticPulse(600);
	}
}
