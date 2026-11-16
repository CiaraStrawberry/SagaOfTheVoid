using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Spacetime Bombs")]
public class SgtSpacetimeBombs : MonoBehaviour
{
	[Tooltip("The key required to spawn bombs")]
	public KeyCode Requires = KeyCode.Space;

	[Tooltip("The bomb prefab that will be spawned")]
	public GameObject BombPrefab;

	[Tooltip("The spacetime we want these bombs to effect")]
	public SgtSpacetime Spacetime;

	public void SpawnBomb()
	{
		if (BombPrefab != null && Spacetime != null)
		{
			SgtSpacetimeBomb component = SgtHelper.CloneGameObject(BombPrefab, transform).GetComponent<SgtSpacetimeBomb>();
			Vector3 localPosition = new Vector3(Random.Range(-5f, 5f), 0f, Random.Range(-5f, 5f));
			component.Spacetime = Spacetime;
			component.transform.localPosition = localPosition;
		}
	}

	protected virtual void Update()
	{
		if (Input.GetKeyDown(Requires))
		{
			SpawnBomb();
		}
	}
}
