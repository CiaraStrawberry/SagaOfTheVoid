using Apex.WorldGeometry;
using UnityEngine;

namespace Apex.Debugging;

[AddComponentMenu("Apex/Game World/Debugging/Clearance Visualizer", 1011)]
[ApexComponent("Debugging")]
public sealed class ClearanceVisualizer : MonoBehaviour
{
	[Tooltip("The color of the clearance text.")]
	public Color textColor = new Color(200f / 255f, 200f / 255f, 200f / 255f);

	[Tooltip("The font size of the clearance text.")]
	public int fontSize = 10;

	private GUIStyle _style;

	private void Start()
	{
	}

	private void OnGUI()
	{
		if (Camera.current == null || !Application.isPlaying)
		{
			return;
		}
		if (_style == null)
		{
			_style = new GUIStyle(GUI.skin.label)
			{
				fontSize = fontSize
			};
		}
		GridComponent[] array = Object.FindObjectsOfType<GridComponent>();
		if (array == null)
		{
			return;
		}
		GridComponent[] array2 = array;
		foreach (GridComponent gridComponent in array2)
		{
			IGrid grid = gridComponent.grid;
			if (grid == null)
			{
				continue;
			}
			CellMatrix cellMatrix = grid.cellMatrix;
			for (int j = 0; j < grid.sizeX; j++)
			{
				for (int k = 0; k < grid.sizeZ; k++)
				{
					if (cellMatrix[j, k] is IHaveClearance { clearance: not (<=0f) } haveClearance)
					{
						Vector3 vector = Camera.current.WorldToScreenPoint(cellMatrix[j, k].position);
						vector.y = (float)Screen.height - vector.y;
						GUI.color = textColor;
						GUI.Label(new Rect(vector.x - 5f, vector.y - 10f, 50f, 20f), haveClearance.clearance.ToString(), _style);
					}
				}
			}
		}
	}
}
