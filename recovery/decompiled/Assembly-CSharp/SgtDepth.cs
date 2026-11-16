using UnityEngine;

[ExecuteInEditMode]
public abstract class SgtDepth : MonoBehaviour
{
	[Tooltip("The layers that will be sampled when calculating the optical depth")]
	public LayerMask Layers = -5;

	[Tooltip("The transition style between 0..1 depth")]
	public SgtEase.Type Ease;

	private static bool busy;

	public float Calculate(Vector3 eye, Vector3 target)
	{
		if (busy)
		{
			Debug.LogError("Calculate is being called recursively");
			return 0f;
		}
		float num = 0f;
		busy = true;
		num = DoCalculate(eye, target);
		busy = false;
		return 1f - SgtEase.Evaluate(Ease, 1f - num);
	}

	protected abstract float DoCalculate(Vector3 eye, Vector3 target);
}
