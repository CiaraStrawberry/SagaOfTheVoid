namespace TrueSync.Physics3D;

public class Contact : IConstraint
{
	public ContactSettings settings;

	public RigidBody body1;

	public RigidBody body2;

	public TSVector normal;

	public TSVector tangent;

	public TSVector realRelPos1;

	public TSVector realRelPos2;

	public TSVector relativePos1;

	public TSVector relativePos2;

	public TSVector p1;

	public TSVector p2;

	public FP accumulatedNormalImpulse = FP.Zero;

	public FP accumulatedTangentImpulse = FP.Zero;

	public FP penetration = FP.Zero;

	public FP initialPen = FP.Zero;

	public FP staticFriction;

	public FP dynamicFriction;

	public FP restitution;

	public FP friction = FP.Zero;

	public FP massNormal = FP.Zero;

	public FP massTangent = FP.Zero;

	public FP restitutionBias = FP.Zero;

	public bool newContact;

	public bool treatBody1AsStatic;

	public bool treatBody2AsStatic;

	public bool body1IsMassPoint;

	public bool body2IsMassPoint;

	public FP lostSpeculativeBounce = FP.Zero;

	public FP speculativeVelocity = FP.Zero;

	public static readonly ResourcePool<Contact> Pool = new ResourcePool<Contact>();

	public FP lastTimeStep = FP.PositiveInfinity;

	public FP Restitution
	{
		get
		{
			return restitution;
		}
		set
		{
			restitution = value;
		}
	}

	public FP StaticFriction
	{
		get
		{
			return staticFriction;
		}
		set
		{
			staticFriction = value;
		}
	}

	public FP DynamicFriction
	{
		get
		{
			return dynamicFriction;
		}
		set
		{
			dynamicFriction = value;
		}
	}

	public RigidBody Body1 => body1;

	public RigidBody Body2 => body2;

	public FP Penetration => penetration;

	public TSVector Position1 => p1;

	public TSVector Position2 => p2;

	public TSVector Tangent => tangent;

	public TSVector Normal => normal;

	public FP AppliedNormalImpulse => accumulatedNormalImpulse;

	public FP AppliedTangentImpulse => accumulatedTangentImpulse;

	public TSVector CalculateRelativeVelocity()
	{
		FP fP = body2.angularVelocity.y * relativePos2.z - body2.angularVelocity.z * relativePos2.y + body2.linearVelocity.x;
		FP fP2 = body2.angularVelocity.z * relativePos2.x - body2.angularVelocity.x * relativePos2.z + body2.linearVelocity.y;
		FP fP3 = body2.angularVelocity.x * relativePos2.y - body2.angularVelocity.y * relativePos2.x + body2.linearVelocity.z;
		TSVector result = default;
		result.x = fP - body1.angularVelocity.y * relativePos1.z + body1.angularVelocity.z * relativePos1.y - body1.linearVelocity.x;
		result.y = fP2 - body1.angularVelocity.z * relativePos1.x + body1.angularVelocity.x * relativePos1.z - body1.linearVelocity.y;
		result.z = fP3 - body1.angularVelocity.x * relativePos1.y + body1.angularVelocity.y * relativePos1.x - body1.linearVelocity.z;
		return result;
	}

