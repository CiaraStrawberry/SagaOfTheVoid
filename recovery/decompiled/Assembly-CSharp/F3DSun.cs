using UnityEngine;

[ExecuteInEditMode]
public class F3DSun : MonoBehaviour
{
	public bool AutoUpdate;

	public F3DPlanet[] Planets;

	public float radius;

	public int PlanetLayer;

	private int sunPosRef;

	private void Awake()
	{
		sunPosRef = Shader.PropertyToID("_SunPos");
	}

	public void UpdatePlanets()
	{
		if (Planets == null || !AutoUpdate)
		{
			return;
		}
		for (int i = 0; i < Planets.Length; i++)
		{
			if (Planets[i] == null)
			{
				Debug.LogWarning("F3DSun : Planet script has been removed from one of the objects in the scene. Please refresh.");
				Planets = null;
				break;
			}
			Renderer[] componentsInChildren = Planets[i].GetComponentsInChildren<Renderer>();
			for (int j = 0; j < componentsInChildren.Length; j++)
			{
				componentsInChildren[j].sharedMaterial.SetVector(sunPosRef, transform.position);
			}
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.55f);
	}

	private void Update()
	{
		UpdatePlanets();
	}
}
