using UnityEngine;

namespace VRTK;

[SDK_Description(typeof(SDK_SimSystem), 0)]
public class SDK_SimBoundaries : SDK_BaseBoundaries
{
	private Transform area;

	public override void InitBoundaries()
	{
	}

	public override Transform GetPlayArea()
	{
		return area;
	}

	public override Vector3[] GetPlayAreaVertices()
	{
		float num = 0.9f;
		float num2 = 1f;
		return new Vector3[8]
		{
			new Vector3(num, 0f, 0f - num),
			new Vector3(0f - num, 0f, 0f - num),
			new Vector3(0f - num, 0f, num),
			new Vector3(num, 0f, num),
			new Vector3(num2, 0f, 0f - num2),
			new Vector3(0f - num2, 0f, 0f - num2),
			new Vector3(0f - num2, 0f, num2),
			new Vector3(num2, 0f, num2)
		};
	}

	public override float GetPlayAreaBorderThickness()
	{
		return 0.1f;
	}

	public override bool IsPlayAreaSizeCalibrated()
	{
		return true;
	}

	public override bool GetDrawAtRuntime()
	{
		return false;
	}

	public override void SetDrawAtRuntime(bool value)
	{
	}
}