	public void Iterate()
	{
		if (treatBody1AsStatic && treatBody2AsStatic)
		{
			return;
		}
		FP fP = body2.linearVelocity.x - body1.linearVelocity.x;
		FP fP2 = body2.linearVelocity.y - body1.linearVelocity.y;
		FP fP3 = body2.linearVelocity.z - body1.linearVelocity.z;
		if (!body1IsMassPoint)
		{
			fP = fP - body1.angularVelocity.y * relativePos1.z + body1.angularVelocity.z * relativePos1.y;
			fP2 = fP2 - body1.angularVelocity.z * relativePos1.x + body1.angularVelocity.x * relativePos1.z;
			fP3 = fP3 - body1.angularVelocity.x * relativePos1.y + body1.angularVelocity.y * relativePos1.x;
		}
		if (!body2IsMassPoint)
		{
			fP = fP + body2.angularVelocity.y * relativePos2.z - body2.angularVelocity.z * relativePos2.y;
			fP2 = fP2 + body2.angularVelocity.z * relativePos2.x - body2.angularVelocity.x * relativePos2.z;
			fP3 = fP3 + body2.angularVelocity.x * relativePos2.y - body2.angularVelocity.y * relativePos2.x;
		}
		if (fP * fP + fP2 * fP2 + fP3 * fP3 < settings.minVelocity * settings.minVelocity)
		{
			return;
		}
		FP fP4 = normal.x * fP + normal.y * fP2 + normal.z * fP3;
		FP fP5 = massNormal * (-fP4 + restitutionBias + speculativeVelocity);
		FP fP6 = accumulatedNormalImpulse;
		accumulatedNormalImpulse = fP6 + fP5;
		if (accumulatedNormalImpulse < FP.Zero)
		{
			accumulatedNormalImpulse = FP.Zero;
		}
		fP5 = accumulatedNormalImpulse - fP6;
		FP fP7 = fP * tangent.x + fP2 * tangent.y + fP3 * tangent.z;
		FP fP8 = friction * accumulatedNormalImpulse;
		FP fP9 = massTangent * -fP7;
		FP fP10 = accumulatedTangentImpulse;
		accumulatedTangentImpulse = fP10 + fP9;
		if (accumulatedTangentImpulse < -fP8)
		{
			accumulatedTangentImpulse = -fP8;
		}
		else if (accumulatedTangentImpulse > fP8)
		{
			accumulatedTangentImpulse = fP8;
		}
		fP9 = accumulatedTangentImpulse - fP10;
		TSVector tSVector = default;
		tSVector.x = normal.x * fP5 + tangent.x * fP9;
		tSVector.y = normal.y * fP5 + tangent.y * fP9;
		tSVector.z = normal.z * fP5 + tangent.z * fP9;
		if (!treatBody1AsStatic)
		{
			body1.linearVelocity.x -= tSVector.x * body1.inverseMass;
			body1.linearVelocity.y -= tSVector.y * body1.inverseMass;
			body1.linearVelocity.z -= tSVector.z * body1.inverseMass;
			if (!body1IsMassPoint)
			{
				FP fP11 = relativePos1.y * tSVector.z - relativePos1.z * tSVector.y;
				FP fP12 = relativePos1.z * tSVector.x - relativePos1.x * tSVector.z;
				FP fP13 = relativePos1.x * tSVector.y - relativePos1.y * tSVector.x;
				FP fP14 = fP11 * body1.invInertiaWorld.M11 + fP12 * body1.invInertiaWorld.M21 + fP13 * body1.invInertiaWorld.M31;
				FP fP15 = fP11 * body1.invInertiaWorld.M12 + fP12 * body1.invInertiaWorld.M22 + fP13 * body1.invInertiaWorld.M32;
				FP fP16 = fP11 * body1.invInertiaWorld.M13 + fP12 * body1.invInertiaWorld.M23 + fP13 * body1.invInertiaWorld.M33;
				body1.angularVelocity.x -= fP14;
				body1.angularVelocity.y -= fP15;
				body1.angularVelocity.z -= fP16;
			}
		}
		if (!treatBody2AsStatic)
		{
			body2.linearVelocity.x += tSVector.x * body2.inverseMass;
			body2.linearVelocity.y += tSVector.y * body2.inverseMass;
			body2.linearVelocity.z += tSVector.z * body2.inverseMass;
			if (!body2IsMassPoint)
			{
				FP fP17 = relativePos2.y * tSVector.z - relativePos2.z * tSVector.y;
				FP fP18 = relativePos2.z * tSVector.x - relativePos2.x * tSVector.z;
				FP fP19 = relativePos2.x * tSVector.y - relativePos2.y * tSVector.x;
				FP fP20 = fP17 * body2.invInertiaWorld.M11 + fP18 * body2.invInertiaWorld.M21 + fP19 * body2.invInertiaWorld.M31;
				FP fP21 = fP17 * body2.invInertiaWorld.M12 + fP18 * body2.invInertiaWorld.M22 + fP19 * body2.invInertiaWorld.M32;
				FP fP22 = fP17 * body2.invInertiaWorld.M13 + fP18 * body2.invInertiaWorld.M23 + fP19 * body2.invInertiaWorld.M33;
				body2.angularVelocity.x += fP20;
				body2.angularVelocity.y += fP21;
				body2.angularVelocity.z += fP22;
			}
		}
	}

