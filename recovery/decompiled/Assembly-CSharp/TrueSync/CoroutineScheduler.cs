using System;
using System.Collections;

namespace TrueSync;

public class CoroutineScheduler
{
	private CoroutineNode first;

	private FP currentTime;

	private AbstractLockstep lockStep;

	public CoroutineScheduler(AbstractLockstep lockStep)
	{
		this.lockStep = lockStep;
	}

	public CoroutineNode StartCoroutine(IEnumerator fiber)
	{
		if (fiber == null)
		{
			return null;
		}
		CoroutineNode coroutineNode = new CoroutineNode(fiber);
		AddCoroutine(coroutineNode);
		return coroutineNode;
	}

	public void StopAllCoroutines()
	{
		first = null;
	}

	public bool HasCoroutines()
	{
		return first != null;
	}

	public void UpdateAllCoroutines()
	{
		InputDataBase currentSimulationData = TrueSyncInput.CurrentSimulationData;
		UpdateAllCoroutines(lockStep.Ticks, TrueSyncManager.Time);
		TrueSyncInput.CurrentSimulationData = (InputData)currentSimulationData;
	}

	public void UpdateAllCoroutines(int frame, FP time)
	{
		currentTime = time;
		CoroutineNode coroutineNode = first;
		while (coroutineNode != null)
		{
			CoroutineNode listNext = coroutineNode.listNext;
			if (coroutineNode.waitForFrame > 0 && frame >= coroutineNode.waitForFrame)
			{
				coroutineNode.waitForFrame = -1;
				UpdateCoroutine(coroutineNode);
			}
			else if (coroutineNode.waitForTime > 0f && time >= coroutineNode.waitForTime)
			{
				coroutineNode.waitForTime = -1f;
				UpdateCoroutine(coroutineNode);
			}
			else if (coroutineNode.waitForCoroutine != null && coroutineNode.waitForCoroutine.finished)
			{
				coroutineNode.waitForCoroutine = null;
				UpdateCoroutine(coroutineNode);
			}
			else if (coroutineNode.waitForFrame == -1 && coroutineNode.waitForTime == -1f && coroutineNode.waitForCoroutine == null)
			{
				UpdateCoroutine(coroutineNode);
			}
			coroutineNode = listNext;
		}
	}

	private void UpdateCoroutine(CoroutineNode coroutine)
	{
		IEnumerator fiber = coroutine.fiber;
		if (coroutine.playerId > -1)
		{
			TrueSyncInput.CurrentSimulationData = (InputData)lockStep.GetInputData(coroutine.playerId);
		}
		if (coroutine.fiber.MoveNext())
		{
			object obj = ((fiber.Current != null) ? fiber.Current : ((object)1));
			if (obj.GetType() == typeof(int))
			{
				coroutine.waitForTime = (int)obj;
				coroutine.waitForTime += currentTime;
				return;
			}
			if (obj.GetType() == typeof(float))
			{
				coroutine.waitForTime = (float)obj;
				coroutine.waitForTime += currentTime;
				return;
			}
			if (obj.GetType() == typeof(FP))
			{
				coroutine.waitForTime = (FP)obj;
				coroutine.waitForTime += currentTime;
				return;
			}
			if (obj.GetType() != typeof(CoroutineNode))
			{
				throw new ArgumentException("CoroutineScheduler: Unexpected coroutine yield type: " + obj.GetType());
			}
			coroutine.waitForCoroutine = (CoroutineNode)obj;
		}
		else
		{
			coroutine.finished = true;
			RemoveCoroutine(coroutine);
		}
	}

	private void AddCoroutine(CoroutineNode coroutine)
	{
		if (first != null)
		{
			coroutine.listNext = first;
			first.listPrevious = coroutine;
		}
		first = coroutine;
	}

	private void RemoveCoroutine(CoroutineNode coroutine)
	{
		if (first == coroutine)
		{
			first = coroutine.listNext;
		}
		else if (coroutine.listNext != null)
		{
			coroutine.listPrevious.listNext = coroutine.listNext;
			coroutine.listNext.listPrevious = coroutine.listPrevious;
		}
		else if (coroutine.listPrevious != null)
		{
			coroutine.listPrevious.listNext = null;
		}
		coroutine.listPrevious = null;
		coroutine.listNext = null;
	}
}
