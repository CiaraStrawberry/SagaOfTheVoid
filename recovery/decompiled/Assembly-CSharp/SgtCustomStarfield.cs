using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[AddComponentMenu("Space Graphics Toolkit/SGT Custom Starfield")]
public class SgtCustomStarfield : SgtPointStarfield
{
	[Tooltip("The stars that will be rendered by this starfield")]
	public List<SgtPointStar> Stars;

	public static SgtCustomStarfield CreateCustomStarfield(int layer = 0, Transform parent = null)
	{
		return CreateCustomStarfield(layer, parent, Vector3.zero, Quaternion.identity, Vector3.one);
	}

	public static SgtCustomStarfield CreateCustomStarfield(int layer, Transform parent, Vector3 localPosition, Quaternion localRotation, Vector3 localScale)
	{
		GameObject gameObject = SgtHelper.CreateGameObject("Custom Starfield", layer, parent, localPosition, localRotation, localScale);
		return gameObject.AddComponent<SgtCustomStarfield>();
	}

	protected override void OnDestroy()
	{
		base.OnDestroy();
		if (Stars != null)
		{
			for (int num = Stars.Count - 1; num >= 0; num--)
			{
				SgtClassPool<SgtPointStar>.Add(Stars[num]);
			}
		}
	}

	protected override int BeginQuads()
	{
		if (Stars != null)
		{
			return Stars.Count;
		}
		return 0;
	}

	protected override void NextQuad(ref SgtPointStar quad, int starIndex)
	{
		quad.CopyFrom(Stars[starIndex]);
	}

	protected override void EndQuads()
	{
	}
}
