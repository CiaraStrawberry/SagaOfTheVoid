using UnityEngine;

namespace Forge3D;

[AddComponentMenu("FORGE3D/Force Field/Force Field")]
public class Forcefield : MonoBehaviour
{
	private new Transform transform;

	private Material[] mat;

	private MeshFilter mesh;

	private int interpolators = 24;

	private int[] shaderPosID;

	private int[] shaderPowID;

	private Vector4[] shaderPos;

	private int curProp;

	private float curTime;

	[Header("Need fix rigidbody:")]
	public bool FixRigidbody;

	[Header("Force Field objects:")]
	public GameObject[] Field;

	[Header("Collision events:")]
	public bool CollisionEnter;

	public bool CollisionStay;

	public bool CollisionExit;

	[Header("Shield settings:")]
	public float DecaySpeed = 2f;

	public float ReactSpeed = 0.1f;

	public bool FixNonUniformScale;

	private void Awake()
	{
		transform = GetComponent<Transform>();
	}

	private void InstallForceField(Forcefield _temp)
	{
		_temp.CollisionEnter = CollisionEnter;
		_temp.CollisionExit = CollisionExit;
		_temp.CollisionStay = CollisionStay;
		_temp.DecaySpeed = DecaySpeed;
		_temp.ReactSpeed = ReactSpeed;
	}

	private void Start()
	{
		if (FixRigidbody && Field.Length > 0)
		{
			GameObject gameObject = new GameObject();
			gameObject.name = base.gameObject.name + "[MeshCollidersFixer]";
			gameObject.transform.parent = null;
			gameObject.transform.position = transform.position;
			gameObject.transform.rotation = transform.rotation;
			MeshColliderChild meshColliderChild = gameObject.gameObject.AddComponent<MeshColliderChild>();
			meshColliderChild.Install(transform);
			for (int i = 0; i < Field.Length; i++)
			{
				if (Field[i] != null)
				{
					if (Field[i].GetComponent<Forcefield>() == null)
					{
						Forcefield temp = Field[i].AddComponent<Forcefield>();
						InstallForceField(temp);
					}
					Field[i].transform.parent = gameObject.transform;
					MeshCollider component = Field[i].GetComponent<MeshCollider>();
					component.enabled = true;
				}
			}
		}
		if (base.gameObject.GetComponent<Renderer>() != null)
		{
			mat = base.gameObject.GetComponent<Renderer>().materials;
		}
		mesh = base.gameObject.GetComponent<MeshFilter>();
		shaderPosID = new int[interpolators];
		shaderPowID = new int[interpolators];
		for (int j = 0; j < interpolators; j++)
		{
			shaderPosID[j] = Shader.PropertyToID("_Pos_" + j);
			shaderPowID[j] = Shader.PropertyToID("_Pow_" + j);
		}
		shaderPos = new Vector4[interpolators];
	}

	public void InstallRuntime()
	{
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

	private void OnCollisionStay(Collision collisionInfo)
	{
		if (CollisionStay)
		{
			ContactPoint[] contacts = collisionInfo.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				OnHit(contactPoint.point);
			}
		}
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		if (CollisionExit)
		{
			ContactPoint[] contacts = collisionInfo.contacts;
			foreach (ContactPoint contactPoint in contacts)
			{
				OnHit(contactPoint.point);
			}
		}
	}

	public void OnHit(Vector3 hitPoint, float hitPower = 0f, float hitAlpha = 1f)
	{
		if (curTime >= ReactSpeed)
		{
			Vector4 vector = mesh.transform.InverseTransformPoint(hitPoint);
			vector.w = Mathf.Clamp(hitAlpha, 0f, 1f);
			shaderPos[curProp] = vector;
			if (FixNonUniformScale && (!Mathf.Approximately(transform.lossyScale.x, transform.lossyScale.y) || !Mathf.Approximately(transform.lossyScale.y, transform.lossyScale.z) || !Mathf.Approximately(transform.lossyScale.y, transform.lossyScale.z)))
			{
				shaderPos[curProp].x *= transform.lossyScale.x;
				shaderPos[curProp].y *= transform.lossyScale.y;
				shaderPos[curProp].z *= transform.lossyScale.z;
			}
			for (int i = 0; i < mat.Length; i++)
			{
				mat[i].SetFloat(shaderPowID[curProp], hitPower);
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
			if (shaderPos[i].w > 0f)
			{
				shaderPos[i].w = Mathf.Lerp(shaderPos[i].w, -0.0001f, Time.deltaTime * DecaySpeed);
				shaderPos[i].w = Mathf.Clamp(shaderPos[i].w, 0f, 1f);
				for (int j = 0; j < mat.Length; j++)
				{
					mat[j].SetVector(shaderPosID[i], shaderPos[i]);
				}
			}
		}
	}

	private void Update()
	{
		curTime += Time.deltaTime;
		FadeMask();
	}
}