	public void UpdatePosition()
	{
		if (body1IsMassPoint)
		{
			TSVector.Add(ref realRelPos1, ref body1.position, out p1);
		}
		else
		{
			TSVector.Transform(ref realRelPos1, ref body1.orientation, out p1);
			TSVector.Add(ref p1, ref body1.position, out p1);
		}
		if (body2IsMassPoint)
		{
			TSVector.Add(ref realRelPos2, ref body2.position, out p2);
		}
		else
		{
			TSVector.Transform(ref realRelPos2, ref body2.orientation, out p2);
			TSVector.Add(ref p2, ref body2.position, out p2);
		}
		TSVector.Subtract(ref p1, ref p2, out var result);
		penetration = TSVector.Dot(ref result, ref normal);
	}

	public void ApplyImpulse(ref TSVector impulse)
	{
		if (!treatBody1AsStatic)
		{
			body1.linearVelocity.x -= impulse.x * body1.inverseMass;
			body1.linearVelocity.y -= impulse.y * body1.inverseMass;
			body1.linearVelocity.z -= impulse.z * body1.inverseMass;
			FP fP = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
			FP fP2 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
			FP fP3 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;
			FP fP4 = fP * body1.invInertiaWorld.M11 + fP2 * body1.invInertiaWorld.M21 + fP3 * body1.invInertiaWorld.M31;
			FP fP5 = fP * body1.invInertiaWorld.M12 + fP2 * body1.invInertiaWorld.M22 + fP3 * body1.invInertiaWorld.M32;
			FP fP6 = fP * body1.invInertiaWorld.M13 + fP2 * body1.invInertiaWorld.M23 + fP3 * body1.invInertiaWorld.M33;
			body1.angularVelocity.x -= fP4;
			body1.angularVelocity.y -= fP5;
			body1.angularVelocity.z -= fP6;
		}
		if (!treatBody2AsStatic)
		{
			body2.linearVelocity.x += impulse.x * body2.inverseMass;
			body2.linearVelocity.y += impulse.y * body2.inverseMass;
			body2.linearVelocity.z += impulse.z * body2.inverseMass;
			FP fP7 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
			FP fP8 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
			FP fP9 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;
			FP fP10 = fP7 * body2.invInertiaWorld.M11 + fP8 * body2.invInertiaWorld.M21 + fP9 * body2.invInertiaWorld.M31;
			FP fP11 = fP7 * body2.invInertiaWorld.M12 + fP8 * body2.invInertiaWorld.M22 + fP9 * body2.invInertiaWorld.M32;
			FP fP12 = fP7 * body2.invInertiaWorld.M13 + fP8 * body2.invInertiaWorld.M23 + fP9 * body2.invInertiaWorld.M33;
			body2.angularVelocity.x += fP10;
			body2.angularVelocity.y += fP11;
			body2.angularVelocity.z += fP12;
		}
	}

