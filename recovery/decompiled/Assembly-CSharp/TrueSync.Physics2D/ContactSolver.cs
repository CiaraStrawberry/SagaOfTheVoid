namespace TrueSync.Physics2D;

public class ContactSolver
{
	public static class WorldManifold
	{
		public static void Initialize(ref Manifold manifold, ref Transform xfA, FP radiusA, ref Transform xfB, FP radiusB, out TSVector2 normal, out FixedArray2<TSVector2> points)
		{
			normal = TSVector2.zero;
			points = default;
			if (manifold.PointCount == 0)
			{
				return;
			}
			switch (manifold.Type)
			{
			case ManifoldType.Circles:
			{
				normal = new TSVector2(1f, 0f);
				TSVector2 tSVector9 = MathUtils.Mul(ref xfA, manifold.LocalPoint);
				TSVector2 tSVector10 = MathUtils.Mul(ref xfB, manifold.Points[0].LocalPoint);
				if (TSVector2.DistanceSquared(tSVector9, tSVector10) > Settings.EpsilonSqr)
				{
					normal = tSVector10 - tSVector9;
					normal.Normalize();
				}
				TSVector2 tSVector11 = tSVector9 + radiusA * normal;
				TSVector2 tSVector12 = tSVector10 - radiusB * normal;
				points[0] = 0.5f * (tSVector11 + tSVector12);
				break;
			}
			case ManifoldType.FaceA:
			{
				normal = MathUtils.Mul(xfA.q, manifold.LocalNormal);
				TSVector2 tSVector5 = MathUtils.Mul(ref xfA, manifold.LocalPoint);
				for (int j = 0; j < manifold.PointCount; j++)
				{
					TSVector2 tSVector6 = MathUtils.Mul(ref xfB, manifold.Points[j].LocalPoint);
					TSVector2 tSVector7 = tSVector6 + (radiusA - TSVector2.Dot(tSVector6 - tSVector5, normal)) * normal;
					TSVector2 tSVector8 = tSVector6 - radiusB * normal;
					points[j] = 0.5f * (tSVector7 + tSVector8);
				}
				break;
			}
			case ManifoldType.FaceB:
			{
				normal = MathUtils.Mul(xfB.q, manifold.LocalNormal);
				TSVector2 tSVector = MathUtils.Mul(ref xfB, manifold.LocalPoint);
				for (int i = 0; i < manifold.PointCount; i++)
				{
					TSVector2 tSVector2 = MathUtils.Mul(ref xfA, manifold.Points[i].LocalPoint);
					TSVector2 tSVector3 = tSVector2 + (radiusB - TSVector2.Dot(tSVector2 - tSVector, normal)) * normal;
					TSVector2 tSVector4 = tSVector2 - radiusA * normal;
					points[i] = 0.5f * (tSVector4 + tSVector3);
				}
				normal = -normal;
				break;
			}
			}
		}
	}

	private static class PositionSolverManifold
	{
		public static void Initialize(ContactPositionConstraint pc, Transform xfA, Transform xfB, int index, out TSVector2 normal, out TSVector2 point, out FP separation)
		{
			switch (pc.type)
			{
			case ManifoldType.Circles:
			{
				TSVector2 tSVector5 = MathUtils.Mul(ref xfA, pc.localPoint);
				TSVector2 tSVector6 = MathUtils.Mul(ref xfB, pc.localPoints[0]);
				normal = tSVector6 - tSVector5;
				normal.Normalize();
				point = 0.5f * (tSVector5 + tSVector6);
				separation = TSVector2.Dot(tSVector6 - tSVector5, normal) - pc.radiusA - pc.radiusB;
				break;
			}
			case ManifoldType.FaceA:
			{
				normal = MathUtils.Mul(xfA.q, pc.localNormal);
				TSVector2 tSVector3 = MathUtils.Mul(ref xfA, pc.localPoint);
				TSVector2 tSVector4 = MathUtils.Mul(ref xfB, pc.localPoints[index]);
				separation = TSVector2.Dot(tSVector4 - tSVector3, normal) - pc.radiusA - pc.radiusB;
				point = tSVector4;
				break;
			}
			case ManifoldType.FaceB:
			{
				normal = MathUtils.Mul(xfB.q, pc.localNormal);
				TSVector2 tSVector = MathUtils.Mul(ref xfB, pc.localPoint);
				TSVector2 tSVector2 = MathUtils.Mul(ref xfA, pc.localPoints[index]);
				separation = TSVector2.Dot(tSVector2 - tSVector, normal) - pc.radiusA - pc.radiusB;
				point = tSVector2;
				normal = -normal;
				break;
			}
			default:
				normal = TSVector2.zero;
				point = TSVector2.zero;
				separation = 0;
				break;
			}
		}
	}

