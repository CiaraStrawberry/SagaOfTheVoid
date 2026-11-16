namespace TrueSync.Physics2D;

public class Island
{
	internal ContactManager _contactManager;

	internal ContactSolver _contactSolver = new ContactSolver();

	internal Contact[] _contacts;

	internal Joint2D[] _joints;

	private static readonly FP LinTolSqr = Settings.LinearSleepTolerance * Settings.LinearSleepTolerance;

	private static readonly FP AngTolSqr = Settings.AngularSleepTolerance * Settings.AngularSleepTolerance;

	public Body[] Bodies;

	public int BodyCount;

	public int ContactCount;

	public int JointCount;

	public Velocity[] _velocities;

	public Position[] _positions;

	public int BodyCapacity;

	public int ContactCapacity;

	public int JointCapacity;

	public void Reset(int bodyCapacity, int contactCapacity, int jointCapacity, ContactManager contactManager)
	{
		BodyCapacity = bodyCapacity;
		ContactCapacity = contactCapacity;
		JointCapacity = jointCapacity;
		BodyCount = 0;
		ContactCount = 0;
		JointCount = 0;
		_contactManager = contactManager;
		if (Bodies == null || Bodies.Length < bodyCapacity)
		{
			Bodies = new Body[bodyCapacity];
			_velocities = new Velocity[bodyCapacity];
			_positions = new Position[bodyCapacity];
		}
		if (_contacts == null || _contacts.Length < contactCapacity)
		{
			_contacts = new Contact[contactCapacity * 2];
		}
		if (_joints == null || _joints.Length < jointCapacity)
		{
			_joints = new Joint2D[jointCapacity * 2];
		}
	}

	public void Clear()
	{
		BodyCount = 0;
		ContactCount = 0;
		JointCount = 0;
	}

	public void Solve(ref TimeStep step, ref TSVector2 gravity)
	{
		FP dt = step.dt;
		for (int i = 0; i < BodyCount; i++)
		{
			Body body = Bodies[i];
			TSVector2 c = body._sweep.C;
			FP a = body._sweep.A;
			TSVector2 linearVelocity = body._linearVelocity;
			FP angularVelocity = body._angularVelocity;
			body._sweep.C0 = body._sweep.C;
			body._sweep.A0 = body._sweep.A;
			if (body.BodyType == BodyType.Dynamic)
			{
				if (body.IgnoreGravity)
				{
					linearVelocity += dt * (body._invMass * body._force);
				}
				else
				{
					linearVelocity += dt * (body.GravityScale * gravity + body._invMass * body._force);
				}
				angularVelocity += dt * body._invI * body._torque;
				linearVelocity *= 1 / (1 + dt * body.LinearDamping);
				angularVelocity *= 1 / (1 + dt * body.AngularDamping);
			}
			_positions[i].c = c;
			_positions[i].a = a;
			_velocities[i].v = linearVelocity;
			_velocities[i].w = angularVelocity;
		}
		SolverData data = new SolverData
		{
			step = step,
			positions = _positions,
			velocities = _velocities
		};
		_contactSolver.Reset(step, ContactCount, _contacts, _positions, _velocities);
		_contactSolver.InitializeVelocityConstraints();
		_contactSolver.WarmStart();
		for (int j = 0; j < JointCount; j++)
		{
			if (_joints[j].Enabled)
			{
				_joints[j].InitVelocityConstraints(ref data);
			}
		}
		for (int k = 0; k < Settings.VelocityIterations; k++)
		{
			for (int l = 0; l < JointCount; l++)
			{
				Joint2D joint2D = _joints[l];
				if (joint2D.Enabled)
				{
					joint2D.SolveVelocityConstraints(ref data);
					joint2D.Validate(step.inv_dt);
				}
			}
			_contactSolver.SolveVelocityConstraints();
		}
		_contactSolver.StoreImpulses();
		for (int m = 0; m < BodyCount; m++)
		{
			TSVector2 c2 = _positions[m].c;
			FP a2 = _positions[m].a;
			TSVector2 v = _velocities[m].v;
			FP w = _velocities[m].w;
			TSVector2 tSVector = dt * v;
			if (TSVector2.Dot(tSVector, tSVector) > Settings.MaxTranslationSquared)
			{
				FP fP = Settings.MaxTranslation / tSVector.magnitude;
				v *= fP;
			}
			FP fP2 = dt * w;
			if (fP2 * fP2 > Settings.MaxRotationSquared)
			{
				FP fP3 = Settings.MaxRotation / FP.Abs(fP2);
				w *= fP3;
			}
			c2 += dt * v;
			a2 += dt * w;
			_positions[m].c = c2;
			_positions[m].a = a2;
			_velocities[m].v = v;
			_velocities[m].w = w;
		}
		bool flag = false;
		for (int n = 0; n < Settings.PositionIterations; n++)
		{
			bool flag2 = _contactSolver.SolvePositionConstraints();
			bool flag3 = true;
			for (int num = 0; num < JointCount; num++)
			{
				Joint2D joint2D2 = _joints[num];
				if (joint2D2.Enabled)
				{
					bool flag4 = joint2D2.SolvePositionConstraints(ref data);
					flag3 = flag3 && flag4;
				}
			}
			if (flag2 && flag3)
			{
				flag = true;
				break;
			}
		}
		for (int num2 = 0; num2 < BodyCount; num2++)
		{
			Body body2 = Bodies[num2];
			body2._sweep.C = _positions[num2].c;
			body2._sweep.A = _positions[num2].a;
			body2._linearVelocity = _velocities[num2].v;
			body2._angularVelocity = _velocities[num2].w;
			body2.SynchronizeTransform();
		}
		Report(_contactSolver._velocityConstraints);
		if (!Settings.AllowSleep)
		{
			return;
		}
		FP fP4 = Settings.MaxFP;
		for (int num3 = 0; num3 < BodyCount; num3++)
		{
			Body body3 = Bodies[num3];
			if (body3.BodyType != BodyType.Static)
			{
				if (!body3.SleepingAllowed || body3._angularVelocity * body3._angularVelocity > AngTolSqr || TSVector2.Dot(body3._linearVelocity, body3._linearVelocity) > LinTolSqr)
				{
					body3._sleepTime = 0f;
					fP4 = 0f;
				}
				else
				{
					body3._sleepTime += dt;
					fP4 = TSMath.Min(fP4, body3._sleepTime);
				}
			}
		}
		if (fP4 >= Settings.TimeToSleep && flag)
		{
			for (int num4 = 0; num4 < BodyCount; num4++)
			{
				Body body4 = Bodies[num4];
				body4.Awake = false;
			}
		}
	}

