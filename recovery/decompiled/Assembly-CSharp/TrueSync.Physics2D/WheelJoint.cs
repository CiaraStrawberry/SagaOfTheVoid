namespace TrueSync.Physics2D;

public class WheelJoint : Joint2D
{
	private TSVector2 _localYAxis;

	private FP _impulse;

	private FP _motorImpulse;

	private FP _springImpulse;

	private FP _maxMotorTorque;

	private FP _motorSpeed;

	private bool _enableMotor;

	private int _indexA;

	private int _indexB;

	private TSVector2 _localCenterA;

	private TSVector2 _localCenterB;

	private FP _invMassA;

	private FP _invMassB;

	private FP _invIA;

	private FP _invIB;

	private TSVector2 _ax;

	private TSVector2 _ay;

	private FP _sAx;

	private FP _sBx;

	private FP _sAy;

	private FP _sBy;

	private FP _mass;

	private FP _motorMass;

	private FP _springMass;

	private FP _bias;

	private FP _gamma;

	private TSVector2 _axis;

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

	public TSVector2 Axis
	{
		get
		{
			return _axis;
		}
		set
		{
			_axis = value;
			LocalXAxis = BodyA.GetLocalVector(_axis);
			_localYAxis = MathUtils.Cross(1f, LocalXAxis);
		}
	}

	public TSVector2 LocalXAxis { get; private set; }

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

	public FP Frequency { get; set; }

	public FP DampingRatio { get; set; }

	public FP JointTranslation
	{
		get
		{
			Body bodyA = BodyA;
			Body bodyB = BodyB;
			TSVector2 worldPoint = bodyA.GetWorldPoint(LocalAnchorA);
			TSVector2 worldPoint2 = bodyB.GetWorldPoint(LocalAnchorB);
			TSVector2 value = worldPoint2 - worldPoint;
			TSVector2 worldVector = bodyA.GetWorldVector(LocalXAxis);
			return TSVector2.Dot(value, worldVector);
		}
	}

