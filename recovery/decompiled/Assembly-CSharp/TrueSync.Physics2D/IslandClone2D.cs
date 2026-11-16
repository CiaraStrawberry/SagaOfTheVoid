using System.Collections.Generic;

namespace TrueSync.Physics2D;

internal class IslandClone2D
{
	private List<ContactCloneKey> _contactsKeys = new List<ContactCloneKey>(10);

	public List<Body> Bodies = new List<Body>(10);

	public int BodyCount;

	public int ContactCount;

	public int JointCount;

	public List<Velocity> _velocities = new List<Velocity>(10);

	public List<Position> _positions = new List<Position>(10);

	public int BodyCapacity;

	public int ContactCapacity;

	public int JointCapacity;

	public void Clone(Island island)
	{
		Bodies.Clear();
		int i = 0;
		for (int num = island.Bodies.Length; i < num; i++)
		{
			Bodies.Add(island.Bodies[i]);
		}
		_velocities.Clear();
		int j = 0;
		for (int num2 = island._velocities.Length; j < num2; j++)
		{
			_velocities.Add(island._velocities[j]);
		}
		_positions.Clear();
		int k = 0;
		for (int num3 = island._positions.Length; k < num3; k++)
		{
			_positions.Add(island._positions[k]);
		}
		_contactsKeys.Clear();
		for (int l = 0; l < island.ContactCount; l++)
		{
			_contactsKeys.Add(island._contacts[l].Key);
		}
		BodyCount = island.BodyCount;
		ContactCount = island.ContactCount;
		JointCount = island.JointCount;
		BodyCapacity = island.BodyCapacity;
		ContactCapacity = island.ContactCapacity;
		JointCapacity = island.JointCapacity;
	}

	public void Restore(Island island, Dictionary<ContactCloneKey, Contact> contactDic)
	{
		island.Reset(BodyCapacity, ContactCapacity, JointCapacity, island._contactManager);
		int i = 0;
		for (int count = Bodies.Count; i < count; i++)
		{
			island.Bodies[i] = Bodies[i];
		}
		int j = 0;
		for (int count2 = _velocities.Count; j < count2; j++)
		{
			ref Velocity reference = ref island._velocities[j];
			reference = _velocities[j];
		}
		int k = 0;
		for (int count3 = _positions.Count; k < count3; k++)
		{
			ref Position reference2 = ref island._positions[k];
			reference2 = _positions[k];
		}
		for (int l = 0; l < ContactCount; l++)
		{
			island._contacts[l] = contactDic[_contactsKeys[l]];
		}
		island.BodyCount = BodyCount;
		island.ContactCount = ContactCount;
		island.JointCount = JointCount;
	}
}