	internal void SolveTOI(ref TimeStep subStep, int toiIndexA, int toiIndexB, bool warmstarting)
	{
		for (int i = 0; i < BodyCount; i++)
		{
			Body body = Bodies[i];
			_positions[i].c = body._sweep.C;
			_positions[i].a = body._sweep.A;
			_velocities[i].v = body._linearVelocity;
			_velocities[i].w = body._angularVelocity;
		}
		_contactSolver.Reset(subStep, ContactCount, _contacts, _positions, _velocities, warmstarting);
		for (int j = 0; j < Settings.TOIPositionIterations; j++)
		{
			if (_contactSolver.SolveTOIPositionConstraints(toiIndexA, toiIndexB))
			{
				break;
			}
		}
		Bodies[toiIndexA]._sweep.C0 = _positions[toiIndexA].c;
		Bodies[toiIndexA]._sweep.A0 = _positions[toiIndexA].a;
		Bodies[toiIndexB]._sweep.C0 = _positions[toiIndexB].c;
		Bodies[toiIndexB]._sweep.A0 = _positions[toiIndexB].a;
		_contactSolver.InitializeVelocityConstraints();
		for (int k = 0; k < Settings.TOIVelocityIterations; k++)
		{
			_contactSolver.SolveVelocityConstraints();
		}
		FP dt = subStep.dt;
		for (int l = 0; l < BodyCount; l++)
		{
			TSVector2 c = _positions[l].c;
			FP a = _positions[l].a;
			TSVector2 v = _velocities[l].v;
			FP w = _velocities[l].w;
			TSVector2 tSVector = dt * v;
			if (TSVector2.Dot(tSVector, tSVector) > Settings.MaxTranslationSquared)
			{
				FP fP = Settings.MaxTranslation / tSVector.magnitude;
				v *= fP;
			}
			FP fP2 = dt * w;
			if (fP2 * fP2 > Settings.MaxRotationSquared)
			{
				FP fP3 = Settings.MaxRotation / FP.Abs(fP2);
				w *= fP3;
			}
			c += dt * v;
			a += dt * w;
			_positions[l].c = c;
			_positions[l].a = a;
			_velocities[l].v = v;
			_velocities[l].w = w;
			Body body2 = Bodies[l];
			body2._sweep.C = c;
			body2._sweep.A = a;
			body2._linearVelocity = v;
			body2._angularVelocity = w;
			body2.SynchronizeTransform();
		}
		Report(_contactSolver._velocityConstraints);
	}

	public void Add(Body body)
	{
		body.IslandIndex = BodyCount;
		Bodies[BodyCount++] = body;
	}

	public void Add(Contact contact)
	{
		_contacts[ContactCount++] = contact;
	}

	public void Add(Joint2D joint)
	{
		_joints[JointCount++] = joint;
	}

	private void Report(ContactVelocityConstraint[] constraints)
	{
		if (_contactManager == null)
		{
			return;
		}
		for (int i = 0; i < ContactCount; i++)
		{
			Contact contact = _contacts[i];
			if (contact.FixtureA.AfterCollision != null)
			{
				contact.FixtureA.AfterCollision(contact.FixtureA, contact.FixtureB, contact, constraints[i]);
			}
			if (contact.FixtureB.AfterCollision != null)
			{
				contact.FixtureB.AfterCollision(contact.FixtureB, contact.FixtureA, contact, constraints[i]);
			}
			if (_contactManager.PostSolve != null)
			{
				_contactManager.PostSolve(contact, constraints[i]);
			}
		}
	}
}
