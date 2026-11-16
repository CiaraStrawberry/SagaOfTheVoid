namespace TrueSync.Physics2D;

public class RevoluteJoint : Joint2D
{
	private TSVector _impulse;

	private FP _motorImpulse;

	private bool _enableMotor;

	private FP _maxMotorTorque;

	private FP _motorSpeed;

	private bool _enableLimit;

	private FP _referenceAngle;

	private FP _lowerAngle;

	private FP _upperAngle;

	private int _indexA;

	private int _indexB;

	private TSVector2 _rA;

	private TSVector2 _rB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private Mat33 _mass;

	private FP _motorMass;

	private LimitState _limitState;

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

	public FP ReferenceAngle
	{
		get
		{
			return _referenceAngle;
		}
		set
		{
			WakeBodies();
			_referenceAngle = value;
		}
	}

	public FP JointAngle => BodyB._sweep.A - BodyA._sweep.A - ReferenceAngle;

	public FP JointSpeed => BodyB._angularVelocity - BodyA._angularVelocity;

	public bool LimitEnabled
	{
		get
		{
			return _enableLimit;
		}
		set
		{
			if (_enableLimit != value)
			{
				WakeBodies();
				_enableLimit = value;
				_impulse.z = 0f;
			}
		}
	}

	public FP LowerLimit
	{
		get
		{
			return _lowerAngle;
		}
		set
		{
			if (_lowerAngle != value)
			{
				WakeBodies();
				_lowerAngle = value;
				_impulse.z = 0f;
			}
		}
	}

