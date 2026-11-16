using UnityEngine;

public class SDP01_DerelictStarship : MonoBehaviour
{
	private Renderer[] myRenderers;

	public int RandomMatChangePer = 20;

	public Material[] OriginalMaterials;

	public Material[] AltMaterials;

	private void Start()
	{
		myRenderers = gameObject.GetComponentsInChildren<Renderer>();
		RandomizeMaterials();
	}

	private void RandomizeMaterials()
	{
		if (myRenderers.Length <= 0)
		{
			return;
		}
		for (int i = 0; i < myRenderers.Length; i++)
		{
			if (Random.Range(0, 100) >= RandomMatChangePer)
			{
				continue;
			}
			for (int j = 0; j < OriginalMaterials.Length; j++)
			{
				if (myRenderers[i].sharedMaterial == OriginalMaterials[j])
				{
					Debug.Log("Changing Material.");
					myRenderers[i].sharedMaterial = AltMaterials[j];
				}
			}
		}
	}
}
