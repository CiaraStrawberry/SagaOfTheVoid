using System.Collections;
using UnityEngine;

namespace Devdog.SciFiDesign;

public class FlockUnit : MonoBehaviour
{
	public float speedMin = 50f;

	public float speedMax = 100f;

	public float rotationSpeed = 1f;

	protected Vector3 avgHeading;

	protected Vector3 avgPosition;

	protected float neighbourDistance = 20f;

	protected Quaternion aimRotation;

	public float speed { get; private set; }

	public FlockGroup parent { get; set; }

	protected void Start()
	{
		speed = Random.Range(speedMin, speedMax);
		neighbourDistance *= neighbourDistance;
		StartCoroutine(RandomUpdate());
	}

	private IEnumerator RandomUpdate()
	{
		while (true)
		{
			yield return new WaitForSeconds(Random.Range(0.2f, 0.5f));
			ApplyRules();
		}
	}

	protected void Update()
	{
		transform.Translate(Vector3.forward * speed * Time.deltaTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, aimRotation, Time.deltaTime * rotationSpeed);
	}

	private void ApplyRules()
	{
		Vector3 zero = Vector3.zero;
		Vector3 zero2 = Vector3.zero;
		float num = speedMin;
		int num2 = 0;
		FlockUnit[] units = parent.units;
		foreach (FlockUnit flockUnit in units)
		{
			if (flockUnit == null || flockUnit == transform)
			{
				continue;
			}
			float num3 = Vector3.SqrMagnitude(transform.position - flockUnit.transform.position);
			if (num3 <= neighbourDistance)
			{
				zero += flockUnit.transform.position;
				num2++;
				if (num3 < 4f)
				{
					zero2 += transform.position - flockUnit.transform.position;
				}
				speed += num + flockUnit.speed;
			}
		}
		if (num2 > 0)
		{
			zero = zero / num2 + (parent.aimGoal.position - transform.position);
			speed = num / (float)num2;
			Vector3 vector = zero + zero2 - transform.position;
			if (vector != Vector3.zero)
			{
				aimRotation = Quaternion.LookRotation(vector);
			}
		}
	}
}
