using Apex.PathFinding;
using UnityEngine;

namespace Apex.Steering.Components;

[RequireComponent(typeof(SteerForPathComponent))]
public abstract class SteerForPathResultProcessorComponent : MonoBehaviour, IProcessPathResults
{
	public int processingOrder;

	int IProcessPathResults.processingOrder => processingOrder;

	private void Awake()
	{
		this.WarnIfMultipleInstances();
	}

	public abstract bool HandleResult(PathResult result, SteerForPathComponent steerer);
}