	public TimeStep _step;

	public Position[] _positions;

	public Velocity[] _velocities;

	public ContactPositionConstraint[] _positionConstraints;

	public ContactVelocityConstraint[] _velocityConstraints;

	public Contact[] _contacts;

	public int _count;

	public void Reset(TimeStep step, int count, Contact[] contacts, Position[] positions, Velocity[] velocities, bool warmstarting = true)
	{
		_step = step;
		_count = count;
		_positions = positions;
		_velocities = velocities;
		_contacts = contacts;
		if (_velocityConstraints == null || _velocityConstraints.Length < count)
		{
			_velocityConstraints = new ContactVelocityConstraint[count * 2];
			_positionConstraints = new ContactPositionConstraint[count * 2];
			for (int i = 0; i < _velocityConstraints.Length; i++)
			{
				_velocityConstraints[i] = new ContactVelocityConstraint();
			}
			for (int j = 0; j < _positionConstraints.Length; j++)
			{
				_positionConstraints[j] = new ContactPositionConstraint();
			}
		}
		for (int k = 0; k < _count; k++)
		{
			Contact contact = contacts[k];
			Fixture fixtureA = contact.FixtureA;
			Fixture fixtureB = contact.FixtureB;
			Shape shape = fixtureA.Shape;
			Shape shape2 = fixtureB.Shape;
			FP radius = shape.Radius;
			FP radius2 = shape2.Radius;
			Body body = fixtureA.Body;
			Body body2 = fixtureB.Body;
			Manifold manifold = contact.Manifold;
			int pointCount = manifold.PointCount;
			ContactVelocityConstraint contactVelocityConstraint = _velocityConstraints[k];
			contactVelocityConstraint.friction = contact.Friction;
			contactVelocityConstraint.restitution = contact.Restitution;
			contactVelocityConstraint.tangentSpeed = contact.TangentSpeed;
			contactVelocityConstraint.indexA = body.IslandIndex;
			contactVelocityConstraint.indexB = body2.IslandIndex;
			contactVelocityConstraint.invMassA = body._invMass;
			contactVelocityConstraint.invMassB = body2._invMass;
			contactVelocityConstraint.invIA = body._invI;
			contactVelocityConstraint.invIB = body2._invI;
			contactVelocityConstraint.contactIndex = k;
			contactVelocityConstraint.pointCount = pointCount;
			contactVelocityConstraint.K.SetZero();
			contactVelocityConstraint.normalMass.SetZero();
			ContactPositionConstraint contactPositionConstraint = _positionConstraints[k];
			contactPositionConstraint.indexA = body.IslandIndex;
			contactPositionConstraint.indexB = body2.IslandIndex;
			contactPositionConstraint.invMassA = body._invMass;
			contactPositionConstraint.invMassB = body2._invMass;
			contactPositionConstraint.localCenterA = body._sweep.LocalCenter;
			contactPositionConstraint.localCenterB = body2._sweep.LocalCenter;
			contactPositionConstraint.invIA = body._invI;
			contactPositionConstraint.invIB = body2._invI;
			contactPositionConstraint.localNormal = manifold.LocalNormal;
			contactPositionConstraint.localPoint = manifold.LocalPoint;
			contactPositionConstraint.pointCount = pointCount;
			contactPositionConstraint.radiusA = radius;
			contactPositionConstraint.radiusB = radius2;
			contactPositionConstraint.type = manifold.Type;
			for (int l = 0; l < pointCount; l++)
			{
				ManifoldPoint manifoldPoint = manifold.Points[l];
				VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.points[l];
				velocityConstraintPoint.normalImpulse = _step.dtRatio * manifoldPoint.NormalImpulse;
				velocityConstraintPoint.tangentImpulse = _step.dtRatio * manifoldPoint.TangentImpulse;
				velocityConstraintPoint.rA = TSVector2.zero;
				velocityConstraintPoint.rB = TSVector2.zero;
				velocityConstraintPoint.normalMass = 0f;
				velocityConstraintPoint.tangentMass = 0f;
				velocityConstraintPoint.velocityBias = 0f;
				ref TSVector2 reference = ref contactPositionConstraint.localPoints[l];
				reference = manifoldPoint.LocalPoint;
			}
		}
	}

