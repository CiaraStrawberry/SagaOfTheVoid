using System.Collections;
using UnityEngine;

namespace HWRWeaponSystem;

public class ObjectPool : MonoBehaviour
{
	[HideInInspector]
	public bool Active;

	[HideInInspector]
	public GameObject Prefab;

	public float LifeTime;

	private Vector3 positionTemp;

	private Quaternion rotationTemp;

	private Vector3 scaleTemp;

	private Rigidbody rigidBody;

	private LineRenderer lineRenderer;

	private TrailRenderer[] trailRenderers;

	private float[] trailTemp;

	private void Awake()
	{
		scaleTemp = transform.localScale;
		positionTemp = transform.position;
		rotationTemp = transform.rotation;
		rigidBody = GetComponent<Rigidbody>();
		lineRenderer = GetComponent<LineRenderer>();
		trailRenderers = GetComponentsInChildren<TrailRenderer>();
		trailTemp = new float[trailRenderers.Length];
		for (int i = 0; i < trailRenderers.Length; i++)
		{
			trailTemp[i] = trailRenderers[i].time;
		}
	}

	private void TrailEnable(bool enabled)
	{
	}

	private void Start()
	{
	}

	private void OnEnable()
	{
		if (LifeTime > 0f)
		{
			StartCoroutine(setDestrying(LifeTime));
		}
	}

	public virtual void OnSpawn(Vector3 position, Vector3 scale, Quaternion rotation, GameObject prefab, float lifeTime)
	{
		if (lifeTime != -1f)
		{
			LifeTime = lifeTime;
		}
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = true;
		}
		Prefab = prefab;
		transform.position = position;
		transform.rotation = rotation;
		transform.localScale = scale;
		scaleTemp = transform.localScale;
		positionTemp = transform.position;
		rotationTemp = transform.rotation;
		if ((bool)rigidBody)
		{
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
		}
		if ((bool)lineRenderer)
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, transform.position);
		}
		if ((bool)GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Play();
		}
		for (int i = 0; i < trailRenderers.Length; i++)
		{
			trailRenderers[i].time = trailTemp[i];
		}
		Active = true;
		gameObject.SetActive(value: true);
		if (LifeTime > 0f)
		{
			StartCoroutine(setDestrying(LifeTime));
		}
	}

	public IEnumerator setDestrying(float time)
	{
		yield return new WaitForSeconds(time);
		OnDestroyed();
	}

	public void SetDestroy(float time)
	{
		StartCoroutine(setDestrying(time));
	}

	public void Destroying(float time)
	{
		SetDestroy(time);
	}

	public void Destroying()
	{
		if ((bool)GetComponent<Renderer>())
		{
			GetComponent<Renderer>().enabled = false;
		}
		transform.localScale = scaleTemp;
		transform.position = positionTemp;
		transform.rotation = rotationTemp;
		if ((bool)rigidBody)
		{
			rigidBody.velocity = Vector3.zero;
			rigidBody.angularVelocity = Vector3.zero;
		}
		if ((bool)lineRenderer)
		{
			lineRenderer.SetPosition(0, transform.position);
			lineRenderer.SetPosition(1, transform.position);
		}
		if ((bool)GetComponent<ParticleSystem>())
		{
			GetComponent<ParticleSystem>().Stop();
		}
		TrailRenderer[] array = trailRenderers;
		foreach (TrailRenderer trailRenderer in array)
		{
			trailRenderer.time = 0f;
			trailRenderer.Clear();
		}
		gameObject.SetActive(value: false);
		Active = false;
	}

	public virtual void OnDestroyed()
	{
		Destroying();
	}
}