	public void ApplyImpulse(TSVector impulse)
	{
		if (!treatBody1AsStatic)
		{
			body1.linearVelocity.x -= impulse.x * body1.inverseMass;
			body1.linearVelocity.y -= impulse.y * body1.inverseMass;
			body1.linearVelocity.z -= impulse.z * body1.inverseMass;
			FP fP = relativePos1.y * impulse.z - relativePos1.z * impulse.y;
			FP fP2 = relativePos1.z * impulse.x - relativePos1.x * impulse.z;
			FP fP3 = relativePos1.x * impulse.y - relativePos1.y * impulse.x;
			FP fP4 = fP * body1.invInertiaWorld.M11 + fP2 * body1.invInertiaWorld.M21 + fP3 * body1.invInertiaWorld.M31;
			FP fP5 = fP * body1.invInertiaWorld.M12 + fP2 * body1.invInertiaWorld.M22 + fP3 * body1.invInertiaWorld.M32;
			FP fP6 = fP * body1.invInertiaWorld.M13 + fP2 * body1.invInertiaWorld.M23 + fP3 * body1.invInertiaWorld.M33;
			body1.angularVelocity.x -= fP4;
			body1.angularVelocity.y -= fP5;
			body1.angularVelocity.z -= fP6;
		}
		if (!treatBody2AsStatic)
		{
			body2.linearVelocity.x += impulse.x * body2.inverseMass;
			body2.linearVelocity.y += impulse.y * body2.inverseMass;
			body2.linearVelocity.z += impulse.z * body2.inverseMass;
			FP fP7 = relativePos2.y * impulse.z - relativePos2.z * impulse.y;
			FP fP8 = relativePos2.z * impulse.x - relativePos2.x * impulse.z;
			FP fP9 = relativePos2.x * impulse.y - relativePos2.y * impulse.x;
			FP fP10 = fP7 * body2.invInertiaWorld.M11 + fP8 * body2.invInertiaWorld.M21 + fP9 * body2.invInertiaWorld.M31;
			FP fP11 = fP7 * body2.invInertiaWorld.M12 + fP8 * body2.invInertiaWorld.M22 + fP9 * body2.invInertiaWorld.M32;
			FP fP12 = fP7 * body2.invInertiaWorld.M13 + fP8 * body2.invInertiaWorld.M23 + fP9 * body2.invInertiaWorld.M33;
			body2.angularVelocity.x += fP10;
			body2.angularVelocity.y += fP11;
			body2.angularVelocity.z += fP12;
		}
	}

