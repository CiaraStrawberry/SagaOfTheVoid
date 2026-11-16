using UnityEngine;
using VRTK;

public class ViveInputHandler : aInput
{
	private enum eButtonStates
	{
		Pressed,
		Released
	}

	public SteamVR_ControllerManager SteamController;

	private GameObject _LeftPaddle;

	private GameObject _RightPaddle;

	private VRTK_ControllerEvents _LeftDeviceEvents;

	private VRTK_ControllerEvents _RightDeviceEvents;

	private InputController _InputController;

	private eButtonStates ScaleButtonPressed = eButtonStates.Released;

	private float _PreviousDistance;

	public override GameObject GetLeftPaddle()
	{
		return _LeftPaddle;
	}

	public override GameObject GetrightPaddle()
	{
		return _RightPaddle;
	}

	private void Start()
	{
		Debug.Log("Start");
		_InputController = gameObject.GetComponent<InputController>();
		_LeftPaddle = SteamController.left;
		_RightPaddle = SteamController.right;
		Debug.Log("Left: " + _LeftPaddle);
		Debug.Log("Right: " + _RightPaddle);
		_LeftDeviceEvents = _LeftPaddle.GetComponent<VRTK_ControllerEvents>();
		_RightDeviceEvents = _RightPaddle.GetComponent<VRTK_ControllerEvents>();
	}

	private void Update()
	{
		if (ScaleButtonPressed == eButtonStates.Released && _LeftDeviceEvents.triggerClicked && _RightDeviceEvents.triggerClicked)
		{
			ScaleButtonPressed = eButtonStates.Pressed;
			Debug.Log(_LeftPaddle);
			Debug.Log(_RightPaddle);
			float obj = (_PreviousDistance = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position));
			if (_InputController.OnScaleStart != null)
			{
				_InputController.OnScaleStart(obj);
			}
		}
		else if (ScaleButtonPressed == eButtonStates.Pressed && _LeftDeviceEvents.triggerClicked && _RightDeviceEvents.triggerClicked)
		{
			float num = 0f;
			float num2 = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position);
			num = ((!(num2 < _PreviousDistance)) ? 0.03f : (-0.03f));
			_PreviousDistance = num2;
			if (_InputController.OnScaleStart != null)
			{
				_InputController.OnScale(num2, num);
			}
		}
		else if (ScaleButtonPressed == eButtonStates.Pressed && (!_LeftDeviceEvents.triggerClicked || !_RightDeviceEvents.triggerClicked))
		{
			ScaleButtonPressed = eButtonStates.Released;
			float obj2 = Vector3.Distance(_LeftPaddle.transform.position, _RightPaddle.transform.position);
			_PreviousDistance = 0f;
			if (_InputController.OnScaleStart != null)
			{
				_InputController.OnScaleEnd(obj2);
			}
		}
	}
}
