using UnityEngine;

namespace Forge3D;

[AddComponentMenu("FORGE3D/Force Field/Force Field Mobile")]
public class Forcefield_Mobile : MonoBehaviour
{
	private Material mat;

	private MeshFilter mesh;

	private int interpolators = 6;

	private int[] shaderPropsID;

	private Vector4[] shaderProps;

	private int curProp;

	private float curTime;

	public GameObject shield;

	[Header("Collision events:")]
	public bool CollisionEnter;

	[Header("Shield settings:")]
	public float DecaySpeed = 2f;

	public float ReactSpeed = 0.1f;

	public bool FixNonUniformScale;

	private void Start()
	{
		mat = shield.GetComponent<Renderer>().material;
		mesh = shield.GetComponent<MeshFilter>();
		shaderPropsID = new int[interpolators];
		for (int i = 0; i < interpolators; i++)
		{
			shaderPropsID[i] = Shader.PropertyToID("_Pos_" + i);
		}
		shaderProps = new Vector4[interpolators];
	}

	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (CollisionEnter)
		{
			ContactPoint[] contacts = collisionInfo.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				OnHit(contactPoint.point);
			}
		}
	}

	public void OnHit(Vector3 hitPoint, float hitAlpha = 1f)
	{
		if (curTime >= ReactSpeed)
		{
			Vector4 vector = mesh.transform.InverseTransformPoint(hitPoint);
			vector.w = Mathf.Clamp(hitAlpha, 0f, 1f);
			shaderProps[curProp] = vector;
			if (FixNonUniformScale && (!Mathf.Approximately(transform.lossyScale.x, transform.lossyScale.y) || !Mathf.Approximately(transform.lossyScale.y, transform.lossyScale.z) || !Mathf.Approximately(transform.lossyScale.y, transform.lossyScale.z)))
			{
				shaderProps[curProp].x *= transform.lossyScale.x;
				shaderProps[curProp].y *= transform.lossyScale.y;
				shaderProps[curProp].z *= transform.lossyScale.z;
			}
			curTime = 0f;
			curProp++;
			if (curProp == interpolators)
			{
				curProp = 0;
			}
		}
	}

	private void FadeMask()
	{
		for (int i = 0; i < interpolators; i++)
		{
			if (shaderProps[i].w > 0f)
			{
				shaderProps[i].w = Mathf.Lerp(shaderProps[i].w, -0.0001f, Time.deltaTime * DecaySpeed);
				shaderProps[i].w = Mathf.Clamp(shaderProps[i].w, 0f, 1f);
				mat.SetVector(shaderPropsID[i], shaderProps[i]);
			}
		}
	}

	private void Update()
	{
		curTime += Time.deltaTime;
		FadeMask();
	}
}
