using PlaygroundSplines;
using UnityEngine;

[ExecuteInEditMode]
public class MoveOverSpline : MonoBehaviour
{
	public PlaygroundSpline spline;

	[Range(0f, 1f)]
	public float progression;

	public bool loop;

	private float _previousProgression = -1f;

	private void Update()
	{
		if (!Mathf.Approximately(progression, _previousProgression))
		{
			transform.position = spline.GetPoint((!loop) ? Mathf.Min(progression, 0.999f) : progression);
			_previousProgression = progression;
		}
	}
}
