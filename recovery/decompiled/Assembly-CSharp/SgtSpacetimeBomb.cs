using UnityEngine;

[AddComponentMenu("Space Graphics Toolkit/SGT Spacetime Bomb")]
public class SgtSpacetimeBomb : MonoBehaviour
{
	[Tooltip("The Spacetime this bomb belongs to")]
	public SgtSpacetime Spacetime;

	[Tooltip("The well this bomb uses")]
	public SgtSpacetimeWell Well;

	[Tooltip("The maximum radius of this bomb")]
	public float Radius;

	[Tooltip("The speed at which this bomb fades away")]
	public float ShrinkSpeed;

	protected virtual void Update()
	{
		if (Well != null)
		{
			Well.Radius = Radius;
		}
		Radius -= ShrinkSpeed * Time.deltaTime;
		if (Radius <= 0f)
		{
			SgtHelper.Destroy(gameObject);
		}
	}

	protected virtual void Start()
	{
		if (Well != null && Spacetime != null && !Spacetime.Wells.Contains(Well))
		{
			Spacetime.Wells.Add(Well);
		}
	}

	protected virtual void OnDestroy()
	{
		if (Well != null && Spacetime != null)
		{
			Spacetime.Wells.Remove(Well);
		}
	}
}
