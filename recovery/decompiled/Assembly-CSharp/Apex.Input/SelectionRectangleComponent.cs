using UnityEngine;

namespace Apex.Input;

[AddComponentMenu("Apex/Game World/Selection Rectangle", 1041)]
public class SelectionRectangleComponent : MonoBehaviour
{
	public float startDeltaThreshold = 3f;

	private Camera _selectionVisualCamera;

	private Transform _selectionVisual;

	private void Awake()
	{
		_selectionVisualCamera = GetComponentInChildren<Camera>();
		_selectionVisual = GetComponentInChildren<MeshRenderer>().transform;
		ToggleEnabled(enabled: false);
	}

	internal void StartSelect()
	{
		ToggleEnabled(enabled: true);
	}

	internal bool HasSelection(Vector3 startScreen, Vector3 endScreen)
	{
		if (Mathf.Abs(startScreen.x - endScreen.x) < startDeltaThreshold || Mathf.Abs(startScreen.y - endScreen.y) < startDeltaThreshold)
		{
			return false;
		}
		DrawSelectionRect(startScreen, endScreen);
		return true;
	}

	internal void EndSelect()
	{
		ToggleEnabled(enabled: false);
	}

	private void ToggleEnabled(bool enabled)
	{
		_selectionVisualCamera.enabled = enabled;
		if (!enabled)
		{
			_selectionVisual.localScale = Vector3.zero;
		}
	}

	private void DrawSelectionRect(Vector3 startScreen, Vector3 endScreen)
	{
		Vector3 vector = _selectionVisualCamera.ScreenToWorldPoint(startScreen);
		Vector3 vector2 = _selectionVisualCamera.ScreenToWorldPoint(endScreen);
		float num = vector2.x - vector.x;
		float num2 = vector2.y - vector.y;
		_selectionVisual.position = new Vector3(vector.x + num / 2f, vector.y + num2 / 2f);
		_selectionVisual.localScale = new Vector3(Mathf.Abs(num), Mathf.Abs(num2), 1f);
	}
}
