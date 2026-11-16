using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnitySampleAssets.CrossPlatformInput;

[RequireComponent(typeof(Image))]
public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IEventSystemHandler
{
	public enum AxisOption
	{
		Both,
		OnlyHorizontal,
		OnlyVertical
	}

	public enum ControlStyle
	{
		Absolute,
		Relative,
		Swipe
	}

	public AxisOption axesToUse;

	public ControlStyle controlStyle;

	public string horizontalAxisName = "Horizontal";

	public string verticalAxisName = "Vertical";

	public float sensitivity = 1f;

	private Vector3 startPos;

	private Vector2 previousDelta;

	private Vector3 m_JoytickOutput;

	private bool useX;

	private bool useY;

	private CrossPlatformInputManager.VirtualAxis horizontalVirtualAxis;

	private CrossPlatformInputManager.VirtualAxis verticalVirtualAxis;

	private bool dragging;

	private int id = -1;

	private Vector3 center;

	private Image image;

	private Vector2 previousTouchPos;

	private void OnEnable()
	{
		CreateVirtualAxes();
		image = GetComponent<Image>();
		center = image.transform.position;
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

	private void UpdateVirtualAxes(Vector3 value)
	{
		value = value.normalized;
		if (useX)
		{
			horizontalVirtualAxis.Update(value.x);
		}
		if (useY)
		{
			verticalVirtualAxis.Update(value.y);
		}
	}

	public void OnPointerDown(PointerEventData data)
	{
		dragging = true;
		id = data.pointerId;
		if (controlStyle != ControlStyle.Absolute)
		{
			center = data.position;
		}
	}

	private void Update()
	{
		if (dragging && Input.touchCount >= id && id != -1)
		{
			if (controlStyle == ControlStyle.Swipe)
			{
				center = previousTouchPos;
				previousTouchPos = Input.touches[id].position;
			}
			Vector2 vector = new Vector2(Input.touches[id].position.x - center.x, Input.touches[id].position.y - center.y);
			Vector2 vector2 = vector.normalized * sensitivity;
			UpdateVirtualAxes(new Vector3(vector2.x, vector2.y, 0f));
		}
	}

	public void OnPointerUp(PointerEventData data)
	{
		dragging = false;
		id = -1;
		UpdateVirtualAxes(Vector3.zero);
	}
}