	public void InitializeVelocityConstraints()
	{
		for (int i = 0; i < _count; i++)
		{
			ContactVelocityConstraint contactVelocityConstraint = _velocityConstraints[i];
			ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
			FP radiusA = contactPositionConstraint.radiusA;
			FP radiusB = contactPositionConstraint.radiusB;
			Manifold manifold = _contacts[contactVelocityConstraint.contactIndex].Manifold;
			int indexA = contactVelocityConstraint.indexA;
			int indexB = contactVelocityConstraint.indexB;
			FP invMassA = contactVelocityConstraint.invMassA;
			FP invMassB = contactVelocityConstraint.invMassB;
			FP invIA = contactVelocityConstraint.invIA;
			FP invIB = contactVelocityConstraint.invIB;
			TSVector2 localCenterA = contactPositionConstraint.localCenterA;
			TSVector2 localCenterB = contactPositionConstraint.localCenterB;
			TSVector2 c = _positions[indexA].c;
			FP a = _positions[indexA].a;
			TSVector2 v = _velocities[indexA].v;
			FP w = _velocities[indexA].w;
			TSVector2 c2 = _positions[indexB].c;
			FP a2 = _positions[indexB].a;
			TSVector2 v2 = _velocities[indexB].v;
			FP w2 = _velocities[indexB].w;
			Transform xfA = default;
			Transform xfB = default;
			xfA.q.Set(a);
			xfB.q.Set(a2);
			xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
			xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
			WorldManifold.Initialize(ref manifold, ref xfA, radiusA, ref xfB, radiusB, out var normal, out var points);
			contactVelocityConstraint.normal = normal;
			int pointCount = contactVelocityConstraint.pointCount;
			for (int j = 0; j < pointCount; j++)
			{
				VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.points[j];
				velocityConstraintPoint.rA = points[j] - c;
				velocityConstraintPoint.rB = points[j] - c2;
				FP fP = MathUtils.Cross(velocityConstraintPoint.rA, contactVelocityConstraint.normal);
				FP fP2 = MathUtils.Cross(velocityConstraintPoint.rB, contactVelocityConstraint.normal);
				FP fP3 = invMassA + invMassB + invIA * fP * fP + invIB * fP2 * fP2;
				velocityConstraintPoint.normalMass = ((!(fP3 > 0f)) ? ((FP)0f) : (1f / fP3));
				TSVector2 b = MathUtils.Cross(contactVelocityConstraint.normal, 1f);
				FP fP4 = MathUtils.Cross(velocityConstraintPoint.rA, b);
				FP fP5 = MathUtils.Cross(velocityConstraintPoint.rB, b);
				FP fP6 = invMassA + invMassB + invIA * fP4 * fP4 + invIB * fP5 * fP5;
				velocityConstraintPoint.tangentMass = ((!(fP6 > 0f)) ? ((FP)0f) : (1f / fP6));
				velocityConstraintPoint.velocityBias = 0f;
				FP fP7 = TSVector2.Dot(contactVelocityConstraint.normal, v2 + MathUtils.Cross(w2, velocityConstraintPoint.rB) - v - MathUtils.Cross(w, velocityConstraintPoint.rA));
				if (fP7 < -Settings.VelocityThreshold)
				{
					velocityConstraintPoint.velocityBias = -contactVelocityConstraint.restitution * fP7;
				}
			}
			if (contactVelocityConstraint.pointCount == 2)
			{
				VelocityConstraintPoint velocityConstraintPoint2 = contactVelocityConstraint.points[0];
				VelocityConstraintPoint velocityConstraintPoint3 = contactVelocityConstraint.points[1];
				FP fP8 = MathUtils.Cross(velocityConstraintPoint2.rA, contactVelocityConstraint.normal);
				FP fP9 = MathUtils.Cross(velocityConstraintPoint2.rB, contactVelocityConstraint.normal);
				FP fP10 = MathUtils.Cross(velocityConstraintPoint3.rA, contactVelocityConstraint.normal);
				FP fP11 = MathUtils.Cross(velocityConstraintPoint3.rB, contactVelocityConstraint.normal);
				FP fP12 = invMassA + invMassB + invIA * fP8 * fP8 + invIB * fP9 * fP9;
				FP fP13 = invMassA + invMassB + invIA * fP10 * fP10 + invIB * fP11 * fP11;
				FP fP14 = invMassA + invMassB + invIA * fP8 * fP10 + invIB * fP9 * fP11;
				FP fP15 = 1000f;
				if (fP12 * fP12 < fP15 * (fP12 * fP13 - fP14 * fP14))
				{
					contactVelocityConstraint.K.ex = new TSVector2(fP12, fP14);
					contactVelocityConstraint.K.ey = new TSVector2(fP14, fP13);
					contactVelocityConstraint.normalMass = contactVelocityConstraint.K.Inverse;
				}
				else
				{
					contactVelocityConstraint.pointCount = 1;
				}
			}
		}
	}

