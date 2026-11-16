using System.Collections.Generic;

namespace TrueSync.Physics2D;

public sealed class BuoyancyController : Controller
{
	public FP AngularDragCoefficient;

	public FP Density;

	public FP LinearDragCoefficient;

	public TSVector2 Velocity;

	private AABB _container;

	private TSVector2 _gravity;

	private TSVector2 _normal;

	private FP _offset;

	private Dictionary<int, Body> _uniqueBodies = new Dictionary<int, Body>();

	public AABB Container
	{
		get
		{
			return _container;
		}
		set
		{
			_container = value;
			_offset = _container.UpperBound.y;
		}
	}

	public BuoyancyController(AABB container, FP density, FP linearDragCoefficient, FP rotationalDragCoefficient, TSVector2 gravity)
		: base(ControllerType.BuoyancyController)
	{
		Container = container;
		_normal = new TSVector2(0, 1);
		Density = density;
		LinearDragCoefficient = linearDragCoefficient;
		AngularDragCoefficient = rotationalDragCoefficient;
		_gravity = gravity;
	}

	public override void Update(FP dt)
	{
		_uniqueBodies.Clear();
		World.QueryAABB((Fixture fixture2) =>
		{
			if (fixture2.Body.IsStatic || !fixture2.Body.Awake)
			{
				return true;
			}
			if (!_uniqueBodies.ContainsKey(fixture2.Body.BodyId))
			{
				_uniqueBodies.Add(fixture2.Body.BodyId, fixture2.Body);
			}
			return true;
		}, ref _container);
		foreach (KeyValuePair<int, Body> uniqueBody in _uniqueBodies)
		{
			Body value = uniqueBody.Value;
			TSVector2 zero = TSVector2.zero;
			TSVector2 zero2 = TSVector2.zero;
			FP fP = 0;
			FP fP2 = 0;
			for (int num = 0; num < value.FixtureList.Count; num++)
			{
				Fixture fixture = value.FixtureList[num];
				if (fixture.Shape.ShapeType == ShapeType.Polygon || fixture.Shape.ShapeType == ShapeType.Circle)
				{
					Shape shape = fixture.Shape;
					FP fP3 = shape.ComputeSubmergedArea(ref _normal, _offset, ref value._xf, out var sc);
					fP += fP3;
					zero.x += fP3 * sc.x;
					zero.y += fP3 * sc.y;
					fP2 += fP3 * shape.Density;
					zero2.x += fP3 * sc.x * shape.Density;
					zero2.y += fP3 * sc.y * shape.Density;
				}
			}
			zero.x /= fP;
			zero.y /= fP;
			zero2.x /= fP2;
			zero2.y /= fP2;
			if (!(fP < Settings.Epsilon))
			{
				TSVector2 force = -Density * fP * _gravity;
				value.ApplyForce(force, zero2);
				TSVector2 force2 = value.GetLinearVelocityFromWorldPoint(zero) - Velocity;
				force2 *= -LinearDragCoefficient * fP;
				value.ApplyForce(force2, zero);
				value.ApplyTorque(-value.Inertia / value.Mass * fP * value.AngularVelocity * AngularDragCoefficient);
			}
		}
	}
}
