namespace TrueSync.Physics2D;

public class PrismaticJoint : Joint2D
{
	private TSVector2 _localYAxisA;

	private TSVector _impulse;

	private FP _lowerTranslation;

	private FP _upperTranslation;

	private FP _maxMotorForce;

	private FP _motorSpeed;

	private bool _enableLimit;

	private bool _enableMotor;

	private LimitState _limitState;

	private int _indexA;

	private int _indexB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private TSVector2 _axis;

	private TSVector2 _perp;

	private FP _s1;

	private FP _s2;

	private FP _a1;

	private FP _a2;

	private Mat33 _K;

	private FP _motorMass;

	private TSVector2 _axis1;

	public TSVector2 LocalAnchorA { get; set; }

	public TSVector2 LocalAnchorB { get; set; }

	public override TSVector2 WorldAnchorA
	{
		get
		{
			return BodyA.GetWorldPoint(LocalAnchorA);
		}
		set
		{
			LocalAnchorA = BodyA.GetLocalPoint(value);
		}
	}

	public override TSVector2 WorldAnchorB
	{
		get
		{
			return BodyB.GetWorldPoint(LocalAnchorB);
		}
		set
		{
			LocalAnchorB = BodyB.GetLocalPoint(value);
		}
	}

	public FP JointTranslation
	{
		get
		{
			TSVector2 value = BodyB.GetWorldPoint(LocalAnchorB) - BodyA.GetWorldPoint(LocalAnchorA);
			TSVector2 worldVector = BodyA.GetWorldVector(LocalXAxis);
			return TSVector2.Dot(value, worldVector);
		}
	}

	public FP JointSpeed
	{
		get
		{
			BodyA.GetTransform(out var transform);
			BodyB.GetTransform(out var transform2);
			TSVector2 tSVector = MathUtils.Mul(ref transform.q, LocalAnchorA - BodyA.LocalCenter);
			TSVector2 tSVector2 = MathUtils.Mul(ref transform2.q, LocalAnchorB - BodyB.LocalCenter);
			TSVector2 tSVector3 = BodyA._sweep.C + tSVector;
			TSVector2 tSVector4 = BodyB._sweep.C + tSVector2;
			TSVector2 value = tSVector4 - tSVector3;
			TSVector2 worldVector = BodyA.GetWorldVector(LocalXAxis);
			TSVector2 linearVelocity = BodyA._linearVelocity;
			TSVector2 linearVelocity2 = BodyB._linearVelocity;
			FP angularVelocity = BodyA._angularVelocity;
			FP angularVelocity2 = BodyB._angularVelocity;
			return TSVector2.Dot(value, MathUtils.Cross(angularVelocity, worldVector)) + TSVector2.Dot(worldVector, linearVelocity2 + MathUtils.Cross(angularVelocity2, tSVector2) - linearVelocity - MathUtils.Cross(angularVelocity, tSVector));
		}
	}

	public bool LimitEnabled
	{
		get
		{
			return _enableLimit;
		}
		set
		{
			if (value != _enableLimit)
			{
				WakeBodies();
				_enableLimit = value;
				_impulse.z = 0;
			}
		}
	}

	public FP LowerLimit
	{
		get
		{
			return _lowerTranslation;
		}
		set
		{
			if (value != _lowerTranslation)
			{
				WakeBodies();
				_lowerTranslation = value;
				_impulse.z = 0f;
			}
		}
	}

	public FP UpperLimit
	{
		get
		{
			return _upperTranslation;
		}
		set
		{
			if (value != _upperTranslation)
			{
				WakeBodies();
				_upperTranslation = value;
				_impulse.z = 0f;
			}
		}
	}

	public bool MotorEnabled
	{
		get
		{
			return _enableMotor;
		}
		set
		{
			WakeBodies();
			_enableMotor = value;
		}
	}

	public FP MotorSpeed
	{
		get
		{
			return _motorSpeed;
		}
		set
		{
			WakeBodies();
			_motorSpeed = value;
		}
	}

	public FP MaxMotorForce
	{
		get
		{
			return _maxMotorForce;
		}
		set
		{
			WakeBodies();
			_maxMotorForce = value;
		}
	}

	public FP MotorImpulse { get; set; }

