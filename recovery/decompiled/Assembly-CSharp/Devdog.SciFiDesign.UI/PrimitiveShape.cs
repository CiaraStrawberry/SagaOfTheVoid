using System;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.SciFiDesign.UI;

[RequireComponent(typeof(RectTransform))]
public class PrimitiveShape : Graphic
{
	public int elementCount = 6;

	public float edgeThickness = 5f;

	protected override void OnPopulateMesh(VertexHelper vh)
	{
		Rect pixelAdjustedRect = GetPixelAdjustedRect();
		vh.Clear();
		for (int i = 0; i < elementCount * 2; i++)
		{
			float f = (float)i / (float)elementCount * (float)Math.PI * 2f;
			float num = Mathf.Cos(f);
			float num2 = Mathf.Sin(f);
			vh.AddVert(new Vector3((num * edgeThickness + num) * pixelAdjustedRect.x, (num2 * edgeThickness + num2) * pixelAdjustedRect.y), color, Vector2.zero);
			vh.AddVert(new Vector3(num * pixelAdjustedRect.x, num2 * pixelAdjustedRect.y), color, Vector2.zero);
			vh.AddTriangle(i, i + 1, i + 2);
		}
	}
}
