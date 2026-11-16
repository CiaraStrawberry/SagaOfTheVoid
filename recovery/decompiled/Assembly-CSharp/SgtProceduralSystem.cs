using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Procedural System")]
public class SgtProceduralSystem : MonoBehaviour
{
	[Tooltip("The sphere mesh used by all stars, planets, etc")]
	public Mesh SphereMesh;

	[Tooltip("The lightshining on all stars, planets, etc")]
	public Light MainLight;

	[Tooltip("The star surface materials")]
	public List<Material> StarMaterials = new List<Material>();

	[Tooltip("The planet surface materials")]
	public List<Material> PlanetMaterials = new List<Material>();

	[Tooltip("The moon surface materials")]
	public List<Material> MoonMaterials = new List<Material>();

	[Tooltip("The gas giant surface cubemap textures")]
	public List<Cubemap> JovianTextures = new List<Cubemap>();

	public List<GameObject> generatedGameObjects = new List<GameObject>();

	[ContextMenu("Clear")]
	public void Clear()
	{
		for (int num = generatedGameObjects.Count - 1; num >= 0; num--)
		{
			SgtHelper.Destroy(generatedGameObjects[num]);
		}
		generatedGameObjects.Clear();
	}

	[ContextMenu("Add Star")]
	public void AddStar()
	{
		GameObject gameObject = AddBasicGameObject("Star", transform, 0f, 0f, 0f, 10f, 0.5f, 1f);
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		SgtAtmosphere sgtAtmosphere = gameObject.AddComponent<SgtAtmosphere>();
		meshFilter.sharedMesh = SphereMesh;
		meshRenderer.sharedMaterial = GetRandomElement(StarMaterials);
		sgtAtmosphere.InnerRenderers = new List<MeshRenderer>();
		sgtAtmosphere.InnerRenderers.Add(meshRenderer);
		sgtAtmosphere.OuterMeshes = new List<Mesh>();
		sgtAtmosphere.OuterMeshes.Add(SphereMesh);
	}

	[ContextMenu("Add Planet")]
	public void AddPlanet()
	{
		GameObject gameObject = AddBasicGameObject("Planet", transform, 5f, 30f, -5f, 10f, 0.5f, 1f);
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		SgtAtmosphere sgtAtmosphere = gameObject.AddComponent<SgtAtmosphere>();
		meshFilter.sharedMesh = SphereMesh;
		meshRenderer.sharedMaterial = GetRandomElement(PlanetMaterials);
		sgtAtmosphere.Lights = new List<Light>();
		sgtAtmosphere.Lights.Add(MainLight);
		sgtAtmosphere.OuterMeshes = new List<Mesh>();
		sgtAtmosphere.OuterMeshes.Add(SphereMesh);
		for (int num = Random.Range(0, 2); num >= 0; num--)
		{
			AddMoon(gameObject.transform);
		}
	}

	public void AddMoon(Transform parent)
	{
		GameObject gameObject = AddBasicGameObject("Moon", parent, 1f, 3f, -30f, 30f, 0.05f, 0.2f);
		MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshFilter.sharedMesh = SphereMesh;
		meshRenderer.sharedMaterial = GetRandomElement(MoonMaterials);
	}

	[ContextMenu("Add Jovian")]
	public void AddJovian()
	{
		GameObject gameObject = AddBasicGameObject("Jovian", transform, 5f, 30f, -5f, 10f, 0.5f, 1f);
		SgtJovian sgtJovian = gameObject.AddComponent<SgtJovian>();
		sgtJovian.MainTex = GetRandomElement(JovianTextures);
		sgtJovian.Lights = new List<Light>();
		sgtJovian.Lights.Add(MainLight);
		sgtJovian.Meshes = new List<Mesh>();
		sgtJovian.Meshes.Add(SphereMesh);
	}

	public GameObject AddBasicGameObject(string name, Transform parent, float minOrbitDistance, float maxOrbitDistance, float minRotationSpeed, float maxRotationSpeed, float minScale, float maxScale)
	{
		GameObject gameObject = new GameObject(name);
		SgtSimpleOrbit sgtSimpleOrbit = gameObject.AddComponent<SgtSimpleOrbit>();
		SgtRotate sgtRotate = gameObject.AddComponent<SgtRotate>();
		float num = Random.Range(maxScale, maxScale);
		gameObject.transform.parent = parent;
		gameObject.transform.localScale = new Vector3(num, num, num);
		sgtSimpleOrbit.Angle = Random.Range(0f, 360f);
		sgtSimpleOrbit.Radius = Random.Range(minOrbitDistance, maxOrbitDistance);
		sgtSimpleOrbit.DegreesPerSecond = Random.Range(minRotationSpeed, maxRotationSpeed);
		sgtRotate.DegreesPerSecond = new Vector3(0f, Random.Range(minRotationSpeed, maxRotationSpeed), 0f);
		generatedGameObjects.Add(gameObject);
		return gameObject;
	}

	protected virtual void Awake()
	{
		Clear();
		if (SphereMesh != null)
		{
			AddStar();
			for (int num = Random.Range(1, 6); num >= 0; num--)
			{
				AddPlanet();
			}
			for (int num2 = Random.Range(1, 4); num2 >= 0; num2--)
			{
				AddJovian();
			}
		}
	}

	private T GetRandomElement<T>(List<T> list)
	{
		if (list != null && list.Count > 0)
		{
			int index = Random.Range(0, list.Count - 1);
			return list[index];
		}
		return default;
	}
}
