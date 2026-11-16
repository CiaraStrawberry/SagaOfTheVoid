using UnityEngine;

namespace Apex.PathFinding;

public abstract class RequestPreProcessorBase : MonoBehaviour, IRequestPreProcessor
{
	[SerializeField]
	private int _priority = 1;

	public int priority
	{
		get
		{
			return _priority;
		}
		set
		{
			_priority = value;
		}
	}

	public abstract bool PreProcess(IPathRequest request);
}
