using UnityEngine;

namespace Apex.Debugging;

[ExecuteInEditMode]
[AddComponentMenu("Apex/Game World/Debugging/Portal Visualizer", 1015)]
[ApexComponent("Debugging")]
public sealed class PortalVisualizer : MonoBehaviour
{
	[Tooltip("Whether to always draw the visualization gizmos for all portals in the scene.\n\nIf checked all portals are drawn regardless of their individual setting.\n\nIf unchecked each portal's individual setting is used.")]
	public bool drawAllPortals = true;

	private static PortalVisualizer _instance;

	private static bool _initialized;

	public static PortalVisualizer instance
	{
		get
		{
			if (!_initialized && _instance == null)
			{
				_initialized = true;
				_instance = Object.FindObjectOfType<PortalVisualizer>();
			}
			return _instance;
		}
	}

	private void Awake()
	{
		if (_instance != null)
		{
			Debug.Log("A Portal Visualizer already exists in the scene!");
			if (Application.isPlaying)
			{
				Object.Destroy(this);
			}
			else
			{
				Object.DestroyImmediate(this);
			}
		}
		else
		{
			_initialized = true;
			_instance = this;
		}
	}
}
