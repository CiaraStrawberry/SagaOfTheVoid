using UnityEngine;

namespace Apex.Messages;

public class GridStatusMessage
{
	public enum StatusCode
	{
		InitializationComplete,
		DisableComplete
	}

	public Bounds gridBounds { get; set; }

	public StatusCode status { get; set; }
}