	public void PrepareForIteration(FP timestep)
	{
		FP fP = body2.angularVelocity.y * relativePos2.z - body2.angularVelocity.z * relativePos2.y + body2.linearVelocity.x;
		FP fP2 = body2.angularVelocity.z * relativePos2.x - body2.angularVelocity.x * relativePos2.z + body2.linearVelocity.y;
		FP fP3 = body2.angularVelocity.x * relativePos2.y - body2.angularVelocity.y * relativePos2.x + body2.linearVelocity.z;
		fP = fP - body1.angularVelocity.y * relativePos1.z + body1.angularVelocity.z * relativePos1.y - body1.linearVelocity.x;
		fP2 = fP2 - body1.angularVelocity.z * relativePos1.x + body1.angularVelocity.x * relativePos1.z - body1.linearVelocity.y;
		fP3 = fP3 - body1.angularVelocity.x * relativePos1.y + body1.angularVelocity.y * relativePos1.x - body1.linearVelocity.z;
		FP zero = FP.Zero;
		TSVector vector = TSVector.zero;
		if (!treatBody1AsStatic)
		{
			zero += body1.inverseMass;
			if (!body1IsMassPoint)
			{
				vector.x = relativePos1.y * normal.z - relativePos1.z * normal.y;
				vector.y = relativePos1.z * normal.x - relativePos1.x * normal.z;
				vector.z = relativePos1.x * normal.y - relativePos1.y * normal.x;
				FP x = vector.x * body1.invInertiaWorld.M11 + vector.y * body1.invInertiaWorld.M21 + vector.z * body1.invInertiaWorld.M31;
				FP y = vector.x * body1.invInertiaWorld.M12 + vector.y * body1.invInertiaWorld.M22 + vector.z * body1.invInertiaWorld.M32;
				FP z = vector.x * body1.invInertiaWorld.M13 + vector.y * body1.invInertiaWorld.M23 + vector.z * body1.invInertiaWorld.M33;
				vector.x = x;
				vector.y = y;
				vector.z = z;
				x = vector.y * relativePos1.z - vector.z * relativePos1.y;
				y = vector.z * relativePos1.x - vector.x * relativePos1.z;
				z = vector.x * relativePos1.y - vector.y * relativePos1.x;
				vector.x = x;
				vector.y = y;
				vector.z = z;
			}
		}
		TSVector vector2 = TSVector.zero;
		if (!treatBody2AsStatic)
		{
			zero += body2.inverseMass;
			if (!body2IsMassPoint)
			{
				vector2.x = relativePos2.y * normal.z - relativePos2.z * normal.y;
				vector2.y = relativePos2.z * normal.x - relativePos2.x * normal.z;
				vector2.z = relativePos2.x * normal.y - relativePos2.y * normal.x;
				FP x2 = vector2.x * body2.invInertiaWorld.M11 + vector2.y * body2.invInertiaWorld.M21 + vector2.z * body2.invInertiaWorld.M31;
				FP y2 = vector2.x * body2.invInertiaWorld.M12 + vector2.y * body2.invInertiaWorld.M22 + vector2.z * body2.invInertiaWorld.M32;
				FP z2 = vector2.x * body2.invInertiaWorld.M13 + vector2.y * body2.invInertiaWorld.M23 + vector2.z * body2.invInertiaWorld.M33;
				vector2.x = x2;
				vector2.y = y2;
				vector2.z = z2;
				x2 = vector2.y * relativePos2.z - vector2.z * relativePos2.y;
				y2 = vector2.z * relativePos2.x - vector2.x * relativePos2.z;
				z2 = vector2.x * relativePos2.y - vector2.y * relativePos2.x;
				vector2.x = x2;
				vector2.y = y2;
				vector2.z = z2;
			}
		}
		if (!treatBody1AsStatic)
		{
			zero += vector.x * normal.x + vector.y * normal.y + vector.z * normal.z;
		}
		if (!treatBody2AsStatic)
		{
			zero += vector2.x * normal.x + vector2.y * normal.y + vector2.z * normal.z;
		}
		massNormal = FP.One / zero;
		FP fP4 = fP * normal.x + fP2 * normal.y + fP3 * normal.z;
		tangent.x = fP - normal.x * fP4;
		tangent.y = fP2 - normal.y * fP4;
		tangent.z = fP3 - normal.z * fP4;
		fP4 = tangent.x * tangent.x + tangent.y * tangent.y + tangent.z * tangent.z;
		if (fP4 != FP.Zero)
		{
			fP4 = FP.Sqrt(fP4);
			tangent.x /= fP4;
			tangent.y /= fP4;
			tangent.z /= fP4;
		}
		FP zero2 = FP.Zero;
		if (treatBody1AsStatic)
		{
			vector.MakeZero();
		}
		else
		{
			zero2 += body1.inverseMass;
			if (!body1IsMassPoint)
			{
				vector.x = relativePos1.y * tangent.z - relativePos1.z * tangent.y;
				vector.y = relativePos1.z * tangent.x - relativePos1.x * tangent.z;
				vector.z = relativePos1.x * tangent.y - relativePos1.y * tangent.x;
				FP x3 = vector.x * body1.invInertiaWorld.M11 + vector.y * body1.invInertiaWorld.M21 + vector.z * body1.invInertiaWorld.M31;
				FP y3 = vector.x * body1.invInertiaWorld.M12 + vector.y * body1.invInertiaWorld.M22 + vector.z * body1.invInertiaWorld.M32;
				FP z3 = vector.x * body1.invInertiaWorld.M13 + vector.y * body1.invInertiaWorld.M23 + vector.z * body1.invInertiaWorld.M33;
				vector.x = x3;
				vector.y = y3;
				vector.z = z3;
				x3 = vector.y * relativePos1.z - vector.z * relativePos1.y;
				y3 = vector.z * relativePos1.x - vector.x * relativePos1.z;
				z3 = vector.x * relativePos1.y - vector.y * relativePos1.x;
				vector.x = x3;
				vector.y = y3;
				vector.z = z3;
			}
		}
		if (treatBody2AsStatic)
		{
			vector2.MakeZero();
		}
		else
		{
			zero2 += body2.inverseMass;
			if (!body2IsMassPoint)
			{
				vector2.x = relativePos2.y * tangent.z - relativePos2.z * tangent.y;
				vector2.y = relativePos2.z * tangent.x - relativePos2.x * tangent.z;
				vector2.z = relativePos2.x * tangent.y - relativePos2.y * tangent.x;
				FP x4 = vector2.x * body2.invInertiaWorld.M11 + vector2.y * body2.invInertiaWorld.M21 + vector2.z * body2.invInertiaWorld.M31;
				FP y4 = vector2.x * body2.invInertiaWorld.M12 + vector2.y * body2.invInertiaWorld.M22 + vector2.z * body2.invInertiaWorld.M32;
				FP z4 = vector2.x * body2.invInertiaWorld.M13 + vector2.y * body2.invInertiaWorld.M23 + vector2.z * body2.invInertiaWorld.M33;
				vector2.x = x4;
				vector2.y = y4;
				vector2.z = z4;
				x4 = vector2.y * relativePos2.z - vector2.z * relativePos2.y;
				y4 = vector2.z * relativePos2.x - vector2.x * relativePos2.z;
				z4 = vector2.x * relativePos2.y - vector2.y * relativePos2.x;
				vector2.x = x4;
				vector2.y = y4;
				vector2.z = z4;
			}
		}
		if (!treatBody1AsStatic)
		{
			zero2 += TSVector.Dot(ref vector, ref tangent);
		}
		if (!treatBody2AsStatic)
		{
			zero2 += TSVector.Dot(ref vector2, ref tangent);
		}
		massTangent = FP.One / zero2;
		restitutionBias = lostSpeculativeBounce;
		speculativeVelocity = FP.Zero;
		FP fP5 = normal.x * fP + normal.y * fP2 + normal.z * fP3;
		if (Penetration > settings.allowedPenetration)
		{
			restitutionBias = settings.bias * (FP.One / timestep) * TSMath.Max(FP.Zero, Penetration - settings.allowedPenetration);
			restitutionBias = TSMath.Clamp(restitutionBias, FP.Zero, settings.maximumBias);
		}
		FP fP6 = timestep / lastTimeStep;
		accumulatedNormalImpulse *= fP6;
		accumulatedTangentImpulse *= fP6;
		FP fP7 = -(tangent.x * fP + tangent.y * fP2 + tangent.z * fP3);
		FP fP8 = massTangent * fP7;
		FP fP9 = -staticFriction * accumulatedNormalImpulse;
		if (fP8 < fP9)
		{
			friction = dynamicFriction;
		}
		else
		{
			friction = staticFriction;
		}
		restitutionBias = TSMath.Max(-restitution * fP5, restitutionBias);
		if (penetration < -settings.allowedPenetration)
		{
			speculativeVelocity = penetration / timestep;
			lostSpeculativeBounce = restitutionBias;
			restitutionBias = FP.Zero;
		}
		else
		{
			lostSpeculativeBounce = FP.Zero;
		}
		TSVector tSVector = default;
		tSVector.x = normal.x * accumulatedNormalImpulse + tangent.x * accumulatedTangentImpulse;
		tSVector.y = normal.y * accumulatedNormalImpulse + tangent.y * accumulatedTangentImpulse;
		tSVector.z = normal.z * accumulatedNormalImpulse + tangent.z * accumulatedTangentImpulse;
		if (!treatBody1AsStatic)
		{
			body1.linearVelocity.x -= tSVector.x * body1.inverseMass;
			body1.linearVelocity.y -= tSVector.y * body1.inverseMass;
			body1.linearVelocity.z -= tSVector.z * body1.inverseMass;
			if (!body1IsMassPoint)
			{
				FP fP10 = relativePos1.y * tSVector.z - relativePos1.z * tSVector.y;
				FP fP11 = relativePos1.z * tSVector.x - relativePos1.x * tSVector.z;
				FP fP12 = relativePos1.x * tSVector.y - relativePos1.y * tSVector.x;
				FP fP13 = fP10 * body1.invInertiaWorld.M11 + fP11 * body1.invInertiaWorld.M21 + fP12 * body1.invInertiaWorld.M31;
				FP fP14 = fP10 * body1.invInertiaWorld.M12 + fP11 * body1.invInertiaWorld.M22 + fP12 * body1.invInertiaWorld.M32;
				FP fP15 = fP10 * body1.invInertiaWorld.M13 + fP11 * body1.invInertiaWorld.M23 + fP12 * body1.invInertiaWorld.M33;
				body1.angularVelocity.x -= fP13;
				body1.angularVelocity.y -= fP14;
				body1.angularVelocity.z -= fP15;
			}
		}
		if (!treatBody2AsStatic)
		{
			body2.linearVelocity.x += tSVector.x * body2.inverseMass;
			body2.linearVelocity.y += tSVector.y * body2.inverseMass;
			body2.linearVelocity.z += tSVector.z * body2.inverseMass;
			if (!body2IsMassPoint)
			{
				FP fP16 = relativePos2.y * tSVector.z - relativePos2.z * tSVector.y;
				FP fP17 = relativePos2.z * tSVector.x - relativePos2.x * tSVector.z;
				FP fP18 = relativePos2.x * tSVector.y - relativePos2.y * tSVector.x;
				FP fP19 = fP16 * body2.invInertiaWorld.M11 + fP17 * body2.invInertiaWorld.M21 + fP18 * body2.invInertiaWorld.M31;
				FP fP20 = fP16 * body2.invInertiaWorld.M12 + fP17 * body2.invInertiaWorld.M22 + fP18 * body2.invInertiaWorld.M32;
				FP fP21 = fP16 * body2.invInertiaWorld.M13 + fP17 * body2.invInertiaWorld.M23 + fP18 * body2.invInertiaWorld.M33;
				body2.angularVelocity.x += fP19;
				body2.angularVelocity.y += fP20;
				body2.angularVelocity.z += fP21;
			}
		}
		lastTimeStep = timestep;
		newContact = false;
	}