	public FP UpperLimit
	{
		get
		{
			return _upperAngle;
		}
		set
		{
			if (_upperAngle != value)
			{
				WakeBodies();
				_upperAngle = value;
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

	public FP MaxMotorTorque
	{
		get
		{
			return _maxMotorTorque;
		}
		set
		{
			WakeBodies();
			_maxMotorTorque = value;
		}
	}

	public FP MotorImpulse
	{
		get
		{
			return _motorImpulse;
		}
		set
		{
			WakeBodies();
			_motorImpulse = value;
		}
	}

	internal RevoluteJoint()
	{
		JointType = JointType.Revolute;
	}

	public RevoluteJoint(Body bodyA, Body bodyB, TSVector2 anchorA, TSVector2 anchorB, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Revolute;
		if (useWorldCoordinates)
		{
			LocalAnchorA = BodyA.GetLocalPoint(anchorA);
			LocalAnchorB = BodyB.GetLocalPoint(anchorB);
		}
		else
		{
			LocalAnchorA = anchorA;
			LocalAnchorB = anchorB;
		}
		ReferenceAngle = BodyB.Rotation - BodyA.Rotation;
		_impulse = TSVector.zero;
		_limitState = LimitState.Inactive;
	}

	public RevoluteJoint(Body bodyA, Body bodyB, TSVector2 anchor, bool useWorldCoordinates = false)
		: this(bodyA, bodyB, anchor, anchor, useWorldCoordinates)
	{
	}

	public void SetLimits(FP lower, FP upper)
	{
		if (lower != _lowerAngle || upper != _upperAngle)
		{
			WakeBodies();
			_upperAngle = upper;
			_lowerAngle = lower;
			_impulse.z = 0f;
		}
	}

	public FP GetMotorTorque(FP invDt)
	{
		return invDt * _motorImpulse;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		TSVector2 tSVector = new TSVector2(_impulse.x, _impulse.y);
		return invDt * tSVector;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * _impulse.z;
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
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		FP a2 = data.positions[_indexB].a;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		Rot q = new Rot(a);
		Rot q2 = new Rot(a2);
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		_rB = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		bool flag = invIA + invIB == 0f;
		_mass.ex.x = invMassA + invMassB + _rA.y * _rA.y * invIA + _rB.y * _rB.y * invIB;
		_mass.ey.x = -_rA.y * _rA.x * invIA - _rB.y * _rB.x * invIB;
		_mass.ez.x = -_rA.y * invIA - _rB.y * invIB;
		_mass.ex.y = _mass.ey.x;
		_mass.ey.y = invMassA + invMassB + _rA.x * _rA.x * invIA + _rB.x * _rB.x * invIB;
		_mass.ez.y = _rA.x * invIA + _rB.x * invIB;
		_mass.ex.z = _mass.ez.x;
		_mass.ey.z = _mass.ez.y;
		_mass.ez.z = invIA + invIB;
		_motorMass = invIA + invIB;
		if (_motorMass > 0f)
		{
			_motorMass = 1f / _motorMass;
		}
		if (!_enableMotor || flag)
		{
			_motorImpulse = 0f;
		}
		if (_enableLimit && !flag)
		{
			FP fP = a2 - a - ReferenceAngle;
			if (FP.Abs(_upperAngle - _lowerAngle) < 2f * Settings.AngularSlop)
			{
				_limitState = LimitState.Equal;
			}
			else if (fP <= _lowerAngle)
			{
				if (_limitState != LimitState.AtLower)
				{
					_impulse.z = 0f;
				}
				_limitState = LimitState.AtLower;
			}
			else if (fP >= _upperAngle)
			{
				if (_limitState != LimitState.AtUpper)
				{
					_impulse.z = 0f;
				}
				_limitState = LimitState.AtUpper;
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
		}
		_impulse *= data.step.dtRatio;
		_motorImpulse *= data.step.dtRatio;
		TSVector2 tSVector = new TSVector2(_impulse.x, _impulse.y);
		v -= invMassA * tSVector;
		w -= invIA * (MathUtils.Cross(_rA, tSVector) + MotorImpulse + _impulse.z);
		v2 += invMassB * tSVector;
		w2 += invIB * (MathUtils.Cross(_rB, tSVector) + MotorImpulse + _impulse.z);
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
		bool flag = invIA + invIB == 0f;
		if (_enableMotor && _limitState != LimitState.Equal && !flag)
		{
			FP fP = w2 - w - _motorSpeed;
			FP fP2 = _motorMass * -fP;
			FP motorImpulse = _motorImpulse;
			FP fP3 = data.step.dt * _maxMotorTorque;
			_motorImpulse = MathUtils.Clamp(_motorImpulse + fP2, -fP3, fP3);
			fP2 = _motorImpulse - motorImpulse;
			w -= invIA * fP2;
			w2 += invIB * fP2;
		}
		if (_enableLimit && _limitState != LimitState.Inactive && !flag)
		{
			TSVector2 tSVector = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			FP z = w2 - w;
			TSVector b = new TSVector(tSVector.x, tSVector.y, z);
			TSVector tSVector2 = _mass.Solve33(b) * -1;
			if (_limitState == LimitState.Equal)
			{
				_impulse += tSVector2;
			}
			else if (_limitState == LimitState.AtLower)
			{
				FP fP4 = _impulse.z + tSVector2.z;
				if (fP4 < 0f)
				{
					TSVector2 b2 = -tSVector + _impulse.z * new TSVector2(_mass.ez.x, _mass.ez.y);
					TSVector2 tSVector3 = _mass.Solve22(b2);
					tSVector2.x = tSVector3.x;
					tSVector2.y = tSVector3.y;
					tSVector2.z = -_impulse.z;
					_impulse.x += tSVector3.x;
					_impulse.y += tSVector3.y;
					_impulse.z = 0f;
				}
				else
				{
					_impulse += tSVector2;
				}
			}
			else if (_limitState == LimitState.AtUpper)
			{
				FP fP5 = _impulse.z + tSVector2.z;
				if (fP5 > 0f)
				{
					TSVector2 b3 = -tSVector + _impulse.z * new TSVector2(_mass.ez.x, _mass.ez.y);
					TSVector2 tSVector4 = _mass.Solve22(b3);
					tSVector2.x = tSVector4.x;
					tSVector2.y = tSVector4.y;
					tSVector2.z = -_impulse.z;
					_impulse.x += tSVector4.x;
					_impulse.y += tSVector4.y;
					_impulse.z = 0f;
				}
				else
				{
					_impulse += tSVector2;
				}
			}
			TSVector2 tSVector5 = new TSVector2(tSVector2.x, tSVector2.y);
			v -= invMassA * tSVector5;
			w -= invIA * (MathUtils.Cross(_rA, tSVector5) + tSVector2.z);
			v2 += invMassB * tSVector5;
			w2 += invIB * (MathUtils.Cross(_rB, tSVector5) + tSVector2.z);
		}
		else
		{
			TSVector2 tSVector6 = v2 + MathUtils.Cross(w2, _rB) - v - MathUtils.Cross(w, _rA);
			TSVector2 tSVector7 = _mass.Solve22(-tSVector6);
			_impulse.x += tSVector7.x;
			_impulse.y += tSVector7.y;
			v -= invMassA * tSVector7;
			w -= invIA * MathUtils.Cross(_rA, tSVector7);
			v2 += invMassB * tSVector7;
			w2 += invIB * MathUtils.Cross(_rB, tSVector7);
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
		FP fP = 0f;
		bool flag = _invIA + _invIB == 0f;
		if (_enableLimit && _limitState != LimitState.Inactive && !flag)
		{
			FP fP2 = a2 - a - ReferenceAngle;
			FP fP3 = 0f;
			if (_limitState == LimitState.Equal)
			{
				FP fP4 = MathUtils.Clamp(fP2 - _lowerAngle, -Settings.MaxAngularCorrection, Settings.MaxAngularCorrection);
				fP3 = -_motorMass * fP4;
				fP = FP.Abs(fP4);
			}
			else if (_limitState == LimitState.AtLower)
			{
				FP fP5 = fP2 - _lowerAngle;
				fP = -fP5;
				fP5 = MathUtils.Clamp(fP5 + Settings.AngularSlop, -Settings.MaxAngularCorrection, 0f);
				fP3 = -_motorMass * fP5;
			}
			else if (_limitState == LimitState.AtUpper)
			{
				FP fP6 = fP2 - _upperAngle;
				fP = fP6;
				fP6 = MathUtils.Clamp(fP6 - Settings.AngularSlop, 0f, Settings.MaxAngularCorrection);
				fP3 = -_motorMass * fP6;
			}
			a -= _invIA * fP3;
			a2 += _invIB * fP3;
		}
		q.Set(a);
		q2.Set(a2);
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		TSVector2 b = c2 + tSVector2 - c - tSVector;
		FP magnitude = b.magnitude;
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		Mat22 mat = default;
		mat.ex.x = invMassA + invMassB + invIA * tSVector.y * tSVector.y + invIB * tSVector2.y * tSVector2.y;
		mat.ex.y = -invIA * tSVector.x * tSVector.y - invIB * tSVector2.x * tSVector2.y;
		mat.ey.x = mat.ex.y;
		mat.ey.y = invMassA + invMassB + invIA * tSVector.x * tSVector.x + invIB * tSVector2.x * tSVector2.x;
		TSVector2 tSVector3 = -mat.Solve(b);
		c -= invMassA * tSVector3;
		a -= invIA * MathUtils.Cross(tSVector, tSVector3);
		c2 += invMassB * tSVector3;
		a2 += invIB * MathUtils.Cross(tSVector2, tSVector3);
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return magnitude <= Settings.LinearSlop && fP <= Settings.AngularSlop;
	}
}
