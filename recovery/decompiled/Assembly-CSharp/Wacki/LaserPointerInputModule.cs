using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Wacki;

public class LaserPointerInputModule : BaseInputModule
{
	private class ControllerData
	{
		public PointerEventData pointerEvent;

		public GameObject currentPoint;

		public GameObject currentPressed;

		public GameObject currentDragging;
	}

	private static LaserPointerInputModule _instance;

	private Camera UICamera;

	private HashSet<IUILaserPointer> _controllers;

	private Dictionary<IUILaserPointer, ControllerData> _controllerData = new Dictionary<IUILaserPointer, ControllerData>();

	public static LaserPointerInputModule instance => _instance;

	protected override void Awake()
	{
		base.Awake();
		if (_instance != null)
		{
			Debug.LogWarning("Trying to instantiate multiple LaserPointerInputModule.");
			Object.DestroyImmediate(gameObject);
		}
		_instance = this;
	}

	protected override void Start()
	{
		base.Start();
		UICamera = new GameObject("UI Camera").AddComponent<Camera>();
		UICamera.clearFlags = CameraClearFlags.Nothing;
		UICamera.cullingMask = 0;
		UICamera.fieldOfView = 5f;
		UICamera.nearClipPlane = 0.01f;
		Canvas[] array = Resources.FindObjectsOfTypeAll<Canvas>();
		Canvas[] array2 = array;
		foreach (Canvas canvas in array2)
		{
			canvas.worldCamera = UICamera;
		}
	}

	public void AddController(IUILaserPointer controller)
	{
		_controllerData.Add(controller, new ControllerData());
	}

	public void RemoveController(IUILaserPointer controller)
	{
		_controllerData.Remove(controller);
	}

	protected void UpdateCameraPosition(IUILaserPointer controller)
	{
		UICamera.transform.position = controller.transform.position;
		UICamera.transform.rotation = controller.transform.rotation;
	}

	public void ClearSelection()
	{
		if ((bool)eventSystem.currentSelectedGameObject)
		{
			eventSystem.SetSelectedGameObject(null);
		}
	}

	private void Select(GameObject go)
	{
		ClearSelection();
		if ((bool)ExecuteEvents.GetEventHandler<ISelectHandler>(go))
		{
			eventSystem.SetSelectedGameObject(go);
		}
	}

	public override void Process()
	{
		foreach (KeyValuePair<IUILaserPointer, ControllerData> controllerDatum in _controllerData)
		{
			IUILaserPointer key = controllerDatum.Key;
			ControllerData value = controllerDatum.Value;
			UpdateCameraPosition(key);
			if (value.pointerEvent == null)
			{
				value.pointerEvent = new PointerEventData(eventSystem);
			}
			else
			{
				value.pointerEvent.Reset();
			}
			value.pointerEvent.delta = Vector2.zero;
			value.pointerEvent.position = new Vector2((float)UICamera.pixelWidth * 0.5f, (float)UICamera.pixelHeight * 0.5f);
			eventSystem.RaycastAll(value.pointerEvent, m_RaycastResultCache);
			value.pointerEvent.pointerCurrentRaycast = BaseInputModule.FindFirstRaycast(m_RaycastResultCache);
			m_RaycastResultCache.Clear();
			if (value.pointerEvent.pointerCurrentRaycast.distance > 0f)
			{
				key.LimitLaserDistance(value.pointerEvent.pointerCurrentRaycast.distance + 0.01f);
			}
			GameObject gameObject = value.pointerEvent.pointerCurrentRaycast.gameObject;
			if (value.currentPoint != gameObject)
			{
				if (value.currentPoint != null)
				{
					key.OnExitControl(value.currentPoint);
				}
				if (gameObject != null)
				{
					key.OnEnterControl(gameObject);
				}
			}
			value.currentPoint = gameObject;
			HandlePointerExitAndEnter(value.pointerEvent, value.currentPoint);
			if (key.ButtonDown())
			{
				ClearSelection();
				value.pointerEvent.pressPosition = value.pointerEvent.position;
				value.pointerEvent.pointerPressRaycast = value.pointerEvent.pointerCurrentRaycast;
				value.pointerEvent.pointerPress = null;
				if (value.currentPoint != null)
				{
					value.currentPressed = value.currentPoint;
					GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy(value.currentPressed, value.pointerEvent, ExecuteEvents.pointerDownHandler);
					if (gameObject2 == null)
					{
						gameObject2 = ExecuteEvents.ExecuteHierarchy(value.currentPressed, value.pointerEvent, ExecuteEvents.pointerClickHandler);
						if (gameObject2 != null)
						{
							value.currentPressed = gameObject2;
						}
					}
					else
					{
						value.currentPressed = gameObject2;
						ExecuteEvents.Execute(gameObject2, value.pointerEvent, ExecuteEvents.pointerClickHandler);
					}
					if (gameObject2 != null)
					{
						value.pointerEvent.pointerPress = gameObject2;
						value.currentPressed = gameObject2;
						Select(value.currentPressed);
					}
					ExecuteEvents.Execute(value.currentPressed, value.pointerEvent, ExecuteEvents.beginDragHandler);
					value.pointerEvent.pointerDrag = value.currentPressed;
					value.currentDragging = value.currentPressed;
				}
			}
			if (key.ButtonUp())
			{
				if (value.currentDragging != null)
				{
					ExecuteEvents.Execute(value.currentDragging, value.pointerEvent, ExecuteEvents.endDragHandler);
					if (value.currentPoint != null)
					{
						ExecuteEvents.ExecuteHierarchy(value.currentPoint, value.pointerEvent, ExecuteEvents.dropHandler);
					}
					value.pointerEvent.pointerDrag = null;
					value.currentDragging = null;
				}
				if ((bool)value.currentPressed)
				{
					ExecuteEvents.Execute(value.currentPressed, value.pointerEvent, ExecuteEvents.pointerUpHandler);
					value.pointerEvent.rawPointerPress = null;
					value.pointerEvent.pointerPress = null;
					value.currentPressed = null;
				}
			}
			if (value.currentDragging != null)
			{
				ExecuteEvents.Execute(value.currentDragging, value.pointerEvent, ExecuteEvents.dragHandler);
			}
			if (eventSystem.currentSelectedGameObject != null)
			{
				ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, GetBaseEventData(), ExecuteEvents.updateSelectedHandler);
			}
		}
	}
}