	public void TreatBodyAsStatic(RigidBodyIndex index)
	{
		if (index == RigidBodyIndex.RigidBody1)
		{
			treatBody1AsStatic = true;
		}
		else
		{
			treatBody2AsStatic = true;
		}
	}

	public void Initialize(RigidBody body1, RigidBody body2, ref TSVector point1, ref TSVector point2, ref TSVector n, FP penetration, bool newContact, ContactSettings settings)
	{
		this.body1 = body1;
		this.body2 = body2;
		normal = n;
		normal.Normalize();
		p1 = point1;
		p2 = point2;
		this.newContact = newContact;
		TSVector.Subtract(ref p1, ref body1.position, out relativePos1);
		TSVector.Subtract(ref p2, ref body2.position, out relativePos2);
		TSVector.Transform(ref relativePos1, ref body1.invOrientation, out realRelPos1);
		TSVector.Transform(ref relativePos2, ref body2.invOrientation, out realRelPos2);
		initialPen = penetration;
		this.penetration = penetration;
		body1IsMassPoint = body1.isParticle;
		body2IsMassPoint = body2.isParticle;
		if (newContact)
		{
			treatBody1AsStatic = body1.isStatic;
			treatBody2AsStatic = body2.isStatic;
			accumulatedNormalImpulse = FP.Zero;
			accumulatedTangentImpulse = FP.Zero;
			lostSpeculativeBounce = FP.Zero;
			switch (settings.MaterialCoefficientMixing)
			{
			case ContactSettings.MaterialCoefficientMixingType.TakeMaximum:
				staticFriction = TSMath.Max(body1.staticFriction, body2.staticFriction);
				dynamicFriction = TSMath.Max(body1.staticFriction, body2.staticFriction);
				restitution = TSMath.Max(body1.restitution, body2.restitution);
				break;
			case ContactSettings.MaterialCoefficientMixingType.TakeMinimum:
				staticFriction = TSMath.Min(body1.staticFriction, body2.staticFriction);
				dynamicFriction = TSMath.Min(body1.staticFriction, body2.staticFriction);
				restitution = TSMath.Min(body1.restitution, body2.restitution);
				break;
			case ContactSettings.MaterialCoefficientMixingType.UseAverage:
				staticFriction = (body1.staticFriction + body2.staticFriction) * FP.Half;
				dynamicFriction = (body1.staticFriction + body2.staticFriction) * FP.Half;
				restitution = (body1.restitution + body2.restitution) * FP.Half;
				break;
			}
		}
		this.settings = settings;
	}
}
