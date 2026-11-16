using UnityEngine;
using UnityEngine.EventSystems;
using UnitySampleAssets.CrossPlatformInput;

public class Joystick : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IDragHandler, IEventSystemHandler
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public int MovementRange = 100;

	public AxisOption axesToUse;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	private Vector3 startPos;

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private void OnEnable()
	{
		startPos = transform.position;
		CreateVirtualAxes();
	}

	private void UpdateVirtualAxes(Vector3 value)
	{
		Vector3 vector = startPos - value;
		vector.y = 0f - vector.y;
		vector /= (float)MovementRange;
		if (useX)
		{
			horizontalVirtualAxis.Update(0f - vector.x);
		}
		if (useY)
		{
			verticalVirtualAxis.Update(vector.y);
		}
	}

	private void CreateVirtualAxes()
	{
		useX = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyHorizontal;
		useY = axesToUse == AxisOption.Both || axesToUse == AxisOption.OnlyVertical;
		if (useX)
		{
			horizontalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(horizontalAxisName);
		}
		if (useY)
		{
			verticalVirtualAxis = new CrossPlatformInputManager.VirtualAxis(verticalAxisName);
		}
	}

	public void OnDrag(PointerEventData data)
	{
		Vector3 zero = Vector3.zero;
		if (useX)
		{
			int value = (int)(data.position.x - startPos.x);
			value = Mathf.Clamp(value, -MovementRange, MovementRange);
			zero.x = value;
		}
		if (useY)
		{
			int value2 = (int)(data.position.y - startPos.y);
			value2 = Mathf.Clamp(value2, -MovementRange, MovementRange);
			zero.y = value2;
		}
		transform.position = new Vector3(startPos.x + zero.x, startPos.y + zero.y, startPos.z + zero.z);
		UpdateVirtualAxes(transform.position);
	}

	public void OnPointerUp(PointerEventData data)
	{
		transform.position = startPos;
		UpdateVirtualAxes(startPos);
	}

	public void OnPointerDown(PointerEventData data)
	{
	}

	private void OnDisable()
	{
		if (useX)
		{
			horizontalVirtualAxis.Remove();
		}
		if (useY)
		{
			verticalVirtualAxis.Remove();
		}
	}
}
