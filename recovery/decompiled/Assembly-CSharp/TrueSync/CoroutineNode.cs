using System.Collections;

namespace TrueSync;

public class CoroutineNode
{
	public CoroutineNode listPrevious;

	public CoroutineNode listNext;

	public IEnumerator fiber;

	public bool finished;

	public int waitForFrame = -1;

	public FP waitForTime = -1f;

	public CoroutineNode waitForCoroutine;

	public int playerId = -1;

	public CoroutineNode(IEnumerator _fiber)
	{
		fiber = _fiber;
		if (TrueSyncInput.CurrentSimulationData != null)
		{
			playerId = TrueSyncInput.CurrentSimulationData.ownerID;
		}
	}
}
