using UnityEngine;

namespace Forge3D;

public class F3DWaspTrackingController : MonoBehaviour
{
	public Transform Target;

	public GameObject Body;

	public GameObject Mount;

	private Vector3 defaultDir;

	private Quaternion defaultRot;

	private Transform headTransform;

	private Transform barrelTransform;

	public float HeadingTrackingSpeed = 2f;

	public float ElevationTrackingSpeed = 2f;

	private Vector3 targetPos;

	private Vector3 headingVetor;

	private float curHeadingAngle;

	private float curElevationAngle;

	public Vector2 HeadingLimit;

	public Vector2 ElevationLimit;

	public bool DebugDraw;

	private void Awake()
	{
		headTransform = Mount.GetComponent<Transform>();
		barrelTransform = Body.GetComponent<Transform>();
	}

	private void Start()
	{
		targetPos = headTransform.transform.position + headTransform.transform.forward * 100f;
		defaultDir = Mount.transform.forward;
		defaultRot = Quaternion.FromToRotation(transform.forward, defaultDir);
	}

	public void SetNewTarget(Vector3 _targetPos)
	{
		targetPos = _targetPos;
	}

	private void Update()
	{
		if (barrelTransform != null && headTransform != null)
		{
			if (Target != null)
			{
				targetPos = Target.position;
			}
			headingVetor = Vector3.Normalize(ProjectVectorOnPlane(headTransform.up, targetPos - headTransform.position));
			float num = SignedVectorAngle(headTransform.forward, headingVetor, headTransform.up);
			float num2 = SignedVectorAngle(defaultRot * headTransform.forward, headingVetor, headTransform.up);
			float num3 = SignedVectorAngle(defaultRot * headTransform.forward, headTransform.forward, headTransform.up);
			float num4 = HeadingTrackingSpeed * Time.deltaTime;
			num4 = ((!(HeadingLimit.x <= -180f) || !(HeadingLimit.y >= 180f)) ? (num4 * Mathf.Sign(num2 - num3)) : (num4 * Mathf.Sign(num)));
			if (Mathf.Abs(num4) > Mathf.Abs(num))
			{
				num4 = num;
			}
			if ((curHeadingAngle + num4 > HeadingLimit.x && curHeadingAngle + num4 < HeadingLimit.y) || (HeadingLimit.x <= -180f && HeadingLimit.y >= 180f))
			{
				curHeadingAngle += num4;
				headTransform.rotation *= Quaternion.Euler(0f, num4, 0f);
			}
			Vector3 otherVector = Vector3.Normalize(ProjectVectorOnPlane(headTransform.right, targetPos - barrelTransform.position));
			float num5 = SignedVectorAngle(barrelTransform.forward, otherVector, headTransform.right);
			float num6 = Mathf.Sign(num5) * ElevationTrackingSpeed * Time.deltaTime;
			if (Mathf.Abs(num6) > Mathf.Abs(num5))
			{
				num6 = num5;
			}
			if (curElevationAngle + num6 < ElevationLimit.y && curElevationAngle + num6 > ElevationLimit.x)
			{
				curElevationAngle += num6;
				barrelTransform.rotation *= Quaternion.Euler(num6, 0f, 0f);
			}
		}
		if (DebugDraw)
		{
			Debug.DrawLine(barrelTransform.position, barrelTransform.position + barrelTransform.forward * Vector3.Distance(barrelTransform.position, targetPos), Color.red);
		}
	}

	private Vector3 ProjectVectorOnPlane(Vector3 planeNormal, Vector3 vector)
	{
		return vector - Vector3.Dot(vector, planeNormal) * planeNormal;
	}

	private float SignedVectorAngle(Vector3 referenceVector, Vector3 otherVector, Vector3 normal)
	{
		Vector3 lhs = Vector3.Cross(normal, referenceVector);
		float num = Vector3.Angle(referenceVector, otherVector);
		return num * Mathf.Sign(Vector3.Dot(lhs, otherVector));
	}
}
