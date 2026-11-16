using UnityEngine;

namespace EasyEditor;

public class ProgressBarAttribute : PropertyAttribute
{
	public float max = 1f;

	public ProgressBarAttribute()
	{
	}

	public ProgressBarAttribute(float max)
	{
		this.max = max;
	}
}
