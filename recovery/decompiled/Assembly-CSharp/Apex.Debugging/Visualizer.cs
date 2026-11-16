using UnityEngine;

namespace Apex.Debugging;

public abstract class Visualizer : MonoBehaviour
{
	[Tooltip("Whether to always draw the visualization gizmos, even when the gameobject is not selected.")]
	public bool drawAlways;

	public void Refresh()
	{
		DrawVisualization();
	}

	protected abstract void DrawVisualization();

	protected virtual void Start()
	{
	}

	private void OnDrawGizmos()
	{
		if (drawAlways && enabled)
		{
			DrawVisualization();
		}
	}

	private void OnDrawGizmosSelected()
	{
		if (!drawAlways && enabled)
		{
			DrawVisualization();
		}
	}
}
