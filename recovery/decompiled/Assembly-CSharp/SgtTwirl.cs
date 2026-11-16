using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Twirl")]
public class SgtTwirl : MonoBehaviour
{
	[Tooltip("The spin rate")]
	public float DegreesPerSecond = 1f;

	[Tooltip("The current axis of rotation")]
	public Vector3 Axis = Vector3.up;

	[Tooltip("The speed at which the axis changes to a new axis")]
	public float Dampening = 2f;

	[SerializeField]
	private Vector3 targetAxis;

	protected virtual void Update()
	{
		if (targetAxis == Vector3.zero || Vector3.Distance(targetAxis, Axis) < 0.1f)
		{
			targetAxis.x = Random.Range(-1f, 1f);
			targetAxis.y = Random.Range(-1f, 1f);
			targetAxis.z = Random.Range(-1f, 1f);
			targetAxis = targetAxis.normalized;
		}
		Axis = SgtHelper.Dampen3(Axis, targetAxis, Dampening, Time.deltaTime, 0.1f);
		transform.Rotate(Axis.normalized, DegreesPerSecond * Time.deltaTime);
	}
}
