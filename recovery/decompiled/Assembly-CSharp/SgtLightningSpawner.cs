using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Lightning Spawner")]
public class SgtLightningSpawner : MonoBehaviour
{
	public List<SgtLightningSpawner> AllLightningSpawner = new List<SgtLightningSpawner>();

	[Tooltip("The minimum delay between lightning spawns")]
	public float DelayMin = 0.25f;

	[Tooltip("The maximum delay between lightning spawns")]
	public float DelayMax = 5f;

	[Tooltip("The minimum life of each spawned lightning")]
	public float LifeMin = 0.5f;

	[Tooltip("The maximum life of each spawned lightning")]
	public float LifeMax = 1f;

	[Tooltip("The radius of the spawned lightning mesh in local coordinates")]
	public float Radius = 1f;

	[Tooltip("The size of the lightning in degrees")]
	public float Size = 10f;

	[Tooltip("The amount of rows and columns in the lightning mesh")]
	[Range(1f, 100f)]
	public int Detail = 10;

	[Tooltip("The random color of the lightning")]
	public Gradient Colors;

	[Tooltip("The brightness of the lightning")]
	public float Brightness = 1f;

	[Tooltip("The random sprite used by the lightning")]
	public List<Sprite> Sprites;

	[NonSerialized]
	public Mesh Mesh;

	[NonSerialized]
	private float cooldown;

	[SerializeField]
	[HideInInspector]
	private bool startCalled;

	[NonSerialized]
	private bool updateMeshCalled;

	private static GradientColorKey[] defaultColors = new GradientColorKey[2]
	{
		new GradientColorKey(Color.white, 0.4f),
		new GradientColorKey(Color.cyan, 0.6f)
	};

	public Sprite RandomSprite
	{
		get
		{
			if (Sprites != null)
			{
				int count = Sprites.Count;
				if (count > 0)
				{
					int index = UnityEngine.Random.Range(0, count);
					return Sprites[index];
				}
			}
			return null;
		}
	}

	public Color RandomColor
	{
		get
		{
			if (Colors != null)
			{
				return Colors.Evaluate(UnityEngine.Random.value);
			}
			return Color.white;
		}
	}

	[ContextMenu("Update Mesh")]
	public void UpdateMesh()
	{
		if (Mesh == null)
		{
			Mesh = SgtHelper.CreateTempMesh("Lightning");
		}
		else
		{
			Mesh.Clear(keepVertexLayout: false);
		}
		int num = Detail + 1;
		Vector3[] array = new Vector3[num * num];
		Vector2[] array2 = new Vector2[num * num];
		int[] array3 = new int[Detail * Detail * 6];
		float num2 = SgtHelper.Reciprocal(Detail);
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num; j++)
			{
				int num3 = j + i * num;
				float num4 = (float)j * num2;
				float num5 = (float)i * num2;
				float x = (num4 - 0.5f) * Size;
				float y = (num5 - 0.5f) * Size;
				ref Vector3 reference = ref array[num3];
				reference = Quaternion.Euler(x, y, 0f) * new Vector3(0f, 0f, Radius);
				ref Vector2 reference2 = ref array2[num3];
				reference2 = new Vector2(num4, num5);
			}
		}
		for (int k = 0; k < Detail; k++)
		{
			for (int l = 0; l < Detail; l++)
			{
				int num6 = (l + k * Detail) * 6;
				int num7 = (array3[num6] = l + k * num);
				array3[num6 + 1] = num7 + 1;
				array3[num6 + 2] = num7 + num;
				array3[num6 + 3] = num7 + num + 1;
				array3[num6 + 4] = num7 + num;
				array3[num6 + 5] = num7 + 1;
			}
		}
		Mesh.vertices = array;
		Mesh.uv = array2;
		Mesh.triangles = array3;
	}

	public SgtLightning Spawn()
	{
		if (Mesh != null && LifeMin > 0f && LifeMax > 0f)
		{
			Sprite randomSprite = RandomSprite;
			if (randomSprite != null)
			{
				SgtLightning sgtLightning = SgtLightning.Create(this);
				Material material = sgtLightning.Material;
				Vector4 vector = SgtHelper.CalculateSpriteUV(randomSprite);
				if (material == null)
				{
					material = SgtHelper.CreateTempMaterial("Lightning (Generated)", "Hidden/SgtLightning");
					sgtLightning.SetMaterial(material);
				}
				sgtLightning.Life = UnityEngine.Random.Range(LifeMin, LifeMax);
				sgtLightning.Age = 0f;
				sgtLightning.SetMesh(Mesh);
				material.SetTexture("_MainTex", randomSprite.texture);
				material.SetColor("_Color", SgtHelper.Brighten(RandomColor, Brightness));
				material.SetFloat("_Age", 0f);
				material.SetVector("_Offset", new Vector2(vector.x, vector.y));
				material.SetVector("_Scale", new Vector2(vector.z - vector.x, vector.w - vector.y));
				sgtLightning.transform.localRotation = UnityEngine.Random.rotation;
				return sgtLightning;
			}
		}
		return null;
	}

	public static SgtLightningSpawner CreateLightningSpawner(int layer = 0, Transform parent = null)
	{
		return CreateLightningSpawner(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtLightningSpawner CreateLightningSpawner(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Lightning Spawner", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtLightningSpawner>();
	}

	protected virtual void Awake()
	{
		ResetDelay();
	}

	protected virtual void OnEnable()
	{
		if (startCalled)
		{
			CheckUpdateCalls();
		}
	}

	protected virtual void Start()
	{
		if (!startCalled)
		{
			startCalled = true;
			if (Colors == null)
			{
				Colors = new Gradient();
				Colors.colorKeys = defaultColors;
			}
			CheckUpdateCalls();
		}
	}

	protected virtual void Update()
	{
		cooldown -= Time.deltaTime;
		if (cooldown <= 0f)
		{
			ResetDelay();
			Spawn();
		}
	}

	protected virtual void OnDestroy()
	{
		if (Mesh != null)
		{
			Mesh.Clear(keepVertexLayout: false);
			SgtObjectPool<Mesh>.Add(Mesh);
		}
	}

	private void ResetDelay()
	{
		cooldown = UnityEngine.Random.Range(DelayMin, DelayMax);
	}

	private void CheckUpdateCalls()
	{
		if (!updateMeshCalled)
		{
			UpdateMesh();
		}
	}
}
