using System.Runtime.InteropServices;
using UnityEngine;
using Valve.VR;

public class SteamVR_TrackedController : MonoBehaviour
{
	public uint controllerIndex;

	public VRControllerState_t controllerState;

	public bool triggerPressed;

	public bool steamPressed;

	public bool menuPressed;

	public bool padPressed;

	public bool padTouched;

	public bool gripped;

	public event ClickedEventHandler MenuButtonClicked;

	public event ClickedEventHandler MenuButtonUnclicked;

	public event ClickedEventHandler TriggerClicked;

	public event ClickedEventHandler TriggerUnclicked;

	public event ClickedEventHandler SteamClicked;

	public event ClickedEventHandler PadClicked;

	public event ClickedEventHandler PadUnclicked;

	public event ClickedEventHandler PadTouched;

	public event ClickedEventHandler PadUntouched;

	public event ClickedEventHandler Gripped;

	public event ClickedEventHandler Ungripped;

	protected virtual void Start()
	{
		if (GetComponent<SteamVR_TrackedObject>() == null)
		{
			gameObject.AddComponent<SteamVR_TrackedObject>();
		}
		if (controllerIndex != 0)
		{
			GetComponent<SteamVR_TrackedObject>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			if (GetComponent<SteamVR_RenderModel>() != null)
			{
				GetComponent<SteamVR_RenderModel>().index = (SteamVR_TrackedObject.EIndex)controllerIndex;
			}
		}
		else
		{
			controllerIndex = (uint)GetComponent<SteamVR_TrackedObject>().index;
		}
	}

	public void SetDeviceIndex(int index)
	{
		controllerIndex = (uint)index;
	}

	public virtual void OnTriggerClicked(ClickedEventArgs e)
	{
		if (TriggerClicked != null)
		{
			TriggerClicked(this, e);
		}
	}

	public virtual void OnTriggerUnclicked(ClickedEventArgs e)
	{
		if (TriggerUnclicked != null)
		{
			TriggerUnclicked(this, e);
		}
	}

	public virtual void OnMenuClicked(ClickedEventArgs e)
	{
		if (MenuButtonClicked != null)
		{
			MenuButtonClicked(this, e);
		}
	}

	public virtual void OnMenuUnclicked(ClickedEventArgs e)
	{
		if (MenuButtonUnclicked != null)
		{
			MenuButtonUnclicked(this, e);
		}
	}

	public virtual void OnSteamClicked(ClickedEventArgs e)
	{
		if (SteamClicked != null)
		{
			SteamClicked(this, e);
		}
	}

	public virtual void OnPadClicked(ClickedEventArgs e)
	{
		if (PadClicked != null)
		{
			PadClicked(this, e);
		}
	}

	public virtual void OnPadUnclicked(ClickedEventArgs e)
	{
		if (PadUnclicked != null)
		{
			PadUnclicked(this, e);
		}
	}

	public virtual void OnPadTouched(ClickedEventArgs e)
	{
		if (PadTouched != null)
		{
			PadTouched(this, e);
		}
	}

	public virtual void OnPadUntouched(ClickedEventArgs e)
	{
		if (PadUntouched != null)
		{
			PadUntouched(this, e);
		}
	}

	public virtual void OnGripped(ClickedEventArgs e)
	{
		if (Gripped != null)
		{
			Gripped(this, e);
		}
	}

	public virtual void OnUngripped(ClickedEventArgs e)
	{
		if (Ungripped != null)
		{
			Ungripped(this, e);
		}
	}

