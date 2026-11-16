using System.Collections;
using System.Collections.Generic;

namespace TrueSync.Physics3D;

public class ArbiterMap : IEnumerable
{
	private Dictionary<ArbiterKey, Arbiter> dictionaryKeys = new Dictionary<ArbiterKey, Arbiter>(new ArbiterKeyComparer());

	private HashList<ArbiterKey> keysSortedList = new HashList<ArbiterKey>();

	public ArbiterKey lookUpKey;

	public Dictionary<ArbiterKey, Arbiter>.ValueCollection Arbiters => dictionaryKeys.Values;

	public ArbiterMap()
	{
		lookUpKey = new ArbiterKey(null, null);
	}

	public bool LookUpArbiter(RigidBody body1, RigidBody body2, out Arbiter arbiter)
	{
		lookUpKey.SetBodies(body1, body2);
		if (!dictionaryKeys.ContainsKey(lookUpKey))
		{
			arbiter = null;
			return false;
		}
		arbiter = dictionaryKeys[lookUpKey];
		return true;
	}

	internal void Add(ArbiterKey key, Arbiter arbiter)
	{
		keysSortedList.Add(key);
		dictionaryKeys.Add(key, arbiter);
	}

	internal void Clear()
	{
		keysSortedList.Clear();
		dictionaryKeys.Clear();
	}

	internal void Remove(Arbiter arbiter)
	{
		lookUpKey.SetBodies(arbiter.body1, arbiter.body2);
		keysSortedList.Remove(lookUpKey);
		dictionaryKeys.Remove(lookUpKey);
	}

	public bool ContainsArbiter(RigidBody body1, RigidBody body2)
	{
		lookUpKey.SetBodies(body1, body2);
		return dictionaryKeys.ContainsKey(lookUpKey);
	}

	public IEnumerator GetEnumerator()
	{
		return dictionaryKeys.Values.GetEnumerator();
	}
}