	public void WarmStart()
	{
		for (int i = 0; i < _count; i++)
		{
			ContactVelocityConstraint contactVelocityConstraint = _velocityConstraints[i];
			int indexA = contactVelocityConstraint.indexA;
			int indexB = contactVelocityConstraint.indexB;
			FP invMassA = contactVelocityConstraint.invMassA;
			FP invIA = contactVelocityConstraint.invIA;
			FP invMassB = contactVelocityConstraint.invMassB;
			FP invIB = contactVelocityConstraint.invIB;
			int pointCount = contactVelocityConstraint.pointCount;
			TSVector2 v = _velocities[indexA].v;
			FP w = _velocities[indexA].w;
			TSVector2 v2 = _velocities[indexB].v;
			FP w2 = _velocities[indexB].w;
			TSVector2 normal = contactVelocityConstraint.normal;
			TSVector2 tSVector = MathUtils.Cross(normal, 1f);
			for (int j = 0; j < pointCount; j++)
			{
				VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.points[j];
				TSVector2 tSVector2 = velocityConstraintPoint.normalImpulse * normal + velocityConstraintPoint.tangentImpulse * tSVector;
				w -= invIA * MathUtils.Cross(velocityConstraintPoint.rA, tSVector2);
				v -= invMassA * tSVector2;
				w2 += invIB * MathUtils.Cross(velocityConstraintPoint.rB, tSVector2);
				v2 += invMassB * tSVector2;
			}
			_velocities[indexA].v = v;
			_velocities[indexA].w = w;
			_velocities[indexB].v = v2;
			_velocities[indexB].w = w2;
		}
	}

