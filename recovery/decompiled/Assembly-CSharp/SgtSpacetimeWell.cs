using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Spacetime Well")]
public class SgtSpacetimeWell : MonoBehaviour
{
	public enum DistributionType
	{
		Gaussian,
		Ripple,
		Twist
	}

	public static List<SgtSpacetimeWell> AllWells = new List<SgtSpacetimeWell>();

	[Tooltip("The method used to deform the spacetime")]
	public DistributionType Distribution;

	[Tooltip("The radius of this spacetime well")]
	public float Radius = 1f;

	[Tooltip("The frequency of the ripple")]
	public float Frequency = 1f;

	[Tooltip("The minimum strength of the well")]
	public float Strength = 1f;

	[Tooltip("The frequency offset")]
	public float Offset;

	[Tooltip("The frequency offset speed per second")]
	public float OffsetSpeed;

	[Tooltip("The size of the twist hole")]
	[Range(0f, 0.9f)]
	public float HoleSize;

	[Tooltip("The power of the twist hole")]
	public float HolePower = 10f;

	public static SgtSpacetimeWell Create(SgtSpacetime spacetime)
	{
		if (spacetime != null)
		{
			GameObject gameObject = SgtHelper.CreateGameObject("Well", spacetime.gameObject.layer, spacetime.transform);
			SgtSpacetimeWell sgtSpacetimeWell = gameObject.AddComponent<SgtSpacetimeWell>();
			spacetime.AddWell(sgtSpacetimeWell);
			return sgtSpacetimeWell;
		}
		return null;
	}

	protected virtual void OnEnable()
	{
		AllWells.Add(this);
	}

	protected virtual void OnDisable()
	{
		AllWells.Remove(this);
	}

	protected virtual void Update()
	{
		Offset += OffsetSpeed * Time.deltaTime;
	}
}
