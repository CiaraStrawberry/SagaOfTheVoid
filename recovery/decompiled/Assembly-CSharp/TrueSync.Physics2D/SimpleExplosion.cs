using System;
using System.Collections.Generic;

namespace TrueSync.Physics2D;

public sealed class SimpleExplosion : PhysicsLogic
{
	public FP Power { get; set; }

	public SimpleExplosion(World world)
		: base(world, PhysicsLogicType.Explosion)
	{
		Power = 1;
	}

	public Dictionary<Body, TSVector2> Activate(TSVector2 pos, FP radius, FP force, FP maxForce)
	{
		HashSet<Body> affectedBodies = new HashSet<Body>();
		AABB aabb = default;
		aabb.LowerBound = pos - new TSVector2(radius);
		aabb.UpperBound = pos + new TSVector2(radius);
		World.QueryAABB((Fixture fixture) =>
		{
			if (TSVector2.Distance(fixture.Body.Position, pos) <= radius && !affectedBodies.Contains(fixture.Body))
			{
				affectedBodies.Add(fixture.Body);
			}
			return true;
		}, ref aabb);
		return ApplyImpulse(pos, radius, force, maxForce, affectedBodies);
	}

	private Dictionary<Body, TSVector2> ApplyImpulse(TSVector2 pos, FP radius, FP force, FP maxForce, HashSet<Body> overlappingBodies)
	{
		Dictionary<Body, TSVector2> dictionary = new Dictionary<Body, TSVector2>(overlappingBodies.Count);
		foreach (Body overlappingBody in overlappingBodies)
		{
			if (IsActiveOn(overlappingBody))
			{
				FP distance = TSVector2.Distance(pos, overlappingBody.Position);
				FP percent = GetPercent(distance, radius);
				TSVector2 tSVector = pos - overlappingBody.Position;
				tSVector *= 1f / FP.Sqrt(tSVector.x * tSVector.x + tSVector.y * tSVector.y);
				tSVector *= TSMath.Min(force * percent, maxForce);
				tSVector *= (FP)(-1);
				overlappingBody.ApplyLinearImpulse(tSVector);
				dictionary.Add(overlappingBody, tSVector);
			}
		}
		return dictionary;
	}

	private FP GetPercent(FP distance, FP radius)
	{
		FP value = (FP)Math.Pow((1 - (distance - radius) / radius).AsFloat(), Power.AsFloat()) - (FP)1;
		if (FP.IsNaN(value))
		{
			return 0f;
		}
		return TSMath.Clamp(value, 0f, 1f);
	}
}
