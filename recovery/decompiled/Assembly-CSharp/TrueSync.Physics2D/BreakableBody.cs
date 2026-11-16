using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public class BreakableBody
{
	private FP[] _angularVelocitiesCache = new FP[8];

	private bool _break;

	private TSVector2[] _velocitiesCache = new TSVector2[8];

	private World _world;

	public bool Broken;

	public Body MainBody;

	public List<Fixture> Parts = new List<Fixture>(8);

	public FP Strength = 500f;

	public BreakableBody(IEnumerable<Vertices> vertices, World world, FP density)
	{
		_world = world;
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Combine(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		MainBody = new Body(_world, null, 0);
		MainBody.BodyType = BodyType.Dynamic;
		foreach (Vertices vertex in vertices)
		{
			PolygonShape shape = new PolygonShape(vertex, density);
			Fixture item = MainBody.CreateFixture(shape);
			Parts.Add(item);
		}
	}

	public BreakableBody(IEnumerable<Shape> shapes, World world)
	{
		_world = world;
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Combine(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		MainBody = new Body(_world, null, 0);
		MainBody.BodyType = BodyType.Dynamic;
		foreach (Shape shape in shapes)
		{
			Fixture item = MainBody.CreateFixture(shape);
			Parts.Add(item);
		}
	}

	private void PostSolve(Contact contact, ContactVelocityConstraint impulse)
	{
		if (!Broken && (Parts.Contains(contact.FixtureA) || Parts.Contains(contact.FixtureB)))
		{
			FP fP = 0f;
			int pointCount = contact.Manifold.PointCount;
			for (int i = 0; i < pointCount; i++)
			{
				fP = TSMath.Max(fP, impulse.points[i].normalImpulse);
			}
			if (fP > Strength)
			{
				_break = true;
			}
		}
	}

	public void Update()
	{
		if (_break)
		{
			Decompose();
			Broken = true;
			_break = false;
		}
		if (!Broken)
		{
			if (Parts.Count > _angularVelocitiesCache.Length)
			{
				_velocitiesCache = new TSVector2[Parts.Count];
				_angularVelocitiesCache = new FP[Parts.Count];
			}
			for (int i = 0; i < Parts.Count; i++)
			{
				ref TSVector2 reference = ref _velocitiesCache[i];
				reference = Parts[i].Body.LinearVelocity;
				ref FP reference2 = ref _angularVelocitiesCache[i];
				reference2 = Parts[i].Body.AngularVelocity;
			}
		}
	}

	private void Decompose()
	{
		ContactManager contactManager = _world.ContactManager;
		contactManager.PostSolve = (PostSolveDelegate)Delegate.Remove(contactManager.PostSolve, new PostSolveDelegate(PostSolve));
		for (int i = 0; i < Parts.Count; i++)
		{
			Fixture fixture = Parts[i];
			Shape shape = fixture.Shape.Clone();
			object userData = fixture.UserData;
			MainBody.DestroyFixture(fixture);
			Body body = BodyFactory.CreateBody(_world);
			body.BodyType = BodyType.Dynamic;
			body.Position = MainBody.Position;
			body.Rotation = MainBody.Rotation;
			body.UserData = MainBody.UserData;
			Fixture fixture2 = body.CreateFixture(shape);
			fixture2.UserData = userData;
			Parts[i] = fixture2;
			body.AngularVelocity = _angularVelocitiesCache[i];
			body.LinearVelocity = _velocitiesCache[i];
		}
		_world.RemoveBody(MainBody);
		_world.RemoveBreakableBody(this);
	}

	public void Break()
	{
		_break = true;
	}
}
