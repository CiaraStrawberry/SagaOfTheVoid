using UnityEngine;

public class AssetPreLoadingScript : MonoBehaviour
{
	private int updates;

	private void Start()
	{
	}

	private void Update()
	{
		if (updates < 30)
		{
			updates++;
			if (updates == 2)
			{
				Object.Destroy(gameObject);
			}
		}
	}
}
