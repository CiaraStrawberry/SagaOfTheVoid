namespace TrueSync.Physics2D;

internal class ContactClone2D
{
	public Contact.ContactType _type;

	public int _toiCount;

	public FP _toi;

	public FP Friction;

	public FP Restitution;

	public Manifold Manifold = default;

	public FP TangentSpeed;

	public bool Enabled;

	public Fixture FixtureA;

	public Fixture FixtureB;

	public ContactEdgeClone2D _nodeA = new ContactEdgeClone2D();

	public ContactEdgeClone2D _nodeB = new ContactEdgeClone2D();

	public int ChildIndexA;

	public int ChildIndexB;

	public bool IsTouching;

	public bool IslandFlag;

	public bool TOIFlag;

	public bool FilterFlag;

	public ContactCloneKey Key => new ContactCloneKey(FixtureA.FixtureId, FixtureB.FixtureId);

	public void Clone(Contact contact)
	{
		_type = contact._type;
		_toiCount = contact._toiCount;
		_toi = contact._toi;
		Friction = contact.Friction;
		Restitution = contact.Restitution;
		TangentSpeed = contact.TangentSpeed;
		Enabled = contact.Enabled;
		ChildIndexA = contact.ChildIndexA;
		ChildIndexB = contact.ChildIndexB;
		IsTouching = contact.IsTouching;
		IslandFlag = contact.IslandFlag;
		TOIFlag = contact.TOIFlag;
		FilterFlag = contact.FilterFlag;
		FixtureA = contact.FixtureA;
		FixtureB = contact.FixtureB;
		_nodeA.Clone(contact._nodeA);
		_nodeB.Clone(contact._nodeB);
		Manifold.LocalNormal = contact.Manifold.LocalNormal;
		Manifold.LocalPoint = contact.Manifold.LocalPoint;
		Manifold.PointCount = contact.Manifold.PointCount;
		for (int i = 0; i < contact.Manifold.PointCount; i++)
		{
			Manifold.Points[i] = contact.Manifold.Points[i];
		}
		Manifold.Type = contact.Manifold.Type;
	}

	public void Restore(Contact contact)
	{
		contact._type = _type;
		contact._toiCount = _toiCount;
		contact._toi = _toi;
		contact.Friction = Friction;
		contact.Restitution = Restitution;
		contact.TangentSpeed = TangentSpeed;
		contact.Enabled = Enabled;
		contact.ChildIndexA = ChildIndexA;
		contact.ChildIndexB = ChildIndexB;
		contact.IsTouching = IsTouching;
		contact.IslandFlag = IslandFlag;
		contact.TOIFlag = TOIFlag;
		contact.FilterFlag = FilterFlag;
		contact.FixtureA = FixtureA;
		contact.FixtureB = FixtureB;
		contact.Manifold.LocalNormal = Manifold.LocalNormal;
		contact.Manifold.LocalPoint = Manifold.LocalPoint;
		contact.Manifold.PointCount = Manifold.PointCount;
		for (int i = 0; i < Manifold.PointCount; i++)
		{
			contact.Manifold.Points[i] = Manifold.Points[i];
		}
		contact.Manifold.Type = Manifold.Type;
	}
}
