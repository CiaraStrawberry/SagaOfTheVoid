using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class DynamicTreeBroadPhaseClone2D
{
	public int[] _moveBuffer;

	public int _moveCapacity;

	public int _moveCount;

	public List<Pair> _pairBuffer = new List<Pair>();

	public int _pairCapacity;

	public int _pairCount;

	public int _proxyCount;

	public int _queryProxyId;

	public DynamicTreeClone2D dynamicTreeClone = new DynamicTreeClone2D();

	public void Clone(DynamicTreeBroadPhase dynamicTreeBroadPhase)
	{
		_moveBuffer = dynamicTreeBroadPhase._moveBuffer;
		_moveCapacity = dynamicTreeBroadPhase._moveCapacity;
		_moveCount = dynamicTreeBroadPhase._moveCount;
		_pairBuffer.Clear();
		int i = 0;
		for (int num = dynamicTreeBroadPhase._pairBuffer.Length; i < num; i++)
		{
			_pairBuffer.Add(dynamicTreeBroadPhase._pairBuffer[i]);
		}
		_pairCapacity = dynamicTreeBroadPhase._pairCapacity;
		_pairCount = dynamicTreeBroadPhase._pairCount;
		_proxyCount = dynamicTreeBroadPhase._proxyCount;
		_queryProxyId = dynamicTreeBroadPhase._queryProxyId;
		dynamicTreeClone.Clone(dynamicTreeBroadPhase._tree);
	}

	public void Restore(DynamicTreeBroadPhase dynamicTreeBroadPhase)
	{
		dynamicTreeBroadPhase._moveBuffer = _moveBuffer;
		dynamicTreeBroadPhase._moveCapacity = _moveCapacity;
		dynamicTreeBroadPhase._moveCount = _moveCount;
		if (dynamicTreeBroadPhase._pairBuffer.Length != _pairBuffer.Count)
		{
			dynamicTreeBroadPhase._pairBuffer = new Pair[_pairBuffer.Count];
		}
		int i = 0;
		for (int count = _pairBuffer.Count; i < count; i++)
		{
			ref Pair reference = ref dynamicTreeBroadPhase._pairBuffer[i];
			reference = _pairBuffer[i];
		}
		dynamicTreeBroadPhase._pairCapacity = _pairCapacity;
		dynamicTreeBroadPhase._pairCount = _pairCount;
		dynamicTreeBroadPhase._proxyCount = _proxyCount;
		dynamicTreeBroadPhase._queryProxyId = _queryProxyId;
		dynamicTreeClone.Restore(dynamicTreeBroadPhase._tree);
	}
}
