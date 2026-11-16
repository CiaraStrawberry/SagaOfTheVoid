using System;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(SgtDebris))]
[RequireComponent(typeof(Rigidbody))]
[AddComponentMenu("Space Graphics Toolkit/SGT Debris Random Velocity")]
public class SgtDebrisVelocity : MonoBehaviour
{
	[Tooltip("The maximum random linear speed this debris can spawn with")]
	public float MaxLinearSpeed = 1f;

	[Tooltip("The maximum random angular speed this debris can spawn with")]
	public float MaxAngularSpeed = 1f;

	[NonSerialized]
	private SgtDebris debris;

	[NonSerialized]
	private Rigidbody body;

	protected virtual void OnEnable()
	{
		if (debris == null)
		{
			debris = GetComponent<SgtDebris>();
		}
		if (body == null)
		{
			body = GetComponent<Rigidbody>();
		}
		SgtDebris sgtDebris = debris;
		sgtDebris.OnSpawn = (Action)Delegate.Combine(sgtDebris.OnSpawn, new Action(Spawn));
		Spawn();
	}

	protected virtual void OnDisable()
	{
		SgtDebris sgtDebris = debris;
		sgtDebris.OnSpawn = (Action)Delegate.Remove(sgtDebris.OnSpawn, new Action(Spawn));
	}

	private void Spawn()
	{
		body.velocity = UnityEngine.Random.insideUnitSphere * MaxLinearSpeed;
		body.angularVelocity = new Vector3(UnityEngine.Random.value, UnityEngine.Random.value, UnityEngine.Random.value) * MaxAngularSpeed;
	}
}
