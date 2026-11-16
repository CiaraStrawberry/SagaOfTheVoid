using UnityEngine;

namespace Apex.Input;

[AddComponentMenu("Apex/Game World/Very basic input receiver", 1045)]
[InputReceiver]
[ApexComponent("Input")]
public class InputReceiverBasic : MonoBehaviour
{
	public bool rightClickSupported = true;

	private InputController _inputController;

	private SelectionRectangleComponent _selectRectangle;

	private Vector3 _lastSelectDownPos;

	private void Awake()
	{
		_inputController = new InputController();
		_selectRectangle = GetComponentInChildren<SelectionRectangleComponent>();
		if (_selectRectangle == null)
		{
			Debug.LogWarning("Missing SelectionRectangleComponent, this is required by the input receiver to handle unit selection.");
		}
		if (Application.platform != RuntimePlatform.WindowsPlayer && Application.platform != RuntimePlatform.WindowsEditor && Application.platform != RuntimePlatform.WindowsWebPlayer && Application.platform != RuntimePlatform.OSXPlayer && Application.platform != RuntimePlatform.OSXEditor && Application.platform != RuntimePlatform.OSXWebPlayer && Application.platform != RuntimePlatform.LinuxPlayer)
		{
			Debug.LogWarning("The default basic input receiver only works on Windows, Linux and Mac.");
		}
	}

	private void Update()
	{
		Movement();
		Selection();
	}

	private void Movement()
	{
		bool flag = false;
		if ((!rightClickSupported) ? (UnityEngine.Input.GetMouseButtonUp(0) && UnityEngine.Input.GetKey(KeyCode.LeftControl)) : UnityEngine.Input.GetMouseButtonUp(1))
		{
			bool key = UnityEngine.Input.GetKey(KeyCode.LeftShift);
			_inputController.SetDestination(UnityEngine.Input.mousePosition, key);
		}
	}

	private void Selection()
	{
		if (_selectRectangle == null || (!rightClickSupported && UnityEngine.Input.GetKey(KeyCode.LeftControl)))
		{
			return;
		}
		bool key = UnityEngine.Input.GetKey(KeyCode.LeftShift);
		if (UnityEngine.Input.GetMouseButtonDown(0))
		{
			_lastSelectDownPos = UnityEngine.Input.mousePosition;
			_selectRectangle.StartSelect();
			return;
		}
		if (UnityEngine.Input.GetMouseButton(0))
		{
			if (_selectRectangle.HasSelection(_lastSelectDownPos, UnityEngine.Input.mousePosition))
			{
				_inputController.SelectUnitRangeTentative(_lastSelectDownPos, UnityEngine.Input.mousePosition, key);
			}
			return;
		}
		if (UnityEngine.Input.GetMouseButtonUp(0))
		{
			if (_selectRectangle.HasSelection(_lastSelectDownPos, UnityEngine.Input.mousePosition))
			{
				_inputController.SelectUnitRange(_lastSelectDownPos, UnityEngine.Input.mousePosition, key);
			}
			else
			{
				_inputController.SelectUnit(_lastSelectDownPos, key);
			}
			_selectRectangle.EndSelect();
			return;
		}
		bool key2 = UnityEngine.Input.GetKey(KeyCode.LeftShift);
		bool key3 = UnityEngine.Input.GetKey(KeyCode.LeftAlt);
		for (int i = 0; i < 5; i++)
		{
			KeyCode key4 = (KeyCode)(49 + i);
			if (UnityEngine.Input.GetKeyUp(key4))
			{
				if (key2)
				{
					_inputController.SelectGroup(i);
				}
				else if (key3)
				{
					_inputController.AssignGroup(i);
				}
				else
				{
					_inputController.SelectUnit(i, toggle: false);
				}
			}
		}
	}
}