	protected virtual void Update()
	{
		CVRSystem system = OpenVR.System;
		if (system != null && system.GetControllerState(controllerIndex, ref controllerState, (uint)Marshal.SizeOf(typeof(VRControllerState_t))))
		{
			ulong num = controllerState.ulButtonPressed & 0x200000000L;
			if (num != 0 && !triggerPressed)
			{
				triggerPressed = true;
				ClickedEventArgs e = default;
				e.controllerIndex = controllerIndex;
				e.flags = (uint)controllerState.ulButtonPressed;
				e.padX = controllerState.rAxis0.x;
				e.padY = controllerState.rAxis0.y;
				OnTriggerClicked(e);
			}
			else if (num == 0 && triggerPressed)
			{
				triggerPressed = false;
				ClickedEventArgs e2 = default;
				e2.controllerIndex = controllerIndex;
				e2.flags = (uint)controllerState.ulButtonPressed;
				e2.padX = controllerState.rAxis0.x;
				e2.padY = controllerState.rAxis0.y;
				OnTriggerUnclicked(e2);
			}
			ulong num2 = controllerState.ulButtonPressed & 4;
			if (num2 != 0 && !gripped)
			{
				gripped = true;
				ClickedEventArgs e3 = default;
				e3.controllerIndex = controllerIndex;
				e3.flags = (uint)controllerState.ulButtonPressed;
				e3.padX = controllerState.rAxis0.x;
				e3.padY = controllerState.rAxis0.y;
				OnGripped(e3);
			}
			else if (num2 == 0 && gripped)
			{
				gripped = false;
				ClickedEventArgs e4 = default;
				e4.controllerIndex = controllerIndex;
				e4.flags = (uint)controllerState.ulButtonPressed;
				e4.padX = controllerState.rAxis0.x;
				e4.padY = controllerState.rAxis0.y;
				OnUngripped(e4);
			}
			ulong num3 = controllerState.ulButtonPressed & 0x100000000L;
			if (num3 != 0 && !padPressed)
			{
				padPressed = true;
				ClickedEventArgs e5 = default;
				e5.controllerIndex = controllerIndex;
				e5.flags = (uint)controllerState.ulButtonPressed;
				e5.padX = controllerState.rAxis0.x;
				e5.padY = controllerState.rAxis0.y;
				OnPadClicked(e5);
			}
			else if (num3 == 0 && padPressed)
			{
				padPressed = false;
				ClickedEventArgs e6 = default;
				e6.controllerIndex = controllerIndex;
				e6.flags = (uint)controllerState.ulButtonPressed;
				e6.padX = controllerState.rAxis0.x;
				e6.padY = controllerState.rAxis0.y;
				OnPadUnclicked(e6);
			}
			ulong num4 = controllerState.ulButtonPressed & 2;
			if (num4 != 0 && !menuPressed)
			{
				menuPressed = true;
				ClickedEventArgs e7 = default;
				e7.controllerIndex = controllerIndex;
				e7.flags = (uint)controllerState.ulButtonPressed;
				e7.padX = controllerState.rAxis0.x;
				e7.padY = controllerState.rAxis0.y;
				OnMenuClicked(e7);
			}
			else if (num4 == 0 && menuPressed)
			{
				menuPressed = false;
				ClickedEventArgs e8 = default;
				e8.controllerIndex = controllerIndex;
				e8.flags = (uint)controllerState.ulButtonPressed;
				e8.padX = controllerState.rAxis0.x;
				e8.padY = controllerState.rAxis0.y;
				OnMenuUnclicked(e8);
			}
			num3 = controllerState.ulButtonTouched & 0x100000000L;
			if (num3 != 0 && !padTouched)
			{
				padTouched = true;
				ClickedEventArgs e9 = default;
				e9.controllerIndex = controllerIndex;
				e9.flags = (uint)controllerState.ulButtonPressed;
				e9.padX = controllerState.rAxis0.x;
				e9.padY = controllerState.rAxis0.y;
				OnPadTouched(e9);
			}
			else if (num3 == 0 && padTouched)
			{
				padTouched = false;
				ClickedEventArgs e10 = default;
				e10.controllerIndex = controllerIndex;
				e10.flags = (uint)controllerState.ulButtonPressed;
				e10.padX = controllerState.rAxis0.x;
				e10.padY = controllerState.rAxis0.y;
				OnPadUntouched(e10);
			}
		}
	}
}