	public TSVector2 Axis
	{
		get
		{
			return _axis1;
		}
		set
		{
			_axis1 = value;
			LocalXAxis = BodyA.GetLocalVector(_axis1);
			LocalXAxis.Normalize();
			_localYAxisA = MathUtils.Cross(1f, LocalXAxis);
		}
	}

	public TSVector2 LocalXAxis { get; private set; }

	public FP ReferenceAngle { get; set; }

	internal PrismaticJoint()
	{
		JointType = JointType.Prismatic;
	}

	public PrismaticJoint(Body bodyA, Body bodyB, TSVector2 anchorA, TSVector2 anchorB, TSVector2 axis, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		Initialize(anchorA, anchorB, axis, useWorldCoordinates);
	}

	public PrismaticJoint(Body bodyA, Body bodyB, TSVector2 anchor, TSVector2 axis, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		Initialize(anchor, anchor, axis, useWorldCoordinates);
	}

	private void Initialize(TSVector2 localAnchorA, TSVector2 localAnchorB, TSVector2 axis, bool useWorldCoordinates)
	{
		JointType = JointType.Prismatic;
		if (useWorldCoordinates)
		{
			LocalAnchorA = BodyA.GetLocalPoint(localAnchorA);
			LocalAnchorB = BodyB.GetLocalPoint(localAnchorB);
		}
		else
		{
			LocalAnchorA = localAnchorA;
			LocalAnchorB = localAnchorB;
		}
		Axis = axis;
		ReferenceAngle = BodyB.Rotation - BodyA.Rotation;
		_limitState = LimitState.Inactive;
	}

	public void SetLimits(FP lower, FP upper)
	{
		if (upper != _upperTranslation || lower != _lowerTranslation)
		{
			WakeBodies();
			_upperTranslation = upper;
			_lowerTranslation = lower;
			_impulse.z = 0f;
		}
	}

