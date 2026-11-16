using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Auto Destroy")]
public class SgtAutoDestroy : MonoBehaviour
{
	[Tooltip("The remaining time until this GameObject is destroyed")]
	public float Seconds = 1f;

	protected virtual void Update()
	{
		Seconds -= Time.deltaTime;
		if (Seconds <= 0f)
		{
			Object.Destroy(gameObject);
		}
	}
}