	public void SolveVelocityConstraints()
	{
		for (int i = 0; i < _count; i++)
		{
			ContactVelocityConstraint contactVelocityConstraint = _velocityConstraints[i];
			int indexA = contactVelocityConstraint.indexA;
			int indexB = contactVelocityConstraint.indexB;
			FP invMassA = contactVelocityConstraint.invMassA;
			FP invIA = contactVelocityConstraint.invIA;
			FP invMassB = contactVelocityConstraint.invMassB;
			FP invIB = contactVelocityConstraint.invIB;
			int pointCount = contactVelocityConstraint.pointCount;
			TSVector2 v = _velocities[indexA].v;
			FP w = _velocities[indexA].w;
			TSVector2 v2 = _velocities[indexB].v;
			FP w2 = _velocities[indexB].w;
			TSVector2 normal = contactVelocityConstraint.normal;
			TSVector2 tSVector = MathUtils.Cross(normal, 1f);
			FP friction = contactVelocityConstraint.friction;
			for (int j = 0; j < pointCount; j++)
			{
				VelocityConstraintPoint velocityConstraintPoint = contactVelocityConstraint.points[j];
				TSVector2 value = v2 + MathUtils.Cross(w2, velocityConstraintPoint.rB) - v - MathUtils.Cross(w, velocityConstraintPoint.rA);
				FP fP = TSVector2.Dot(value, tSVector) - contactVelocityConstraint.tangentSpeed;
				FP fP2 = velocityConstraintPoint.tangentMass * -fP;
				FP fP3 = friction * velocityConstraintPoint.normalImpulse;
				FP fP4 = MathUtils.Clamp(velocityConstraintPoint.tangentImpulse + fP2, -fP3, fP3);
				fP2 = fP4 - velocityConstraintPoint.tangentImpulse;
				velocityConstraintPoint.tangentImpulse = fP4;
				TSVector2 tSVector2 = fP2 * tSVector;
				v -= invMassA * tSVector2;
				w -= invIA * MathUtils.Cross(velocityConstraintPoint.rA, tSVector2);
				v2 += invMassB * tSVector2;
				w2 += invIB * MathUtils.Cross(velocityConstraintPoint.rB, tSVector2);
			}
			if (contactVelocityConstraint.pointCount == 1)
			{
				VelocityConstraintPoint velocityConstraintPoint2 = contactVelocityConstraint.points[0];
				TSVector2 value2 = v2 + MathUtils.Cross(w2, velocityConstraintPoint2.rB) - v - MathUtils.Cross(w, velocityConstraintPoint2.rA);
				FP fP5 = TSVector2.Dot(value2, normal);
				FP fP6 = -velocityConstraintPoint2.normalMass * (fP5 - velocityConstraintPoint2.velocityBias);
				FP fP7 = TSMath.Max(velocityConstraintPoint2.normalImpulse + fP6, 0f);
				fP6 = fP7 - velocityConstraintPoint2.normalImpulse;
				velocityConstraintPoint2.normalImpulse = fP7;
				TSVector2 tSVector3 = fP6 * normal;
				v -= invMassA * tSVector3;
				w -= invIA * MathUtils.Cross(velocityConstraintPoint2.rA, tSVector3);
				v2 += invMassB * tSVector3;
				w2 += invIB * MathUtils.Cross(velocityConstraintPoint2.rB, tSVector3);
			}
			else
			{
				VelocityConstraintPoint velocityConstraintPoint3 = contactVelocityConstraint.points[0];
				VelocityConstraintPoint velocityConstraintPoint4 = contactVelocityConstraint.points[1];
				TSVector2 tSVector4 = new TSVector2(velocityConstraintPoint3.normalImpulse, velocityConstraintPoint4.normalImpulse);
				TSVector2 value3 = v2 + MathUtils.Cross(w2, velocityConstraintPoint3.rB) - v - MathUtils.Cross(w, velocityConstraintPoint3.rA);
				TSVector2 value4 = v2 + MathUtils.Cross(w2, velocityConstraintPoint4.rB) - v - MathUtils.Cross(w, velocityConstraintPoint4.rA);
				FP fP8 = TSVector2.Dot(value3, normal);
				FP fP9 = TSVector2.Dot(value4, normal);
				TSVector2 v3 = new TSVector2
				{
					x = fP8 - velocityConstraintPoint3.velocityBias,
					y = fP9 - velocityConstraintPoint4.velocityBias
				};
				v3 -= MathUtils.Mul(ref contactVelocityConstraint.K, tSVector4);
				TSVector2 tSVector5 = -MathUtils.Mul(ref contactVelocityConstraint.normalMass, v3);
				if (tSVector5.x >= 0f && tSVector5.y >= 0f)
				{
					TSVector2 tSVector6 = tSVector5 - tSVector4;
					TSVector2 tSVector7 = tSVector6.x * normal;
					TSVector2 tSVector8 = tSVector6.y * normal;
					v -= invMassA * (tSVector7 + tSVector8);
					w -= invIA * (MathUtils.Cross(velocityConstraintPoint3.rA, tSVector7) + MathUtils.Cross(velocityConstraintPoint4.rA, tSVector8));
					v2 += invMassB * (tSVector7 + tSVector8);
					w2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.rB, tSVector7) + MathUtils.Cross(velocityConstraintPoint4.rB, tSVector8));
					velocityConstraintPoint3.normalImpulse = tSVector5.x;
					velocityConstraintPoint4.normalImpulse = tSVector5.y;
				}
				else
				{
					tSVector5.x = -velocityConstraintPoint3.normalMass * v3.x;
					tSVector5.y = 0f;
					fP8 = 0f;
					fP9 = contactVelocityConstraint.K.ex.y * tSVector5.x + v3.y;
					if (tSVector5.x >= 0f && fP9 >= 0f)
					{
						TSVector2 tSVector9 = tSVector5 - tSVector4;
						TSVector2 tSVector10 = tSVector9.x * normal;
						TSVector2 tSVector11 = tSVector9.y * normal;
						v -= invMassA * (tSVector10 + tSVector11);
						w -= invIA * (MathUtils.Cross(velocityConstraintPoint3.rA, tSVector10) + MathUtils.Cross(velocityConstraintPoint4.rA, tSVector11));
						v2 += invMassB * (tSVector10 + tSVector11);
						w2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.rB, tSVector10) + MathUtils.Cross(velocityConstraintPoint4.rB, tSVector11));
						velocityConstraintPoint3.normalImpulse = tSVector5.x;
						velocityConstraintPoint4.normalImpulse = tSVector5.y;
					}
					else
					{
						tSVector5.x = 0f;
						tSVector5.y = -velocityConstraintPoint4.normalMass * v3.y;
						fP8 = contactVelocityConstraint.K.ey.x * tSVector5.y + v3.x;
						fP9 = 0f;
						if (tSVector5.y >= 0f && fP8 >= 0f)
						{
							TSVector2 tSVector12 = tSVector5 - tSVector4;
							TSVector2 tSVector13 = tSVector12.x * normal;
							TSVector2 tSVector14 = tSVector12.y * normal;
							v -= invMassA * (tSVector13 + tSVector14);
							w -= invIA * (MathUtils.Cross(velocityConstraintPoint3.rA, tSVector13) + MathUtils.Cross(velocityConstraintPoint4.rA, tSVector14));
							v2 += invMassB * (tSVector13 + tSVector14);
							w2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.rB, tSVector13) + MathUtils.Cross(velocityConstraintPoint4.rB, tSVector14));
							velocityConstraintPoint3.normalImpulse = tSVector5.x;
							velocityConstraintPoint4.normalImpulse = tSVector5.y;
						}
						else
						{
							tSVector5.x = 0f;
							tSVector5.y = 0f;
							fP8 = v3.x;
							fP9 = v3.y;
							if (fP8 >= 0f && fP9 >= 0f)
							{
								TSVector2 tSVector15 = tSVector5 - tSVector4;
								TSVector2 tSVector16 = tSVector15.x * normal;
								TSVector2 tSVector17 = tSVector15.y * normal;
								v -= invMassA * (tSVector16 + tSVector17);
								w -= invIA * (MathUtils.Cross(velocityConstraintPoint3.rA, tSVector16) + MathUtils.Cross(velocityConstraintPoint4.rA, tSVector17));
								v2 += invMassB * (tSVector16 + tSVector17);
								w2 += invIB * (MathUtils.Cross(velocityConstraintPoint3.rB, tSVector16) + MathUtils.Cross(velocityConstraintPoint4.rB, tSVector17));
								velocityConstraintPoint3.normalImpulse = tSVector5.x;
								velocityConstraintPoint4.normalImpulse = tSVector5.y;
							}
						}
					}
				}
			}
			_velocities[indexA].v = v;
			_velocities[indexA].w = w;
			_velocities[indexB].v = v2;
			_velocities[indexB].w = w2;
		}
	}

	public void StoreImpulses()
	{
		for (int i = 0; i < _count; i++)
		{
			ContactVelocityConstraint contactVelocityConstraint = _velocityConstraints[i];
			Manifold manifold = _contacts[contactVelocityConstraint.contactIndex].Manifold;
			for (int j = 0; j < contactVelocityConstraint.pointCount; j++)
			{
				ManifoldPoint value = manifold.Points[j];
				value.NormalImpulse = contactVelocityConstraint.points[j].normalImpulse;
				value.TangentImpulse = contactVelocityConstraint.points[j].tangentImpulse;
				manifold.Points[j] = value;
			}
			_contacts[contactVelocityConstraint.contactIndex].Manifold = manifold;
		}
	}

	public bool SolvePositionConstraints()
	{
		FP fP = 0f;
		for (int i = 0; i < _count; i++)
		{
			ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
			int indexA = contactPositionConstraint.indexA;
			int indexB = contactPositionConstraint.indexB;
			TSVector2 localCenterA = contactPositionConstraint.localCenterA;
			FP invMassA = contactPositionConstraint.invMassA;
			FP invIA = contactPositionConstraint.invIA;
			TSVector2 localCenterB = contactPositionConstraint.localCenterB;
			FP invMassB = contactPositionConstraint.invMassB;
			FP invIB = contactPositionConstraint.invIB;
			int pointCount = contactPositionConstraint.pointCount;
			TSVector2 c = _positions[indexA].c;
			FP a = _positions[indexA].a;
			TSVector2 c2 = _positions[indexB].c;
			FP a2 = _positions[indexB].a;
			for (int j = 0; j < pointCount; j++)
			{
				Transform xfA = default;
				Transform xfB = default;
				xfA.q.Set(a);
				xfB.q.Set(a2);
				xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
				xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
				PositionSolverManifold.Initialize(contactPositionConstraint, xfA, xfB, j, out var normal, out var point, out var separation);
				TSVector2 a3 = point - c;
				TSVector2 a4 = point - c2;
				fP = TSMath.Min(fP, separation);
				FP fP2 = MathUtils.Clamp(Settings.Baumgarte * (separation + Settings.LinearSlop), -Settings.MaxLinearCorrection, 0f);
				FP fP3 = MathUtils.Cross(a3, normal);
				FP fP4 = MathUtils.Cross(a4, normal);
				FP fP5 = invMassA + invMassB + invIA * fP3 * fP3 + invIB * fP4 * fP4;
				FP fP6 = ((!(fP5 > 0f)) ? ((FP)0f) : (-fP2 / fP5));
				TSVector2 tSVector = fP6 * normal;
				c -= invMassA * tSVector;
				a -= invIA * MathUtils.Cross(a3, tSVector);
				c2 += invMassB * tSVector;
				a2 += invIB * MathUtils.Cross(a4, tSVector);
			}
			_positions[indexA].c = c;
			_positions[indexA].a = a;
			_positions[indexB].c = c2;
			_positions[indexB].a = a2;
		}
		return fP >= -3f * Settings.LinearSlop;
	}

	public bool SolveTOIPositionConstraints(int toiIndexA, int toiIndexB)
	{
		FP fP = 0f;
		for (int i = 0; i < _count; i++)
		{
			ContactPositionConstraint contactPositionConstraint = _positionConstraints[i];
			int indexA = contactPositionConstraint.indexA;
			int indexB = contactPositionConstraint.indexB;
			TSVector2 localCenterA = contactPositionConstraint.localCenterA;
			TSVector2 localCenterB = contactPositionConstraint.localCenterB;
			int pointCount = contactPositionConstraint.pointCount;
			FP fP2 = 0f;
			FP fP3 = 0f;
			if (indexA == toiIndexA || indexA == toiIndexB)
			{
				fP2 = contactPositionConstraint.invMassA;
				fP3 = contactPositionConstraint.invIA;
			}
			FP fP4 = 0f;
			FP fP5 = 0f;
			if (indexB == toiIndexA || indexB == toiIndexB)
			{
				fP4 = contactPositionConstraint.invMassB;
				fP5 = contactPositionConstraint.invIB;
			}
			TSVector2 c = _positions[indexA].c;
			FP a = _positions[indexA].a;
			TSVector2 c2 = _positions[indexB].c;
			FP a2 = _positions[indexB].a;
			for (int j = 0; j < pointCount; j++)
			{
				Transform xfA = default;
				Transform xfB = default;
				xfA.q.Set(a);
				xfB.q.Set(a2);
				xfA.p = c - MathUtils.Mul(xfA.q, localCenterA);
				xfB.p = c2 - MathUtils.Mul(xfB.q, localCenterB);
				PositionSolverManifold.Initialize(contactPositionConstraint, xfA, xfB, j, out var normal, out var point, out var separation);
				TSVector2 a3 = point - c;
				TSVector2 a4 = point - c2;
				fP = TSMath.Min(fP, separation);
				FP fP6 = MathUtils.Clamp(Settings.Baumgarte * (separation + Settings.LinearSlop), -Settings.MaxLinearCorrection, 0f);
				FP fP7 = MathUtils.Cross(a3, normal);
				FP fP8 = MathUtils.Cross(a4, normal);
				FP fP9 = fP2 + fP4 + fP3 * fP7 * fP7 + fP5 * fP8 * fP8;
				FP fP10 = ((!(fP9 > 0f)) ? ((FP)0f) : (-fP6 / fP9));
				TSVector2 tSVector = fP10 * normal;
				c -= fP2 * tSVector;
				a -= fP3 * MathUtils.Cross(a3, tSVector);
				c2 += fP4 * tSVector;
				a2 += fP5 * MathUtils.Cross(a4, tSVector);
			}
			_positions[indexA].c = c;
			_positions[indexA].a = a;
			_positions[indexB].c = c2;
			_positions[indexB].a = a2;
		}
		return fP >= -1.5f * Settings.LinearSlop;
	}
}