	public FP GetMotorForce(FP invDt)
	{
		return invDt * MotorImpulse;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * (_impulse.x * _perp + (MotorImpulse + _impulse.z) * _axis);
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * _impulse.y;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		_indexA = BodyA.IslandIndex;
		_indexB = BodyB.IslandIndex;
		_localCenterA = BodyA._sweep.LocalCenter;
		_localCenterB = BodyB._sweep.LocalCenter;
		_invMassA = BodyA._invMass;
		_invMassB = BodyB._invMass;
		_invIA = BodyA._invI;
		_invIB = BodyB._invI;
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 c2 = data.positions[_indexB].c;
		FP a2 = data.positions[_indexB].a;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		TSVector2 tSVector3 = c2 - c + tSVector2 - tSVector;
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		_axis = MathUtils.Mul(q, LocalXAxis);
		_a1 = MathUtils.Cross(tSVector3 + tSVector, _axis);
		_a2 = MathUtils.Cross(tSVector2, _axis);
		_motorMass = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
		if (_motorMass > 0f)
		{
			_motorMass = 1f / _motorMass;
		}
		_perp = MathUtils.Mul(q, _localYAxisA);
		_s1 = MathUtils.Cross(tSVector3 + tSVector, _perp);
		_s2 = MathUtils.Cross(tSVector2, _perp);
		FP x = invMassA + invMassB + invIA * _s1 * _s1 + invIB * _s2 * _s2;
		FP fP = invIA * _s1 + invIB * _s2;
		FP fP2 = invIA * _s1 * _a1 + invIB * _s2 * _a2;
		FP fP3 = invIA + invIB;
		if (fP3 == 0f)
		{
			fP3 = 1f;
		}
		FP fP4 = invIA * _a1 + invIB * _a2;
		FP z = invMassA + invMassB + invIA * _a1 * _a1 + invIB * _a2 * _a2;
		_K.ex = new TSVector(x, fP, fP2);
		_K.ey = new TSVector(fP, fP3, fP4);
		_K.ez = new TSVector(fP2, fP4, z);
		if (_enableLimit)
		{
			FP fP5 = TSVector2.Dot(_axis, tSVector3);
			if (FP.Abs(_upperTranslation - _lowerTranslation) < 2f * Settings.LinearSlop)
			{
				_limitState = LimitState.Equal;
			}
			else if (fP5 <= _lowerTranslation)
			{
				if (_limitState != LimitState.AtLower)
				{
					_limitState = LimitState.AtLower;
					_impulse.z = 0f;
				}
			}
			else if (fP5 >= _upperTranslation)
			{
				if (_limitState != LimitState.AtUpper)
				{
					_limitState = LimitState.AtUpper;
					_impulse.z = 0f;
				}
			}
			else
			{
				_limitState = LimitState.Inactive;
				_impulse.z = 0f;
			}
		}
		else
		{
			_limitState = LimitState.Inactive;
			_impulse.z = 0f;
		}
		if (!_enableMotor)
		{
			MotorImpulse = 0f;
		}
		_impulse *= data.step.dtRatio;
		MotorImpulse *= data.step.dtRatio;
		TSVector2 tSVector4 = _impulse.x * _perp + (MotorImpulse + _impulse.z) * _axis;
		FP fP6 = _impulse.x * _s1 + _impulse.y + (MotorImpulse + _impulse.z) * _a1;
		FP fP7 = _impulse.x * _s2 + _impulse.y + (MotorImpulse + _impulse.z) * _a2;
		v -= invMassA * tSVector4;
		w -= invIA * fP6;
		v2 += invMassB * tSVector4;
		w2 += invIB * fP7;
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		if (_enableMotor && _limitState != LimitState.Equal)
		{
			FP fP = TSVector2.Dot(_axis, v2 - v) + _a2 * w2 - _a1 * w;
			FP fP2 = _motorMass * (_motorSpeed - fP);
			FP motorImpulse = MotorImpulse;
			FP fP3 = data.step.dt * _maxMotorForce;
			MotorImpulse = MathUtils.Clamp(MotorImpulse + fP2, -fP3, fP3);
			fP2 = MotorImpulse - motorImpulse;
			TSVector2 tSVector = fP2 * _axis;
			FP fP4 = fP2 * _a1;
			FP fP5 = fP2 * _a2;
			v -= invMassA * tSVector;
			w -= invIA * fP4;
			v2 += invMassB * tSVector;
			w2 += invIB * fP5;
		}
		TSVector2 tSVector2 = new TSVector2
		{
			x = TSVector2.Dot(_perp, v2 - v) + _s2 * w2 - _s1 * w,
			y = w2 - w
		};
		if (_enableLimit && _limitState != LimitState.Inactive)
		{
			FP z = TSVector2.Dot(_axis, v2 - v) + _a2 * w2 - _a1 * w;
			TSVector tSVector3 = new TSVector(tSVector2.x, tSVector2.y, z);
			TSVector impulse = _impulse;
			TSVector tSVector4 = _K.Solve33(tSVector3 * -1);
			_impulse += tSVector4;
			if (_limitState == LimitState.AtLower)
			{
				_impulse.z = TSMath.Max(_impulse.z, 0f);
			}
			else if (_limitState == LimitState.AtUpper)
			{
				_impulse.z = TSMath.Min(_impulse.z, 0f);
			}
			TSVector2 b = -tSVector2 - (_impulse.z - impulse.z) * new TSVector2(_K.ez.x, _K.ez.y);
			TSVector2 tSVector5 = _K.Solve22(b) + new TSVector2(impulse.x, impulse.y);
			_impulse.x = tSVector5.x;
			_impulse.y = tSVector5.y;
			tSVector4 = _impulse - impulse;
			TSVector2 tSVector6 = tSVector4.x * _perp + tSVector4.z * _axis;
			FP fP6 = tSVector4.x * _s1 + tSVector4.y + tSVector4.z * _a1;
			FP fP7 = tSVector4.x * _s2 + tSVector4.y + tSVector4.z * _a2;
			v -= invMassA * tSVector6;
			w -= invIA * fP6;
			v2 += invMassB * tSVector6;
			w2 += invIB * fP7;
		}
		else
		{
			TSVector2 tSVector7 = _K.Solve22(-tSVector2);
			_impulse.x += tSVector7.x;
			_impulse.y += tSVector7.y;
			TSVector2 tSVector8 = tSVector7.x * _perp;
			FP fP8 = tSVector7.x * _s1 + tSVector7.y;
			FP fP9 = tSVector7.x * _s2 + tSVector7.y;
			v -= invMassA * tSVector8;
			w -= invIA * fP8;
			v2 += invMassB * tSVector8;
			w2 += invIB * fP9;
		}
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 c2 = data.positions[_indexB].c;
		FP a2 = data.positions[_indexB].a;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		TSVector2 tSVector3 = c2 + tSVector2 - c - tSVector;
		TSVector2 tSVector4 = MathUtils.Mul(q, LocalXAxis);
		FP fP = MathUtils.Cross(tSVector3 + tSVector, tSVector4);
		FP fP2 = MathUtils.Cross(tSVector2, tSVector4);
		TSVector2 tSVector5 = MathUtils.Mul(q, _localYAxisA);
		FP fP3 = MathUtils.Cross(tSVector3 + tSVector, tSVector5);
		FP fP4 = MathUtils.Cross(tSVector2, tSVector5);
		TSVector2 tSVector6 = new TSVector2
		{
			x = TSVector2.Dot(tSVector5, tSVector3),
			y = a2 - a - ReferenceAngle
		};
		FP fP5 = FP.Abs(tSVector6.x);
		FP fP6 = FP.Abs(tSVector6.y);
		bool flag = false;
		FP z = 0f;
		if (_enableLimit)
		{
			FP fP7 = TSVector2.Dot(tSVector4, tSVector3);
			if (FP.Abs(_upperTranslation - _lowerTranslation) < 2f * Settings.LinearSlop)
			{
				z = MathUtils.Clamp(fP7, -Settings.MaxLinearCorrection, Settings.MaxLinearCorrection);
				fP5 = TSMath.Max(fP5, FP.Abs(fP7));
				flag = true;
			}
			else if (fP7 <= _lowerTranslation)
			{
				z = MathUtils.Clamp(fP7 - _lowerTranslation + Settings.LinearSlop, -Settings.MaxLinearCorrection, 0f);
				fP5 = TSMath.Max(fP5, _lowerTranslation - fP7);
				flag = true;
			}
			else if (fP7 >= _upperTranslation)
			{
				z = MathUtils.Clamp(fP7 - _upperTranslation - Settings.LinearSlop, 0f, Settings.MaxLinearCorrection);
				fP5 = TSMath.Max(fP5, fP7 - _upperTranslation);
				flag = true;
			}
		}
		TSVector tSVector7;
		if (flag)
		{
			FP x = invMassA + invMassB + invIA * fP3 * fP3 + invIB * fP4 * fP4;
			FP fP8 = invIA * fP3 + invIB * fP4;
			FP fP9 = invIA * fP3 * fP + invIB * fP4 * fP2;
			FP fP10 = invIA + invIB;
			if (fP10 == 0f)
			{
				fP10 = 1f;
			}
			FP fP11 = invIA * fP + invIB * fP2;
			FP z2 = invMassA + invMassB + invIA * fP * fP + invIB * fP2 * fP2;
			tSVector7 = new Mat33
			{
				ex = new TSVector(x, fP8, fP9),
				ey = new TSVector(fP8, fP10, fP11),
				ez = new TSVector(fP9, fP11, z2)
			}.Solve33(new TSVector
			{
				x = tSVector6.x,
				y = tSVector6.y,
				z = z
			} * -1);
		}
		else
		{
			FP x2 = invMassA + invMassB + invIA * fP3 * fP3 + invIB * fP4 * fP4;
			FP fP12 = invIA * fP3 + invIB * fP4;
			FP fP13 = invIA + invIB;
			if (fP13 == 0f)
			{
				fP13 = 1f;
			}
			TSVector2 tSVector8 = new Mat22
			{
				ex = new TSVector2(x2, fP12),
				ey = new TSVector2(fP12, fP13)
			}.Solve(-tSVector6);
			tSVector7 = new TSVector
			{
				x = tSVector8.x,
				y = tSVector8.y,
				z = 0f
			};
		}
		TSVector2 tSVector9 = tSVector7.x * tSVector5 + tSVector7.z * tSVector4;
		FP fP14 = tSVector7.x * fP3 + tSVector7.y + tSVector7.z * fP;
		FP fP15 = tSVector7.x * fP4 + tSVector7.y + tSVector7.z * fP2;
		c -= invMassA * tSVector9;
		a -= invIA * fP14;
		c2 += invMassB * tSVector9;
		a2 += invIB * fP15;
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return fP5 <= Settings.LinearSlop && fP6 <= Settings.AngularSlop;
	}
}
