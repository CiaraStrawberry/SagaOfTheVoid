using UnityEngine;

namespace Devdog.SciFiDesign.UI;

public class TransformRotationFromMousePosition : MonoBehaviour
{
	[SerializeField]
	private Vector3 _rotation;

	private float _prevX;

	private float _prevY;

	protected void Update()
	{
		if (!Mathf.Approximately(Input.mousePosition.x, _prevX) || !Mathf.Approximately(Input.mousePosition.y, _prevY))
		{
			Vector2 vector = new Vector2(Input.mousePosition.x / (float)Screen.width, Input.mousePosition.y / (float)Screen.height);
			vector.x -= 0.5f;
			vector.x *= 2f;
			vector.y -= 0.5f;
			vector.y *= 2f;
			Vector3 rotation = _rotation;
			rotation.x *= vector.y;
			rotation.y *= vector.x;
			transform.rotation = Quaternion.Euler(rotation);
		}
		_prevX = Input.mousePosition.x;
		_prevY = Input.mousePosition.y;
	}
}