	public FP JointSpeed
	{
		get
		{
			FP angularVelocity = BodyA.AngularVelocity;
			FP angularVelocity2 = BodyB.AngularVelocity;
			return angularVelocity2 - angularVelocity;
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

	internal WheelJoint()
	{
		JointType = JointType.Wheel;
	}

	public WheelJoint(Body bodyA, Body bodyB, TSVector2 anchor, TSVector2 axis, bool useWorldCoordinates = false)
		: base(bodyA, bodyB)
	{
		JointType = JointType.Wheel;
		if (useWorldCoordinates)
		{
			LocalAnchorA = bodyA.GetLocalPoint(anchor);
			LocalAnchorB = bodyB.GetLocalPoint(anchor);
		}
		else
		{
			LocalAnchorA = bodyA.GetLocalPoint(bodyB.GetWorldPoint(anchor));
			LocalAnchorB = anchor;
		}
		Axis = axis;
	}

	public FP GetMotorTorque(FP invDt)
	{
		return invDt * _motorImpulse;
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * (_impulse * _ay + _springImpulse * _ax);
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * _motorImpulse;
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
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
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
		TSVector2 tSVector3 = c2 + tSVector2 - c - tSVector;
		_ay = MathUtils.Mul(q, _localYAxis);
		_sAy = MathUtils.Cross(tSVector3 + tSVector, _ay);
		_sBy = MathUtils.Cross(tSVector2, _ay);
		_mass = invMassA + invMassB + invIA * _sAy * _sAy + invIB * _sBy * _sBy;
		if (_mass > 0f)
		{
			_mass = 1f / _mass;
		}
		_springMass = 0f;
		_bias = 0f;
		_gamma = 0f;
		if (Frequency > 0f)
		{
			_ax = MathUtils.Mul(q, LocalXAxis);
			_sAx = MathUtils.Cross(tSVector3 + tSVector, _ax);
			_sBx = MathUtils.Cross(tSVector2, _ax);
			FP fP = invMassA + invMassB + invIA * _sAx * _sAx + invIB * _sBx * _sBx;
			if (fP > 0f)
			{
				_springMass = 1f / fP;
				FP fP2 = TSVector2.Dot(tSVector3, _ax);
				FP fP3 = 2f * Settings.Pi * Frequency;
				FP fP4 = 2f * _springMass * DampingRatio * fP3;
				FP fP5 = _springMass * fP3 * fP3;
				FP dt = data.step.dt;
				_gamma = dt * (fP4 + dt * fP5);
				if (_gamma > 0f)
				{
					_gamma = 1f / _gamma;
				}
				_bias = fP2 * dt * fP5 * _gamma;
				_springMass = fP + _gamma;
				if (_springMass > 0f)
				{
					_springMass = 1f / _springMass;
				}
			}
		}
		else
		{
			_springImpulse = 0f;
		}
		if (_enableMotor)
		{
			_motorMass = invIA + invIB;
			if (_motorMass > 0f)
			{
				_motorMass = 1f / _motorMass;
			}
		}
		else
		{
			_motorMass = 0f;
			_motorImpulse = 0f;
		}
		_impulse *= data.step.dtRatio;
		_springImpulse *= data.step.dtRatio;
		_motorImpulse *= data.step.dtRatio;
		TSVector2 tSVector4 = _impulse * _ay + _springImpulse * _ax;
		FP fP6 = _impulse * _sAy + _springImpulse * _sAx + _motorImpulse;
		FP fP7 = _impulse * _sBy + _springImpulse * _sBx + _motorImpulse;
		v -= _invMassA * tSVector4;
		w -= _invIA * fP6;
		v2 += _invMassB * tSVector4;
		w2 += _invIB * fP7;
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
		data.velocities[_indexB].v = v2;
		data.velocities[_indexB].w = w2;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		FP invMassA = _invMassA;
		FP invMassB = _invMassB;
		FP invIA = _invIA;
		FP invIB = _invIB;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 v2 = data.velocities[_indexB].v;
		FP w2 = data.velocities[_indexB].w;
		FP fP = TSVector2.Dot(_ax, v2 - v) + _sBx * w2 - _sAx * w;
		FP fP2 = -_springMass * (fP + _bias + _gamma * _springImpulse);
		_springImpulse += fP2;
		TSVector2 tSVector = fP2 * _ax;
		FP fP3 = fP2 * _sAx;
		FP fP4 = fP2 * _sBx;
		v -= invMassA * tSVector;
		w -= invIA * fP3;
		v2 += invMassB * tSVector;
		w2 += invIB * fP4;
		FP fP5 = w2 - w - _motorSpeed;
		FP fP6 = -_motorMass * fP5;
		FP motorImpulse = _motorImpulse;
		FP fP7 = data.step.dt * _maxMotorTorque;
		_motorImpulse = MathUtils.Clamp(_motorImpulse + fP6, -fP7, fP7);
		fP6 = _motorImpulse - motorImpulse;
		w -= invIA * fP6;
		w2 += invIB * fP6;
		FP fP8 = TSVector2.Dot(_ay, v2 - v) + _sBy * w2 - _sAy * w;
		FP fP9 = -_mass * fP8;
		_impulse += fP9;
		TSVector2 tSVector2 = fP9 * _ay;
		FP fP10 = fP9 * _sAy;
		FP fP11 = fP9 * _sBy;
		v -= invMassA * tSVector2;
		w -= invIA * fP10;
		v2 += invMassB * tSVector2;
		w2 += invIB * fP11;
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
		TSVector2 tSVector = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		TSVector2 tSVector2 = MathUtils.Mul(q2, LocalAnchorB - _localCenterB);
		TSVector2 tSVector3 = c2 - c + tSVector2 - tSVector;
		TSVector2 tSVector4 = MathUtils.Mul(q, _localYAxis);
		FP fP = MathUtils.Cross(tSVector3 + tSVector, tSVector4);
		FP fP2 = MathUtils.Cross(tSVector2, tSVector4);
		FP fP3 = TSVector2.Dot(tSVector3, tSVector4);
		FP fP4 = _invMassA + _invMassB + _invIA * _sAy * _sAy + _invIB * _sBy * _sBy;
		FP fP5 = ((!(fP4 != 0f)) ? ((FP)0f) : (-fP3 / fP4));
		TSVector2 tSVector5 = fP5 * tSVector4;
		FP fP6 = fP5 * fP;
		FP fP7 = fP5 * fP2;
		c -= _invMassA * tSVector5;
		a -= _invIA * fP6;
		c2 += _invMassB * tSVector5;
		a2 += _invIB * fP7;
		data.positions[_indexA].c = c;
		data.positions[_indexA].a = a;
		data.positions[_indexB].c = c2;
		data.positions[_indexB].a = a2;
		return FP.Abs(fP3) <= Settings.LinearSlop;
	}
}
