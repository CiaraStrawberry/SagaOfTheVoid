using UnityEngine;

namespace Devdog.General;

public static class TransformExtensionMethods
{
	public static void ResetTRS(this Transform transform)
	{
		transform.localPosition = Vector3.zero;
		transform.localRotation = Quaternion.identity;
		transform.localScale = Vector3.one;
	}

	public static void ResetTRSRect(this Transform transform)
	{
		transform.ResetTRS();
		RectTransform component = transform.GetComponent<RectTransform>();
		if (component != null)
		{
			component.anchoredPosition = Vector2.zero;
		}
	}
}
