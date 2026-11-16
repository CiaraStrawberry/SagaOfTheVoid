using UnityEngine;

namespace Apex.WorldGeometry;

[AddComponentMenu("")]
public class GridAwareComponent : MonoBehaviour, IGridSource
{
	public IGrid grid { get; private set; }

	private void Start()
	{
		grid = GridManager.instance.GetGrid(transform.position);
	}
}
