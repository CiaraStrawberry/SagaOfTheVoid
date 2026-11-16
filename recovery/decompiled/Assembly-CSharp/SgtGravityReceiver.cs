using System;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Space Graphics Toolkit/SGT Gravity Receiver")]
public class SgtGravityReceiver : MonoBehaviour
{
	[NonSerialized]
	private Rigidbody body;

	protected virtual void FixedUpdate()
	{
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
		for (int num = SgtGravitySource.AllGravitySources.Count - 1; num >= 0; num--)
		{
			SgtGravitySource sgtGravitySource = SgtGravitySource.AllGravitySources[num];
			if (sgtGravitySource.transform != transform)
			{
				float num2 = body.mass * sgtGravitySource.Mass;
				Vector3 vector = sgtGravitySource.transform.position - transform.position;
				float sqrMagnitude = vector.sqrMagnitude;
				if (sqrMagnitude > 0f)
				{
					float num3 = num2 / sqrMagnitude;
					body.AddForce(vector.normalized * num3 * Time.fixedDeltaTime, ForceMode.Acceleration);
				}
			}
		}
	}
}
