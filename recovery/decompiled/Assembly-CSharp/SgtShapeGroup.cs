using System.Collections.Generic;
using UnityEngine;

public class SgtShapeGroup : MonoBehaviour
{
	[Tooltip("The shapes associated with this group")]
	public List<SgtShape> Shapes;

	public float GetDensity(Vector3 worldPosition)
	{
		float num = 0f;
		if (Shapes != null)
		{
			for (int num2 = Shapes.Count - 1; num2 >= 0; num2--)
			{
				SgtShape sgtShape = Shapes[num2];
				if (sgtShape != null)
				{
					float density = sgtShape.GetDensity(worldPosition);
					if (density > num)
					{
						num = density;
					}
				}
			}
		}
		return num;
	}
}
