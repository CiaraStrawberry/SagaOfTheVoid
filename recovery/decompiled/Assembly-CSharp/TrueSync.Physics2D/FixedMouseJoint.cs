namespace TrueSync.Physics2D;

public class FixedMouseJoint : Joint2D
{
	private TSVector2 _worldAnchor;

	private FP _frequency;

	private FP _dampingRatio;

	private FP _beta;

	private TSVector2 _impulse;

	private FP _maxForce;

	private FP _gamma;

	private int _indexA;

	private TSVector2 _rA;

	private TSVector2 _localCenterA;

	private FP _invMassA;

	private FP _invIA;

	private Mat22 _mass;

	private TSVector2 _C;

	public TSVector2 LocalAnchorA { get; set; }

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
			return _worldAnchor;
		}
		set
		{
			WakeBodies();
			_worldAnchor = value;
		}
	}

	public FP MaxForce
	{
		get
		{
			return _maxForce;
		}
		set
		{
			_maxForce = value;
		}
	}

	public FP Frequency
	{
		get
		{
			return _frequency;
		}
		set
		{
			_frequency = value;
		}
	}

	public FP DampingRatio
	{
		get
		{
			return _dampingRatio;
		}
		set
		{
			_dampingRatio = value;
		}
	}

	public FixedMouseJoint(Body body, TSVector2 worldAnchor)
		: base(body)
	{
		JointType = JointType.FixedMouse;
		Frequency = 5f;
		DampingRatio = 0.7f;
		MaxForce = 1000 * body.Mass;
		_worldAnchor = worldAnchor;
		LocalAnchorA = MathUtils.MulT(BodyA._xf, worldAnchor);
	}

	public override TSVector2 GetReactionForce(FP invDt)
	{
		return invDt * _impulse;
	}

	public override FP GetReactionTorque(FP invDt)
	{
		return invDt * 0f;
	}

	internal override void InitVelocityConstraints(ref SolverData data)
	{
		_indexA = BodyA.IslandIndex;
		_localCenterA = BodyA._sweep.LocalCenter;
		_invMassA = BodyA._invMass;
		_invIA = BodyA._invI;
		TSVector2 c = data.positions[_indexA].c;
		FP a = data.positions[_indexA].a;
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		Rot q = new Rot(a);
		FP mass = BodyA.Mass;
		FP fP = 2f * Settings.Pi * Frequency;
		FP fP2 = 2f * mass * DampingRatio * fP;
		FP fP3 = mass * (fP * fP);
		FP dt = data.step.dt;
		_gamma = dt * (fP2 + dt * fP3);
		if (_gamma != 0f)
		{
			_gamma = 1f / _gamma;
		}
		_beta = dt * fP3 * _gamma;
		_rA = MathUtils.Mul(q, LocalAnchorA - _localCenterA);
		Mat22 mat = default;
		mat.ex.x = _invMassA + _invIA * _rA.y * _rA.y + _gamma;
		mat.ex.y = -_invIA * _rA.x * _rA.y;
		mat.ey.x = mat.ex.y;
		mat.ey.y = _invMassA + _invIA * _rA.x * _rA.x + _gamma;
		_mass = mat.Inverse;
		_C = c + _rA - _worldAnchor;
		_C *= _beta;
		w *= (FP)0.98f;
		_impulse *= data.step.dtRatio;
		v += _invMassA * _impulse;
		w += _invIA * MathUtils.Cross(_rA, _impulse);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
	}

	internal override void SolveVelocityConstraints(ref SolverData data)
	{
		TSVector2 v = data.velocities[_indexA].v;
		FP w = data.velocities[_indexA].w;
		TSVector2 tSVector = v + MathUtils.Cross(w, _rA);
		TSVector2 tSVector2 = MathUtils.Mul(ref _mass, -(tSVector + _C + _gamma * _impulse));
		TSVector2 impulse = _impulse;
		_impulse += tSVector2;
		FP fP = data.step.dt * MaxForce;
		if (_impulse.LengthSquared() > fP * fP)
		{
			_impulse *= fP / _impulse.magnitude;
		}
		tSVector2 = _impulse - impulse;
		v += _invMassA * tSVector2;
		w += _invIA * MathUtils.Cross(_rA, tSVector2);
		data.velocities[_indexA].v = v;
		data.velocities[_indexA].w = w;
	}

	internal override bool SolvePositionConstraints(ref SolverData data)
	{
		return true;
	}
}
