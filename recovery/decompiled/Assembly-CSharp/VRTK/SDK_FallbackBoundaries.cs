using UnityEngine;

namespace VRTK;

[SDK_Description(typeof(SDK_FallbackSystem), 0)]
public class SDK_FallbackBoundaries : SDK_BaseBoundaries
{
	public override void InitBoundaries()
	{
	}

	public override Transform GetPlayArea()
	{
		return null;
	}

	public override Vector3[] GetPlayAreaVertices()
	{
		return null;
	}

	public override float GetPlayAreaBorderThickness()
	{
		return 0f;
	}

	public override bool IsPlayAreaSizeCalibrated()
	{
		return false;
	}

	public override bool GetDrawAtRuntime()
	{
		return false;
	}

	public override void SetDrawAtRuntime(bool value)
	{
	}
}
